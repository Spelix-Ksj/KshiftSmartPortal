using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using KShiftSmartPortal.ViewModels;
using KShiftSmartPortalWeb.Utils;

namespace KShiftSmartPortalWeb.Controllers
{
    /// <summary>
    /// 리소스 관리 화면 컨트롤러
    /// SCM_RESOURCE_MASTER 테이블의 CRUD 기능을 제공합니다.
    /// </summary>
    public class ResourceManagerController : BaseController
    {
        /// <summary>
        /// 해당 케이스+회사의 리소스 전체 목록을 조회합니다.
        /// </summary>
        /// <param name="caseNo">케이스번호</param>
        /// <param name="companyNo">회사번호</param>
        /// <returns>리소스 목록</returns>
        public List<ResourceViewModel> GetResourceList(string caseNo, string companyNo)
        {
            var result = new List<ResourceViewModel>();

            try
            {
                using (var conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT CASE_NO, COMPANY_NO, RESOURCE_NO, RESOURCE_NAME,
                               RESOURCE_TYPE, PARENT_RESOURCE_NO, ORG_NO,
                               RES_LEVEL, USE_YN, VIEW_ORDER, RMK,
                               IN_USER, IN_DATE, UP_USER, UP_DATE
                        FROM SCM_RESOURCE_MASTER
                        START WITH (PARENT_RESOURCE_NO IS NULL OR PARENT_RESOURCE_NO = '')
                               AND CASE_NO = :caseNo AND COMPANY_NO = :companyNo
                        CONNECT BY NOCYCLE PRIOR RESOURCE_NO = PARENT_RESOURCE_NO
                               AND CASE_NO = :caseNo2 AND COMPANY_NO = :companyNo2
                        ORDER SIBLINGS BY VIEW_ORDER, RESOURCE_NO";

                    using (var cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("caseNo", caseNo));
                        cmd.Parameters.Add(new OracleParameter("companyNo", companyNo));
                        cmd.Parameters.Add(new OracleParameter("caseNo2", caseNo));
                        cmd.Parameters.Add(new OracleParameter("companyNo2", companyNo));

                        SqlLogger.LogCommand(cmd, "리소스 전체 목록 조회");

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(MapReaderToViewModel(reader));
                            }
                        }
                    }
                }

