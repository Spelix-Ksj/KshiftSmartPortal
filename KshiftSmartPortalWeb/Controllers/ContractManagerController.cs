using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using KShiftSmartPortalWeb.Models;
using KShiftSmartPortalWeb.Utils;

namespace KShiftSmartPortalWeb.Controllers
{
    /// <summary>
    /// 계약정보 관리 컨트롤러
    /// </summary>
    public class ContractManagerController
    {
        private string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["OracleConnection"].ConnectionString; }
        }

        #region 케이스 목록 조회

        /// <summary>
        /// 회사별 케이스 목록 조회
        /// </summary>
        public DataTable GetCaseList(string companyNo)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT CASE_NO, CASE_NAME, COMPANY_NO, VIEW_ORDER
                        FROM SCM_CASE_MASTER
                        WHERE COMPANY_NO = :COMPANY_NO
                        ORDER BY VIEW_ORDER";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2).Value = companyNo;

                        SqlLogger.LogCommand(cmd, "케이스 목록 조회");

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
                SqlLogger.LogError(ex, "케이스 목록 조회 실패");
                throw;
            }
        }

        #endregion

        #region 계약 마스터 조회

        /// <summary>
        /// 계약 마스터 목록 조회 (전체)
        /// </summary>
        public List<ScmContractMaster> GetContractMasterList(string caseNo, string companyNo)
        {
            List<ScmContractMaster> result = new List<ScmContractMaster>();

            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT *
                        FROM SCM_CONTRACT_MASTER
                        WHERE CASE_NO = :CASE_NO
                          AND COMPANY_NO = :COMPANY_NO
                        ORDER BY CONTRACT_ID";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("CASE_NO", OracleDbType.Varchar2).Value = caseNo;
                        cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2).Value = companyNo;

                        SqlLogger.LogCommand(cmd, "계약 마스터 조회 (전체)");

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(MapMasterReaderToModel(reader));
                            }
                        }

                        SqlLogger.LogResult(result.Count, "계약 마스터 조회 완료");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "계약 마스터 조회 실패");
                throw;
            }
        }

        /// <summary>
        /// 계약 마스터 목록 조회 (기간 조건)
        /// </summary>
        public List<ScmContractMaster> GetContractMasterList(string caseNo, string companyNo, DateTime dtStart, DateTime dtEnd)
        {
            List<ScmContractMaster> result = new List<ScmContractMaster>();

            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT *
                        FROM SCM_CONTRACT_MASTER
                        WHERE CASE_NO = :CASE_NO
                          AND COMPANY_NO = :COMPANY_NO
                          AND CNTR_DT BETWEEN :DT_START AND :DT_END
                        ORDER BY CONTRACT_ID";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("CASE_NO", OracleDbType.Varchar2).Value = caseNo;
                        cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2).Value = companyNo;
                        cmd.Parameters.Add("DT_START", OracleDbType.Date).Value = dtStart;
                        cmd.Parameters.Add("DT_END", OracleDbType.Date).Value = dtEnd;

                        SqlLogger.LogCommand(cmd, "계약 마스터 조회 (기간)");

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(MapMasterReaderToModel(reader));
                            }
                        }

                        SqlLogger.LogResult(result.Count, "계약 마스터 조회 완료");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "계약 마스터 조회 실패");
                throw;
            }
        }

        #endregion

        #region 계약 상세 조회

        /// <summary>
        /// 계약 상세 목록 조회 (전체)
        /// </summary>
        public List<ScmContractDetail> GetContractDetailList(string caseNo, string companyNo)
        {
            List<ScmContractDetail> result = new List<ScmContractDetail>();

            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT *
                        FROM SCM_CONTRACT_DETAIL
                        WHERE CASE_NO = :CASE_NO
                          AND COMPANY_NO = :COMPANY_NO
                        ORDER BY CONTRACT_ID";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("CASE_NO", OracleDbType.Varchar2).Value = caseNo;
                        cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2).Value = companyNo;

                        SqlLogger.LogCommand(cmd, "계약 상세 조회");

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(MapDetailReaderToModel(reader));
                            }
                        }

                        SqlLogger.LogResult(result.Count, "계약 상세 조회 완료");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "계약 상세 조회 실패");
                throw;
            }
        }

        /// <summary>
        /// 계약 상세 목록 조회 (기간 조건)
        /// </summary>
        public List<ScmContractDetail> GetContractDetailList(string caseNo, string companyNo, DateTime dtStart, DateTime dtEnd)
        {
            List<ScmContractDetail> result = new List<ScmContractDetail>();

            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT D.*
                        FROM SCM_CONTRACT_DETAIL D
                        INNER JOIN SCM_CONTRACT_MASTER M ON D.CONTRACT_ID = M.CONTRACT_ID 
                                                        AND D.CASE_NO = M.CASE_NO 
                                                        AND D.COMPANY_NO = M.COMPANY_NO
                        WHERE D.CASE_NO = :CASE_NO
                          AND D.COMPANY_NO = :COMPANY_NO
                          AND M.CNTR_DT BETWEEN :DT_START AND :DT_END
                        ORDER BY D.CONTRACT_ID";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("CASE_NO", OracleDbType.Varchar2).Value = caseNo;
                        cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2).Value = companyNo;
                        cmd.Parameters.Add("DT_START", OracleDbType.Date).Value = dtStart;
                        cmd.Parameters.Add("DT_END", OracleDbType.Date).Value = dtEnd;

                        SqlLogger.LogCommand(cmd, "계약 상세 조회 (기간)");

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(MapDetailReaderToModel(reader));
                            }
                        }

                        SqlLogger.LogResult(result.Count, "계약 상세 조회 완료");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "계약 상세 조회 실패");
                throw;
            }
        }

        #endregion

        #region 통합 조회 (마스터 + 상세)

        /// <summary>
        /// 계약 정보 통합 조회 (마스터 + 상세 조인)
        /// </summary>
        public List<ScmContractMaster> GetContractFullList(string caseNo, string companyNo, bool selectAll, DateTime? dtStart, DateTime? dtEnd)
        {
            List<ScmContractMaster> masterList;
            List<ScmContractDetail> detailList;

            // 마스터 조회
            if (selectAll || caseNo == "MASTER")
            {
                masterList = GetContractMasterList(caseNo, companyNo);
                detailList = GetContractDetailList(caseNo, companyNo);
            }
            else
            {
                masterList = GetContractMasterList(caseNo, companyNo, dtStart.Value, dtEnd.Value);
                detailList = GetContractDetailList(caseNo, companyNo, dtStart.Value, dtEnd.Value);
            }

            // 마스터-상세 연결
            foreach (var master in masterList)
            {
                master.DetailInfo = detailList.Find(d => d.CONTRACT_ID == master.CONTRACT_ID);
            }

            return masterList;
        }

        #endregion

        #region 코드 목록 조회

        /// <summary>
        /// 스케줄방식 목록 조회 (SCM14)
        /// </summary>
        public DataTable GetScheduleMethodList()
        {
            return GetCodeDetailList("SCM14");
        }

        /// <summary>
        /// 계약카테고리 목록 조회 (SCM22)
        /// </summary>
        public DataTable GetContractCategoryList()
        {
            return GetCodeDetailList("SCM22");
        }

        /// <summary>
        /// 계약구분 목록 조회 (SCM23)
        /// </summary>
        public DataTable GetContractTypeList()
        {
            return GetCodeDetailList("SCM23");
        }

        /// <summary>
        /// 작업난이도 목록 조회 (SCM24)
        /// </summary>
        public DataTable GetDifficultyList()
        {
            return GetCodeDetailList("SCM24");
        }

        /// <summary>
        /// 공통 코드 상세 목록 조회
        /// </summary>
        private DataTable GetCodeDetailList(string stdCode)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT STD_CODE_DET, STD_CODE_DET_NAME, STD_CODE_DET_SH_NAME, VIEW_ORDER
                        FROM STD_CODE_DETAIL_LAN
                        WHERE LAN_CODE = 'ko-kr'
                          AND STD_CODE = :STD_CODE
                          AND NVL(USE_YN, 'Y') = 'Y'
                        ORDER BY VIEW_ORDER";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("STD_CODE", OracleDbType.Varchar2).Value = stdCode;

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
                SqlLogger.LogError(ex, $"코드 목록 조회 실패 (STD_CODE: {stdCode})");
                throw;
            }
        }

        #endregion

        #region Mapping Methods

        /// <summary>
        /// 마스터 DataReader를 모델로 매핑
        /// </summary>
        private ScmContractMaster MapMasterReaderToModel(OracleDataReader reader)
        {
            return new ScmContractMaster
            {
                CASE_NO = GetSafeString(reader, "CASE_NO"),
                COMPANY_NO = GetSafeString(reader, "COMPANY_NO"),
                CONTRACT_ID = GetSafeString(reader, "CONTRACT_ID"),
                STD_CASE_NO = GetSafeString(reader, "STD_CASE_NO"),
                STD_CONTRACT_ID = GetSafeString(reader, "STD_CONTRACT_ID"),
                PROP1 = GetSafeString(reader, "PROP1"),
                PROP2 = GetSafeString(reader, "PROP2"),
                CONTRACT_NAME = GetSafeString(reader, "CONTRACT_NAME"),
                CONTRACT_CATEGORY = GetSafeString(reader, "CONTRACT_CATEGORY"),
                CONTRACT_TYPE = GetSafeString(reader, "CONTRACT_TYPE"),
                CONTRACT_NO = GetSafeString(reader, "CONTRACT_NO"),
                POR_NO = GetSafeString(reader, "POR_NO"),
                SEQ_NO = GetSafeString(reader, "SEQ_NO"),
                POR_DT = GetSafeDateTime(reader, "POR_DT"),
                CNTR_DT = GetSafeDateTime(reader, "CNTR_DT"),
                CNTR_INIT_DT = GetSafeDateTime(reader, "CNTR_INIT_DT"),
                MP_DT = GetSafeDateTime(reader, "MP_DT"),
                MP_INIT_DT = GetSafeDateTime(reader, "MP_INIT_DT"),
                PRODUCT_TYPE = GetSafeString(reader, "PRODUCT_TYPE"),
                PRODUCT_DESC = GetSafeString(reader, "PRODUCT_DESC"),
                CNTR_EA = GetSafeInt(reader, "CNTR_EA"),
                CNTR_PIECE_WGT = GetSafeDecimal(reader, "CNTR_PIECE_WGT"),
                CNTR_WGT = GetSafeDecimal(reader, "CNTR_WGT"),
                PROJECT_NO = GetSafeString(reader, "PROJECT_NO"),
                BLOCK_NO = GetSafeString(reader, "BLOCK_NO"),
                MARK_NO = GetSafeString(reader, "MARK_NO"),
                OWNER = GetSafeString(reader, "OWNER"),
                TAG1 = GetSafeString(reader, "TAG1"),
                EST_ST_DT = GetSafeDateTime(reader, "EST_ST_DT"),
                EST_FI_DT = GetSafeDateTime(reader, "EST_FI_DT"),
                PLAN_ST_DT = GetSafeDateTime(reader, "PLAN_ST_DT"),
                PLAN_FI_DT = GetSafeDateTime(reader, "PLAN_FI_DT"),
                RESULT_ST_DT = GetSafeDateTime(reader, "RESULT_ST_DT"),
                RESULT_FI_DT = GetSafeDateTime(reader, "RESULT_FI_DT"),
                RMK = GetSafeString(reader, "RMK")
            };
        }

        /// <summary>
        /// 상세 DataReader를 모델로 매핑
        /// </summary>
        private ScmContractDetail MapDetailReaderToModel(OracleDataReader reader)
        {
            return new ScmContractDetail
            {
                CASE_NO = GetSafeString(reader, "CASE_NO"),
                COMPANY_NO = GetSafeString(reader, "COMPANY_NO"),
                CONTRACT_ID = GetSafeString(reader, "CONTRACT_ID"),
                ACT_NO = GetSafeString(reader, "ACT_NO"),
                OWNER_DEPT = GetSafeString(reader, "OWNER_DEPT"),
                MAIN_CONTRACTOR = GetSafeString(reader, "MAIN_CONTRACTOR"),
                SUB_CONTRACTOR = GetSafeString(reader, "SUB_CONTRACTOR"),
                SUB_CONTRACTOR2 = GetSafeString(reader, "SUB_CONTRACTOR2"),
                SUB_CONTRACTOR2_DESC = GetSafeString(reader, "SUB_CONTRACTOR2_DESC"),
                MS_NO = GetSafeString(reader, "MS_NO"),
                MS_DT = GetSafeDateTime(reader, "MS_DT"),
                MS_CONTRACTOR = GetSafeString(reader, "MS_CONTRACTOR"),
                PAINT_MS_NO = GetSafeString(reader, "PAINT_MS_NO"),
                PAINT_MS_DT = GetSafeDateTime(reader, "PAINT_MS_DT"),
                PAINT_MS_CONTRACTOR = GetSafeString(reader, "PAINT_MS_CONTRACTOR"),
                DRAWING_DT = GetSafeDateTime(reader, "DRAWING_DT"),
                DRAWING_INIT_DT = GetSafeDateTime(reader, "DRAWING_INIT_DT"),
                MAT_LOG_REQ_DT = GetSafeDateTime(reader, "MAT_LOG_REQ_DT"),
                MAT_LOG_DT = GetSafeDateTime(reader, "MAT_LOG_DT"),
                MAT_LOG_REQ_NO = GetSafeString(reader, "MAT_LOG_REQ_NO"),
                MAT_LOG_POS_NM = GetSafeString(reader, "MAT_LOG_POS_NM"),
                MAT_LOG_REQ_PER = GetSafeString(reader, "MAT_LOG_REQ_PER"),
                MAT_LOG_REQ_DEPT = GetSafeString(reader, "MAT_LOG_REQ_DEPT"),
                MAT_LOG_REQ_TEL = GetSafeString(reader, "MAT_LOG_REQ_TEL"),
                MAT_LOG_PER = GetSafeString(reader, "MAT_LOG_PER"),
                MPPL_PROJ = GetSafeString(reader, "MPPL_PROJ"),
                MPPL_NO = GetSafeString(reader, "MPPL_NO"),
                MPPL_SEQ = GetSafeString(reader, "MPPL_SEQ"),
                MP_RES_DT = GetSafeDateTime(reader, "MP_RES_DT"),
                MAKING_DT = GetSafeDateTime(reader, "MAKING_DT"),
                MAKING_RES_DT = GetSafeDateTime(reader, "MAKING_RES_DT"),
                PAINTING_DT = GetSafeDateTime(reader, "PAINTING_DT"),
                PAINTING_RES_DT = GetSafeDateTime(reader, "PAINTING_RES_DT"),
                INSPECTION_DT = GetSafeDateTime(reader, "INSPECTION_DT"),
                INSPECTION_RES_DT = GetSafeDateTime(reader, "INSPECTION_RES_DT"),
                TAG1 = GetSafeString(reader, "TAG1"),
                TAG2 = GetSafeString(reader, "TAG2"),
                TAG3 = GetSafeString(reader, "TAG3"),
                TAG4 = GetSafeString(reader, "TAG4"),
                TAG5 = GetSafeString(reader, "TAG5")
            };
        }

        #endregion

        #region Helper Methods

        private string GetSafeString(OracleDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
            }
            catch { return string.Empty; }
        }

        private DateTime? GetSafeDateTime(OracleDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? (DateTime?)null : reader.GetDateTime(ordinal);
            }
            catch { return null; }
        }

        private int? GetSafeInt(OracleDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? (int?)null : Convert.ToInt32(reader.GetValue(ordinal));
            }
            catch { return null; }
        }

        private decimal? GetSafeDecimal(OracleDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? (decimal?)null : Convert.ToDecimal(reader.GetValue(ordinal));
            }
            catch { return null; }
        }

        #endregion
    }
}