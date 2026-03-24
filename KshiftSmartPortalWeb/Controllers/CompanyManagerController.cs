using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using KShiftSmartPortal.ViewModels;
using KShiftSmartPortalWeb.Utils;

namespace KShiftSmartPortalWeb.Controllers
{
    /// <summary>
    /// 고객사 관리 화면 컨트롤러
    /// STD_COMPANY_MASTER 테이블의 CRUD 기능을 제공합니다.
    /// </summary>
    public class CompanyManagerController : BaseController
    {
        /// <summary>
        /// 고객사 전체 목록을 조회합니다.
        /// </summary>
        /// <param name="companyType">회사 유형 ("*"이면 전체)</param>
        /// <returns>고객사 목록</returns>
        public List<CompanyViewModel> GetCompanyFullList(string companyType)
        {
            var result = new List<CompanyViewModel>();

            try
            {
                using (var conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT COMPANY_NO, COMPANY_NAME, COMPANY_SH_NAME, COMPANY_ENG_NAME,
                               COMPANY_TYPE, USE_YN, REPRESENTATIVES, PROP1,
                               TEL_NO, FAX_NO, LOCATION, SPEC_INFO,
                               CAL_ID,
                               VIEW_ORDER, RMK,
                               IN_USER, IN_DATE, UP_USER, UP_DATE
                        FROM STD_COMPANY_MASTER";

                    if (companyType != "*")
                    {
                        query += " WHERE COMPANY_TYPE = :companyType";
                    }

                    query += " ORDER BY COMPANY_TYPE, VIEW_ORDER";

                    using (var cmd = new OracleCommand(query, conn))
                    {
                        if (companyType != "*")
                        {
                            cmd.Parameters.Add(new OracleParameter("companyType", companyType));
                        }

                        SqlLogger.LogCommand(cmd, "고객사 전체 목록 조회");

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(MapReaderToViewModel(reader));
                            }
                        }
                    }
                }

