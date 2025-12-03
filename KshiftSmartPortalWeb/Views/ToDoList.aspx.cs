using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Data;
using KShiftSmartPortal.ViewModels;
using KShiftSmartPortalWeb.Controllers;

namespace KShiftSmartPortalWeb
{
    /// <summary>
    /// To-Do List 페이지 코드비하인드 (XPO + PopupEditForm 방식)
    /// ContractManagerXpo와 동일한 패턴 적용
    /// </summary>
    public partial class ToDoList : System.Web.UI.Page
    {
        private ToDoListController _controller = new ToDoListController();
        private LoginController _loginController = new LoginController();

        private const string SESSION_KEY_DATA = "ToDoList_Data";

        private List<ToDoListViewModel> GridData
        {
            get { return Session[SESSION_KEY_DATA] as List<ToDoListViewModel>; }
            set { Session[SESSION_KEY_DATA] = value; }
        }

        #region Page Events

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
                dtBaseDate.Value = DateTime.Today;
                GridData = null;
            }
            catch (Exception ex)
            {
                ShowMessage($"페이지 초기화 오류: {ex.Message}");
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

                if (cmbCompany.Items.FindByValue("100") != null)
                    cmbCompany.Value = "100";
                else if (cmbCompany.Items.Count > 0)
                    cmbCompany.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowMessage($"Company 목록 로드 오류: {ex.Message}");
            }
        }

        protected void cmbCompanyType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCompanyList();
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

        protected void btnReset_Click(object sender, EventArgs e)
        {
            cmbCompanyType.SelectedIndex = 0;
            LoadCompanyList();
            dtBaseDate.Value = DateTime.Today;

            GridData = null;
            gridToDoList.DataSource = null;
            gridToDoList.DataBind();
            lblRecordCount.Text = "조회된 데이터가 없습니다.";
        }

        /// <summary>
        /// 삭제 버튼 클릭
        /// </summary>
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (gridToDoList.FocusedRowIndex < 0)
                {
                    ShowMessage("삭제할 데이터를 선택하세요.");
                    return;
                }

                // 4개 복합키 모두 가져오기
                object[] keyValues = (object[])gridToDoList.GetRowValues(gridToDoList.FocusedRowIndex,
                    new string[] { "COMPANY_NO", "CASE_NO", "PROJECT_NO", "ORDER_NO" });

                string companyNo = keyValues[0]?.ToString();
                string caseNo = keyValues[1]?.ToString();
                string projectNo = keyValues[2]?.ToString();
                string orderNo = keyValues[3]?.ToString();

                if (string.IsNullOrEmpty(orderNo) || string.IsNullOrEmpty(caseNo) ||
                    string.IsNullOrEmpty(companyNo) || string.IsNullOrEmpty(projectNo))
                {
                    ShowMessage("필수 정보가 없습니다.");
                    return;
                }

                bool result = _controller.DeleteWorkOrder(caseNo, companyNo, projectNo, orderNo);

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

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (GridData != null && GridData.Count > 0)
                {
                    gridToDoList.DataSource = GridData;
                    gridToDoList.DataBind();

                    string fileName = $"ToDoList_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
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

        protected void gridToDoList_PageIndexChanged(object sender, EventArgs e)
        {
            BindGridFromSession();
        }

        protected void gridToDoList_BeforeColumnSortingGrouping(object sender, ASPxGridViewBeforeColumnGroupingSortingEventArgs e)
        {
            BindGridFromSession();
        }

        protected void gridToDoList_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            BindGridFromSession();
        }

        /// <summary>
        /// PopupEditForm에서 저장 버튼 클릭 시 호출
        /// </summary>
        protected void gridToDoList_RowUpdating(object sender, ASPxDataUpdatingEventArgs e)
        {
            try
            {
                string userId = Session["UserID"]?.ToString() ?? "SYSTEM";

                // 4개 복합키 모두 e.Keys에서 가져오기 (KeyFieldName이 복합키로 설정됨)
                string companyNo = e.Keys["COMPANY_NO"]?.ToString();
                string caseNo = e.Keys["CASE_NO"]?.ToString();
                string projectNo = e.Keys["PROJECT_NO"]?.ToString();
                string orderNo = e.Keys["ORDER_NO"]?.ToString();

                if (string.IsNullOrEmpty(orderNo) || string.IsNullOrEmpty(caseNo) ||
                    string.IsNullOrEmpty(companyNo) || string.IsNullOrEmpty(projectNo))
                {
                    e.Cancel = true;
                    ShowMessage($"필수 정보가 없습니다. (Company:{companyNo}, Case:{caseNo}, Project:{projectNo}, Order:{orderNo})");
                    return;
                }

                // 수정 가능 필드 값 가져오기
                DateTime? compDate = e.NewValues["COMP_DATE"] as DateTime?;
                decimal? planMhr = e.NewValues["PLAN_MHR"] as decimal?;
                decimal? realMhr = e.NewValues["REAL_MHR"] as decimal?;
                decimal? planMp = e.NewValues["PLAN_MP"] as decimal?;
                decimal? realMp = e.NewValues["REAL_MP"] as decimal?;

                // XPO 방식으로 업데이트
                bool result = _controller.UpdateWorkOrder(
                    caseNo, companyNo, projectNo, orderNo,
                    compDate, planMhr, realMhr, planMp, realMp,
                    userId);

                if (result)
                {
                    ShowMessage("데이터가 수정되었습니다.");
                    LoadData();
                }
                else
                {
                    ShowMessage("수정에 실패했습니다.");
                }

                // e.Cancel 설정하지 않음 (자동으로 팝업 닫힘)
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                ShowMessage($"수정 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 행 렌더링 시 상태에 따른 스타일 적용
        /// </summary>
        protected void gridToDoList_HtmlRowPrepared(object sender, ASPxGridViewTableRowEventArgs e)
        {
            if (e.RowType != GridViewRowType.Data) return;

            // STATUS 컬럼 찾기
            var statusColumn = gridToDoList.Columns["STATUS"] as GridViewDataColumn;
            if (statusColumn != null)
            {
                int statusIndex = statusColumn.VisibleIndex;
                if (statusIndex >= 0 && statusIndex < e.Row.Cells.Count)
                {
                    string status = e.GetValue("STATUS")?.ToString();
                    switch (status)
                    {
                        case "완료":
                            e.Row.Cells[statusIndex].CssClass = "status-complete";
                            break;
                        case "진행중":
                            e.Row.Cells[statusIndex].CssClass = "status-inprogress";
                            break;
                        case "예정":
                        case "미정":
                            e.Row.Cells[statusIndex].CssClass = "status-pending";
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 세션에서 그리드 데이터 바인딩
        /// </summary>
        private void BindGridFromSession()
        {
            if (GridData != null)
            {
                gridToDoList.DataSource = GridData;
                gridToDoList.DataBind();
            }
        }

        #endregion

        #region Data Methods

        /// <summary>
        /// 데이터 조회
        /// </summary>
        private void LoadData()
        {
            string companyNo = cmbCompany.Value?.ToString();
            string userId = Session["UserID"]?.ToString();
            DateTime baseDate = dtBaseDate.Value != null ? (DateTime)dtBaseDate.Value : DateTime.Today;

            if (string.IsNullOrEmpty(companyNo))
            {
                ShowMessage("Company를 선택하세요.");
                return;
            }

            if (string.IsNullOrEmpty(userId))
            {
                ShowMessage("로그인 정보가 없습니다.");
                return;
            }

            // XPO 방식 조회
            List<ToDoListViewModel> dataList = _controller.GetToDoList(companyNo, userId, baseDate);

            GridData = dataList;
            gridToDoList.DataSource = dataList;
            gridToDoList.DataBind();

            lblRecordCount.Text = $"총 <strong>{dataList.Count}</strong>건의 데이터가 조회되었습니다.";
        }

        #endregion

        #region Helper Methods

        private void ShowMessage(string message)
        {
            string script = $"alert('{message.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "alertScript", script, true);
        }

        #endregion
    }
}