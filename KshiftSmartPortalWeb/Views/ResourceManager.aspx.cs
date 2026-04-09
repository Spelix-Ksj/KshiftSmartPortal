using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Bootstrap;
using DevExpress.Web.Data;
using KShiftSmartPortal.ViewModels;
using KShiftSmartPortalWeb.Controllers;
using KShiftSmartPortalWeb.Utils;

namespace KShiftSmartPortalWeb
{
    /// <summary>
    /// 리소스 관리 페이지 코드비하인드 (Bootstrap GridView + PopupEditForm 방식)
    /// 트리 구조는 ResLevel 기반 들여쓰기(padding-left)로 표현
    /// </summary>
    public partial class ResourceManager : BasePage
    {
        private ResourceManagerController _controller = new ResourceManagerController();

        private const string SESSION_KEY_DATA = "ResourceManager_Data";

        private List<ResourceViewModel> GridData
        {
            get { return Session[SESSION_KEY_DATA] as List<ResourceViewModel>; }
            set { Session[SESSION_KEY_DATA] = value; }
        }

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!CheckSession()) return;

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
                // 검색 패널 Company 콤보박스 초기화
                LoadCompanyCombo();

                // Case 콤보박스 초기화
                LoadCaseList();

                // 등록 폼 콤보박스 초기화
                InitializeAddForm();

