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
    /// To-Do List 페이지 코드비하인드
    /// 작업지시 목록 조회 및 수정 기능을 제공합니다.
    /// </summary>
    public partial class ToDoList : System.Web.UI.Page
    {
        // 컨트롤러 인스턴스
        private ToDoListController _controller = new ToDoListController();
        private LoginController _loginController = new LoginController();

        // 세션 키
        private const string SESSION_KEY_DATA = "ToDoList_Data";

        /// <summary>
        /// 그리드 데이터 세션 관리
        /// </summary>
        private List<ToDoListViewModel> GridData
        {
            get { return Session[SESSION_KEY_DATA] as List<ToDoListViewModel>; }
            set { Session[SESSION_KEY_DATA] = value; }
        }

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            // 로그인 체크
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

        /// <summary>
        /// 페이지 초기화
        /// </summary>
        private void InitializePage()
        {
            try
            {
                // 콤보박스 초기화
                LoadCompanyTypeList();
                LoadCompanyList();

                // 기준일 기본값 (오늘)
                dtBaseDate.Value = DateTime.Today;

                // 그리드 데이터 초기화
                GridData = null;
            }
            catch (Exception ex)
            {
                ShowMessage($"페이지 초기화 오류: {ex.Message}");
            }
        }

        #endregion

        #region ComboBox 초기화

        /// <summary>
        /// Company 유형 콤보박스 초기화
        /// </summary>
        private void LoadCompanyTypeList()
        {
            cmbCompanyType.Items.Clear();
            cmbCompanyType.Items.Add("전체", "*");
            cmbCompanyType.Items.Add("협력사", "VENDOR");
            cmbCompanyType.Items.Add("MASTER", "MASTER");
            cmbCompanyType.SelectedIndex = 0;
        }

        /// <summary>
        /// Company 콤보박스 로드
        /// </summary>
        private void LoadCompanyList()
        {
            try
            {
                DataTable dt = _loginController.GetCompanyList();
                string companyType = cmbCompanyType.Value?.ToString() ?? "*";

                cmbCompany.Items.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    // 유형 필터링
                    if (companyType == "*" || row["COMPANY_TYPE"]?.ToString() == companyType)
                    {
                        string text = $"{row["COMPANY_NO"]} - {row["COMPANY_NAME"]}";
                        cmbCompany.Items.Add(text, row["COMPANY_NO"].ToString());
                    }
                }

                // 기본값 설정 (100 또는 첫번째 항목)
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

        /// <summary>
        /// 조회 버튼 클릭
        /// </summary>
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

        /// <summary>
        /// 초기화 버튼 클릭
        /// </summary>
        protected void btnReset_Click(object sender, EventArgs e)
        {
            // 콤보박스 초기화
            cmbCompanyType.SelectedIndex = 0;
            LoadCompanyList();

            // 기준일 초기화
            dtBaseDate.Value = DateTime.Today;

            // 그리드 초기화
            GridData = null;
            gridToDoList.DataSource = null;
            gridToDoList.DataBind();
            lblRecordCount.Text = "조회된 데이터가 없습니다.";
        }

        /// <summary>
        /// 저장 버튼 클릭 (BatchEdit 저장)
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // BatchEdit 모드에서 UpdateEdit 호출하여 변경사항 서버로 전송
                gridToDoList.UpdateEdit();
                ShowMessage("데이터가 저장되었습니다.");
                LoadData();
            }
            catch (Exception ex)
            {
                ShowMessage($"저장 오류: {ex.Message}");
            }
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

                // Key 값 가져오기
                var orderNo = gridToDoList.GetRowValues(gridToDoList.FocusedRowIndex, "ORDER_NO")?.ToString();
                var caseNo = gridToDoList.GetRowValues(gridToDoList.FocusedRowIndex, "CASE_NO")?.ToString();
                var companyNo = gridToDoList.GetRowValues(gridToDoList.FocusedRowIndex, "COMPANY_NO")?.ToString();

                if (string.IsNullOrEmpty(orderNo) || string.IsNullOrEmpty(caseNo) || string.IsNullOrEmpty(companyNo))
                {
                    ShowMessage("필수 정보가 없습니다.");
                    return;
                }

                bool result = _controller.DeleteWorkOrder(caseNo, companyNo, orderNo);

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

        /// <summary>
        /// 엑셀 다운로드 버튼 클릭
        /// </summary>
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
        /// 행 업데이트 이벤트 (단일 행 수정 시)
        /// </summary>
        protected void gridToDoList_RowUpdating(object sender, ASPxDataUpdatingEventArgs e)
        {
            try
            {
                string userId = Session["UserID"]?.ToString() ?? "SYSTEM";
                string orderNo = e.Keys["ORDER_NO"]?.ToString();
                string caseNo = e.OldValues["CASE_NO"]?.ToString();
                string companyNo = e.OldValues["COMPANY_NO"]?.ToString();

                // 수정 가능 필드 값 가져오기
                DateTime? compDate = e.NewValues["COMP_DATE"] as DateTime?;
                decimal? planMhr = e.NewValues["PLAN_MHR"] as decimal?;
                decimal? realMhr = e.NewValues["REAL_MHR"] as decimal?;
                decimal? planMp = e.NewValues["PLAN_MP"] as decimal?;
                decimal? realMp = e.NewValues["REAL_MP"] as decimal?;

                bool result = _controller.UpdateWorkOrder(
                    caseNo, companyNo, orderNo,
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

                e.Cancel = true;
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                ShowMessage($"수정 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// BatchEdit 일괄 업데이트 이벤트
        /// </summary>
        protected void gridToDoList_BatchUpdate(object sender, ASPxDataBatchUpdateEventArgs e)
        {
            try
            {
                string userId = Session["UserID"]?.ToString() ?? "SYSTEM";
                List<ToDoListViewModel> updateItems = new List<ToDoListViewModel>();

                // 수정된 행 처리
                foreach (var item in e.UpdateValues)
                {
                    var vm = new ToDoListViewModel
                    {
                        ORDER_NO = item.Keys["ORDER_NO"]?.ToString(),
                        CASE_NO = item.OldValues["CASE_NO"]?.ToString(),
                        COMPANY_NO = item.OldValues["COMPANY_NO"]?.ToString(),
                        COMP_DATE = item.NewValues["COMP_DATE"] as DateTime?,
                        PLAN_MHR = item.NewValues["PLAN_MHR"] as decimal?,
                        REAL_MHR = item.NewValues["REAL_MHR"] as decimal?,
                        PLAN_MP = item.NewValues["PLAN_MP"] as decimal?,
                        REAL_MP = item.NewValues["REAL_MP"] as decimal?
                    };
                    updateItems.Add(vm);
                }

                if (updateItems.Count > 0)
                {
                    int successCount = _controller.BatchUpdateWorkOrders(updateItems, userId);
                    ShowMessage($"{successCount}건의 데이터가 저장되었습니다.");
                }

                e.Handled = true;
            }
            catch (Exception ex)
            {
                ShowMessage($"일괄 저장 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 행 렌더링 시 상태에 따른 스타일 적용
        /// </summary>
        protected void gridToDoList_HtmlRowPrepared(object sender, ASPxGridViewTableRowEventArgs e)
        {
            if (e.RowType != GridViewRowType.Data) return;

            // 상태 컬럼 인덱스 찾기
            int statusIndex = gridToDoList.Columns.IndexOf(gridToDoList.Columns["STATUS"]);
            if (statusIndex >= 0)
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

            // 컨트롤러에서 데이터 조회
            List<ToDoListViewModel> dataList = _controller.GetToDoList(companyNo, userId, baseDate);

            // 세션에 저장
            GridData = dataList;

            // 그리드 바인딩
            gridToDoList.DataSource = dataList;
            gridToDoList.DataBind();

            // 건수 표시
            lblRecordCount.Text = $"총 <strong>{dataList.Count}</strong>건의 데이터가 조회되었습니다.";
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// 메시지 표시 (클라이언트 Alert)
        /// </summary>
        private void ShowMessage(string message)
        {
            string script = $"alert('{message.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "alertScript", script, true);
        }

        #endregion
    }
}