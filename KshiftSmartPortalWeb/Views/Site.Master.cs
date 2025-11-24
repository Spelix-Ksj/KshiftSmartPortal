using System;
using System.Web.UI;

namespace KShiftSmartPortalWeb.Views
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 세션 체크
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
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
