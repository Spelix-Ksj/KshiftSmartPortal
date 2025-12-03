using System;
using System.Data;
using System.Web;
using System.Web.UI;
using KShiftSmartPortalWeb.Controllers;

namespace KShiftSmartPortalWeb
{
    public partial class Login : System.Web.UI.Page
    {
        private LoginController _loginController = new LoginController();

        protected void Page_PreInit(object sender, EventArgs e)
        {
            // 테마는 Page_Load가 아닌 Page_PreInit에서 설정해야 합니다
            // 하지만 DevExpress 컨트롤은 동적으로 설정 가능
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 이미 로그인되어 있으면 메인 페이지로 리디렉션
                if (Session["UserID"] != null)
                {
                    Response.Redirect("Home.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                // 저장된 테마 로드 및 적용
                LoadAndApplyTheme();

                LoadCompanyList();
                SetFocusOnLoad();
            }
        }

        /// <summary>
        /// 테마 로드 및 적용
        /// </summary>
        private void LoadAndApplyTheme()
        {
            string theme = "Office365"; // 기본 테마

            // 쿠키에서 테마 가져오기
            if (Request.Cookies["AppTheme"] != null)
            {
                theme = Request.Cookies["AppTheme"].Value;
                cmbTheme.Value = theme;
            }
            else
            {
                cmbTheme.Value = "Office365"; // 기본 테마
            }

            // DevExpress 컨트롤에 테마 적용
            ApplyDevExpressTheme(theme);
        }

        /// <summary>
        /// DevExpress 컨트롤에 테마 적용
        /// </summary>
        private void ApplyDevExpressTheme(string theme)
        {
            if (cmbCompany != null) cmbCompany.Theme = theme;
            if (txtUserId != null) txtUserId.Theme = theme;
            if (txtPassword != null) txtPassword.Theme = theme;
            if (btnLogin != null) btnLogin.Theme = theme;
            if (cmbTheme != null) cmbTheme.Theme = theme;
        }

        /// <summary>
        /// 테마 선택 변경 이벤트
        /// </summary>
        protected void cmbTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTheme.Value != null)
            {
                string selectedTheme = cmbTheme.Value.ToString();

                // 쿠키에 테마 저장 (30일)
                HttpCookie themeCookie = new HttpCookie("AppTheme", selectedTheme);
                themeCookie.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Add(themeCookie);

                // 세션에도 저장
                Session["AppTheme"] = selectedTheme;

                // 페이지 리로드하여 Global.asax에서 테마 적용
                Response.Redirect(Request.RawUrl, false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        /// <summary>
        /// 회사 목록 로드
        /// </summary>
        private void LoadCompanyList()
        {
            try
            {
                DataTable dt = _loginController.GetCompanyList();

                cmbCompany.DataSource = dt;
                cmbCompany.DataBind();

                // 기본값 설정 (1002 - SPELIX)
                if (dt.Rows.Count > 0)
                {
                    var defaultRow = dt.Select("COMPANY_NO = '100'");
                    if (defaultRow.Length > 0)
                    {
                        cmbCompany.Value = "100";
                    }
                    else
                    {
                        cmbCompany.SelectedIndex = 0;
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

                // 로그인 처리 (Controller 사용)
                LoginResult result = _loginController.AuthenticateUser(companyNo, userId, password);

                if (result.Success)
                {
                    // 로그인 성공
                    Session["UserID"] = result.UserId;
                    Session["CompanyNo"] = result.CompanyNo;
                    Session["DomainCategory"] = result.DomainCategory;
                    Session["LoginTime"] = DateTime.Now;
                    Session.Timeout = 30; // 30분

                    // 테마 세션 설정
                    if (cmbTheme.Value != null)
                    {
                        Session["AppTheme"] = cmbTheme.Value.ToString();
                    }

                    // 메인 바로가기 페이지로 리디렉션
                    Response.Redirect("Home.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    // 로그인 실패
                    ShowError(result.Message);
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
        /// 오류 메시지 표시
        /// </summary>
        private void ShowError(string message)
        {
            pnlError.Visible = true;
            pnlError.CssClass = "error-panel visible";
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