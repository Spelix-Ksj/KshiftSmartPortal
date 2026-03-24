using System;
using System.Collections.Generic;
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
    /// 고객사 관리 페이지 코드비하인드 (Bootstrap + PopupEditForm 방식)
    /// 모바일 최적화 버전
    /// </summary>
    public partial class CompanyManager : BasePage
    {
        private CompanyManagerController _controller = new CompanyManagerController();

        private const string SESSION_KEY_DATA = "CompanyManager_Data";

        private List<CompanyViewModel> GridData
        {
            get { return Session[SESSION_KEY_DATA] as List<CompanyViewModel>; }
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
                // 검색 패널 콤보박스 초기화
                ComboBoxHelper.InitializeCompanyTypeCombo(cmbCompanyType);

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
        /// 등록 폼 콤보박스 초기화
        /// </summary>
        private void InitializeAddForm()
        {
            try
            {
                // 고객사 유형 콤보 (등록 폼용 - 전체 제외)
                cmbAddCompanyType.Items.Clear();
                cmbAddCompanyType.Items.Add("협력사", "VENDOR");
                cmbAddCompanyType.Items.Add("MASTER", "MASTER");
                if (cmbAddCompanyType.Items.Count > 0)
                    cmbAddCompanyType.SelectedIndex = 0;

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
                string companyNo = null;

                // 모바일 카드 선택 확인 (hidden field 우선)
                string hdnCompanyNo = Request.Form["hdnSelCompanyNo"];
                if (!string.IsNullOrEmpty(hdnCompanyNo))
                {
                    companyNo = hdnCompanyNo;
                }
                else if (gridCompany.FocusedRowIndex >= 0)
                {
                    // 데스크탑 그리드에서 선택된 행
                    object keyValue = gridCompany.GetRowValues(gridCompany.FocusedRowIndex, "CompanyNo");
                    companyNo = keyValue?.ToString();
                }
                else
                {
                    ShowMessage("삭제할 고객사를 선택하세요.");
                    return;
                }

                if (string.IsNullOrEmpty(companyNo))
                {
                    ShowMessage("고객사 번호 정보가 없습니다.");
                    return;
                }

                bool result = _controller.DeleteCompany(companyNo);

                if (result)
                {
                    ShowMessage("고객사가 삭제되었습니다.");
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
                    gridCompany.DataSource = GridData;
                    gridCompany.DataBind();

                    string fileName = $"CompanyList_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
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
        /// 고객사 등록 버튼 클릭
        /// </summary>
        protected void btnAddSave_Click(object sender, EventArgs e)
        {
            if (!EnsureAuthenticated()) return;

            try
            {
                string companyNo = txtAddCompanyNo.Text?.Trim();
                string companyName = txtAddCompanyName.Text?.Trim();
                string userId = Session["UserID"]?.ToString();

                // 유효성 검사
                if (string.IsNullOrEmpty(companyNo))
                {
                    ShowMessage("고객사 번호를 입력하세요.");
                    return;
                }
                if (string.IsNullOrEmpty(companyName))
                {
                    ShowMessage("고객사명을 입력하세요.");
                    return;
                }
                if (string.IsNullOrEmpty(userId))
                {
                    ShowMessage("로그인 정보가 없습니다.");
                    return;
                }

                // 중복 체크
                if (_controller.IsCompanyNoDuplicate(companyNo))
                {
                    ShowMessage("이미 존재하는 고객사 번호입니다.");
                    return;
                }

                var model = new CompanyViewModel
                {
                    CompanyNo = companyNo,
                    CompanyName = companyName,
                    CompanyShName = txtAddCompanyShName.Text?.Trim(),
                    CompanyEngName = txtAddCompanyEngName.Text?.Trim(),
                    CompanyType = cmbAddCompanyType.Value?.ToString(),
                    UseYn = cmbAddUseYn.Value?.ToString() ?? "Y",
                    Representatives = txtAddRepresentatives.Text?.Trim(),
                    Prop1 = txtAddProp1.Text?.Trim(),
                    TelNo = txtAddTelNo.Text?.Trim(),
                    FaxNo = txtAddFaxNo.Text?.Trim(),
                    Location = txtAddLocation.Text?.Trim(),
                    SpecInfo = txtAddSpecInfo.Text?.Trim(),
                    ViewOrder = (double)spnAddViewOrder.Number,
                    Rmk = txtAddRmk.Text?.Trim()
                };

                bool result = _controller.InsertCompany(model, userId);

                if (result)
                {
                    ShowMessage("고객사가 등록되었습니다.");
                    ClearAddForm();
                    LoadData();
                }
                else
                {
                    ShowMessage("등록에 실패했습니다. 고객사 번호 중복 여부를 확인하세요.");
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
            txtAddCompanyNo.Text = "";
            txtAddCompanyName.Text = "";
            txtAddCompanyShName.Text = "";
            txtAddCompanyEngName.Text = "";
            if (cmbAddCompanyType.Items.Count > 0)
                cmbAddCompanyType.SelectedIndex = 0;
            if (cmbAddUseYn.Items.Count > 0)
                cmbAddUseYn.SelectedIndex = 0;
            txtAddRepresentatives.Text = "";
            txtAddProp1.Text = "";
            txtAddTelNo.Text = "";
            txtAddFaxNo.Text = "";
            txtAddLocation.Text = "";
            txtAddSpecInfo.Text = "";
            spnAddViewOrder.Number = 0;
            txtAddRmk.Text = "";
        }

        #endregion

        #region Grid Events

        protected void gridCompany_PageIndexChanged(object sender, EventArgs e)
        {
            BindGridFromSession();
        }

        protected void gridCompany_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            BindGridFromSession();

            if (string.IsNullOrEmpty(e.Parameters)) return;

            // 중복 체크 콜백
            if (e.Parameters.StartsWith("DUPCHECK:"))
            {
                string companyNo = e.Parameters.Substring(9);
                bool isDuplicate = _controller.IsCompanyNoDuplicate(companyNo);

                if (isDuplicate)
                {
                    gridCompany.JSProperties["cpMessage"] = "이미 존재하는 고객사 번호입니다.";
                    gridCompany.JSProperties["cpDupResult"] = "DUPLICATE";
                }
                else
                {
                    gridCompany.JSProperties["cpMessage"] = "사용 가능한 고객사 번호입니다.";
                    gridCompany.JSProperties["cpDupResult"] = "OK";
                }
            }
            // 모바일 수정 콜백
            else if (e.Parameters.StartsWith("EDIT:"))
            {
                string companyNo = e.Parameters.Substring(5);

                if (GridData != null)
                {
                    for (int i = 0; i < GridData.Count; i++)
                    {
                        if (GridData[i].CompanyNo == companyNo)
                        {
                            gridCompany.StartEdit(i);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// PopupEditForm에서 저장 버튼 클릭 시 호출
        /// </summary>
        protected void gridCompany_RowUpdating(object sender, ASPxDataUpdatingEventArgs e)
        {
            e.Cancel = true;

            if (Session["UserID"] == null)
            {
                ShowMessageCallback(gridCompany, "세션이 만료되었습니다. 다시 로그인해주세요.");
                return;
            }

            try
            {
                string userId = Session["UserID"].ToString();
                string companyNo = e.Keys["CompanyNo"]?.ToString();

                if (string.IsNullOrEmpty(companyNo))
                {
                    ShowMessageCallback(gridCompany, "고객사 번호 정보가 없습니다.");
                    return;
                }

                // 기존 데이터 조회
                var existing = _controller.GetCompanyByNo(companyNo);
                if (existing == null)
                {
                    ShowMessageCallback(gridCompany, "고객사 정보를 찾을 수 없습니다.");
                    return;
                }

                // 수정 필드 반영
                existing.CompanyName = e.NewValues["CompanyName"]?.ToString() ?? existing.CompanyName;
                existing.CompanyShName = e.NewValues["CompanyShName"]?.ToString();
                existing.CompanyEngName = e.NewValues["CompanyEngName"]?.ToString();
                existing.CompanyType = e.NewValues["CompanyType"]?.ToString() ?? existing.CompanyType;
                existing.UseYn = e.NewValues["UseYn"]?.ToString() ?? existing.UseYn;
                existing.Representatives = e.NewValues["Representatives"]?.ToString();
                existing.Prop1 = e.NewValues["Prop1"]?.ToString();
                existing.TelNo = e.NewValues["TelNo"]?.ToString();
                existing.FaxNo = e.NewValues["FaxNo"]?.ToString();
                existing.Location = e.NewValues["Location"]?.ToString();
                existing.SpecInfo = e.NewValues["SpecInfo"]?.ToString();
                existing.Rmk = e.NewValues["Rmk"]?.ToString();

                if (e.NewValues["ViewOrder"] != null)
                {
                    existing.ViewOrder = Convert.ToDouble(e.NewValues["ViewOrder"]);
                }

                bool result = _controller.UpdateCompany(existing, userId);

                if (result)
                {
                    gridCompany.CancelEdit();
                    LoadData();
                    ShowMessageCallback(gridCompany, "고객사 정보가 수정되었습니다.");
                    gridCompany.JSProperties["cpNeedRefresh"] = true;
                }
                else
                {
                    ShowMessageCallback(gridCompany, "수정에 실패했습니다.");
                }
            }
            catch (Exception ex)
            {
                ShowMessageCallback(gridCompany, $"수정 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 세션에서 그리드 데이터 바인딩
        /// </summary>
        private void BindGridFromSession()
        {
            if (GridData != null)
            {
                gridCompany.DataSource = GridData;
                gridCompany.DataBind();

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

                int pageSize = gridCompany.SettingsPager.PageSize;
                int totalPages = (int)Math.Ceiling((double)GridData.Count / pageSize);
                int currentPage = gridCompany.PageIndex + 1;
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
            string companyType = cmbCompanyType.Value != null ? cmbCompanyType.Value.ToString() : "*";

            List<CompanyViewModel> dataList = _controller.GetCompanyFullList(companyType);

            GridData = dataList;
            gridCompany.DataSource = dataList;
            gridCompany.DataBind();

            BindMobileCards();

            lblRecordCount.Text = $"총 <strong>{dataList.Count}</strong>건의 데이터가 조회되었습니다.";
        }

        #endregion
    }
}
