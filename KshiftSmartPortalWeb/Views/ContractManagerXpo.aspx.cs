using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using DevExpress.Web;
using KShiftSmartPortalWeb.Controllers;
using KShiftSmartPortalWeb.Models;

namespace KShiftSmartPortalWeb
{
    public partial class ContractManagerXpo : System.Web.UI.Page
    {
        private ContractManagerXpoController _xpoController = new ContractManagerXpoController();
        private LoginController _loginController = new LoginController();

        private const string SESSION_KEY_DATA = "ContractManagerXpo_Data";

        private List<ContractViewModel> GridData
        {
            get { return Session[SESSION_KEY_DATA] as List<ContractViewModel>; }
            set { Session[SESSION_KEY_DATA] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
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
                BindGridFromSession();
            }
        }

        private void InitializePage()
        {
            try
            {
                LoadCompanyTypeList();
                LoadCompanyList();
                LoadCaseList();

                dtStartDate.Value = DateTime.Today.AddYears(-1);
                dtEndDate.Value = DateTime.Today;

                chkSelectAll.Checked = true;
                GridData = null;
            }
            catch (Exception ex)
            {
                ShowMessage($"페이지 초기화 오류: {ex.Message}");
            }
        }

        #region ComboBox 초기화 (기존)
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
                    cmbCompany.Value = "1002";
                else if (cmbCompany.Items.Count > 0)
                    cmbCompany.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowMessage($"Company 목록 로드 오류: {ex.Message}");
            }
        }

        private void LoadCaseList()
        {
            try
            {
                string companyNo = cmbCompany.Value?.ToString();
                if (string.IsNullOrEmpty(companyNo)) return;

                // 실제 controller의 GetCaseList 사용 (Oracle)
                DataTable dt = _xpoController.GetCaseList(companyNo);

                cmbCase.DataSource = dt;
                cmbCase.DataBind();

                if (cmbCase.Items.Count > 0)
                    cmbCase.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowMessage($"케이스 목록 로드 오류: {ex.Message}");
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
                ShowMessage($"데이터 조회 오류: {ex.Message}");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                gridContracts.UpdateEdit();
                ShowMessage("데이터가 저장되었습니다.");
                LoadData();
            }
            catch (Exception ex)
            {
                ShowMessage($"저장 오류: {ex.Message}");
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (gridContracts.FocusedRowIndex < 0)
                {
                    ShowMessage("삭제할 데이터를 선택하세요.");
                    return;
                }

                var contractId = gridContracts.GetRowValues(gridContracts.FocusedRowIndex, "CONTRACT_ID")?.ToString();
                var caseNo = gridContracts.GetRowValues(gridContracts.FocusedRowIndex, "CASE_NO")?.ToString();
                var companyNo = gridContracts.GetRowValues(gridContracts.FocusedRowIndex, "COMPANY_NO")?.ToString();

                if (string.IsNullOrEmpty(contractId) || string.IsNullOrEmpty(caseNo) || string.IsNullOrEmpty(companyNo))
                {
                    ShowMessage("필수 정보가 없습니다.");
                    return;
                }

                bool result = _xpoController.DeleteContract(caseNo, companyNo, contractId);

                if (result)
                {
                    ShowMessage("데이터가 삭제되었습니다.");
                    LoadData();
                }
                else
                {
                    ShowMessage("삭제 작업에 실패했습니다.");
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"삭제 오류: {ex.Message}");
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

                    string fileName = $"계약정보_XPO_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    gridExporter.WriteXlsxToResponse(fileName);
                }
                else
                {
                    ShowMessage("내보낼 데이터가 없습니다. 먼저 조회를 실행하세요.");
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"엑셀 내보내기 오류: {ex.Message}");
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

        protected void gridContracts_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                string userId = Session["UserID"]?.ToString() ?? "SYSTEM";
                string contractId = e.Keys["CONTRACT_ID"]?.ToString();
                string caseNo = e.OldValues["CASE_NO"]?.ToString();
                string companyNo = e.OldValues["COMPANY_NO"]?.ToString();

                if (string.IsNullOrEmpty(contractId) || string.IsNullOrEmpty(caseNo) || string.IsNullOrEmpty(companyNo))
                {
                    e.Cancel = true;
                    ShowMessage("필수 정보가 없습니다.");
                    return;
                }

                var contract = new ScmContractMaster
                {
                    CASE_NO = caseNo,
                    COMPANY_NO = companyNo,
                    CONTRACT_ID = contractId,
                    CONTRACT_NAME = e.NewValues["CONTRACT_NAME"]?.ToString(),
                    CONTRACT_NO = e.NewValues["CONTRACT_NO"]?.ToString(),
                    STD_CASE_NO = e.NewValues["STD_CASE_NO"]?.ToString() ?? "MASTER",
                    STD_CONTRACT_ID = e.NewValues["STD_CONTRACT_ID"]?.ToString(),
                    PROP1 = e.NewValues["PROP1"]?.ToString(),
                    PROP2 = e.NewValues["PROP2"]?.ToString(),
                    CONTRACT_CATEGORY = e.NewValues["CONTRACT_CATEGORY"]?.ToString() ?? "ING",
                    CONTRACT_TYPE = e.NewValues["CONTRACT_TYPE"]?.ToString() ?? "CONTRACTED",
                    POR_NO = e.NewValues["POR_NO"]?.ToString(),
                    SEQ_NO = e.NewValues["SEQ_NO"]?.ToString(),
                    POR_DT = e.NewValues["POR_DT"] as DateTime? ?? DateTime.Now,
                    CNTR_DT = e.NewValues["CNTR_DT"] as DateTime? ?? DateTime.Now,
                    CNTR_INIT_DT = e.NewValues["CNTR_INIT_DT"] as DateTime?,
                    MP_DT = e.NewValues["MP_DT"] as DateTime? ?? DateTime.Now,
                    MP_INIT_DT = e.NewValues["MP_INIT_DT"] as DateTime?,
                    PRODUCT_TYPE = e.NewValues["PRODUCT_TYPE"]?.ToString(),
                    PRODUCT_DESC = e.NewValues["PRODUCT_DESC"]?.ToString(),
                    CNTR_EA = e.NewValues["CNTR_EA"] as int? ?? 0,
                    CNTR_PIECE_WGT = e.NewValues["CNTR_PIECE_WGT"] as decimal? ?? 0,
                    CNTR_WGT = e.NewValues["CNTR_WGT"] as decimal? ?? 0,
                    PROJECT_NO = e.NewValues["PROJECT_NO"]?.ToString(),
                    BLOCK_NO = e.NewValues["BLOCK_NO"]?.ToString(),
                    MARK_NO = e.NewValues["MARK_NO"]?.ToString(),
                    OWNER = e.NewValues["OWNER"]?.ToString(),
                    TAG1 = e.NewValues["TAG1"]?.ToString(),
                    EST_ST_DT = e.NewValues["EST_ST_DT"] as DateTime?,
                    EST_FI_DT = e.NewValues["EST_FI_DT"] as DateTime?,
                    PLAN_ST_DT = e.NewValues["PLAN_ST_DT"] as DateTime?,
                    PLAN_FI_DT = e.NewValues["PLAN_FI_DT"] as DateTime?,
                    RESULT_ST_DT = e.NewValues["RESULT_ST_DT"] as DateTime?,
                    RESULT_FI_DT = e.NewValues["RESULT_FI_DT"] as DateTime?,
                    RMK = e.NewValues["RMK"]?.ToString()
                };

                bool result = _xpoController.SaveContract(contract, userId);

                if (result)
                {
                    ShowMessage("데이터가 수정되었습니다.");
                    LoadData();
                }
                else
                {
                    ShowMessage("수정에 실패했습니다.");
                }

                e.Cancel = true;
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                ShowMessage($"수정 오류: {ex.Message}");
            }
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

            List<ScmContractMaster> masterList = _xpoController.GetContractFullList(caseNo, companyNo, selectAll, dtStart, dtEnd);

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

        #region Helper
        private void ShowMessage(string message)
        {
            string script = $"alert('{message.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowMessage", script, true);
        }
        #endregion


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
    }
}
