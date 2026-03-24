using System;
using System.Web;
using System.Web.UI;
using DevExpress.Web;

namespace KShiftSmartPortalWeb
{
    /// <summary>
    /// 모든 업무 페이지의 기본 클래스.
    /// 세션 관리, 메시지 표시 등 공통 기능을 제공합니다.
    /// Login.aspx, Logout.aspx 등 인증이 필요 없는 페이지는 상속하지 않습니다.
    /// </summary>
    public abstract class BasePage : Page
    {
        /// <summary>
        /// 세션에서 현재 로그인한 사용자 ID를 가져옵니다.
        /// </summary>
        protected string CurrentUserId
        {
            get { return Session["UserID"]?.ToString(); }
        }

        /// <summary>
        /// 세션에서 현재 사용자의 회사번호를 가져옵니다.
        /// </summary>
        protected string CurrentCompanyNo
        {
            get { return Session["CompanyNo"]?.ToString(); }
        }

        /// <summary>
        /// 세션이 유효한지 확인합니다.
        /// 유효하지 않으면 로그인 페이지로 리디렉트합니다.
        /// </summary>
        /// <returns>세션이 유효하면 true, 리디렉트했으면 false</returns>
        protected bool CheckSession()
        {
            if (Session["UserID"] == null)
            {
                if (IsCallback)
                {
                    ASPxWebControl.RedirectOnCallback("~/Views/Login.aspx");
                }
                else
                {
                    Response.Redirect("~/Views/Login.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// JavaScript alert으로 메시지를 표시합니다.
        /// XSS 방지를 위해 JavaScriptStringEncode를 사용합니다.
        /// </summary>
        protected void ShowMessage(string message)
        {
            string script = $"alert({HttpUtility.JavaScriptStringEncode(message, addDoubleQuotes: true)});";
            ScriptManager.RegisterStartupScript(this, GetType(), "alertScript", script, true);
        }

        /// <summary>
        /// DevExpress GridView 콜백 모드에서 메시지를 표시합니다.
        /// 클라이언트 측에서 cpMessage를 읽어 alert으로 표시해야 합니다.
        /// </summary>
        protected void ShowMessageCallback(ASPxGridView grid, string message)
        {
            grid.JSProperties["cpMessage"] = message;
        }

        /// <summary>
        /// 세션이 만료된 경우 메시지를 표시하고 로그인 페이지로 리디렉트합니다.
        /// 버튼 클릭 핸들러 등에서 사용합니다.
        /// </summary>
        protected bool EnsureAuthenticated()
        {
            if (Session["UserID"] == null)
            {
                string loginUrl = ResolveUrl("~/Views/Login.aspx");
                string script = $"alert({HttpUtility.JavaScriptStringEncode("세션이 만료되었습니다. 다시 로그인해주세요.", addDoubleQuotes: true)}); window.location.href='{loginUrl}';";
                ScriptManager.RegisterStartupScript(this, GetType(), "sessionExpired", script, true);
                return false;
            }
            return true;
        }
    }
}
