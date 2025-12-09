using System;
using System.Data;
using System.Web.UI;
using KShiftSmartPortalWeb.Controllers;

namespace KShiftSmartPortalWeb
{
    public partial class Login : System.Web.UI.Page
    {
        private LoginController _loginController = new LoginController();

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
                DataTable dt = _loginController.GetCompanyList();

                cmbCompany.DataSource = dt;
                cmbCompany.DataBind();

                // 기본값 설정 (100 - SPELIX)
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
                ShowError("Failed to load company list: " + ex.Message);
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

                string companyNo = cmbCompany.Value != null ? cmbCompany.Value.ToString() : null;
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
                ShowError("Login error: " + ex.Message);
            }
        }

        /// <summary>
        /// 입력값 검증
        /// </summary>
        private bool ValidateInput()
        {
            if (cmbCompany.Value == null || string.IsNullOrEmpty(cmbCompany.Value.ToString()))
            {
                ShowError("Please select a company.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtUserId.Text))
            {
                ShowError("Please enter ID.");
                txtUserId.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                ShowError("Please enter password.");
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
            pnlError.CssClass = "error-box show";
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
