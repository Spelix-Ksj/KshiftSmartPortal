using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Bootstrap;
using DevExpress.Web.Data;
using KShiftSmartPortal.ViewModels;
using KShiftSmartPortalWeb.Controllers;

namespace KShiftSmartPortalWeb
{
    /// <summary>
    /// To-Do List 페이지 코드비하인드 (Bootstrap + XPO + PopupEditForm 방식)
    /// 모바일 최적화 버전
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
                Response.Redirect("~/Views/Login.aspx", false);
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

                // 등록 폼 초기화
                InitializeAddForm();
            }
            catch (Exception ex)
            {
                ShowMessage($"페이지 초기화 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 등록 폼 콤보박스 초기화
        /// </summary>
        private void InitializeAddForm()
        {
            try
            {
                string companyNo = cmbCompany.Value != null ? cmbCompany.Value.ToString() : "100";

                // 케이스 목록 로드
                DataTable dtCase = _controller.GetActiveCaseList(companyNo);
                cmbAddCase.Items.Clear();
                foreach (DataRow row in dtCase.Rows)
                {
                    string text = $"{row["CASE_NO"]} - {row["CASE_NAME"]}";
                    cmbAddCase.Items.Add(text, row["CASE_NO"].ToString());
                }
                if (cmbAddCase.Items.Count > 0)
                    cmbAddCase.SelectedIndex = 0;

                // 프로젝트 목록 로드
                DataTable dtProject = _controller.GetProjectList(companyNo);
                cmbAddProject.Items.Clear();
                foreach (DataRow row in dtProject.Rows)
                {
                    string text = $"{row["PROJECT_NO"]}";
                    if (row["PROJECT_NAME"] != DBNull.Value && !string.IsNullOrEmpty(row["PROJECT_NAME"].ToString()))
                        text += $" ({row["PROJECT_NAME"]})";
                    cmbAddProject.Items.Add(text, row["PROJECT_NO"].ToString());
                }
                if (cmbAddProject.Items.Count > 0)
                    cmbAddProject.SelectedIndex = 0;

                // 날짜 기본값 설정
                dtAddWorkSt.Value = DateTime.Today;
                dtAddWorkFi.Value = DateTime.Today;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"등록 폼 초기화 오류: {ex.Message}");
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
                string companyType = cmbCompanyType.Value != null ? cmbCompanyType.Value.ToString() : "*";

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

            // 모바일 카드 뷰도 초기화
            rptMobileCards.DataSource = null;
            rptMobileCards.DataBind();
            pnlNoData.Visible = true;
            lblMobilePageInfo.Text = "0 / 0";

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

        /// <summary>
        /// 작업 실적 등록 버튼 클릭
        /// </summary>
        protected void btnAddSave_Click(object sender, EventArgs e)
        {
            try
            {
                string companyNo = cmbCompany.Value != null ? cmbCompany.Value.ToString() : null;
                string caseNo = cmbAddCase.Value != null ? cmbAddCase.Value.ToString() : null;
                string projectNo = cmbAddProject.Value != null ? cmbAddProject.Value.ToString() : null;
                string orderName = txtAddOrderName.Text?.Trim();
                string workList = txtAddWorkList.Text?.Trim();
                string rmk = txtAddRmk.Text?.Trim();
                string userId = Session["UserID"] != null ? Session["UserID"].ToString() : null;

                // 유효성 검사
                if (string.IsNullOrEmpty(companyNo))
                {
                    ShowMessage("Company를 선택하세요.");
                    return;
                }
                if (string.IsNullOrEmpty(caseNo))
                {
                    ShowMessage("케이스를 선택하세요.");
                    return;
                }
                if (string.IsNullOrEmpty(projectNo))
                {
                    ShowMessage("프로젝트를 선택하세요.");
                    return;
                }
                if (string.IsNullOrEmpty(orderName))
                {
                    ShowMessage("작업 내용을 입력하세요.");
                    return;
                }
                if (string.IsNullOrEmpty(userId))
                {
                    ShowMessage("로그인 정보가 없습니다.");
                    return;
                }

                // 날짜 및 숫자 값 가져오기
                DateTime? workSt = dtAddWorkSt.Value as DateTime?;
                DateTime? workFi = dtAddWorkFi.Value as DateTime?;
                DateTime? compDate = dtAddCompDate.Value as DateTime?;
                decimal? planMhr = spnAddPlanMhr.Number as decimal?;
                decimal? realMhr = spnAddRealMhr.Number as decimal?;
                decimal? planMp = spnAddPlanMp.Number as decimal?;
                decimal? realMp = spnAddRealMp.Number as decimal?;

                // 등록 처리
                bool result = _controller.InsertWorkOrder(
                    companyNo, caseNo, projectNo,
                    orderName, workList,
                    workSt, workFi,
                    planMhr, realMhr, planMp, realMp,
                    compDate, rmk,
                    userId);

                if (result)
                {
                    ShowMessage("작업 실적이 등록되었습니다.");

                    // 폼 초기화 및 데이터 새로고침
                    ClearAddForm();
                    LoadData();
                }
                else
                {
                    ShowMessage("등록에 실패했습니다.");
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"등록 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 등록 폼 필드 초기화
        /// </summary>
        private void ClearAddForm()
        {
            txtAddOrderName.Text = "";
            txtAddWorkList.Text = "";
            txtAddRmk.Text = "";
            dtAddWorkSt.Value = DateTime.Today;
            dtAddWorkFi.Value = DateTime.Today;
            dtAddCompDate.Value = null;
            spnAddPlanMhr.Number = 0;
            spnAddRealMhr.Number = 0;
            spnAddPlanMp.Number = 0;
            spnAddRealMp.Number = 0;
        }

        #endregion

        #region Grid Events

        protected void gridToDoList_PageIndexChanged(object sender, EventArgs e)
        {
            BindGridFromSession();
        }

        protected void gridToDoList_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            BindGridFromSession();

            // 모바일 카드에서 수정 버튼 클릭 시 처리
            if (!string.IsNullOrEmpty(e.Parameters) && e.Parameters.StartsWith("EDIT:"))
            {
                string keyData = e.Parameters.Substring(5); // "EDIT:" 제거
                string[] keys = keyData.Split('|');
                if (keys.Length == 4)
                {
                    string companyNo = keys[0];
                    string caseNo = keys[1];
                    string projectNo = keys[2];
                    string orderNo = keys[3];

                    // GridData에서 해당 행의 인덱스 찾기
                    if (GridData != null)
                    {
                        for (int i = 0; i < GridData.Count; i++)
                        {
                            var item = GridData[i];
                            if (item.COMPANY_NO == companyNo && item.CASE_NO == caseNo &&
                                item.PROJECT_NO == projectNo && item.ORDER_NO == orderNo)
                            {
                                gridToDoList.StartEdit(i);
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// PopupEditForm에서 저장 버튼 클릭 시 호출
        /// </summary>
        protected void gridToDoList_RowUpdating(object sender, ASPxDataUpdatingEventArgs e)
        {
            // List<T> 데이터소스는 기본 업데이트를 지원하지 않으므로 항상 Cancel
            e.Cancel = true;

            try
            {
                string userId = Session["UserID"] != null ? Session["UserID"].ToString() : "SYSTEM";

                // 4개 복합키 모두 e.Keys에서 가져오기 (KeyFieldName이 복합키로 설정됨)
                string companyNo = e.Keys["COMPANY_NO"] != null ? e.Keys["COMPANY_NO"].ToString() : null;
                string caseNo = e.Keys["CASE_NO"] != null ? e.Keys["CASE_NO"].ToString() : null;
                string projectNo = e.Keys["PROJECT_NO"] != null ? e.Keys["PROJECT_NO"].ToString() : null;
                string orderNo = e.Keys["ORDER_NO"] != null ? e.Keys["ORDER_NO"].ToString() : null;

                if (string.IsNullOrEmpty(orderNo) || string.IsNullOrEmpty(caseNo) ||
                    string.IsNullOrEmpty(companyNo) || string.IsNullOrEmpty(projectNo))
                {
                    ShowMessageCallback("필수 정보가 없습니다.");
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
                    // 저장 성공: 팝업 닫기 및 데이터 새로고침
                    gridToDoList.CancelEdit();
                    LoadData();
                    ShowMessageCallback("데이터가 수정되었습니다.");
                    // 모바일 뷰에서 새로고침 필요 플래그
                    gridToDoList.JSProperties["cpNeedRefresh"] = true;
                }
                else
                {
                    ShowMessageCallback("수정에 실패했습니다.");
                }
            }
            catch (Exception ex)
            {
                ShowMessageCallback($"수정 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 행 렌더링 시 상태에 따른 스타일 적용
        /// </summary>
        protected void gridToDoList_HtmlRowPrepared(object sender, ASPxGridViewTableRowEventArgs e)
        {
            if (e.RowType != GridViewRowType.Data) return;

            // STATUS 값 가져오기
            object statusObj = e.GetValue("STATUS");
            string status = statusObj != null ? statusObj.ToString() : null;

            // 상태에 따른 행 스타일 적용
            switch (status)
            {
                case "완료":
                    e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#d4edda");
                    break;
                case "진행중":
                    e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#fff3cd");
                    break;
                case "예정":
                case "미정":
                    e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#f8d7da");
                    break;
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

                // 모바일 카드 뷰도 바인딩
                BindMobileCards();
            }
        }

        /// <summary>
        /// 모바일 카드 뷰 바인딩
        /// </summary>
        private void BindMobileCards()
        {
            if (GridData != null && GridData.Count > 0)
            {
                rptMobileCards.DataSource = GridData;
                rptMobileCards.DataBind();
                pnlNoData.Visible = false;

                // 페이지 정보 업데이트
                int pageSize = gridToDoList.SettingsPager.PageSize;
                int totalPages = (int)Math.Ceiling((double)GridData.Count / pageSize);
                int currentPage = gridToDoList.PageIndex + 1;
                lblMobilePageInfo.Text = $"{currentPage} / {totalPages}";
            }
            else
            {
                rptMobileCards.DataSource = null;
                rptMobileCards.DataBind();
                pnlNoData.Visible = true;
                lblMobilePageInfo.Text = "0 / 0";
            }
        }

        #endregion

        #region Data Methods

        /// <summary>
        /// 데이터 조회
        /// </summary>
        private void LoadData()
        {
            string companyNo = cmbCompany.Value != null ? cmbCompany.Value.ToString() : null;
            string userId = Session["UserID"] != null ? Session["UserID"].ToString() : null;
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

            // 모바일 카드 뷰 바인딩
            BindMobileCards();

            lblRecordCount.Text = $"총 <strong>{dataList.Count}</strong>건의 데이터가 조회되었습니다.";
        }

        #endregion

        #region Helper Methods

        private void ShowMessage(string message)
        {
            string script = $"alert('{message.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "alertScript", script, true);
        }

        /// <summary>
        /// 콜백 모드에서 메시지 표시 (JSProperties 사용)
        /// </summary>
        private void ShowMessageCallback(string message)
        {
            gridToDoList.JSProperties["cpMessage"] = message;
        }

        /// <summary>
        /// 모바일 카드 상태 배지 CSS 클래스 반환
        /// </summary>
        protected string GetStatusBadgeClass(string status)
        {
            switch (status)
            {
                case "완료":
                    return "badge-complete";
                case "진행중":
                    return "badge-inprogress";
                case "예정":
                case "미정":
                default:
                    return "badge-pending";
            }
        }

        #endregion
    }
}
