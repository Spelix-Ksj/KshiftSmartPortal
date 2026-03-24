using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using KShiftSmartPortal.ViewModels;
using KShiftSmartPortalWeb.Utils;

namespace KShiftSmartPortalWeb.Controllers
{
    /// <summary>
    /// 조직 관리 화면 컨트롤러
    /// STD_OBS_MASTER 테이블의 CRUD 기능을 제공합니다.
    /// </summary>
    public class OrganizationManagerController : BaseController
    {
        /// <summary>
        /// 해당 회사의 조직 전체 목록을 조회합니다.
        /// </summary>
        /// <param name="companyNo">회사번호</param>
        /// <returns>조직 목록</returns>
        public List<OrganizationViewModel> GetOrganizationList(string companyNo)
        {
            var result = new List<OrganizationViewModel>();

            try
            {
                using (var conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT COMPANY_NO, ORG_NO, ORG_NAME, PARENT_OBS_NO,
                               OBS_TYPE, OBS_LEVEL, USE_YN, VIEW_ORDER, RMK,
                               IN_USER, IN_DATE, UP_USER, UP_DATE
                        FROM STD_OBS_MASTER
                        WHERE COMPANY_NO = :companyNo
                        ORDER BY OBS_LEVEL, VIEW_ORDER";

                    using (var cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("companyNo", companyNo));

                        SqlLogger.LogCommand(cmd, "조직 전체 목록 조회");

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(MapReaderToViewModel(reader));
                            }
                        }
                    }
                }

