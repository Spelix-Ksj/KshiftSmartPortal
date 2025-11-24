using System;
using System.Diagnostics;
using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace KShiftSmartPortalWeb.Utils
{
    /// <summary>
    /// SQL 쿼리 로깅 유틸리티
    /// </summary>
    public static class SqlLogger
    {
        /// <summary>
        /// OracleCommand의 전체 쿼리 정보를 출력창에 표시
        /// </summary>
        /// <param name="cmd">OracleCommand 객체</param>
        /// <param name="description">쿼리 설명 (선택사항)</param>
        public static void LogCommand(OracleCommand cmd, string description = null)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("=".PadRight(80, '='));

                if (!string.IsNullOrEmpty(description))
                {
                    sb.AppendLine($"[SQL 쿼리] {description}");
                    sb.AppendLine("-".PadRight(80, '-'));
                }

                // 원본 쿼리
                sb.AppendLine("원본 쿼리:");
                sb.AppendLine(cmd.CommandText);
                sb.AppendLine();

                // 파라메터 정보
                if (cmd.Parameters.Count > 0)
                {
                    sb.AppendLine("파라메터:");
                    foreach (OracleParameter param in cmd.Parameters)
                    {
                        string paramValue = param.Value == null || param.Value == DBNull.Value
                            ? "NULL"
                            : $"'{param.Value}'";
                        sb.AppendLine($"  {param.ParameterName} = {paramValue}");
                    }
                    sb.AppendLine();
                }

                // 실행 가능한 쿼리 (파라메터가 치환된 형태)
                sb.AppendLine("실행 쿼리 (파라메터 치환됨):");
                string executableQuery = GetExecutableQuery(cmd);
                sb.AppendLine(executableQuery);
                sb.AppendLine("=".PadRight(80, '='));

                // 출력창에 표시
                Debug.WriteLine(sb.ToString());

                // 콘솔에도 출력 (ASP.NET 개발 서버 로그에 표시됨)
                Console.WriteLine(sb.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SQL 로깅 중 오류 발생: {ex.Message}");
            }
        }

        /// <summary>
        /// 파라메터가 치환된 실행 가능한 쿼리 문자열 생성
        /// </summary>
        /// <param name="cmd">OracleCommand 객체</param>
        /// <returns>실행 가능한 쿼리 문자열</returns>
        private static string GetExecutableQuery(OracleCommand cmd)
        {
            string query = cmd.CommandText;

            foreach (OracleParameter param in cmd.Parameters)
            {
                string paramValue;

                if (param.Value == null || param.Value == DBNull.Value)
                {
                    paramValue = "NULL";
                }
                else
                {
                    switch (param.OracleDbType)
                    {
                        case OracleDbType.Varchar2:
                        case OracleDbType.NVarchar2:
                        case OracleDbType.Char:
                        case OracleDbType.NChar:
                            paramValue = $"'{param.Value.ToString().Replace("'", "''")}'";
                            break;

                        case OracleDbType.Date:
                        case OracleDbType.TimeStamp:
                            paramValue = param.Value.ToString() == "SYSDATE"
                                ? "SYSDATE"
                                : $"TO_DATE('{param.Value}', 'YYYY-MM-DD HH24:MI:SS')";
                            break;

                        case OracleDbType.Int32:
                        case OracleDbType.Int64:
                        case OracleDbType.Decimal:
                        case OracleDbType.Double:
                            paramValue = param.Value.ToString();
                            break;

                        default:
                            paramValue = $"'{param.Value}'";
                            break;
                    }
                }

                // 파라메터 이름을 값으로 치환
                query = query.Replace($":{param.ParameterName}", paramValue);
            }

            return query;
        }

        /// <summary>
        /// 쿼리 실행 결과 로깅
        /// </summary>
        /// <param name="affectedRows">영향받은 행 수</param>
        /// <param name="description">설명</param>
        public static void LogResult(int affectedRows, string description = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("-".PadRight(80, '-'));

            if (!string.IsNullOrEmpty(description))
            {
                sb.AppendLine($"[실행 결과] {description}");
            }

            sb.AppendLine($"영향받은 행 수: {affectedRows}");
            sb.AppendLine("=".PadRight(80, '='));

            Debug.WriteLine(sb.ToString());
            Console.WriteLine(sb.ToString());
        }

        /// <summary>
        /// 에러 로깅
        /// </summary>
        /// <param name="ex">예외 객체</param>
        /// <param name="description">설명</param>
        public static void LogError(Exception ex, string description = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("!".PadRight(80, '!'));
            sb.AppendLine("[SQL 에러]");

            if (!string.IsNullOrEmpty(description))
            {
                sb.AppendLine($"설명: {description}");
            }

            sb.AppendLine($"에러 메시지: {ex.Message}");
            sb.AppendLine($"스택 트레이스: {ex.StackTrace}");
            sb.AppendLine("!".PadRight(80, '!'));

            Debug.WriteLine(sb.ToString());
            Console.WriteLine(sb.ToString());
        }
    }
}
