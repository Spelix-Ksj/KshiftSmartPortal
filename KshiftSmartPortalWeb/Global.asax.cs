using System;
using System.Web;
using DevExpress.Web;

namespace KShiftSmartPortalWeb
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // 애플리케이션 시작 시 초기화
        }

        /// <summary>
        /// 매 요청 전에 테마 적용 (핵심!)
        /// </summary>
        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            // 세션이 없으면 스킵
            if (HttpContext.Current.Session == null)
                return;

            // 세션에서 테마 가져오기
            string theme = HttpContext.Current.Session["AppTheme"] as string;

            // 세션에 없으면 쿠키에서 가져오기
            if (string.IsNullOrEmpty(theme))
            {
                HttpCookie themeCookie = HttpContext.Current.Request.Cookies["AppTheme"];
                if (themeCookie != null)
                {
                    theme = themeCookie.Value;
                }
            }

            // 기본 테마
            if (string.IsNullOrEmpty(theme))
            {
                theme = "Office365";
            }

            // DevExpress 전역 테마 설정
            ASPxWebControl.GlobalTheme = theme;
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // 세션 시작 시 쿠키에서 테마 로드
            HttpCookie themeCookie = HttpContext.Current.Request.Cookies["AppTheme"];
            if (themeCookie != null)
            {
                HttpContext.Current.Session["AppTheme"] = themeCookie.Value;
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // 오류 처리
        }

        protected void Session_End(object sender, EventArgs e)
        {
            // 세션 종료
        }

        protected void Application_End(object sender, EventArgs e)
        {
            // 애플리케이션 종료
        }
    }
}