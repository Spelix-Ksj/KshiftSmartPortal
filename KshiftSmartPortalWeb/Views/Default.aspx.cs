using System;
using System.Collections.Generic;
using System.Web.UI;
using ScmBlockContractWeb.Controllers;
using ScmBlockContractWeb.Models;

namespace ScmBlockContractWeb
{
    public partial class Default : System.Web.UI.Page
    {
        private ContractController _contractController = new ContractController();

        protected void Page_Load(object sender, EventArgs e)
        {
            // 세션 체크
            if (Session["UserId"] == null && Session["UserID"] == null)
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

            // 테마 적용
            ApplyTheme();

            // 기본 날짜 설정 (최근 1년)
            dtStartDate.Value = DateTime.Now.AddYears(-1);
            dtEndDate.Value = DateTime.Now;

            // 초기 데이터 로드 비활성화 (사용자가 조회 버튼 클릭 시에만 로드)
            // LoadData();
        }

        /// <summary>
        /// 테마 적용
        /// </summary>
        private void ApplyTheme()
        {
            string theme = "Office365"; // 기본 테마

            // 세션에서 테마 가져오기
            if (Session["AppTheme"] != null)
            {
                theme = Session["AppTheme"].ToString();
            }
            // 쿠키에서 테마 가져오기
            else if (Request.Cookies["AppTheme"] != null)
            {
                theme = Request.Cookies["AppTheme"].Value;
                Session["AppTheme"] = theme;
            }

            // DevExpress 컨트롤에 테마 적용
            ApplyDevExpressTheme(theme);
        }

        /// <summary>
        /// DevExpress 컨트롤에 테마 적용
        /// </summary>
        private void ApplyDevExpressTheme(string theme)
        {
            // GridView 테마 설정
            if (gridContracts != null)
            {
                gridContracts.Theme = theme;
            }

            // 콤보박스 테마 설정
            if (cmbCompanyType != null) cmbCompanyType.Theme = theme;
            if (cmbCompany != null) cmbCompany.Theme = theme;
            if (cmbCase != null) cmbCase.Theme = theme;

            // 날짜 컨트롤 테마 설정
            if (dtStartDate != null) dtStartDate.Theme = theme;
            if (dtEndDate != null) dtEndDate.Theme = theme;

            // 체크박스 테마 설정
            if (chkSelectAll != null) chkSelectAll.Theme = theme;

            // 버튼 테마 설정
            if (btnSearch != null) btnSearch.Theme = theme;
            if (btnReset != null) btnReset.Theme = theme;
            if (btnExport != null) btnExport.Theme = theme;

            // 레이블 테마 설정
            if (lblRecordCount != null) lblRecordCount.Theme = theme;
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
                    ShowMessage("조회된 데이터가 없습니다.", "info");
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
        private void ShowMessage(string message, string type)
        {
            string script = $"alert('{message.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowMessage", script, true);
        }
    }
}