                SqlLogger.LogResult(result.Count, "리소스 목록 조회 완료");
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "리소스 전체 목록 조회 실패");
            }

            return result;
        }

        /// <summary>
        /// 특정 리소스 1건을 조회합니다. (3-컬럼 복합 PK)
        /// </summary>
        /// <param name="caseNo">케이스번호</param>
        /// <param name="companyNo">회사번호</param>
        /// <param name="resourceNo">리소스번호</param>
        /// <returns>리소스 정보 (없으면 null)</returns>
        public ResourceViewModel GetResourceByKey(string caseNo, string companyNo, string resourceNo)
        {
            try
            {
                using (var conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT CASE_NO, COMPANY_NO, RESOURCE_NO, RESOURCE_NAME,
                               RESOURCE_TYPE, PARENT_RESOURCE_NO, ORG_NO,
                               RES_LEVEL, USE_YN, VIEW_ORDER, RMK,
                               IN_USER, IN_DATE, UP_USER, UP_DATE
                        FROM SCM_RESOURCE_MASTER
                        WHERE CASE_NO = :caseNo AND COMPANY_NO = :companyNo AND RESOURCE_NO = :resourceNo";

                    using (var cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("caseNo", caseNo));
                        cmd.Parameters.Add(new OracleParameter("companyNo", companyNo));
                        cmd.Parameters.Add(new OracleParameter("resourceNo", resourceNo));

                        SqlLogger.LogCommand(cmd, "리소스 단건 조회");

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapReaderToViewModel(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, $"리소스 단건 조회 실패: {caseNo}/{companyNo}/{resourceNo}");
            }

            return null;
        }

        /// <summary>
        /// 리소스를 신규 등록합니다.
        /// </summary>
        /// <param name="model">리소스 정보</param>
        /// <param name="userId">등록자 ID</param>
        /// <returns>성공 여부</returns>
        public bool InsertResource(ResourceViewModel model, string userId)
        {
            try
            {
                // 중복 체크
                if (IsResourceNoDuplicate(model.CaseNo, model.CompanyNo, model.ResourceNo))
                {
                    return false;
                }

                using (var conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        INSERT INTO SCM_RESOURCE_MASTER (
                            CASE_NO, COMPANY_NO, RESOURCE_NO, RESOURCE_NAME,
                            RESOURCE_TYPE, PARENT_RESOURCE_NO, ORG_NO,
                            RES_LEVEL, USE_YN, VIEW_ORDER, RMK,
                            IN_USER, IN_DATE
                        ) VALUES (
                            :caseNo, :companyNo, :resourceNo, :resourceName,
                            :resourceType, :parentResourceNo, :orgNo,
                            :resLevel, :useYn, :viewOrder, :rmk,
                            :inUser, SYSDATE
                        )";

                    using (var cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("caseNo", model.CaseNo ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("companyNo", model.CompanyNo ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("resourceNo", model.ResourceNo ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("resourceName", model.ResourceName ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("resourceType", model.ResourceType ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("parentResourceNo", model.ParentResourceNo ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("orgNo", model.OrgNo ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("resLevel", model.ResLevel));
                        cmd.Parameters.Add(new OracleParameter("useYn", model.UseYn ?? "Y"));
                        cmd.Parameters.Add(new OracleParameter("viewOrder", model.ViewOrder));
                        cmd.Parameters.Add(new OracleParameter("rmk", model.Rmk ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("inUser", userId ?? (object)DBNull.Value));

                        SqlLogger.LogCommand(cmd, "리소스 신규 등록");

                        int affected = cmd.ExecuteNonQuery();
                        SqlLogger.LogResult(affected, "리소스 등록 완료");

                        return affected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, $"리소스 등록 실패: {model.CaseNo}/{model.CompanyNo}/{model.ResourceNo}");
                return false;
            }
        }

        /// <summary>
        /// 리소스 정보를 수정합니다.
        /// </summary>
        /// <param name="model">리소스 정보</param>
        /// <param name="userId">수정자 ID</param>
        /// <returns>성공 여부</returns>
        public bool UpdateResource(ResourceViewModel model, string userId)
        {
            try
            {
                using (var conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        UPDATE SCM_RESOURCE_MASTER SET
                            RESOURCE_NAME = :resourceName,
                            RESOURCE_TYPE = :resourceType,
                            PARENT_RESOURCE_NO = :parentResourceNo,
                            ORG_NO = :orgNo,
                            RES_LEVEL = :resLevel,
                            USE_YN = :useYn,
                            VIEW_ORDER = :viewOrder,
                            RMK = :rmk,
                            UP_USER = :upUser,
                            UP_DATE = SYSDATE
                        WHERE CASE_NO = :caseNo AND COMPANY_NO = :companyNo AND RESOURCE_NO = :resourceNo";

                    using (var cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("resourceName", model.ResourceName ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("resourceType", model.ResourceType ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("parentResourceNo", model.ParentResourceNo ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("orgNo", model.OrgNo ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("resLevel", model.ResLevel));
                        cmd.Parameters.Add(new OracleParameter("useYn", model.UseYn ?? "Y"));
                        cmd.Parameters.Add(new OracleParameter("viewOrder", model.ViewOrder));
                        cmd.Parameters.Add(new OracleParameter("rmk", model.Rmk ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("upUser", userId ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("caseNo", model.CaseNo));
                        cmd.Parameters.Add(new OracleParameter("companyNo", model.CompanyNo));
                        cmd.Parameters.Add(new OracleParameter("resourceNo", model.ResourceNo));

                        SqlLogger.LogCommand(cmd, "리소스 정보 수정");

                        int affected = cmd.ExecuteNonQuery();
                        SqlLogger.LogResult(affected, "리소스 수정 완료");

                        return affected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, $"리소스 수정 실패: {model.CaseNo}/{model.CompanyNo}/{model.ResourceNo}");
                return false;
            }
        }

        /// <summary>
        /// 리소스를 삭제합니다.
        /// </summary>
        /// <param name="caseNo">케이스번호</param>
        /// <param name="companyNo">회사번호</param>
        /// <param name="resourceNo">리소스번호</param>
        /// <returns>성공 여부</returns>
        public bool DeleteResource(string caseNo, string companyNo, string resourceNo)
        {
            try
            {
                using (var conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = "DELETE FROM SCM_RESOURCE_MASTER WHERE CASE_NO = :caseNo AND COMPANY_NO = :companyNo AND RESOURCE_NO = :resourceNo";

                    using (var cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("caseNo", caseNo));
                        cmd.Parameters.Add(new OracleParameter("companyNo", companyNo));
                        cmd.Parameters.Add(new OracleParameter("resourceNo", resourceNo));

                        SqlLogger.LogCommand(cmd, "리소스 삭제");

                        int affected = cmd.ExecuteNonQuery();
                        SqlLogger.LogResult(affected, "리소스 삭제 완료");

                        return affected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, $"리소스 삭제 실패: {caseNo}/{companyNo}/{resourceNo}");
                return false;
            }
        }

        /// <summary>
        /// 리소스번호 중복 여부를 확인합니다. (3-컬럼 복합 PK)
        /// </summary>
        /// <param name="caseNo">케이스번호</param>
        /// <param name="companyNo">회사번호</param>
        /// <param name="resourceNo">리소스번호</param>
        /// <returns>중복이면 true</returns>
        public bool IsResourceNoDuplicate(string caseNo, string companyNo, string resourceNo)
        {
            try
            {
                using (var conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = "SELECT COUNT(*) FROM SCM_RESOURCE_MASTER WHERE CASE_NO = :caseNo AND COMPANY_NO = :companyNo AND RESOURCE_NO = :resourceNo";

                    using (var cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("caseNo", caseNo));
                        cmd.Parameters.Add(new OracleParameter("companyNo", companyNo));
                        cmd.Parameters.Add(new OracleParameter("resourceNo", resourceNo));

                        SqlLogger.LogCommand(cmd, "리소스번호 중복 체크");

                        object result = cmd.ExecuteScalar();
                        int count = Convert.ToInt32(result);

                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, $"리소스번호 중복 체크 실패: {caseNo}/{companyNo}/{resourceNo}");
                return false;
            }
        }

        /// <summary>
        /// 케이스 목록을 조회합니다. (SCM_CASE_MASTER에서 해당 회사의 전체 케이스)
        /// </summary>
        /// <param name="companyNo">회사번호</param>
        /// <returns>케이스 목록 DataTable (CASE_NO, CASE_NAME)</returns>
        public DataTable GetCaseList(string companyNo)
        {
            try
            {
                using (var conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT CASE_NO, CASE_NAME
                        FROM SCM_CASE_MASTER
                        WHERE COMPANY_NO = :companyNo
                        ORDER BY CASE_NO";

                    using (var cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("companyNo", companyNo));

                        SqlLogger.LogCommand(cmd, "케이스 목록 조회");

                        using (var adapter = new OracleDataAdapter(cmd))
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
                SqlLogger.LogError(ex, $"케이스 목록 조회 실패: {companyNo}");
                return new DataTable();
            }
        }

        #region Private Helper

        /// <summary>
        /// OracleDataReader를 ResourceViewModel로 매핑합니다.
        /// </summary>
        private ResourceViewModel MapReaderToViewModel(OracleDataReader reader)
        {
            return new ResourceViewModel
            {
                CaseNo = reader["CASE_NO"] as string,
                CompanyNo = reader["COMPANY_NO"] as string,
                ResourceNo = reader["RESOURCE_NO"] as string,
                ResourceName = reader["RESOURCE_NAME"] as string,
                ResourceType = reader["RESOURCE_TYPE"] as string,
                ParentResourceNo = reader["PARENT_RESOURCE_NO"] as string,
                OrgNo = reader["ORG_NO"] as string,
                ResLevel = reader["RES_LEVEL"] != DBNull.Value ? Convert.ToInt32(reader["RES_LEVEL"]) : 0,
                UseYn = reader["USE_YN"] as string,
                ViewOrder = reader["VIEW_ORDER"] != DBNull.Value ? Convert.ToDouble(reader["VIEW_ORDER"]) : 0,
                Rmk = reader["RMK"] as string,
                InUser = reader["IN_USER"] as string,
                InDate = reader["IN_DATE"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["IN_DATE"]) : null,
                UpUser = reader["UP_USER"] as string,
                UpDate = reader["UP_DATE"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["UP_DATE"]) : null
            };
        }

        #endregion
    }
}
