using System;
using System.Collections.Generic;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;
using ScmBlockContractWeb.Models;

namespace ScmBlockContractWeb.Controllers
{
    /// <summary>
    /// 계약 데이터 비즈니스 로직을 담당하는 컨트롤러
    /// </summary>
    public class ContractController
    {
        private string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["OracleConnection"].ConnectionString;
            }
        }

        /// <summary>
        /// 계약 목록 조회
        /// </summary>
        /// <param name="companyNo">회사 번호</param>
        /// <param name="caseNo">케이스 번호</param>
        /// <returns>계약 목록</returns>
        public List<ScmContractDetail> GetContractList(string companyNo = null, string caseNo = null)
        {
            List<ScmContractDetail> contractList = new List<ScmContractDetail>();

            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = BuildQuery(companyNo, caseNo);

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        // 파라미터 추가
                        AddParameters(cmd, companyNo, caseNo);

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ScmContractDetail contract = MapReaderToModel(reader);
                                contractList.Add(contract);
                            }
                        }
                    }
                }

                return contractList;
            }
            catch (Exception ex)
            {
                throw new Exception($"계약 목록 조회 중 오류 발생: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 쿼리 생성
        /// </summary>
        private string BuildQuery(string companyNo, string caseNo)
        {
            string query = @"
                SELECT 
                    CASE_NO, COMPANY_NO, CONTRACT_ID, ACT_NO,
                    OWNER_DEPT, MAIN_CONTRACTOR, SUB_CONTRACTOR, 
                    SUB_CONTRACTOR2, SUB_CONTRACTOR2_DESC,
                    MS_NO, MS_DT, MS_CONTRACTOR,
                    PAINT_MS_NO, PAINT_MS_DT, PAINT_MS_CONTRACTOR,
                    DRAWING_DT, DRAWING_INIT_DT,
                    MAT_LOG_REQ_DT, MAT_LOG_DT, MAT_LOG_REQ_NO,
                    MAT_LOG_POS_NM, MAT_LOG_REQ_PER, MAT_LOG_REQ_DEPT,
                    MAT_LOG_REQ_TEL, MAT_LOG_PER,
                    MPPL_PROJ, MPPL_NO, MPPL_SEQ,
                    MP_RES_DT, MAKING_DT, MAKING_RES_DT,
                    PAINTING_DT, PAINTING_RES_DT,
                    INSPECTION_DT, INSPECTION_RES_DT,
                    TAG1, TAG2, TAG3, TAG4, TAG5
                FROM 
                    SCM_CONTRACT_DETAIL
                WHERE 
                    1=1";

            if (!string.IsNullOrEmpty(companyNo))
            {
                query += " AND COMPANY_NO = :COMPANY_NO";
            }

            if (!string.IsNullOrEmpty(caseNo))
            {
                query += " AND CASE_NO = :CASE_NO";
            }

            query += " ORDER BY CONTRACT_ID";

            return query;
        }

        /// <summary>
        /// 쿼리 파라미터 추가
        /// </summary>
        private void AddParameters(OracleCommand cmd, string companyNo, string caseNo)
        {
            if (!string.IsNullOrEmpty(companyNo))
            {
                cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2, 50).Value = companyNo;
            }

            if (!string.IsNullOrEmpty(caseNo))
            {
                cmd.Parameters.Add("CASE_NO", OracleDbType.Varchar2, 50).Value = caseNo;
            }
        }

        /// <summary>
        /// DataReader를 모델로 매핑
        /// </summary>
        private ScmContractDetail MapReaderToModel(OracleDataReader reader)
        {
            ScmContractDetail contract = new ScmContractDetail();

            // 기본 정보
            contract.COMPANY_NO = GetSafeString(reader, "COMPANY_NO");
            contract.CASE_NO = GetSafeString(reader, "CASE_NO");
            contract.CONTRACT_ID = GetSafeString(reader, "CONTRACT_ID");
            contract.ACT_NO = GetSafeString(reader, "ACT_NO");

            // 담당자 정보
            contract.OWNER_DEPT = GetSafeString(reader, "OWNER_DEPT");
            contract.MAIN_CONTRACTOR = GetSafeString(reader, "MAIN_CONTRACTOR");
            contract.SUB_CONTRACTOR = GetSafeString(reader, "SUB_CONTRACTOR");
            contract.SUB_CONTRACTOR2 = GetSafeString(reader, "SUB_CONTRACTOR2");
            contract.SUB_CONTRACTOR2_DESC = GetSafeString(reader, "SUB_CONTRACTOR2_DESC");

            // 도급 정보
            contract.MS_NO = GetSafeString(reader, "MS_NO");
            contract.MS_DT = GetSafeDateTime(reader, "MS_DT");
            contract.MS_CONTRACTOR = GetSafeString(reader, "MS_CONTRACTOR");
            contract.PAINT_MS_NO = GetSafeString(reader, "PAINT_MS_NO");
            contract.PAINT_MS_DT = GetSafeDateTime(reader, "PAINT_MS_DT");
            contract.PAINT_MS_CONTRACTOR = GetSafeString(reader, "PAINT_MS_CONTRACTOR");

            // 도면 정보
            contract.DRAWING_DT = GetSafeDateTime(reader, "DRAWING_DT");
            contract.DRAWING_INIT_DT = GetSafeDateTime(reader, "DRAWING_INIT_DT");

            // 자재 배송 정보
            contract.MAT_LOG_REQ_DT = GetSafeDateTime(reader, "MAT_LOG_REQ_DT");
            contract.MAT_LOG_DT = GetSafeDateTime(reader, "MAT_LOG_DT");
            contract.MAT_LOG_REQ_NO = GetSafeString(reader, "MAT_LOG_REQ_NO");
            contract.MAT_LOG_POS_NM = GetSafeString(reader, "MAT_LOG_POS_NM");
            contract.MAT_LOG_REQ_PER = GetSafeString(reader, "MAT_LOG_REQ_PER");
            contract.MAT_LOG_REQ_DEPT = GetSafeString(reader, "MAT_LOG_REQ_DEPT");
            contract.MAT_LOG_REQ_TEL = GetSafeString(reader, "MAT_LOG_REQ_TEL");
            contract.MAT_LOG_PER = GetSafeString(reader, "MAT_LOG_PER");

            // MPPL 정보
            contract.MPPL_PROJ = GetSafeString(reader, "MPPL_PROJ");
            contract.MPPL_NO = GetSafeString(reader, "MPPL_NO");
            contract.MPPL_SEQ = GetSafeString(reader, "MPPL_SEQ");

            // 생산 공정
            contract.MP_RES_DT = GetSafeDateTime(reader, "MP_RES_DT");
            contract.MAKING_DT = GetSafeDateTime(reader, "MAKING_DT");
            contract.MAKING_RES_DT = GetSafeDateTime(reader, "MAKING_RES_DT");
            contract.PAINTING_DT = GetSafeDateTime(reader, "PAINTING_DT");
            contract.PAINTING_RES_DT = GetSafeDateTime(reader, "PAINTING_RES_DT");
            contract.INSPECTION_DT = GetSafeDateTime(reader, "INSPECTION_DT");
            contract.INSPECTION_RES_DT = GetSafeDateTime(reader, "INSPECTION_RES_DT");

            // TAG
            contract.TAG1 = GetSafeString(reader, "TAG1");
            contract.TAG2 = GetSafeString(reader, "TAG2");
            contract.TAG3 = GetSafeString(reader, "TAG3");
            contract.TAG4 = GetSafeString(reader, "TAG4");
            contract.TAG5 = GetSafeString(reader, "TAG5");

            return contract;
        }

        #region Helper Methods

        /// <summary>
        /// 안전한 문자열 읽기
        /// </summary>
        private string GetSafeString(OracleDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 안전한 DateTime 읽기
        /// </summary>
        private DateTime? GetSafeDateTime(OracleDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? (DateTime?)null : reader.GetDateTime(ordinal);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
