using System;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using KShiftSmartPortalWeb.Utils;

namespace KShiftSmartPortalWeb.Controllers
{
    /// <summary>
    /// 모든 Controller의 기본 클래스.
    /// Oracle 연결 관리, 공통 데이터 조회 기능을 제공합니다.
    /// </summary>
    public abstract class BaseController
    {
        protected string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["OracleConnection"].ConnectionString; }
        }

        /// <summary>
        /// 회사 목록을 조회합니다. (STD_COMPANY_MASTER)
        /// </summary>
        public DataTable GetCompanyList()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT COMPANY_NO, COMPANY_NAME, COMPANY_TYPE
                        FROM STD_COMPANY_MASTER
                        WHERE NVL(USE_YN, 'Y') = 'Y'
                        ORDER BY COMPANY_TYPE, VIEW_ORDER";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "회사 목록 조회 실패");
                return new DataTable();
            }
        }
    }
}
