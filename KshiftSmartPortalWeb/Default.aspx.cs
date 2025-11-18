using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web.UI;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using ScmBlockContractWeb.Models;

namespace ScmBlockContractWeb
{
    public partial class Default : System.Web.UI.Page
    {
        private string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["OracleConnection"].ConnectionString;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // 세션 체크
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                InitializePage();
            }
        }

        /// <summary>
        /// 페이지 초기화
        /// </summary>
        private void InitializePage()
        {
            // 사용자 정보 표시
            if (Session["UserID"] != null)
            {
                lblUserName.Text = Session["UserID"].ToString();
            }

            // 기본 날짜 설정 (최근 1년)
            dtStartDate.Value = DateTime.Now.AddYears(-1);
            dtEndDate.Value = DateTime.Now;

            // 초기 데이터 로드 비활성화 (사용자가 조회 버튼 클릭 시에만 로드)
            // LoadData();
        }

        /// <summary>
        /// 조회 버튼 클릭 이벤트
        /// </summary>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                ShowMessage($"데이터 조회 중 오류가 발생했습니다: {ex.Message}", "error");
            }
        }

        /// <summary>
        /// 초기화 버튼 클릭 이벤트
        /// </summary>
        protected void btnReset_Click(object sender, EventArgs e)
        {
            // 조회 조건 초기화
            cmbCompanyType.SelectedIndex = 0;
            cmbCompany.Value = "1002";
            cmbCase.Value = "MASTER";
            chkSelectAll.Checked = true;
            dtStartDate.Value = DateTime.Now.AddYears(-1);
            dtEndDate.Value = DateTime.Now;

            // 그리드 초기화
            gridContracts.DataSource = null;
            gridContracts.DataBind();
            lblRecordCount.Text = "조회된 데이터가 없습니다.";
        }

        /// <summary>
        /// 엑셀 내보내기 버튼 클릭 이벤트
        /// </summary>
        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                gridContracts.ExportXlsxToResponse("SCM_Contract_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");
            }
            catch (Exception ex)
            {
                ShowMessage($"엑셀 내보내기 중 오류가 발생했습니다: {ex.Message}", "error");
            }
        }

        /// <summary>
        /// 데이터 로드
        /// </summary>
        private void LoadData()
        {
            List<ScmContractDetail> contractList = new List<ScmContractDetail>();

            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = BuildQuery();
                    
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        // 파라미터 추가
                        AddParameters(cmd);

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
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

                                contractList.Add(contract);
                            }
                        }
                    }
                }

                // 그리드에 데이터 바인딩
                gridContracts.DataSource = contractList;
                gridContracts.DataBind();

                // 레코드 수 표시
                lblRecordCount.Text = $"총 {contractList.Count}건의 데이터가 조회되었습니다.";

                if (contractList.Count == 0)
                {
                    ShowMessage("조회된 데이터가 없습니다.", "info");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"데이터베이스 조회 중 오류 발생: {ex.Message}", ex);
            }
        }

        private string BuildQuery()
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

            if (cmbCompany.Value != null && !string.IsNullOrEmpty(cmbCompany.Value.ToString()))
            {
                query += " AND COMPANY_NO = :COMPANY_NO";
            }

            if (cmbCase.Value != null && !string.IsNullOrEmpty(cmbCase.Value.ToString()))
            {
                query += " AND CASE_NO = :CASE_NO";
            }

            // 날짜 조건은 제거 (실제 테이블에 계약일 컬럼이 없음)

            query += " ORDER BY CONTRACT_ID";

            return query;
        }

        /// <summary>
        /// 쿼리 파라미터 추가
        /// </summary>
        private void AddParameters(OracleCommand cmd)
        {
            // Company 파라미터
            if (cmbCompany.Value != null && !string.IsNullOrEmpty(cmbCompany.Value.ToString()))
            {
                cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2, 50).Value = cmbCompany.Value.ToString();
            }

            // Case 파라미터
            if (cmbCase.Value != null && !string.IsNullOrEmpty(cmbCase.Value.ToString()))
            {
                cmd.Parameters.Add("CASE_NO", OracleDbType.Varchar2, 50).Value = cmbCase.Value.ToString();
            }
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

        /// <summary>
        /// 안전한 Decimal 읽기
        /// </summary>
        private decimal? GetSafeDecimal(OracleDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? (decimal?)null : reader.GetDecimal(ordinal);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 메시지 표시
        /// </summary>
        private void ShowMessage(string message, string type)
        {
            string script = $"alert('{message.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowMessage", script, true);
        }

        #endregion
    }
}
