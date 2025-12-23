using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using DevExpress.Xpo;
using Oracle.ManagedDataAccess.Client;
using KShiftSmartPortal.Database;
using KShiftSmartPortalWeb.Models;
using KShiftSmartPortalWeb.Utils;
using KShiftSmartPortal.ViewModels;

namespace KShiftSmartPortalWeb.Controllers
{
    /// <summary>
    /// To-Do List 화면 컨트롤러 (XPO 방식)
    /// 작업지시 목록 조회 및 수정 기능을 제공합니다.
    /// 4개 테이블 조인: SCM_CASE_MASTER, STD_PERSONNEL_INFO, SCM_WORK_ORDER_MASTER, SCM_WORK_ORDER_DETAIL
    /// </summary>
    public class ToDoListController
    {
        private string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["OracleConnection"].ConnectionString; }
        }

        #region 조회 메서드 (XPO 방식)

        /// <summary>
        /// To-Do List 조회 (XPO + LINQ 방식)
        /// </summary>
        public List<ToDoListViewModel> GetToDoList(string companyNo, string userId, DateTime baseDate)
        {
            try
            {
                using (var uow = new UnitOfWork())
                {
                    // 1. D: SCM_CASE_MASTER - 활성화된 케이스 조회 (PROP1 = 'Y')
                    SqlLogger.LogXpoQuery("SCM_CASE_MASTER", "SELECT",
                        "COMPANY_NO = :companyNo AND PROP1 = 'Y'",
                        new Dictionary<string, object> { { "companyNo", companyNo } },
                        "활성화된 케이스 조회");

                    var activeCases = new XPQuery<SCM_CASE_MASTER>(uow)
                        .Where(d => d.CompoundKey1.COMPANY_NO == companyNo && d.PROP1 == "Y")
                        .ToList();

                    if (activeCases.Count == 0)
                    {
                        SqlLogger.LogResult(0, "활성화된 케이스 없음");
                        return new List<ToDoListViewModel>();
                    }

                    var activeCaseNos = activeCases.Select(d => d.CompoundKey1.CASE_NO).ToList();

                    // 2. C: STD_PERSONNEL_INFO - 사용자의 EMP_NO 조회
                    SqlLogger.LogXpoQuery("STD_PERSONNEL_INFO", "SELECT",
                        "COMPANY_NO = :companyNo AND USER_ID = :userId",
                        new Dictionary<string, object> { { "companyNo", companyNo }, { "userId", userId } },
                        "사용자 EMP_NO 조회");

                    var personnel = new XPQuery<STD_PERSONNEL_INFO>(uow)
                        .FirstOrDefault(c => c.CompoundKey1.COMPANY_NO == companyNo && c.USER_ID == userId);

                    if (personnel == null)
                    {
                        SqlLogger.LogResult(0, $"사용자 정보 없음: {userId}");
                        return new List<ToDoListViewModel>();
                    }

                    string empNo = personnel.CompoundKey1.EMP_NO;

                    // 3. B: SCM_WORK_ORDER_MASTER 조회
                    var baseDateOnly = baseDate.Date;
                    // 기준일 기준 7일 전부터 조회 (최근 등록 데이터도 보이도록)
                    var weekAgo = baseDateOnly.AddDays(-7);

                    SqlLogger.LogXpoQuery("SCM_WORK_ORDER_MASTER", "SELECT",
                        "COMPANY_NO = :companyNo AND CASE_NO IN (...) AND EMP_NO = :empNo AND (COMP_DATE IS NULL/MinValue OR WORK_ST >= :weekAgo OR (WORK_ST <= :baseDate AND WORK_FI >= :baseDate))",
                        new Dictionary<string, object> { { "companyNo", companyNo }, { "empNo", empNo }, { "baseDate", baseDateOnly }, { "weekAgo", weekAgo } },
                        "작업지시 목록 조회");

                    var workOrders = new XPQuery<SCM_WORK_ORDER_MASTER>(uow)
                        .Where(b => b.CompoundKey1.COMPANY_NO == companyNo
                                 && activeCaseNos.Contains(b.CompoundKey1.CASE_NO)
                                 && b.EMP_NO == empNo
                                 && (b.COMP_DATE == DateTime.MinValue || b.COMP_DATE == null
                                     || b.WORK_ST >= weekAgo
                                     || (b.WORK_ST <= baseDateOnly && b.WORK_FI >= baseDateOnly)))
                        .OrderByDescending(b => b.IN_DATE)
                        .ThenBy(b => b.WORK_ST)
                        .ToList();

                    // 4. XPO 엔티티를 ViewModel로 변환
                    // XPO 필드가 non-nullable인 경우 처리
                    var result = workOrders.Select(b => new ToDoListViewModel
                    {
                        COMPANY_NO = b.CompoundKey1.COMPANY_NO,
                        CASE_NO = b.CompoundKey1.CASE_NO,
                        PROJECT_NO = b.CompoundKey1.PROJECT_NO,
                        ORDER_NO = b.CompoundKey1.ORDER_NO,
                        PROP01 = b.PROP01,
                        ORDER_NAME = b.ORDER_NAME,
                        PROP02 = b.PROP02,
                        WORK_LIST = b.WORK_LIST,
                        // DateTime 필드: MinValue면 null로 변환
                        WORK_ST = b.WORK_ST == DateTime.MinValue ? (DateTime?)null : b.WORK_ST,
                        WORK_FI = b.WORK_FI == DateTime.MinValue ? (DateTime?)null : b.WORK_FI,
                        DUE_DATE = b.DUE_DATE == DateTime.MinValue ? (DateTime?)null : b.DUE_DATE,
                        QM_DATE = b.QM_DATE == DateTime.MinValue ? (DateTime?)null : b.QM_DATE,
                        COMP_DATE = b.COMP_DATE == DateTime.MinValue ? (DateTime?)null : b.COMP_DATE,
                        QM_COMP_DATE = b.QM_COMP_DATE == DateTime.MinValue ? (DateTime?)null : b.QM_COMP_DATE,
                        // decimal 필드: 그대로 할당 (0이면 0으로)
                        PLAN_MHR = b.PLAN_MHR,
                        REAL_MHR = b.REAL_MHR,
                        PLAN_MP = b.PLAN_MP,
                        REAL_MP = b.REAL_MP
                    }).ToList();

                    SqlLogger.LogResult(result.Count, "To-Do List 조회 완료 (XPO)");
                    return result;
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "To-Do List 조회 실패 (XPO)");
                throw;
            }
        }

        #endregion

        #region 저장 메서드 (XPO 방식)

        /// <summary>
        /// 작업지시 수정 저장 (XPO 방식)
        /// COMP_DATE, PLAN_MHR, REAL_MHR, PLAN_MP, REAL_MP 필드만 수정 가능
        /// </summary>
        public bool UpdateWorkOrder(string caseNo, string companyNo, string projectNo, string orderNo,
            DateTime? compDate, decimal? planMhr, decimal? realMhr, decimal? planMp, decimal? realMp,
            string userId)
        {
            try
            {
                using (var uow = new UnitOfWork())
                {
                    // XPO로 기존 데이터 조회
                    SqlLogger.LogXpoQuery("SCM_WORK_ORDER_MASTER", "SELECT",
                        "COMPANY_NO = :companyNo AND CASE_NO = :caseNo AND PROJECT_NO = :projectNo AND ORDER_NO = :orderNo",
                        new Dictionary<string, object> {
                            { "companyNo", companyNo }, { "caseNo", caseNo },
                            { "projectNo", projectNo }, { "orderNo", orderNo }
                        },
                        "작업지시 수정 대상 조회");

                    var workOrder = uow.Query<SCM_WORK_ORDER_MASTER>().FirstOrDefault(m =>
                        m.CompoundKey1.COMPANY_NO == companyNo &&
                        m.CompoundKey1.CASE_NO == caseNo &&
                        m.CompoundKey1.PROJECT_NO == projectNo &&
                        m.CompoundKey1.ORDER_NO == orderNo);

                    if (workOrder == null)
                    {
                        SqlLogger.LogResult(0, "수정 대상 없음");
                        return false;
                    }

                    // 수정 가능 필드만 업데이트
                    // nullable -> non-nullable 변환: null이면 MinValue 또는 0 사용
                    workOrder.COMP_DATE = compDate ?? DateTime.MinValue;
                    workOrder.PLAN_MHR = planMhr ?? 0;
                    workOrder.REAL_MHR = realMhr ?? 0;
                    workOrder.PLAN_MP = planMp ?? 0;
                    workOrder.REAL_MP = realMp ?? 0;

                    // 감사 필드
                    workOrder.UP_USER = userId;
                    workOrder.UP_DATE = DateTime.Now;

                    SqlLogger.LogXpoQuery("SCM_WORK_ORDER_MASTER", "UPDATE",
                        "COMPANY_NO = :companyNo AND CASE_NO = :caseNo AND PROJECT_NO = :projectNo AND ORDER_NO = :orderNo",
                        new Dictionary<string, object> {
                            { "companyNo", companyNo }, { "caseNo", caseNo },
                            { "projectNo", projectNo }, { "orderNo", orderNo },
                            { "COMP_DATE", compDate }, { "PLAN_MHR", planMhr },
                            { "REAL_MHR", realMhr }, { "PLAN_MP", planMp }, { "REAL_MP", realMp }
                        },
                        "작업지시 수정");

                    // 커밋
                    uow.CommitChanges();

                    SqlLogger.LogResult(1, "작업지시 수정 완료 (XPO)");
                    return true;
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "작업지시 수정 실패 (XPO)");
                throw;
            }
        }

        /// <summary>
        /// 일괄 저장 (BatchEdit 모드용 - XPO 방식)
        /// </summary>
        public int BatchUpdateWorkOrders(List<ToDoListViewModel> items, string userId)
        {
            int successCount = 0;

            try
            {
                using (var uow = new UnitOfWork())
                {
                    foreach (var item in items)
                    {
                        // XPO로 기존 데이터 조회
                        var workOrder = uow.Query<SCM_WORK_ORDER_MASTER>().FirstOrDefault(m =>
                            m.CompoundKey1.COMPANY_NO == item.COMPANY_NO &&
                            m.CompoundKey1.CASE_NO == item.CASE_NO &&
                            m.CompoundKey1.PROJECT_NO == item.PROJECT_NO &&
                            m.CompoundKey1.ORDER_NO == item.ORDER_NO);

                        if (workOrder != null)
                        {
                            // 수정 가능 필드만 업데이트
                            // nullable -> non-nullable 변환
                            workOrder.COMP_DATE = item.COMP_DATE ?? DateTime.MinValue;
                            workOrder.PLAN_MHR = item.PLAN_MHR ?? 0;
                            workOrder.REAL_MHR = item.REAL_MHR ?? 0;
                            workOrder.PLAN_MP = item.PLAN_MP ?? 0;
                            workOrder.REAL_MP = item.REAL_MP ?? 0;

                            // 감사 필드
                            workOrder.UP_USER = userId;
                            workOrder.UP_DATE = DateTime.Now;

                            successCount++;
                        }
                    }

                    // 일괄 커밋
                    uow.CommitChanges();

                    SqlLogger.LogResult(successCount, "일괄 저장 완료 (XPO)");
                }

                return successCount;
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "일괄 저장 실패 (XPO)");
                throw;
            }
        }

        #endregion

        #region 등록 메서드 (XPO 방식)

        /// <summary>
        /// 작업지시 신규 등록 (XPO 방식)
        /// SCM_WORK_ORDER_MASTER와 SCM_WORK_ORDER_DETAIL 동시 Insert
        /// </summary>
        public bool InsertWorkOrder(
            string companyNo, string caseNo, string projectNo,
            string orderName, string workList,
            DateTime? workSt, DateTime? workFi,
            decimal? planMhr, decimal? realMhr, decimal? planMp, decimal? realMp,
            DateTime? compDate, string rmk,
            string userId)
        {
            try
            {
                using (var uow = new UnitOfWork())
                {
                    // 1. ORDER_NO 자동 생성 (날짜 + 시퀀스)
                    string orderNo = GenerateOrderNo(uow, companyNo, caseNo, projectNo);

                    // 2. 사용자 EMP_NO 조회
                    var personnel = new XPQuery<STD_PERSONNEL_INFO>(uow)
                        .FirstOrDefault(c => c.CompoundKey1.COMPANY_NO == companyNo && c.USER_ID == userId);
                    string empNo = personnel?.CompoundKey1.EMP_NO ?? userId;

                    SqlLogger.LogXpoQuery("SCM_WORK_ORDER_MASTER", "INSERT",
                        "신규 작업지시 등록",
                        new Dictionary<string, object> {
                            { "companyNo", companyNo }, { "caseNo", caseNo },
                            { "projectNo", projectNo }, { "orderNo", orderNo },
                            { "orderName", orderName }
                        },
                        "작업지시 마스터 등록");

                    // 3. SCM_WORK_ORDER_MASTER 등록
                    var master = new SCM_WORK_ORDER_MASTER(uow);
                    master.CompoundKey1 = new SCM_WORK_ORDER_MASTER.CompoundKey1Struct
                    {
                        COMPANY_NO = companyNo,
                        CASE_NO = caseNo,
                        PROJECT_NO = projectNo,
                        ORDER_NO = orderNo
                    };
                    master.ORDER_NAME = orderName ?? "";
                    master.WORK_LIST = workList ?? "";
                    master.EMP_NO = empNo;
                    master.WORK_ST = workSt ?? DateTime.Today;
                    master.WORK_FI = workFi ?? DateTime.Today;
                    master.DUE_DATE = workFi ?? DateTime.Today;
                    master.PLAN_MHR = planMhr ?? 0;
                    master.REAL_MHR = realMhr ?? 0;
                    master.PLAN_MP = planMp ?? 0;
                    master.REAL_MP = realMp ?? 0;
                    master.COMP_DATE = compDate ?? DateTime.MinValue;
                    master.RMK = rmk ?? "";
                    // 감사 필드
                    master.IN_USER = userId;
                    master.IN_DATE = DateTime.Now;
                    master.UP_USER = userId;
                    master.UP_DATE = DateTime.Now;

                    SqlLogger.LogXpoQuery("SCM_WORK_ORDER_DETAIL", "INSERT",
                        "신규 작업지시 상세 등록",
                        new Dictionary<string, object> {
                            { "companyNo", companyNo }, { "caseNo", caseNo },
                            { "projectNo", projectNo }, { "orderNo", orderNo }
                        },
                        "작업지시 상세 등록");

                    // 4. SCM_WORK_ORDER_DETAIL 등록 (동일 Key)
                    var detail = new SCM_WORK_ORDER_DETAIL(uow);
                    detail.CompoundKey1 = new SCM_WORK_ORDER_DETAIL.CompoundKey1Struct
                    {
                        COMPANY_NO = companyNo,
                        CASE_NO = caseNo,
                        PROJECT_NO = projectNo,
                        ORDER_NO = orderNo
                    };
                    // 감사 필드
                    detail.IN_USER = userId;
                    detail.IN_DATE = DateTime.Now;
                    detail.UP_USER = userId;
                    detail.UP_DATE = DateTime.Now;

                    // 5. 커밋
                    uow.CommitChanges();

                    SqlLogger.LogResult(1, $"작업지시 등록 완료 - ORDER_NO: {orderNo}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "작업지시 등록 실패 (XPO)");
                throw;
            }
        }

        /// <summary>
        /// ORDER_NO 자동 생성 (yyyyMMdd + 3자리 시퀀스)
        /// </summary>
        private string GenerateOrderNo(UnitOfWork uow, string companyNo, string caseNo, string projectNo)
        {
            string datePrefix = DateTime.Today.ToString("yyyyMMdd");
            string pattern = datePrefix + "%";

            // 오늘 날짜로 시작하는 ORDER_NO 중 최대값 조회
            var existingOrders = new XPQuery<SCM_WORK_ORDER_MASTER>(uow)
                .Where(m => m.CompoundKey1.COMPANY_NO == companyNo
                         && m.CompoundKey1.CASE_NO == caseNo
                         && m.CompoundKey1.PROJECT_NO == projectNo
                         && m.CompoundKey1.ORDER_NO.StartsWith(datePrefix))
                .Select(m => m.CompoundKey1.ORDER_NO)
                .ToList();

            int nextSeq = 1;
            if (existingOrders.Any())
            {
                // 기존 시퀀스 중 최대값 + 1
                var maxSeq = existingOrders
                    .Select(o => {
                        if (o.Length > 8 && int.TryParse(o.Substring(8), out int seq))
                            return seq;
                        return 0;
                    })
                    .Max();
                nextSeq = maxSeq + 1;
            }

            return $"{datePrefix}{nextSeq:D3}";
        }

        /// <summary>
        /// 프로젝트 목록 조회 (활성화된 케이스 기반)
        /// </summary>
        public DataTable GetProjectList(string companyNo)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT DISTINCT M.PROJECT_NO, M.PROP01 AS PROJECT_NAME
                        FROM SCM_WORK_ORDER_MASTER M
                        INNER JOIN SCM_CASE_MASTER C
                            ON M.COMPANY_NO = C.COMPANY_NO AND M.CASE_NO = C.CASE_NO
                        WHERE M.COMPANY_NO = :companyNo
                          AND C.PROP1 = 'Y'
                        ORDER BY M.PROJECT_NO";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(":companyNo", companyNo);
                        SqlLogger.LogCommand(cmd, "프로젝트 목록 조회");

                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "프로젝트 목록 조회 실패");
                throw;
            }
        }

        /// <summary>
        /// 활성화된 케이스 목록 조회
        /// </summary>
        public DataTable GetActiveCaseList(string companyNo)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT CASE_NO, CASE_NAME
                        FROM SCM_CASE_MASTER
                        WHERE COMPANY_NO = :companyNo
                          AND PROP1 = 'Y'
                        ORDER BY CASE_NO";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(":companyNo", companyNo);
                        SqlLogger.LogCommand(cmd, "활성 케이스 목록 조회");

                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "활성 케이스 목록 조회 실패");
                throw;
            }
        }

        #endregion

        #region 삭제 메서드 (XPO 방식)

        /// <summary>
        /// 작업지시 삭제 (XPO 방식)
        /// </summary>
        public bool DeleteWorkOrder(string caseNo, string companyNo, string projectNo, string orderNo)
        {
            try
            {
                using (var uow = new UnitOfWork())
                {
                    var whereParams = new Dictionary<string, object> {
                        { "companyNo", companyNo }, { "caseNo", caseNo },
                        { "projectNo", projectNo }, { "orderNo", orderNo }
                    };

                    // 상세 조회 및 삭제
                    SqlLogger.LogXpoQuery("SCM_WORK_ORDER_DETAIL", "SELECT",
                        "COMPANY_NO = :companyNo AND CASE_NO = :caseNo AND PROJECT_NO = :projectNo AND ORDER_NO = :orderNo",
                        whereParams, "작업지시 상세 조회 (삭제용)");

                    var detail = uow.Query<SCM_WORK_ORDER_DETAIL>().FirstOrDefault(d =>
                        d.CompoundKey1.COMPANY_NO == companyNo &&
                        d.CompoundKey1.CASE_NO == caseNo &&
                        d.CompoundKey1.PROJECT_NO == projectNo &&
                        d.CompoundKey1.ORDER_NO == orderNo);

                    if (detail != null)
                    {
                        SqlLogger.LogXpoQuery("SCM_WORK_ORDER_DETAIL", "DELETE",
                            "COMPANY_NO = :companyNo AND CASE_NO = :caseNo AND PROJECT_NO = :projectNo AND ORDER_NO = :orderNo",
                            whereParams, "작업지시 상세 삭제");
                        uow.Delete(detail);
                    }

                    // 마스터 조회 및 삭제
                    SqlLogger.LogXpoQuery("SCM_WORK_ORDER_MASTER", "SELECT",
                        "COMPANY_NO = :companyNo AND CASE_NO = :caseNo AND PROJECT_NO = :projectNo AND ORDER_NO = :orderNo",
                        whereParams, "작업지시 마스터 조회 (삭제용)");

                    var master = uow.Query<SCM_WORK_ORDER_MASTER>().FirstOrDefault(m =>
                        m.CompoundKey1.COMPANY_NO == companyNo &&
                        m.CompoundKey1.CASE_NO == caseNo &&
                        m.CompoundKey1.PROJECT_NO == projectNo &&
                        m.CompoundKey1.ORDER_NO == orderNo);

                    if (master == null)
                    {
                        SqlLogger.LogResult(0, "삭제 대상 없음");
                        return false;
                    }

                    SqlLogger.LogXpoQuery("SCM_WORK_ORDER_MASTER", "DELETE",
                        "COMPANY_NO = :companyNo AND CASE_NO = :caseNo AND PROJECT_NO = :projectNo AND ORDER_NO = :orderNo",
                        whereParams, "작업지시 마스터 삭제");

                    uow.Delete(master);
                    uow.CommitChanges();

                    SqlLogger.LogResult(1, "작업지시 삭제 완료 (XPO)");
                    return true;
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "작업지시 삭제 실패 (XPO)");
                throw;
            }
        }

        #endregion

        #region 공통 메서드

        /// <summary>
        /// Company 목록 조회 (Oracle 직접 조회)
        /// </summary>
        public DataTable GetCompanyList()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT COMPANY_NO, COMPANY_NAME, COMPANY_TYPE
                        FROM STD_COMPANY_MASTER
                        WHERE USE_YN = 'Y'
                        ORDER BY COMPANY_NO";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        SqlLogger.LogCommand(cmd, "Company 목록 조회");

                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "Company 목록 조회 실패");
                throw;
            }
        }

        #endregion
    }
}