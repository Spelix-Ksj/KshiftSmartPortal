using System;
using System.Collections.Generic;
using System.Web.UI;
using KShiftSmartPortalWeb.Controllers;
using KShiftSmartPortalWeb.Models;

namespace KShiftSmartPortalWeb
{
    public partial class Home : System.Web.UI.Page
    {
        private ShortcutController _shortcutController = new ShortcutController();

        protected void Page_Load(object sender, EventArgs e)
        {
            // 세션 체크
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Views/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                InitializePage();
            }
            else
            {
                // PostBack 처리
                HandlePostBack();
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

            // 바로가기 로드
            LoadShortcuts();
        }

        /// <summary>
        /// 바로가기 목록 로드
        /// </summary>
        private void LoadShortcuts()
        {
            try
            {
                string userId = Session["UserID"]?.ToString();
                string companyNo = Session["CompanyNo"]?.ToString();

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(companyNo))
                {
                    return;
                }

                // Controller를 통해 바로가기 조회
                List<UserShortcut> shortcuts = _shortcutController.GetUserShortcuts(userId, companyNo);

                // Repeater에 데이터 바인딩
                rptShortcuts.DataSource = shortcuts;
                rptShortcuts.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage($"바로가기 로드 중 오류 발생: {ex.Message}");
            }
        }

        /// <summary>
        /// 편집 모드 버튼 클릭 이벤트
        /// </summary>
        protected void btnEditMode_Click(object sender, EventArgs e)
        {
            // 편집 모드 토글
            bool isEditMode = hdnEditMode.Value == "true";
            
            if (isEditMode)
            {
                // 편집 모드 종료
                hdnEditMode.Value = "false";
                btnEditMode.Text = "편집 모드";
                btnEditMode.CssClass = "btn-edit-mode";
                
                // 자바스크립트로 클래스 제거
                ScriptManager.RegisterStartupScript(this, GetType(), "ExitEditMode",
                    "document.getElementById('" + shortcutsContainer.ClientID + "').classList.remove('edit-mode');", true);
            }
            else
            {
                // 편집 모드 진입
                hdnEditMode.Value = "true";
                btnEditMode.Text = "완료";
                btnEditMode.CssClass = "btn-edit-mode active";
                
                // 자바스크립트로 클래스 추가
                ScriptManager.RegisterStartupScript(this, GetType(), "EnterEditMode",
                    "document.getElementById('" + shortcutsContainer.ClientID + "').classList.add('edit-mode');", true);
            }
        }

        /// <summary>
        /// PostBack 처리 (추가/수정/삭제)
        /// </summary>
        private void HandlePostBack()
        {
            string action = hdnAction.Value;
            
            if (string.IsNullOrEmpty(action))
            {
                return;
            }

            try
            {
                string userId = Session["UserID"]?.ToString();
                string companyNo = Session["CompanyNo"]?.ToString();

                switch (action.ToLower())
                {
                    case "delete":
                        int shortcutOrder = 0;
                        if (int.TryParse(hdnShortcutOrder.Value, out shortcutOrder))
                        {
                            _shortcutController.DeleteShortcut(userId, companyNo, shortcutOrder);
                            ShowMessage("바로가기가 삭제되었습니다.");
                            LoadShortcuts(); // 재로드
                        }
                        break;

                    case "add":
                        // ShortcutOrder를 서버에서 자동 계산 (DB의 MAX + 1)
                        int nextOrder = _shortcutController.GetNextShortcutOrder(userId, companyNo);

                        UserShortcut newShortcut = new UserShortcut
                        {
                            UserId = userId,
                            CompanyNo = companyNo,
                            ShortcutOrder = nextOrder,
                            MenuName = hdnMenuName.Value,
                            MenuUrl = hdnMenuUrl.Value,
                            MenuIcon = hdnMenuIcon.Value,
                            MenuColor = hdnMenuColor.Value,
                            IsEnabled = "Y",
                            IsLocked = "N"
                        };

                        _shortcutController.SaveShortcut(newShortcut);
                        ShowMessage("바로가기가 추가되었습니다.");
                        LoadShortcuts(); // 재로드
                        break;

                    case "edit":
                        int editOrder = 0;
                        if (int.TryParse(hdnShortcutOrder.Value, out editOrder))
                        {
                            UserShortcut editShortcut = new UserShortcut
                            {
                                UserId = userId,
                                CompanyNo = companyNo,
                                ShortcutOrder = editOrder,
                                MenuName = hdnMenuName.Value,
                                MenuUrl = hdnMenuUrl.Value,
                                MenuIcon = hdnMenuIcon.Value,
                                MenuColor = hdnMenuColor.Value,
                                IsEnabled = "Y",
                                IsLocked = "N"
                            };

                            _shortcutController.SaveShortcut(editShortcut);
                            ShowMessage("바로가기가 수정되었습니다.");
                            LoadShortcuts(); // 재로드
                        }
                        break;
                }

                // Hidden Field 초기화
                hdnAction.Value = string.Empty;
                hdnShortcutOrder.Value = string.Empty;
            }
            catch (Exception ex)
            {
                ShowMessage($"처리 중 오류 발생: {ex.Message}");
            }
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
