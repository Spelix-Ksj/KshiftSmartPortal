using System;
using System.Web.UI;
using DevExpress.Web;

namespace KShiftSmartPortalWeb.Views
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 세션 체크
            if (Session["UserID"] == null)
            {
                // 콜백 요청 중에는 Response.Redirect 사용 불가
                if (Page is ICallbackEventHandler || Request["__CALLBACKID"] != null)
                {
                    ASPxWebControl.RedirectOnCallback("~/Views/Login.aspx");
                }
                else
                {
                    Response.Redirect("~/Views/Login.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                return;
            }

            if (!IsPostBack)
            {
                // 사용자 정보 표시
                if (Session["UserID"] != null)
                {
                    lblUserName.Text = Session["UserID"].ToString();
                }
            }
        }
    }
}
