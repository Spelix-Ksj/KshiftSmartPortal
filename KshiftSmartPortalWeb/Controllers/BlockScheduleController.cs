using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;
using KShiftSmartPortalWeb.Utils;

namespace KShiftSmartPortalWeb.Controllers
{
    /// <summary>
    /// 블록제작 일정 조회 결과 홀더.
    /// 컬럼이 SCM_EXCEL_TEMP_DETAIL로 런타임 결정되므로 행 POCO 대신 DataTable로 전달한다.
    /// </summary>
    public class BlockScheduleResult
    {
        // 2단계(스프레드시트 셀 주입)용 키
        public string TempNo;
        public string TableName;
        public string SheetName;
        public string ExcelName;
        public int StRow;
        public int StCol;

        /// <summary>TEMP_DETAIL 정의 행 중 Data에 실재하는 컬럼만</summary>
        public DataTable ColumnDefs;
        public DataTable Data;

        /// <summary>null이면 성공</summary>
        public string Error;
    }

    /// <summary>
    /// 블록제작 일정 조회 컨트롤러 (ALIS ScmBlock*PlanMan 9탭 공용, 조회 전용)
    /// 테이블명/컬럼/기준일정 컬럼은 SCM_EXCEL_TEMP_MASTER/DETAIL에서 런타임 결정된다.
    /// </summary>
    public class BlockScheduleController : BaseController
    {
        /// <summary>동적 SQL 조각(테이블명·기준일정 컬럼) 허용 패턴 (.NET의 $는 끝 개행을 허용하므로 \A…\z 사용)</summary>
        private static readonly Regex SafeIdent = new Regex(@"\A[A-Z0-9_]+\z");

        private const string DefaultOrderBy = "PROJECT_NO, ITM_COD";

