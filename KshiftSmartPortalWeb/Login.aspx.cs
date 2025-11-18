using System;
using System.Data;
using System.Web;
using System.Web.UI;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Text.RegularExpressions;

namespace ScmBlockContractWeb
{
    public partial class Login : System.Web.UI.Page
    {
        // 로그인 실패 제한 횟수
        private const int MAX_LOGIN_FAIL_COUNT = 5;

        private string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["OracleConnection"].ConnectionString;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 이미 로그인되어 있으면 메인 페이지로 리디렉션
                if (Session["UserID"] != null)
                {
                    Response.Redirect("Default.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                LoadCompanyList();
                SetFocusOnLoad();
            }
        }

        /// <summary>
        /// 회사 목록 로드
        /// </summary>
        private void LoadCompanyList()
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

                            cmbCompany.DataSource = dt;
                            cmbCompany.DataBind();

                            // 기본값 설정 (1002 - SPELIX)
                            if (dt.Rows.Count > 0)
                            {
                                var defaultRow = dt.Select("COMPANY_NO = '1002'");
                                if (defaultRow.Length > 0)
                                {
                                    cmbCompany.Value = "1002";
                                }
                                else
                                {
                                    cmbCompany.SelectedIndex = 0;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"회사 목록 로드 실패: {ex.Message}");
            }
        }

        /// <summary>
        /// 로그인 버튼 클릭 이벤트
        /// </summary>
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                // 입력값 검증
                if (!ValidateInput())
                {
                    return;
                }

                string companyNo = cmbCompany.Value?.ToString();
                string userId = txtUserId.Text.Trim();
                string password = txtPassword.Text;

                // SQL Injection 방지 - 입력값 검증
                if (!IsValidUserId(userId))
                {
                    ShowError("아이디 형식이 올바르지 않습니다.");
                    return;
                }

                // 로그인 처리
                if (AuthenticateUser(companyNo, userId, password))
                {
                    // 로그인 성공
                    Session["UserID"] = userId;
                    Session["CompanyNo"] = companyNo;
                    Session["LoginTime"] = DateTime.Now;
                    Session.Timeout = 30; // 30분

                    // 로그인 실패 횟수 초기화
                    UpdateLoginFailCount(userId, companyNo, 0);

                    // 메인 페이지로 리디렉션
                    // Response.Redirect("Default.aspx");
                    Response.Redirect("Default.aspx", false);  // ✅ 예외 없음
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
                else
                {
                    // 로그인 실패
                    int failCount = IncrementLoginFailCount(userId, companyNo);

                    if (failCount >= MAX_LOGIN_FAIL_COUNT)
                    {
                        // 계정 잠금
                        BlockUser(userId, companyNo, $"비밀번호 {MAX_LOGIN_FAIL_COUNT}회 이상 실패");
                        ShowError($"로그인 {MAX_LOGIN_FAIL_COUNT}회 실패로 계정이 잠겼습니다. 관리자에게 문의하세요.");
                    }
                    else
                    {
                        ShowError($"로그인 실패 ({failCount}/{MAX_LOGIN_FAIL_COUNT}회). 아이디 또는 암호를 확인하세요.");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"로그인 처리 중 오류 발생: {ex.Message}");
            }
        }

        /// <summary>
        /// 입력값 검증
        /// </summary>
        private bool ValidateInput()
        {
            if (cmbCompany.Value == null || string.IsNullOrEmpty(cmbCompany.Value.ToString()))
            {
                ShowError("회사를 선택하세요.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtUserId.Text))
            {
                ShowError("아이디를 입력하세요.");
                txtUserId.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                ShowError("암호를 입력하세요.");
                txtPassword.Focus();
                return false;
            }

            return true;
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
        /// 사용자 인증
        /// </summary>
        private bool AuthenticateUser(string companyNo, string userId, string password)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    // ⭐ SQL Injection 방지: 파라미터화된 쿼리 사용
                    string query = @"
                        SELECT USER_ID, DOMAIN_CATEGORY, IS_BLOCKED, BLOCKED_REASON 
                        FROM TCM_USER 
                        WHERE COMPANY_NO = :COMPANY_NO 
                          AND USER_ID = :USER_ID 
                          AND USER_PW = :USER_PW";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        // 파라미터 바인딩
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
                                    ShowError($"계정이 잠겨 있습니다. 사유: {blockReason}");
                                    return false;
                                }

                                // 세션에 추가 정보 저장
                                Session["DomainCategory"] = GetSafeString(reader, "DOMAIN_CATEGORY");

                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"사용자 인증 실패: {ex.Message}", ex);
            }

            return false;
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

                    // 현재 실패 횟수 조회
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

                    // 실패 횟수 증가
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

        /// <summary>
        /// 오류 메시지 표시
        /// </summary>
        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;
        }

        /// <summary>
        /// 페이지 로드 시 포커스 설정
        /// </summary>
        private void SetFocusOnLoad()
        {
            txtUserId.Focus();
        }
    }
}
