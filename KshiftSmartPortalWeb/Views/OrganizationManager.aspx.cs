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
    /// 조직 관리 페이지 코드비하인드 (Bootstrap + PopupEditForm 방식)
    /// 모바일 최적화 버전
    /// </summary>
    public partial class OrganizationManager : BasePage
    {
        private OrganizationManagerController _controller = new OrganizationManagerController();

        private const string SESSION_KEY_DATA = "OrganizationManager_Data";

        private List<OrganizationViewModel> GridData
        {
            get { return Session[SESSION_KEY_DATA] as List<OrganizationViewModel>; }
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
                string orgNo = null;

                // 모바일 카드 선택 확인 (hidden field 우선)
                string hdnOrgNo = Request.Form["hdnSelOrgNo"];
                if (!string.IsNullOrEmpty(hdnOrgNo))
                {
                    orgNo = hdnOrgNo;
                }
                else if (gridOrg.FocusedRowIndex >= 0)
                {
                    // 데스크탑 그리드에서 선택된 행
                    object keyValue = gridOrg.GetRowValues(gridOrg.FocusedRowIndex, "OrgNo");
                    orgNo = keyValue?.ToString();
                }
                else
                {
                    ShowMessage("삭제할 조직을 선택하세요.");
                    return;
                }

                if (string.IsNullOrEmpty(orgNo))
                {
                    ShowMessage("조직번호 정보가 없습니다.");
                    return;
                }

                // 현재 선택된 회사번호 가져오기
                string companyNo = cmbCompany.Value?.ToString();
                if (string.IsNullOrEmpty(companyNo))
                {
                    ShowMessage("회사를 먼저 선택하세요.");
                    return;
                }

                bool result = _controller.DeleteOrganization(companyNo, orgNo);

                if (result)
                {
                    ShowMessage("조직이 삭제되었습니다.");
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
                    gridOrg.DataSource = GridData;
                    gridOrg.DataBind();

                    string fileName = $"OrganizationList_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
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
        /// 조직 등록 버튼 클릭
        /// </summary>
        protected void btnAddSave_Click(object sender, EventArgs e)
        {
            if (!EnsureAuthenticated()) return;

            try
            {
                string orgNo = txtAddOrgNo.Text?.Trim();
                string orgName = txtAddOrgName.Text?.Trim();
                string userId = Session["UserID"]?.ToString();

                // 현재 선택된 회사번호
                string companyNo = cmbCompany.Value?.ToString();

                // 유효성 검사
                if (string.IsNullOrEmpty(companyNo))
                {
                    ShowMessage("회사를 먼저 선택하세요.");
                    return;
                }
                if (string.IsNullOrEmpty(orgNo))
                {
                    ShowMessage("조직번호를 입력하세요.");
                    return;
                }
                if (string.IsNullOrEmpty(orgName))
                {
                    ShowMessage("조직명을 입력하세요.");
                    return;
                }
                if (string.IsNullOrEmpty(userId))
                {
                    ShowMessage("로그인 정보가 없습니다.");
                    return;
                }

                // 중복 체크
                if (_controller.IsOrgNoDuplicate(companyNo, orgNo))
                {
                    ShowMessage("이미 존재하는 조직번호입니다.");
                    return;
                }

                var model = new OrganizationViewModel
                {
                    CompanyNo = companyNo,
                    OrgNo = orgNo,
                    OrgName = orgName,
                    ParentObsNo = txtAddParentObsNo.Text?.Trim(),
                    ObsType = txtAddObsType.Text?.Trim(),
                    ObsLevel = (int)spnAddObsLevel.Number,
                    UseYn = cmbAddUseYn.Value?.ToString() ?? "Y",
                    ViewOrder = (double)spnAddViewOrder.Number,
                    Rmk = txtAddRmk.Text?.Trim()
                };

                bool result = _controller.InsertOrganization(model, userId);

                if (result)
                {
                    ShowMessage("조직이 등록되었습니다.");
                    ClearAddForm();
                    LoadData();
                }
                else
                {
                    ShowMessage("등록에 실패했습니다. 조직번호 중복 여부를 확인하세요.");
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
            txtAddOrgNo.Text = "";
            txtAddOrgName.Text = "";
            txtAddParentObsNo.Text = "";
            txtAddObsType.Text = "";
            spnAddObsLevel.Number = 1;
            if (cmbAddUseYn.Items.Count > 0)
                cmbAddUseYn.SelectedIndex = 0;
            spnAddViewOrder.Number = 0;
            txtAddRmk.Text = "";
        }

        #endregion

        #region Grid Events

        protected void gridOrg_PageIndexChanged(object sender, EventArgs e)
        {
            BindGridFromSession();
        }

        protected void gridOrg_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            BindGridFromSession();

            if (string.IsNullOrEmpty(e.Parameters)) return;

            // 중복 체크 콜백
            if (e.Parameters.StartsWith("DUPCHECK:"))
            {
                string orgNo = e.Parameters.Substring(9);
                string companyNo = cmbCompany.Value?.ToString();

                if (string.IsNullOrEmpty(companyNo))
                {
                    gridOrg.JSProperties["cpMessage"] = "회사를 먼저 선택하세요.";
                    return;
                }

                bool isDuplicate = _controller.IsOrgNoDuplicate(companyNo, orgNo);

                if (isDuplicate)
                {
                    gridOrg.JSProperties["cpMessage"] = "이미 존재하는 조직번호입니다.";
                    gridOrg.JSProperties["cpDupResult"] = "DUPLICATE";
                }
                else
                {
                    gridOrg.JSProperties["cpMessage"] = "사용 가능한 조직번호입니다.";
                    gridOrg.JSProperties["cpDupResult"] = "OK";
                }
            }
            // 모바일 수정 콜백
            else if (e.Parameters.StartsWith("EDIT:"))
            {
                string orgNo = e.Parameters.Substring(5);

                if (GridData != null)
                {
                    for (int i = 0; i < GridData.Count; i++)
                    {
                        if (GridData[i].OrgNo == orgNo)
                        {
                            gridOrg.StartEdit(i);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// PopupEditForm에서 저장 버튼 클릭 시 호출
        /// </summary>
        protected void gridOrg_RowUpdating(object sender, ASPxDataUpdatingEventArgs e)
        {
            e.Cancel = true;

            if (Session["UserID"] == null)
            {
                ShowMessageCallback(gridOrg, "세션이 만료되었습니다. 다시 로그인해주세요.");
                return;
            }

            try
            {
                string userId = Session["UserID"].ToString();
                string orgNo = e.Keys["OrgNo"]?.ToString();

                if (string.IsNullOrEmpty(orgNo))
                {
                    ShowMessageCallback(gridOrg, "조직번호 정보가 없습니다.");
                    return;
                }

                string companyNo = cmbCompany.Value?.ToString();
                if (string.IsNullOrEmpty(companyNo))
                {
                    ShowMessageCallback(gridOrg, "회사 정보가 없습니다.");
                    return;
                }

                // 기존 데이터 조회
                var existing = _controller.GetOrganizationByKey(companyNo, orgNo);
                if (existing == null)
                {
                    ShowMessageCallback(gridOrg, "조직 정보를 찾을 수 없습니다.");
                    return;
                }

                // 수정 필드 반영
                existing.OrgName = e.NewValues["OrgName"]?.ToString() ?? existing.OrgName;
                existing.ParentObsNo = e.NewValues["ParentObsNo"]?.ToString();
                existing.ObsType = e.NewValues["ObsType"]?.ToString();
                existing.UseYn = e.NewValues["UseYn"]?.ToString() ?? existing.UseYn;
                existing.Rmk = e.NewValues["Rmk"]?.ToString();

                if (e.NewValues["ObsLevel"] != null)
                {
                    existing.ObsLevel = Convert.ToInt32(e.NewValues["ObsLevel"]);
                }

                if (e.NewValues["ViewOrder"] != null)
                {
                    existing.ViewOrder = Convert.ToDouble(e.NewValues["ViewOrder"]);
                }

                bool result = _controller.UpdateOrganization(existing, userId);

                if (result)
                {
                    gridOrg.CancelEdit();
                    LoadData();
                    ShowMessageCallback(gridOrg, "조직 정보가 수정되었습니다.");
                    gridOrg.JSProperties["cpNeedRefresh"] = true;
                }
                else
                {
                    ShowMessageCallback(gridOrg, "수정에 실패했습니다.");
                }
            }
            catch (Exception ex)
            {
                ShowMessageCallback(gridOrg, $"수정 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 세션에서 그리드 데이터 바인딩
        /// </summary>
        private void BindGridFromSession()
        {
            if (GridData != null)
            {
                gridOrg.DataSource = GridData;
                gridOrg.DataBind();

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

                int pageSize = gridOrg.SettingsPager.PageSize;
                int totalPages = (int)Math.Ceiling((double)GridData.Count / pageSize);
                int currentPage = gridOrg.PageIndex + 1;
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

            if (string.IsNullOrEmpty(companyNo))
            {
                ShowMessage("회사를 선택하세요.");
                return;
            }

            List<OrganizationViewModel> dataList = _controller.GetOrganizationList(companyNo);

            GridData = dataList;
            gridOrg.DataSource = dataList;
            gridOrg.DataBind();

            BindMobileCards();

            lblRecordCount.Text = $"총 <strong>{dataList.Count}</strong>건의 데이터가 조회되었습니다.";
        }

        #endregion
    }
}