        /// <summary>탭별 ORDER BY 오버라이드 (키 = 뷰명). 없으면 DefaultOrderBy.</summary>
        private static readonly Dictionary<string, string> OrderByOverrides =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "ScmBlockAssPlanMan", "NVL(PROP01,'999'), PROJECT_NO, ITM_COD" }
            };

        /// <summary>
        /// 일정 데이터를 조회한다. 실패 시 Error에 메시지를 담고 Data는 빈 DataTable. throw 하지 않는다.
        /// </summary>
        /// <param name="dateCol">기준일정 컬럼(PROP_NAME). "*"이면 기간 조건 생략</param>
        public BlockScheduleResult GetSchedule(string viewName, string companyNo, string caseNo,
                                               string dateCol, DateTime dtStart, DateTime dtEnd)
        {
            var result = new BlockScheduleResult { Data = new DataTable(), ColumnDefs = new DataTable() };

            DataRow master = GetTemplateMaster(companyNo, viewName);
            if (master == null)
            {
                result.Error = "템플릿(SCM_EXCEL_TEMP_MASTER, TAG1=뷰명)이 없습니다.";
                return result;
            }

            // 테이블명은 문자열 치환 자리이므로 허용 패턴 검증 (Oracle 비인용 식별자는 대소문자 무관 → 대문자화)
            string tableName = (master["TAG2"]?.ToString() ?? "").Trim().ToUpperInvariant();
            if (!SafeIdent.IsMatch(tableName))
            {
                result.Error = "템플릿의 테이블명(TAG2)이 올바르지 않습니다.";
                return result;
            }

            DataTable defs = GetTemplateDetail(companyNo, viewName);
            if (defs.Rows.Count == 0)
            {
                result.Error = "템플릿 컬럼 정의(SCM_EXCEL_TEMP_DETAIL)가 없습니다.";
                return result;
            }

            // 기준일정 컬럼은 화이트리스트(PROP2='Y' 행의 PROP_NAME) 완전일치가 아니면 데이터 쿼리 실행 금지
            string safeDateCol = ResolveDateCol(defs, dateCol);
            if (safeDateCol == null)
            {
                result.Error = "기준일정 컬럼이 템플릿 설정과 일치하지 않습니다.";
                return result;
            }

            DataTable data = GetScheduleData(tableName, OrderByFor(viewName), caseNo, companyNo, safeDateCol, dtStart, dtEnd);
            if (data == null)
            {
                result.Error = "일정 데이터 조회 중 오류가 발생했습니다.";
                return result;
            }

            FillMeta(result, master, tableName);
            result.Data = data;
            result.ColumnDefs = FilterExistingColumns(defs, data);
            return result;
        }

        /// <summary>
        /// SCM_EXCEL_TEMP_DETAIL 컬럼 정의 조회 (ALIS 그리드와 동일하게 USE_YN/OBJ_NAME 무필터, VIEW_ORDER 순).
        /// 기준일정 콤보(PROP2='Y')와 그리드 컬럼 생성에 공용.
        /// </summary>
        public DataTable GetTemplateDetail(string companyNo, string viewName)
        {
            const string query = @"
                SELECT A.TEMP_NO, A.PROP_NAME, A.TAG1, A.TAG2, A.TAG3, A.KEY_FIELD, A.PROP2, A.VIEW_ORDER, A.COLUMN_CELL, A.OBJ_NAME
                FROM SCM_EXCEL_TEMP_DETAIL A, SCM_EXCEL_TEMP_MASTER B
                WHERE B.COMPANY_NO = :companyNo AND UPPER(B.TAG1) = UPPER(:viewName)
                  AND A.COMPANY_NO = B.COMPANY_NO AND A.TEMP_NO = B.TEMP_NO
                ORDER BY A.VIEW_ORDER";
            try
            {
                using (var conn = new OracleConnection(ConnectionString))
                using (var cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("companyNo", companyNo));
                    cmd.Parameters.Add(new OracleParameter("viewName", viewName));
                    SqlLogger.LogCommand(cmd, "블록제작 템플릿 컬럼 정의 조회");
                    return Fill(cmd, "블록제작 템플릿 컬럼 정의 조회 완료");
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, $"블록제작 템플릿 컬럼 정의 조회 실패: {companyNo}/{viewName}");
                return new DataTable();
            }
        }

        /// <summary>SCM_EXCEL_TEMP_MASTER 1행 조회 (없거나 실패 시 null)</summary>
        private DataRow GetTemplateMaster(string companyNo, string viewName)
        {
            const string query = @"
                SELECT TEMP_NO, TAG2, TAG3, EXCEL_NAME, ST_ROW, ST_COL
                FROM SCM_EXCEL_TEMP_MASTER
                WHERE COMPANY_NO = :companyNo AND UPPER(TAG1) = UPPER(:viewName)";
            try
            {
                using (var conn = new OracleConnection(ConnectionString))
                using (var cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("companyNo", companyNo));
                    cmd.Parameters.Add(new OracleParameter("viewName", viewName));
                    SqlLogger.LogCommand(cmd, "블록제작 템플릿 마스터 조회");
                    DataTable dt = Fill(cmd, "블록제작 템플릿 마스터 조회 완료");
                    return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, $"블록제작 템플릿 마스터 조회 실패: {companyNo}/{viewName}");
                return null;
            }
        }

        /// <summary>
        /// 일정 데이터 조회. tableName은 SafeIdent 통과값, dateCol은 화이트리스트 통과값("*"면 기간 조건 생략),
        /// 나머지는 전부 바인드 변수. 실패 시 null.
        /// </summary>
        private DataTable GetScheduleData(string tableName, string orderBy, string caseNo, string companyNo,
                                          string dateCol, DateTime dtStart, DateTime dtEnd)
        {
            string dateClause = dateCol == "*" ? "" : $" AND {dateCol} BETWEEN :dtStart AND :dtEnd";
            string query = $"SELECT * FROM {tableName} WHERE CASE_NO = :caseNo AND COMPANY_NO = :companyNo{dateClause} ORDER BY {orderBy}";
            try
            {
                using (var conn = new OracleConnection(ConnectionString))
                using (var cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("caseNo", caseNo));
                    cmd.Parameters.Add(new OracleParameter("companyNo", companyNo));
                    if (dateCol != "*")
                    {
                        cmd.Parameters.Add(new OracleParameter("dtStart", OracleDbType.Date) { Value = dtStart.Date });
                        // ALIS와 동일하게 종료일 00:00 기준 바인드(건수 일치 목적)
                        cmd.Parameters.Add(new OracleParameter("dtEnd", OracleDbType.Date) { Value = dtEnd.Date });
                    }
                    SqlLogger.LogCommand(cmd, $"블록제작 일정 조회 [{tableName}]");
                    return Fill(cmd, "블록제작 일정 조회 완료");
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, $"블록제작 일정 조회 실패: {tableName}/{caseNo}/{companyNo}");
                return null;
            }
        }

        /// <summary>
        /// 기준일정 컬럼 화이트리스트 검증. "*"는 그대로, 그 외는 PROP2='Y' 행 PROP_NAME과 대소문자 무시 완전일치한
        /// DB 원본 컬럼명을 반환. 불일치면 null.
        /// </summary>
        private static string ResolveDateCol(DataTable defs, string dateCol)
        {
            if (dateCol == "*") return dateCol;

            string matched = defs.AsEnumerable()
                .Where(r => r["PROP2"]?.ToString() == "Y")
                .Select(r => r["PROP_NAME"]?.ToString())
                .FirstOrDefault(p => string.Equals(p, dateCol, StringComparison.OrdinalIgnoreCase));

            // 화이트리스트 통과 후에도 문자열 치환 자리이므로 식별자 패턴 재확인
            return matched != null && SafeIdent.IsMatch(matched.ToUpperInvariant()) ? matched : null;
        }

        /// <summary>2단계(스프레드시트 셀 주입)용 메타 채우기</summary>
        private static void FillMeta(BlockScheduleResult result, DataRow master, string tableName)
        {
            result.TempNo = master["TEMP_NO"]?.ToString();
            result.TableName = tableName;
            // 블록 계열 ALIS는 TAG3를 읽지 않고 시트명 "SCHEDULE" 상수 사용 — 2단계 주입 시 TAG3 대신 상수 사용할 것
            result.SheetName = master["TAG3"]?.ToString();   // 끝 공백 유효 → Trim 금지
            result.ExcelName = master["EXCEL_NAME"]?.ToString();
            // ST_ROW 미설정 시 기본 2 (ALIS `?? 2`와 동일)
            result.StRow = master["ST_ROW"] == DBNull.Value ? 2 : Convert.ToInt32(master["ST_ROW"]);
            result.StCol = master["ST_COL"] == DBNull.Value ? 0 : Convert.ToInt32(master["ST_COL"]);
        }

        /// <summary>
        /// 정의 행 중 Data에 실재하는 컬럼만 남긴다(PROP_NAME은 DataTable 실제 컬럼명으로 정규화). 나머지는 스킵 로그.
        /// </summary>
        private static DataTable FilterExistingColumns(DataTable defs, DataTable data)
        {
            DataTable kept = defs.Clone();
            foreach (DataRow d in defs.Rows)
            {
                string prop = d["PROP_NAME"]?.ToString();
                if (!string.IsNullOrEmpty(prop) && data.Columns.Contains(prop))
                {
                    kept.ImportRow(d);
                    kept.Rows[kept.Rows.Count - 1]["PROP_NAME"] = data.Columns[prop].ColumnName;
                }
                else
                {
                    SqlLogger.LogError(new Exception($"PROP_NAME={prop}"), $"컬럼 스킵: {prop} (조회 결과에 없는 컬럼)");
                }
            }
            return kept;
        }

        private static string OrderByFor(string viewName)
        {
            return OrderByOverrides.TryGetValue(viewName ?? "", out string orderBy) ? orderBy : DefaultOrderBy;
        }

        /// <summary>커맨드 실행 → DataTable (어댑터가 연결을 열고 닫음) + 결과 로그</summary>
        private static DataTable Fill(OracleCommand cmd, string description)
        {
            using (var adapter = new OracleDataAdapter(cmd))
            {
                var dt = new DataTable();
                adapter.Fill(dt);
                SqlLogger.LogResult(dt.Rows.Count, description);
                return dt;
            }
        }
    }
}
