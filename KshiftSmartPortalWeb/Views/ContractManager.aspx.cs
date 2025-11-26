using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using DevExpress.Web;
using KShiftSmartPortalWeb.Controllers;
using KShiftSmartPortalWeb.Models;

namespace KShiftSmartPortalWeb
{
    public partial class ContractManager : System.Web.UI.Page
    {
        private ContractManagerController _controller = new ContractManagerController();
        private LoginController _loginController = new LoginController();

        // 세션 키 상수
        private const string SESSION_KEY_DATA = "ContractManager_Data";

        #region Properties

        /// <summary>
        /// 세션에 저장된 데이터 (페이징/필터링 시 유지)
        /// </summary>
        private List<ContractViewModel> GridData
        {
            get { return Session[SESSION_KEY_DATA] as List<ContractViewModel>; }
            set { Session[SESSION_KEY_DATA] = value; }
        }

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            // 세션 체크
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack && !IsCallback)
            {
                InitializePage();
            }
            else
            {
                // 포스트백/콜백 시 데이터 재바인딩
                BindGridFromSession();
            }
        }

        /// <summary>
        /// 페이지 초기화
        /// </summary>
        private void InitializePage()
        {
            try
            {
                // Company 구분 로드
                LoadCompanyTypeList();

                // Company 목록 로드
                LoadCompanyList();

                // 케이스 목록 로드
                LoadCaseList();

                // 기본 날짜 설정 (1년 전 ~ 오늘)
                dtStartDate.Value = DateTime.Today.AddYears(-1);
                dtEndDate.Value = DateTime.Today;

                // 조회조건 체크박스 기본값
                chkSelectAll.Checked = true;

                // 세션 데이터 초기화
                GridData = null;
            }
            catch (Exception ex)
            {
                ShowMessage($"페이지 초기화 실패: {ex.Message}");
            }
        }

        #endregion

        #region Grid Events

        protected void gridContracts_PageIndexChanged(object sender, EventArgs e)
        {
            BindGridFromSession();
        }

        protected void gridContracts_BeforeColumnSortingGrouping(object sender, ASPxGridViewBeforeColumnGroupingSortingEventArgs e)
        {
            BindGridFromSession();
        }

        protected void gridContracts_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            BindGridFromSession();
        }

        private void BindGridFromSession()
        {
            if (GridData != null)
            {
                gridContracts.DataSource = GridData;
                gridContracts.DataBind();
            }
        }

        #endregion

        #region ComboBox 초기화

        private void LoadCompanyTypeList()
        {
            cmbCompanyType.Items.Clear();
            cmbCompanyType.Items.Add("전체", "*");
            cmbCompanyType.Items.Add("협력사", "VENDOR");
            cmbCompanyType.Items.Add("MASTER", "MASTER");
            cmbCompanyType.SelectedIndex = 0;
        }

        private void LoadCompanyList()
        {
            try
            {
                DataTable dt = _loginController.GetCompanyList();
                string companyType = cmbCompanyType.Value?.ToString() ?? "*";

                cmbCompany.Items.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    if (companyType == "*" || row["COMPANY_TYPE"]?.ToString() == companyType)
                    {
                        string text = $"{row["COMPANY_NO"]} - {row["COMPANY_NAME"]}";
                        cmbCompany.Items.Add(text, row["COMPANY_NO"].ToString());
                    }
                }

                if (cmbCompany.Items.FindByValue("1002") != null)
                {
                    cmbCompany.Value = "1002";
                }
                else if (cmbCompany.Items.Count > 0)
                {
                    cmbCompany.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Company 목록 로드 실패: {ex.Message}");
            }
        }

        private void LoadCaseList()
        {
            try
            {
                string companyNo = cmbCompany.Value?.ToString();
                if (string.IsNullOrEmpty(companyNo)) return;

                DataTable dt = _controller.GetCaseList(companyNo);

                cmbCase.DataSource = dt;
                cmbCase.DataBind();

                if (cmbCase.Items.Count > 0)
                {
                    cmbCase.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"케이스 목록 로드 실패: {ex.Message}");
            }
        }

        protected void cmbCompanyType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCompanyList();
            LoadCaseList();
        }

        protected void cmbCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCaseList();
        }

        #endregion

        #region Button Events

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                ShowMessage($"데이터 조회 실패: {ex.Message}");
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            cmbCompanyType.SelectedIndex = 0;
            LoadCompanyList();
            LoadCaseList();

            chkSelectAll.Checked = true;
            dtStartDate.Value = DateTime.Today.AddYears(-1);
            dtEndDate.Value = DateTime.Today;

            GridData = null;
            gridContracts.DataSource = null;
            gridContracts.DataBind();
            lblRecordCount.Text = "조회된 데이터가 없습니다.";
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (GridData != null && GridData.Count > 0)
                {
                    gridContracts.DataSource = GridData;
                    gridContracts.DataBind();

                    string fileName = $"계약정보_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    gridExporter.WriteXlsxToResponse(fileName);
                }
                else
                {
                    ShowMessage("내보낼 데이터가 없습니다. 먼저 조회를 실행하세요.");
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"엑셀 내보내기 실패: {ex.Message}");
            }
        }

        #endregion

        #region Data Methods

        private void LoadData()
        {
            string caseNo = cmbCase.Value?.ToString();
            string companyNo = cmbCompany.Value?.ToString();
            bool selectAll = chkSelectAll.Checked;
            DateTime? dtStart = dtStartDate.Value as DateTime?;
            DateTime? dtEnd = dtEndDate.Value as DateTime?;

            if (string.IsNullOrEmpty(caseNo) || string.IsNullOrEmpty(companyNo))
            {
                ShowMessage("케이스와 Company를 선택하세요.");
                return;
            }

            List<ScmContractMaster> masterList = _controller.GetContractFullList(caseNo, companyNo, selectAll, dtStart, dtEnd);
            var viewModelList = ConvertToViewModel(masterList);

            GridData = viewModelList;

            gridContracts.DataSource = viewModelList;
            gridContracts.DataBind();

            lblRecordCount.Text = $"총 <strong>{viewModelList.Count}</strong>건의 데이터가 조회되었습니다.";
        }

        private List<ContractViewModel> ConvertToViewModel(List<ScmContractMaster> masterList)
        {
            var result = new List<ContractViewModel>();

            foreach (var master in masterList)
            {
                var vm = new ContractViewModel
                {
                    COMPANY_NO = master.COMPANY_NO,
                    CASE_NO = master.CASE_NO,
                    CONTRACT_ID = master.CONTRACT_ID,
                    STD_CASE_NO = master.STD_CASE_NO,
                    STD_CONTRACT_ID = master.STD_CONTRACT_ID,
                    PROP1 = master.PROP1,
                    PROP2 = master.PROP2,
                    CONTRACT_NAME = master.CONTRACT_NAME,
                    CONTRACT_CATEGORY = master.CONTRACT_CATEGORY,
                    CONTRACT_TYPE = master.CONTRACT_TYPE,
                    CONTRACT_NO = master.CONTRACT_NO,
                    POR_NO = master.POR_NO,
                    SEQ_NO = master.SEQ_NO,
                    POR_DT = master.POR_DT,
                    CNTR_DT = master.CNTR_DT,
                    CNTR_INIT_DT = master.CNTR_INIT_DT,
                    MP_DT = master.MP_DT,
                    MP_INIT_DT = master.MP_INIT_DT,
                    PRODUCT_TYPE = master.PRODUCT_TYPE,
                    PRODUCT_DESC = master.PRODUCT_DESC,
                    CNTR_EA = master.CNTR_EA,
                    CNTR_PIECE_WGT = master.CNTR_PIECE_WGT,
                    CNTR_WGT = master.CNTR_WGT,
                    PROJECT_NO = master.PROJECT_NO,
                    BLOCK_NO = master.BLOCK_NO,
                    MARK_NO = master.MARK_NO,
                    OWNER = master.OWNER,
                    TAG1 = master.TAG1,
                    EST_ST_DT = master.EST_ST_DT,
                    EST_FI_DT = master.EST_FI_DT,
                    PLAN_ST_DT = master.PLAN_ST_DT,
                    PLAN_FI_DT = master.PLAN_FI_DT,
                    RESULT_ST_DT = master.RESULT_ST_DT,
                    RESULT_FI_DT = master.RESULT_FI_DT,
                    RMK = master.RMK
                };

                if (master.DetailInfo != null)
                {
                    var detail = master.DetailInfo;
                    vm.OWNER_DEPT = detail.OWNER_DEPT;
                    vm.MAIN_CONTRACTOR = detail.MAIN_CONTRACTOR;
                    vm.SUB_CONTRACTOR = detail.SUB_CONTRACTOR;
                    vm.MS_NO = detail.MS_NO;
                    vm.MS_DT = detail.MS_DT;
                    vm.MS_CONTRACTOR = detail.MS_CONTRACTOR;
                    vm.PAINT_MS_NO = detail.PAINT_MS_NO;
                    vm.PAINT_MS_DT = detail.PAINT_MS_DT;
                    vm.PAINT_MS_CONTRACTOR = detail.PAINT_MS_CONTRACTOR;
                    vm.DRAWING_DT = detail.DRAWING_DT;
                    vm.DRAWING_INIT_DT = detail.DRAWING_INIT_DT;
                    vm.MAT_LOG_DT = detail.MAT_LOG_DT;
                    vm.MAT_LOG_REQ_NO = detail.MAT_LOG_REQ_NO;
                    vm.MAT_LOG_POS_NM = detail.MAT_LOG_POS_NM;
                    vm.MAT_LOG_REQ_PER = detail.MAT_LOG_REQ_PER;
                    vm.MAT_LOG_REQ_DEPT = detail.MAT_LOG_REQ_DEPT;
                    vm.MAT_LOG_REQ_TEL = detail.MAT_LOG_REQ_TEL;
                    vm.MAT_LOG_PER = detail.MAT_LOG_PER;
                    vm.MP_RES_DT = detail.MP_RES_DT;
                    vm.MAKING_DT = detail.MAKING_DT;
                    vm.MAKING_RES_DT = detail.MAKING_RES_DT;
                    vm.PAINTING_DT = detail.PAINTING_DT;
                    vm.PAINTING_RES_DT = detail.PAINTING_RES_DT;
                    vm.INSPECTION_DT = detail.INSPECTION_DT;
                    vm.INSPECTION_RES_DT = detail.INSPECTION_RES_DT;
                    vm.MPPL_PROJ = detail.MPPL_PROJ;
                    vm.MPPL_NO = detail.MPPL_NO;
                    vm.MPPL_SEQ = detail.MPPL_SEQ;
                }

                result.Add(vm);
            }

            return result;
        }

        #endregion

        #region Helper Methods

        private void ShowMessage(string message)
        {
            string script = $"alert('{message.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowMessage", script, true);
        }

        #endregion
    }

    #region ViewModel

    [Serializable]
    public class ContractViewModel
    {
        public string COMPANY_NO { get; set; }
        public string CASE_NO { get; set; }
        public string CONTRACT_ID { get; set; }
        public string STD_CASE_NO { get; set; }
        public string STD_CONTRACT_ID { get; set; }
        public string PROP1 { get; set; }
        public string PROP2 { get; set; }
        public string CONTRACT_NAME { get; set; }
        public string CONTRACT_CATEGORY { get; set; }
        public string CONTRACT_TYPE { get; set; }
        public string CONTRACT_NO { get; set; }
        public string POR_NO { get; set; }
        public string SEQ_NO { get; set; }
        public DateTime? POR_DT { get; set; }
        public DateTime? CNTR_DT { get; set; }
        public DateTime? CNTR_INIT_DT { get; set; }
        public DateTime? MP_DT { get; set; }
        public DateTime? MP_INIT_DT { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public string PRODUCT_DESC { get; set; }
        public int? CNTR_EA { get; set; }
        public decimal? CNTR_PIECE_WGT { get; set; }
        public decimal? CNTR_WGT { get; set; }
        public string PROJECT_NO { get; set; }
        public string BLOCK_NO { get; set; }
        public string MARK_NO { get; set; }
        public string OWNER { get; set; }
        public string TAG1 { get; set; }
        public string OWNER_DEPT { get; set; }
        public string MAIN_CONTRACTOR { get; set; }
        public string SUB_CONTRACTOR { get; set; }
        public string MS_NO { get; set; }
        public DateTime? MS_DT { get; set; }
        public string MS_CONTRACTOR { get; set; }
        public string PAINT_MS_NO { get; set; }
        public DateTime? PAINT_MS_DT { get; set; }
        public string PAINT_MS_CONTRACTOR { get; set; }
        public DateTime? DRAWING_DT { get; set; }
        public DateTime? DRAWING_INIT_DT { get; set; }
        public DateTime? MAT_LOG_DT { get; set; }
        public string MAT_LOG_REQ_NO { get; set; }
        public string MAT_LOG_POS_NM { get; set; }
        public string MAT_LOG_REQ_PER { get; set; }
        public string MAT_LOG_REQ_DEPT { get; set; }
        public string MAT_LOG_REQ_TEL { get; set; }
        public string MAT_LOG_PER { get; set; }
        public DateTime? MP_RES_DT { get; set; }
        public DateTime? MAKING_DT { get; set; }
        public DateTime? MAKING_RES_DT { get; set; }
        public DateTime? PAINTING_DT { get; set; }
        public DateTime? PAINTING_RES_DT { get; set; }
        public DateTime? INSPECTION_DT { get; set; }
        public DateTime? INSPECTION_RES_DT { get; set; }
        public string MPPL_PROJ { get; set; }
        public string MPPL_NO { get; set; }
        public string MPPL_SEQ { get; set; }
        public DateTime? EST_ST_DT { get; set; }
        public DateTime? EST_FI_DT { get; set; }
        public DateTime? PLAN_ST_DT { get; set; }
        public DateTime? PLAN_FI_DT { get; set; }
        public DateTime? RESULT_ST_DT { get; set; }
        public DateTime? RESULT_FI_DT { get; set; }
        public string RMK { get; set; }
    }

    #endregion
}