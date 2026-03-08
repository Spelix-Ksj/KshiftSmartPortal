using System;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Alis.ChatBot.Models
{
    public class GatewayApiClient
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private readonly string _baseUrl;
        private string _token;
        private DateTime _tokenExpiry;

        static GatewayApiClient()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            // 내부망 자체서명 인증서 허용
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, errors) => true;
            _httpClient.Timeout = TimeSpan.FromSeconds(120);
        }

        public GatewayApiClient(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
        }

        public bool IsLoggedIn
        {
            get { return !string.IsNullOrEmpty(_token) && DateTime.UtcNow < _tokenExpiry; }
        }

        /// <summary>
        /// 로그인하여 JWT 토큰을 획득합니다.
        /// </summary>
        public async Task<bool> LoginAsync(string userId, string password)
        {
            try
            {
                var request = new LoginRequest { UserId = userId, Password = password };
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_baseUrl + "/api/auth/login", content);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return false;
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseJson);

                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    _token = loginResponse.Token;
                    _tokenExpiry = loginResponse.ExpiresAt.ToUniversalTime();
                    return true;
                }

                return false;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        /// <summary>
        /// 토큰이 유효한지 확인합니다. 만료 시 예외를 발생시킵니다.
        /// </summary>
        private void EnsureToken()
        {
            if (string.IsNullOrEmpty(_token) || DateTime.UtcNow >= _tokenExpiry.AddMinutes(-5))
            {
                throw new InvalidOperationException("토큰이 만료되었습니다. 다시 로그인하세요.");
            }
        }

        /// <summary>
        /// 자연어 질문으로 SQL을 생성합니다.
        /// </summary>
        public async Task<AskResponse> AskAsync(string question, string modelKey = null)
        {
            try
            {
                EnsureToken();

                var request = new AskRequest { Question = question, ModelKey = modelKey };
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, _baseUrl + "/api/chat/ask");
                httpRequest.Content = content;
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                var response = await _httpClient.SendAsync(httpRequest);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AskResponse>(responseJson);
            }
            catch (HttpRequestException ex)
            {
                return new AskResponse { Error = "HTTP 오류: " + ex.Message };
            }
            catch (TaskCanceledException)
            {
                return new AskResponse { Error = "요청 시간이 초과되었습니다." };
            }
            catch (Exception ex)
            {
                return new AskResponse { Error = "오류: " + ex.Message };
            }
        }

        /// <summary>
        /// SQL을 실행하고 결과를 반환합니다.
        /// </summary>
        public async Task<ExecuteResponse> ExecuteAsync(string sql, string question, string reasoning = "", string modelKey = null)
        {
            try
            {
                EnsureToken();

                var request = new ExecuteRequest
                {
                    Sql = sql,
                    Question = question,
                    Reasoning = reasoning,
                    ModelKey = modelKey
                };
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, _baseUrl + "/api/chat/execute");
                httpRequest.Content = content;
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                var response = await _httpClient.SendAsync(httpRequest);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ExecuteResponse>(responseJson);
            }
            catch (HttpRequestException ex)
            {
                return new ExecuteResponse { Error = "HTTP 오류: " + ex.Message };
            }
            catch (TaskCanceledException)
            {
                return new ExecuteResponse { Error = "요청 시간이 초과되었습니다." };
            }
            catch (Exception ex)
            {
                return new ExecuteResponse { Error = "오류: " + ex.Message };
            }
        }

        /// <summary>
        /// 서버 헬스체크를 수행합니다.
        /// </summary>
        public async Task<bool> CheckHealthAsync()
        {
            try
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, _baseUrl + "/api/chat/health");

                if (!string.IsNullOrEmpty(_token))
                {
                    httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                }

                var response = await _httpClient.SendAsync(httpRequest);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 로그아웃하고 토큰을 초기화합니다.
        /// </summary>
        public void Logout()
        {
            _token = null;
            _tokenExpiry = DateTime.MinValue;
        }

        /// <summary>
        /// ExecuteResponse를 DataTable로 변환합니다.
        /// </summary>
        public static DataTable ToDataTable(ExecuteResponse response)
        {
            var table = new DataTable();

            if (response == null || response.Columns == null || response.Rows == null)
                return table;

            // 컬럼 생성
            foreach (var column in response.Columns)
            {
                table.Columns.Add(column, typeof(string));
            }

            // 행 추가
            foreach (var row in response.Rows)
            {
                var dataRow = table.NewRow();
                for (int i = 0; i < row.Count && i < response.Columns.Count; i++)
                {
                    dataRow[i] = row[i] != null ? row[i].ToString() : string.Empty;
                }
                table.Rows.Add(dataRow);
            }

            return table;
        }
    }
}