                SqlLogger.LogResult(result.Count, "고객사 목록 조회 완료");
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "고객사 전체 목록 조회 실패");
            }

            return result;
        }

        /// <summary>
        /// 특정 고객사 1건을 조회합니다.
        /// </summary>
        /// <param name="companyNo">고객사 번호</param>
        /// <returns>고객사 정보 (없으면 null)</returns>
        public CompanyViewModel GetCompanyByNo(string companyNo)
        {
            try
            {
                using (var conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT COMPANY_NO, COMPANY_NAME, COMPANY_SH_NAME, COMPANY_ENG_NAME,
                               COMPANY_TYPE, USE_YN, REPRESENTATIVES, PROP1,
                               TEL_NO, FAX_NO, LOCATION, SPEC_INFO,
                               CAL_ID,
                               VIEW_ORDER, RMK,
                               IN_USER, IN_DATE, UP_USER, UP_DATE
                        FROM STD_COMPANY_MASTER
                        WHERE COMPANY_NO = :companyNo";

                    using (var cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("companyNo", companyNo));

                        SqlLogger.LogCommand(cmd, "고객사 단건 조회");

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
                SqlLogger.LogError(ex, $"고객사 단건 조회 실패: {companyNo}");
            }

            return null;
        }

        /// <summary>
        /// 고객사를 신규 등록합니다.
        /// </summary>
        /// <param name="model">고객사 정보</param>
        /// <param name="userId">등록자 ID</param>
        /// <returns>성공 여부</returns>
        public bool InsertCompany(CompanyViewModel model, string userId)
        {
            try
            {
                // 중복 체크
                if (IsCompanyNoDuplicate(model.CompanyNo))
                {
                    return false;
                }

                using (var conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        INSERT INTO STD_COMPANY_MASTER (
                            COMPANY_NO, COMPANY_NAME, COMPANY_SH_NAME, COMPANY_ENG_NAME,
                            COMPANY_TYPE, USE_YN, REPRESENTATIVES, PROP1,
                            TEL_NO, FAX_NO, LOCATION, SPEC_INFO,
                            VIEW_ORDER, RMK,
                            IN_USER, IN_DATE
                        ) VALUES (
                            :companyNo, :companyName, :companyShName, :companyEngName,
                            :companyType, :useYn, :representatives, :prop1,
                            :telNo, :faxNo, :location, :specInfo,
                            :viewOrder, :rmk,
                            :inUser, SYSDATE
                        )";

                    using (var cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("companyNo", model.CompanyNo ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("companyName", model.CompanyName ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("companyShName", model.CompanyShName ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("companyEngName", model.CompanyEngName ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("companyType", model.CompanyType ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("useYn", model.UseYn ?? "Y"));
                        cmd.Parameters.Add(new OracleParameter("representatives", model.Representatives ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("prop1", model.Prop1 ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("telNo", model.TelNo ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("faxNo", model.FaxNo ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("location", model.Location ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("specInfo", model.SpecInfo ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("viewOrder", model.ViewOrder));
                        cmd.Parameters.Add(new OracleParameter("rmk", model.Rmk ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("inUser", userId ?? (object)DBNull.Value));

                        SqlLogger.LogCommand(cmd, "고객사 신규 등록");

                        int affected = cmd.ExecuteNonQuery();
                        SqlLogger.LogResult(affected, "고객사 등록 완료");

                        return affected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, $"고객사 등록 실패: {model.CompanyNo}");
                return false;
            }
        }

        /// <summary>
        /// 고객사 정보를 수정합니다.
        /// </summary>
        /// <param name="model">고객사 정보</param>
        /// <param name="userId">수정자 ID</param>
        /// <returns>성공 여부</returns>
        public bool UpdateCompany(CompanyViewModel model, string userId)
        {
            try
            {
                using (var conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        UPDATE STD_COMPANY_MASTER SET
                            COMPANY_NAME = :companyName,
                            COMPANY_SH_NAME = :companyShName,
                            COMPANY_ENG_NAME = :companyEngName,
                            COMPANY_TYPE = :companyType,
                            USE_YN = :useYn,
                            REPRESENTATIVES = :representatives,
                            PROP1 = :prop1,
                            TEL_NO = :telNo,
                            FAX_NO = :faxNo,
                            LOCATION = :location,
                            SPEC_INFO = :specInfo,
                            VIEW_ORDER = :viewOrder,
                            RMK = :rmk,
                            UP_USER = :upUser,
                            UP_DATE = SYSDATE
                        WHERE COMPANY_NO = :companyNo";

                    using (var cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("companyName", model.CompanyName ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("companyShName", model.CompanyShName ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("companyEngName", model.CompanyEngName ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("companyType", model.CompanyType ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("useYn", model.UseYn ?? "Y"));
                        cmd.Parameters.Add(new OracleParameter("representatives", model.Representatives ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("prop1", model.Prop1 ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("telNo", model.TelNo ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("faxNo", model.FaxNo ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("location", model.Location ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("specInfo", model.SpecInfo ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("viewOrder", model.ViewOrder));
                        cmd.Parameters.Add(new OracleParameter("rmk", model.Rmk ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("upUser", userId ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("companyNo", model.CompanyNo));

                        SqlLogger.LogCommand(cmd, "고객사 정보 수정");

                        int affected = cmd.ExecuteNonQuery();
                        SqlLogger.LogResult(affected, "고객사 수정 완료");

                        return affected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, $"고객사 수정 실패: {model.CompanyNo}");
                return false;
            }
        }

        /// <summary>
        /// 고객사를 삭제합니다.
        /// </summary>
        /// <param name="companyNo">고객사 번호</param>
        /// <returns>성공 여부</returns>
        public bool DeleteCompany(string companyNo)
        {
            try
            {
                using (var conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = "DELETE FROM STD_COMPANY_MASTER WHERE COMPANY_NO = :companyNo";

                    using (var cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("companyNo", companyNo));

                        SqlLogger.LogCommand(cmd, "고객사 삭제");

                        int affected = cmd.ExecuteNonQuery();
                        SqlLogger.LogResult(affected, "고객사 삭제 완료");

                        return affected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, $"고객사 삭제 실패: {companyNo}");
                return false;
            }
        }

        /// <summary>
        /// 고객사 번호 중복 여부를 확인합니다.
        /// </summary>
        /// <param name="companyNo">고객사 번호</param>
        /// <returns>중복이면 true</returns>
        public bool IsCompanyNoDuplicate(string companyNo)
        {
            try
            {
                using (var conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = "SELECT COUNT(*) FROM STD_COMPANY_MASTER WHERE COMPANY_NO = :companyNo";

                    using (var cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("companyNo", companyNo));

                        SqlLogger.LogCommand(cmd, "고객사 번호 중복 체크");

                        object result = cmd.ExecuteScalar();
                        int count = Convert.ToInt32(result);

                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, $"고객사 번호 중복 체크 실패: {companyNo}");
                return false;
            }
        }

        #region Private Helper

        /// <summary>
        /// OracleDataReader를 CompanyViewModel로 매핑합니다.
        /// </summary>
        private CompanyViewModel MapReaderToViewModel(OracleDataReader reader)
        {
            return new CompanyViewModel
            {
                CompanyNo = reader["COMPANY_NO"] as string,
                CompanyName = reader["COMPANY_NAME"] as string,
                CompanyShName = reader["COMPANY_SH_NAME"] as string,
                CompanyEngName = reader["COMPANY_ENG_NAME"] as string,
                CompanyType = reader["COMPANY_TYPE"] as string,
                UseYn = reader["USE_YN"] as string,
                Representatives = reader["REPRESENTATIVES"] as string,
                Prop1 = reader["PROP1"] as string,
                TelNo = reader["TEL_NO"] as string,
                FaxNo = reader["FAX_NO"] as string,
                Location = reader["LOCATION"] as string,
                SpecInfo = reader["SPEC_INFO"] as string,
                CalId = reader["CAL_ID"] as string,
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
