using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Services;

namespace KShiftSmartPortalWeb
{
    public partial class ChatBot : System.Web.UI.Page
    {
        private const string API_BASE_URL = "https://hq.spelix.co.kr";

        static ChatBot()
        {
            // TLS 1.2 지원 및 자체서명 인증서 허용
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (s, c, ch, e) => true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 외부 API 프록시 — 브라우저 CORS 우회용
        /// </summary>
        [WebMethod]
        public static string ProxyApi(string endpoint, string method, string body, string token)
        {
            try
            {
                var url = API_BASE_URL + endpoint;
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = method ?? "GET";
                request.ContentType = "application/json; charset=utf-8";
                request.Timeout = 120000;

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Add("Authorization", "Bearer " + token);
                }

                if ((method == "POST") && !string.IsNullOrEmpty(body))
                {
                    var bytes = Encoding.UTF8.GetBytes(body);
                    request.ContentLength = bytes.Length;
                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(bytes, 0, bytes.Length);
                    }
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse errorResponse)
                {
                    using (var reader = new StreamReader(errorResponse.GetResponseStream(), Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
                throw new Exception("API 서버 연결 실패: " + ex.Message);
            }
        }
    }
}
