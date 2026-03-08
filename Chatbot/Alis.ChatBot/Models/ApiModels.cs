using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Alis.ChatBot.Models
{
    public class LoginRequest
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("expiresAt")]
        public DateTime ExpiresAt { get; set; }
    }

    public class AskRequest
    {
        [JsonProperty("question")]
        public string Question { get; set; }

        [JsonProperty("modelKey")]
        public string ModelKey { get; set; }
    }

    public class AskResponse
    {
        [JsonProperty("sql")]
        public string Sql { get; set; }

        [JsonProperty("reasoning")]
        public string Reasoning { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }

    public class ExecuteRequest
    {
        [JsonProperty("sql")]
        public string Sql { get; set; }

        [JsonProperty("question")]
        public string Question { get; set; }

        [JsonProperty("reasoning")]
        public string Reasoning { get; set; }

        [JsonProperty("modelKey")]
        public string ModelKey { get; set; }
    }

    public class ExecuteResponse
    {
        [JsonProperty("columns")]
        public List<string> Columns { get; set; }

        [JsonProperty("rows")]
        public List<List<object>> Rows { get; set; }

        [JsonProperty("rowCount")]
        public int RowCount { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("report")]
        public string Report { get; set; }
    }
}