                SqlLogger.LogResult(result.Count, "조직 목록 조회 완료");
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "조직 전체 목록 조회 실패");
            }

            return result;
        }

        /// <summary>
        /// 특정 조직 1건을 조회합니다. (복합 PK)
        /// </summary>
        /// <param name="companyNo">회사번호</param>
        /// <param name="orgNo">조직번호</param>
        /// <returns>조직 정보 (없으면 null)</returns>
        public OrganizationViewModel GetOrganizationByKey(string companyNo, string orgNo)
        {
            try
            {
                using (var conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT COMPANY_NO, ORG_NO, ORG_NAME, PARENT_OBS_NO,
                               OBS_TYPE, OBS_LEVEL, USE_YN, VIEW_ORDER, RMK,
                               IN_USER, IN_DATE, UP_USER, UP_DATE
                        FROM STD_OBS_MASTER
                        WHERE COMPANY_NO = :companyNo AND ORG_NO = :orgNo";

                    using (var cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("companyNo", companyNo));
                        cmd.Parameters.Add(new OracleParameter("orgNo", orgNo));

                        SqlLogger.LogCommand(cmd, "조직 단건 조회");

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
                SqlLogger.LogError(ex, $"조직 단건 조회 실패: {companyNo}/{orgNo}");
            }

            return null;
        }

        /// <summary>
        /// 조직을 신규 등록합니다.
        /// </summary>
        /// <param name="model">조직 정보</param>
        /// <param name="userId">등록자 ID</param>
        /// <returns>성공 여부</returns>
        public bool InsertOrganization(OrganizationViewModel model, string userId)
        {
            try
            {
                // 중복 체크
                if (IsOrgNoDuplicate(model.CompanyNo, model.OrgNo))
                {
                    return false;
                }

                using (var conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        INSERT INTO STD_OBS_MASTER (
                            COMPANY_NO, ORG_NO, ORG_NAME, PARENT_OBS_NO,
                            OBS_TYPE, OBS_LEVEL, USE_YN, VIEW_ORDER, RMK,
                            IN_USER, IN_DATE
                        ) VALUES (
                            :companyNo, :orgNo, :orgName, :parentObsNo,
                            :obsType, :obsLevel, :useYn, :viewOrder, :rmk,
                            :inUser, SYSDATE
                        )";

                    using (var cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("companyNo", model.CompanyNo ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("orgNo", model.OrgNo ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("orgName", model.OrgName ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("parentObsNo", model.ParentObsNo ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("obsType", model.ObsType ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("obsLevel", model.ObsLevel));
                        cmd.Parameters.Add(new OracleParameter("useYn", model.UseYn ?? "Y"));
                        cmd.Parameters.Add(new OracleParameter("viewOrder", model.ViewOrder));
                        cmd.Parameters.Add(new OracleParameter("rmk", model.Rmk ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("inUser", userId ?? (object)DBNull.Value));

                        SqlLogger.LogCommand(cmd, "조직 신규 등록");

                        int affected = cmd.ExecuteNonQuery();
                        SqlLogger.LogResult(affected, "조직 등록 완료");

                        return affected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, $"조직 등록 실패: {model.CompanyNo}/{model.OrgNo}");
                return false;
            }
        }

        /// <summary>
        /// 조직 정보를 수정합니다.
        /// </summary>
        /// <param name="model">조직 정보</param>
        /// <param name="userId">수정자 ID</param>
        /// <returns>성공 여부</returns>
        public bool UpdateOrganization(OrganizationViewModel model, string userId)
        {
            try
            {
                using (var conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        UPDATE STD_OBS_MASTER SET
                            ORG_NAME = :orgName,
                            PARENT_OBS_NO = :parentObsNo,
                            OBS_TYPE = :obsType,
                            OBS_LEVEL = :obsLevel,
                            USE_YN = :useYn,
                            VIEW_ORDER = :viewOrder,
                            RMK = :rmk,
                            UP_USER = :upUser,
                            UP_DATE = SYSDATE
                        WHERE COMPANY_NO = :companyNo AND ORG_NO = :orgNo";

                    using (var cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("orgName", model.OrgName ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("parentObsNo", model.ParentObsNo ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("obsType", model.ObsType ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("obsLevel", model.ObsLevel));
                        cmd.Parameters.Add(new OracleParameter("useYn", model.UseYn ?? "Y"));
                        cmd.Parameters.Add(new OracleParameter("viewOrder", model.ViewOrder));
                        cmd.Parameters.Add(new OracleParameter("rmk", model.Rmk ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("upUser", userId ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("companyNo", model.CompanyNo));
                        cmd.Parameters.Add(new OracleParameter("orgNo", model.OrgNo));

                        SqlLogger.LogCommand(cmd, "조직 정보 수정");

                        int affected = cmd.ExecuteNonQuery();
                        SqlLogger.LogResult(affected, "조직 수정 완료");

                        return affected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, $"조직 수정 실패: {model.CompanyNo}/{model.OrgNo}");
                return false;
            }
        }

        /// <summary>
        /// 조직을 삭제합니다.
        /// </summary>
        /// <param name="companyNo">회사번호</param>
        /// <param name="orgNo">조직번호</param>
        /// <returns>성공 여부</returns>
        public bool DeleteOrganization(string companyNo, string orgNo)
        {
            try
            {
                using (var conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = "DELETE FROM STD_OBS_MASTER WHERE COMPANY_NO = :companyNo AND ORG_NO = :orgNo";

                    using (var cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("companyNo", companyNo));
                        cmd.Parameters.Add(new OracleParameter("orgNo", orgNo));

                        SqlLogger.LogCommand(cmd, "조직 삭제");

                        int affected = cmd.ExecuteNonQuery();
                        SqlLogger.LogResult(affected, "조직 삭제 완료");

                        return affected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, $"조직 삭제 실패: {companyNo}/{orgNo}");
                return false;
            }
        }

        /// <summary>
        /// 조직번호 중복 여부를 확인합니다. (복합 PK)
        /// </summary>
        /// <param name="companyNo">회사번호</param>
        /// <param name="orgNo">조직번호</param>
        /// <returns>중복이면 true</returns>
        public bool IsOrgNoDuplicate(string companyNo, string orgNo)
        {
            try
            {
                using (var conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = "SELECT COUNT(*) FROM STD_OBS_MASTER WHERE COMPANY_NO = :companyNo AND ORG_NO = :orgNo";

                    using (var cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("companyNo", companyNo));
                        cmd.Parameters.Add(new OracleParameter("orgNo", orgNo));

                        SqlLogger.LogCommand(cmd, "조직번호 중복 체크");

                        object result = cmd.ExecuteScalar();
                        int count = Convert.ToInt32(result);

                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, $"조직번호 중복 체크 실패: {companyNo}/{orgNo}");
                return false;
            }
        }

        #region Private Helper

        /// <summary>
        /// OracleDataReader를 OrganizationViewModel로 매핑합니다.
        /// </summary>
        private OrganizationViewModel MapReaderToViewModel(OracleDataReader reader)
        {
            return new OrganizationViewModel
            {
                CompanyNo = reader["COMPANY_NO"] as string,
                OrgNo = reader["ORG_NO"] as string,
                OrgName = reader["ORG_NAME"] as string,
                ParentObsNo = reader["PARENT_OBS_NO"] as string,
                ObsType = reader["OBS_TYPE"] as string,
                ObsLevel = reader["OBS_LEVEL"] != DBNull.Value ? Convert.ToInt32(reader["OBS_LEVEL"]) : 0,
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