                GridData = null;
            }
            catch (Exception ex)
            {
                ShowMessage($"페이지 초기화 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// Company 콤보박스 로드
        /// </summary>
        private void LoadCompanyCombo()
        {
            try
            {
                DataTable dt = _controller.GetCompanyList();
                string defaultCompanyNo = Session["CompanyNo"] != null ? Session["CompanyNo"].ToString() : null;

                ComboBoxHelper.LoadCompanyCombo(cmbCompany, dt, "*", defaultCompanyNo);
            }
            catch (Exception ex)
            {
                ShowMessage($"Company 목록 로드 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// Case 콤보박스 로드 (선택된 회사 기준)
        /// </summary>
        private void LoadCaseList()
        {
            try
            {
                cmbCase.Items.Clear();

                string companyNo = cmbCompany.Value?.ToString();
                if (string.IsNullOrEmpty(companyNo))
                {
                    return;
                }

                DataTable dt = _controller.GetCaseList(companyNo);

                foreach (DataRow row in dt.Rows)
                {
                    string caseNo = row["CASE_NO"]?.ToString();
                    string caseName = row["CASE_NAME"]?.ToString();
                    cmbCase.Items.Add(caseName, caseNo);
                }

                if (cmbCase.Items.Count > 0)
                {
                    cmbCase.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"케이스 목록 로드 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 등록 폼 콤보박스 초기화
        /// </summary>
        private void InitializeAddForm()
        {
            try
            {
                // 사용여부 콤보
                cmbAddUseYn.Items.Clear();
                cmbAddUseYn.Items.Add("사용", "Y");
                cmbAddUseYn.Items.Add("미사용", "N");
                cmbAddUseYn.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"등록 폼 초기화 오류: {ex.Message}");
            }
        }

        #endregion

        #region Button Events

        /// <summary>
        /// Company 콤보 변경 시 Case 콤보 갱신
        /// </summary>
        protected void cmbCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCaseList();
        }

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
        /// 삭제 버튼 클릭
        /// </summary>
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (!EnsureAuthenticated()) return;

            try
            {
                string resourceNo = null;

                // 모바일 카드 선택 확인 (hidden field 우선)
                string hdnResNo = Request.Form["hdnSelResourceNo"];
                if (!string.IsNullOrEmpty(hdnResNo))
                {
                    resourceNo = hdnResNo;
                }
                else if (gridResource.FocusedRowIndex >= 0)
                {
                    // 데스크탑 그리드에서 선택된 행
                    object keyValue = gridResource.GetRowValues(gridResource.FocusedRowIndex, "ResourceNo");
                    resourceNo = keyValue?.ToString();
                }
                else
                {
                    ShowMessage("삭제할 리소스를 선택하세요.");
                    return;
                }

                if (string.IsNullOrEmpty(resourceNo))
                {
                    ShowMessage("리소스번호 정보가 없습니다.");
                    return;
                }

                // 현재 선택된 회사번호, 케이스번호 가져오기
                string companyNo = cmbCompany.Value?.ToString();
                string caseNo = cmbCase.Value?.ToString();

                if (string.IsNullOrEmpty(companyNo))
                {
                    ShowMessage("회사를 먼저 선택하세요.");
                    return;
                }
                if (string.IsNullOrEmpty(caseNo))
                {
                    ShowMessage("케이스를 먼저 선택하세요.");
                    return;
                }

                bool result = _controller.DeleteResource(caseNo, companyNo, resourceNo);

                if (result)
                {
                    ShowMessage("리소스가 삭제되었습니다.");
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
                    gridResource.DataSource = GridData;
                    gridResource.DataBind();

                    string fileName = $"ResourceList_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
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
        /// 리소스 등록 버튼 클릭
        /// </summary>
        protected void btnAddSave_Click(object sender, EventArgs e)
        {
            if (!EnsureAuthenticated()) return;

            try
            {
                string resourceNo = txtAddResourceNo.Text?.Trim();
                string resourceName = txtAddResourceName.Text?.Trim();
                string userId = Session["UserID"]?.ToString();

                // 현재 선택된 회사번호, 케이스번호
                string companyNo = cmbCompany.Value?.ToString();
                string caseNo = cmbCase.Value?.ToString();

                // 유효성 검사
                if (string.IsNullOrEmpty(companyNo))
                {
                    ShowMessage("회사를 먼저 선택하세요.");
                    return;
                }
                if (string.IsNullOrEmpty(caseNo))
                {
                    ShowMessage("케이스를 먼저 선택하세요.");
                    return;
                }
                if (string.IsNullOrEmpty(resourceNo))
                {
                    ShowMessage("리소스번호를 입력하세요.");
                    return;
                }
                if (string.IsNullOrEmpty(resourceName))
                {
                    ShowMessage("리소스명을 입력하세요.");
                    return;
                }
                if (string.IsNullOrEmpty(userId))
                {
                    ShowMessage("로그인 정보가 없습니다.");
                    return;
                }

                // 중복 체크
                if (_controller.IsResourceNoDuplicate(caseNo, companyNo, resourceNo))
                {
                    ShowMessage("이미 존재하는 리소스번호입니다.");
                    return;
                }

                var model = new ResourceViewModel
                {
                    CaseNo = caseNo,
                    CompanyNo = companyNo,
                    ResourceNo = resourceNo,
                    ResourceName = resourceName,
                    ResourceType = txtAddResourceType.Text?.Trim(),
                    ParentResourceNo = txtAddParentResourceNo.Text?.Trim(),
                    OrgNo = txtAddOrgNo.Text?.Trim(),
                    ResLevel = (int)spnAddResLevel.Number,
                    UseYn = cmbAddUseYn.Value?.ToString() ?? "Y",
                    ViewOrder = (double)spnAddViewOrder.Number,
                    Rmk = txtAddRmk.Text?.Trim()
                };

                bool result = _controller.InsertResource(model, userId);

                if (result)
                {
                    ShowMessage("리소스가 등록되었습니다.");
                    ClearAddForm();
                    LoadData();
                }
                else
                {
                    ShowMessage("등록에 실패했습니다. 리소스번호 중복 여부를 확인하세요.");
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
            txtAddResourceNo.Text = "";
            txtAddResourceName.Text = "";
            txtAddResourceType.Text = "";
            txtAddParentResourceNo.Text = "";
            txtAddOrgNo.Text = "";
            spnAddResLevel.Number = 1;
            if (cmbAddUseYn.Items.Count > 0)
                cmbAddUseYn.SelectedIndex = 0;
            spnAddViewOrder.Number = 0;
            txtAddRmk.Text = "";
        }

        #endregion

        #region Grid Events

        protected void gridResource_PageIndexChanged(object sender, EventArgs e)
        {
            BindGridFromSession();
        }

        protected void gridResource_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            BindGridFromSession();

            if (string.IsNullOrEmpty(e.Parameters)) return;

            // 중복 체크 콜백 (3개 PK 전달: DUPCHECK:caseNo|companyNo|resourceNo)
            if (e.Parameters.StartsWith("DUPCHECK:"))
            {
                string paramStr = e.Parameters.Substring(9);
                string[] parts = paramStr.Split('|');

                if (parts.Length < 3)
                {
                    gridResource.JSProperties["cpMessage"] = "중복 체크 파라미터가 올바르지 않습니다.";
                    return;
                }

                string caseNo = parts[0];
                string companyNo = parts[1];
                string resourceNo = parts[2];

                if (string.IsNullOrEmpty(companyNo))
                {
                    gridResource.JSProperties["cpMessage"] = "회사를 먼저 선택하세요.";
                    return;
                }
                if (string.IsNullOrEmpty(caseNo))
                {
                    gridResource.JSProperties["cpMessage"] = "케이스를 먼저 선택하세요.";
                    return;
                }

                bool isDuplicate = _controller.IsResourceNoDuplicate(caseNo, companyNo, resourceNo);

                if (isDuplicate)
                {
                    gridResource.JSProperties["cpMessage"] = "이미 존재하는 리소스번호입니다.";
                    gridResource.JSProperties["cpDupResult"] = "DUPLICATE";
                }
                else
                {
                    gridResource.JSProperties["cpMessage"] = "사용 가능한 리소스번호입니다.";
                    gridResource.JSProperties["cpDupResult"] = "OK";
                }
            }
            // 모바일 수정 콜백
            else if (e.Parameters.StartsWith("EDIT:"))
            {
                string resourceNo = e.Parameters.Substring(5);

                if (GridData != null)
                {
                    for (int i = 0; i < GridData.Count; i++)
                    {
                        if (GridData[i].ResourceNo == resourceNo)
                        {
                            gridResource.StartEdit(i);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// PopupEditForm에서 저장 버튼 클릭 시 호출
        /// </summary>
        protected void gridResource_RowUpdating(object sender, ASPxDataUpdatingEventArgs e)
        {
            e.Cancel = true;

            if (Session["UserID"] == null)
            {
                ShowMessageCallback(gridResource, "세션이 만료되었습니다. 다시 로그인해주세요.");
                return;
            }

            try
            {
                string userId = Session["UserID"].ToString();
                string resourceNo = e.Keys["ResourceNo"]?.ToString();

                if (string.IsNullOrEmpty(resourceNo))
                {
                    ShowMessageCallback(gridResource, "리소스번호 정보가 없습니다.");
                    return;
                }

                string companyNo = cmbCompany.Value?.ToString();
                string caseNo = cmbCase.Value?.ToString();

                if (string.IsNullOrEmpty(companyNo))
                {
                    ShowMessageCallback(gridResource, "회사 정보가 없습니다.");
                    return;
                }
                if (string.IsNullOrEmpty(caseNo))
                {
                    ShowMessageCallback(gridResource, "케이스 정보가 없습니다.");
                    return;
                }

                // 기존 데이터 조회
                var existing = _controller.GetResourceByKey(caseNo, companyNo, resourceNo);
                if (existing == null)
                {
                    ShowMessageCallback(gridResource, "리소스 정보를 찾을 수 없습니다.");
                    return;
                }

                // 수정 필드 반영
                existing.ResourceName = e.NewValues["ResourceName"]?.ToString() ?? existing.ResourceName;
                existing.ResourceType = e.NewValues["ResourceType"]?.ToString();
                existing.ParentResourceNo = e.NewValues["ParentResourceNo"]?.ToString();
                existing.OrgNo = e.NewValues["OrgNo"]?.ToString();
                existing.UseYn = e.NewValues["UseYn"]?.ToString() ?? existing.UseYn;
                existing.Rmk = e.NewValues["Rmk"]?.ToString();

                if (e.NewValues["ResLevel"] != null)
                {
                    existing.ResLevel = Convert.ToInt32(e.NewValues["ResLevel"]);
                }

                if (e.NewValues["ViewOrder"] != null)
                {
                    existing.ViewOrder = Convert.ToDouble(e.NewValues["ViewOrder"]);
                }

                bool result = _controller.UpdateResource(existing, userId);

                if (result)
                {
                    gridResource.CancelEdit();
                    LoadData();
                    ShowMessageCallback(gridResource, "리소스 정보가 수정되었습니다.");
                    gridResource.JSProperties["cpNeedRefresh"] = true;
                }
                else
                {
                    ShowMessageCallback(gridResource, "수정에 실패했습니다.");
                }
            }
            catch (Exception ex)
            {
                ShowMessageCallback(gridResource, $"수정 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// ResourceName 컬럼에 ResLevel 기반 들여쓰기 적용
        /// </summary>
        protected void gridResource_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            if (e.DataColumn.FieldName == "ResourceName")
            {
                object levelObj = e.GetValue("ResLevel");
                int level = 1;
                if (levelObj != null && levelObj != DBNull.Value)
                {
                    level = Convert.ToInt32(levelObj);
                }

                if (level > 1)
                {
                    int paddingLeft = (level - 1) * 25;
                    e.Cell.Style.Add("padding-left", paddingLeft + "px");
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
                gridResource.DataSource = GridData;
                gridResource.DataBind();

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

                int pageSize = gridResource.SettingsPager.PageSize;
                int totalPages = (int)Math.Ceiling((double)GridData.Count / pageSize);
                int currentPage = gridResource.PageIndex + 1;
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
            string companyNo = cmbCompany.Value?.ToString();
            string caseNo = cmbCase.Value?.ToString();

            if (string.IsNullOrEmpty(companyNo))
            {
                ShowMessage("회사를 선택하세요.");
                return;
            }
            if (string.IsNullOrEmpty(caseNo))
            {
                ShowMessage("케이스를 선택하세요.");
                return;
            }

            List<ResourceViewModel> dataList = _controller.GetResourceList(caseNo, companyNo);

            GridData = dataList;
            gridResource.DataSource = dataList;
            gridResource.DataBind();

            BindMobileCards();

            lblRecordCount.Text = $"총 <strong>{dataList.Count}</strong>건의 데이터가 조회되었습니다.";
        }

        #endregion
    }
}
