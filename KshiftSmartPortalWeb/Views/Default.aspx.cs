using System;
using System.Collections.Generic;
using System.Web.UI;
using KShiftSmartPortalWeb.Controllers;
using KShiftSmartPortalWeb.Models;

namespace KShiftSmartPortalWeb
{
    public partial class Default : System.Web.UI.Page
    {
        private ContractController _contractController = new ContractController();

        protected void Page_Load(object sender, EventArgs e)
        {
            // 세션 체크
            if (Session["UserId"] == null && Session["UserID"] == null)
            {
                Response.Redirect("~/Views/Login.aspx", false);
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
                ShowMessage($"데이터 조회 중 오류가 발생했습니다: {ex.Message}");
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
                // BootstrapGridView도 ASPxGridViewExporter 사용 가능
                gridContracts.ExportXlsxToResponse("SCM_Contract_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");
            }
            catch (Exception ex)
            {
                ShowMessage($"엑셀 내보내기 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        /// <summary>
        /// 데이터 로드 (Controller 사용)
        /// </summary>
        private void LoadData()
        {
            try
            {
                string companyNo = cmbCompany.Value?.ToString();
                string caseNo = cmbCase.Value?.ToString();

                // Controller를 통해 데이터 조회
                List<ScmContractDetail> contractList = _contractController.GetContractList(companyNo, caseNo);

                // 그리드에 데이터 바인딩
                gridContracts.DataSource = contractList;
                gridContracts.DataBind();

                // 레코드 수 표시
                lblRecordCount.Text = $"총 {contractList.Count}건의 데이터가 조회되었습니다.";

                if (contractList.Count == 0)
                {
                    ShowMessage("조회된 데이터가 없습니다.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"데이터 조회 중 오류 발생: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// ObjectDataSource를 위한 Select 메서드
        /// </summary>
        public List<ScmContractDetail> GetContractData()
        {
            string companyNo = cmbCompany.Value?.ToString();
            string caseNo = cmbCase.Value?.ToString();

            return _contractController.GetContractList(companyNo, caseNo);
        }

        /// <summary>
        /// 메시지 표시
        /// </summary>
        private void ShowMessage(string message)
        {
            string script = $"alert('{message.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowMessage", script, true);
        }
    }
}