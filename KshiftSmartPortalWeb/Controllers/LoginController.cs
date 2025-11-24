using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Text.RegularExpressions;

namespace KShiftSmartPortalWeb.Controllers
{
    /// <summary>
    /// 로그인 비즈니스 로직을 담당하는 컨트롤러
    /// </summary>
    public class LoginController
    {
        private const int MAX_LOGIN_FAIL_COUNT = 5;

        private string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["OracleConnection"].ConnectionString;
            }
        }

        /// <summary>
        /// 회사 목록 조회
        /// </summary>
        public DataTable GetCompanyList()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT COMPANY_NO, COMPANY_NAME 
                        FROM STD_COMPANY_MASTER 
                        WHERE NVL(USE_YN, 'Y') = 'Y' 
                        ORDER BY COMPANY_TYPE, VIEW_ORDER";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
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
                throw new Exception($"회사 목록 조회 실패: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 사용자 인증
        /// </summary>
        public LoginResult AuthenticateUser(string companyNo, string userId, string password)
        {
            LoginResult result = new LoginResult();

            // 입력값 검증
            if (!IsValidUserId(userId))
            {
                result.Success = false;
                result.Message = "아이디 형식이 올바르지 않습니다.";
                return result;
            }

            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT USER_ID, DOMAIN_CATEGORY, IS_BLOCKED, BLOCKED_REASON, 
                               NVL(TAG05, '0') AS FAIL_COUNT
                        FROM TCM_USER 
                        WHERE COMPANY_NO = :COMPANY_NO 
                          AND USER_ID = :USER_ID 
                          AND USER_PW = :USER_PW";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2, 128).Value = companyNo;
                        cmd.Parameters.Add("USER_ID", OracleDbType.Varchar2, 50).Value = userId;
                        cmd.Parameters.Add("USER_PW", OracleDbType.Varchar2, 100).Value = password;

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // 계정 잠금 확인
                                string isBlocked = GetSafeString(reader, "IS_BLOCKED");
                                if (isBlocked == "Y")
                                {
                                    string blockReason = GetSafeString(reader, "BLOCKED_REASON");
                                    result.Success = false;
                                    result.Message = $"계정이 잠겨 있습니다. 사유: {blockReason}";
                                    return result;
                                }

                                // 로그인 성공
                                result.Success = true;
                                result.UserId = userId;
                                result.CompanyNo = companyNo;
                                result.DomainCategory = GetSafeString(reader, "DOMAIN_CATEGORY");

                                // 실패 횟수 초기화
                                UpdateLoginFailCount(userId, companyNo, 0);

                                return result;
                            }
                            else
                            {
                                // 로그인 실패
                                int failCount = IncrementLoginFailCount(userId, companyNo);

                                if (failCount >= MAX_LOGIN_FAIL_COUNT)
                                {
                                    BlockUser(userId, companyNo, $"비밀번호 {MAX_LOGIN_FAIL_COUNT}회 이상 실패");
                                    result.Message = $"로그인 {MAX_LOGIN_FAIL_COUNT}회 실패로 계정이 잠겼습니다. 관리자에게 문의하세요.";
                                }
                                else
                                {
                                    result.Message = $"로그인 실패 ({failCount}/{MAX_LOGIN_FAIL_COUNT}회). 아이디 또는 암호를 확인하세요.";
                                }

                                result.Success = false;
                                return result;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"로그인 처리 중 오류 발생: {ex.Message}";
                return result;
            }
        }

        /// <summary>
        /// 아이디 유효성 검증 (SQL Injection 방지)
        /// </summary>
        private bool IsValidUserId(string userId)
        {
            // 영문, 숫자, _, - 만 허용 (3-50자)
            return Regex.IsMatch(userId, @"^[a-zA-Z0-9_-]{3,50}$");
        }

        /// <summary>
        /// 로그인 실패 횟수 증가
        /// </summary>
        private int IncrementLoginFailCount(string userId, string companyNo)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string selectQuery = @"
                        SELECT NVL(TAG05, '0') AS FAIL_COUNT 
                        FROM TCM_USER 
                        WHERE COMPANY_NO = :COMPANY_NO 
                          AND USER_ID = :USER_ID";

                    int failCount = 0;

                    using (OracleCommand selectCmd = new OracleCommand(selectQuery, conn))
                    {
                        selectCmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2, 128).Value = companyNo;
                        selectCmd.Parameters.Add("USER_ID", OracleDbType.Varchar2, 50).Value = userId;

                        object result = selectCmd.ExecuteScalar();
                        if (result != null)
                        {
                            int.TryParse(result.ToString(), out failCount);
                        }
                    }

                    failCount++;
                    UpdateLoginFailCount(userId, companyNo, failCount);

                    return failCount;
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 로그인 실패 횟수 업데이트
        /// </summary>
        private void UpdateLoginFailCount(string userId, string companyNo, int failCount)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string updateQuery = @"
                        UPDATE TCM_USER 
                        SET TAG05 = :FAIL_COUNT 
                        WHERE COMPANY_NO = :COMPANY_NO 
                          AND USER_ID = :USER_ID";

                    using (OracleCommand cmd = new OracleCommand(updateQuery, conn))
                    {
                        cmd.Parameters.Add("FAIL_COUNT", OracleDbType.Varchar2, 10).Value = failCount.ToString();
                        cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2, 128).Value = companyNo;
                        cmd.Parameters.Add("USER_ID", OracleDbType.Varchar2, 50).Value = userId;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // 실패 횟수 업데이트 실패는 무시
            }
        }

        /// <summary>
        /// 사용자 계정 잠금
        /// </summary>
        private void BlockUser(string userId, string companyNo, string reason)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string updateQuery = @"
                        UPDATE TCM_USER 
                        SET IS_BLOCKED = 'Y', 
                            BLOCKED_REASON = :REASON,
                            TAG05 = :FAIL_COUNT
                        WHERE COMPANY_NO = :COMPANY_NO 
                          AND USER_ID = :USER_ID";

                    using (OracleCommand cmd = new OracleCommand(updateQuery, conn))
                    {
                        cmd.Parameters.Add("REASON", OracleDbType.Varchar2, 200).Value = reason;
                        cmd.Parameters.Add("FAIL_COUNT", OracleDbType.Varchar2, 10).Value = MAX_LOGIN_FAIL_COUNT.ToString();
                        cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2, 128).Value = companyNo;
                        cmd.Parameters.Add("USER_ID", OracleDbType.Varchar2, 50).Value = userId;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // 계정 잠금 실패는 무시
            }
        }

        /// <summary>
        /// 안전한 문자열 읽기
        /// </summary>
        private string GetSafeString(OracleDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// 로그인 결과 모델
    /// </summary>
    public class LoginResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
        public string CompanyNo { get; set; }
        public string DomainCategory { get; set; }
    }
}
