using System;
using System.Collections.Generic;
using KShiftSmartPortalWeb.Models;
using KShiftSmartPortalWeb.Repositories;

namespace KShiftSmartPortalWeb.Services
{
    /// <summary>
    /// 바로가기 비즈니스 로직 서비스
    /// </summary>
    public class ShortcutService
    {
        private readonly UserShortcutRepository _repository;

        public ShortcutService()
        {
            _repository = new UserShortcutRepository();
        }

        /// <summary>
        /// 사용자별 바로가기 목록 조회 (기본 바로가기 자동 생성 포함)
        /// </summary>
        public List<UserShortcut> GetUserShortcuts(string userId, string companyNo)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(companyNo))
            {
                throw new ArgumentException("사용자 ID와 회사 번호는 필수입니다.");
            }

            // 바로가기 조회
            List<UserShortcut> shortcuts = _repository.GetUserShortcuts(userId, companyNo);

            // 바로가기가 없으면 기본 바로가기 생성
            if (shortcuts.Count == 0)
            {
                CreateDefaultShortcuts(userId, companyNo);
                shortcuts = _repository.GetUserShortcuts(userId, companyNo);
            }

            return shortcuts;
        }

        /// <summary>
        /// 다음 바로가기 순서 조회 (최대값 + 1)
        /// </summary>
        public int GetNextShortcutOrder(string userId, string companyNo)
        {
            int maxOrder = _repository.GetMaxShortcutOrder(userId, companyNo);
            return maxOrder + 1;
        }

        /// <summary>
        /// 바로가기 저장 (추가 또는 수정)
        /// </summary>
        public void SaveShortcut(UserShortcut shortcut)
        {
            // ShortcutOrder가 0이면 자동으로 다음 순서 계산
            if (shortcut.ShortcutOrder <= 0)
            {
                shortcut.ShortcutOrder = GetNextShortcutOrder(shortcut.UserId, shortcut.CompanyNo);
            }

            // 유효성 검사
            ValidateShortcut(shortcut);

            // 기존 데이터 확인
            bool exists = _repository.ShortcutExists(shortcut.UserId, shortcut.CompanyNo, shortcut.ShortcutOrder);

            if (exists)
            {
                // 수정
                _repository.UpdateShortcut(shortcut);
            }
            else
            {
                // 추가
                _repository.InsertShortcut(shortcut);
            }
        }

        /// <summary>
        /// 바로가기 추가
        /// </summary>
        public void AddShortcut(UserShortcut shortcut)
        {
            // 유효성 검사
            ValidateShortcut(shortcut);

            // 중복 확인
            bool exists = _repository.ShortcutExists(shortcut.UserId, shortcut.CompanyNo, shortcut.ShortcutOrder);
            if (exists)
            {
                throw new InvalidOperationException($"바로가기 순서 {shortcut.ShortcutOrder}는 이미 존재합니다.");
            }

            // 추가
            _repository.InsertShortcut(shortcut);
        }

        /// <summary>
        /// 바로가기 수정
        /// </summary>
        public void UpdateShortcut(UserShortcut shortcut)
        {
            // 유효성 검사
            ValidateShortcut(shortcut);

            // 존재 확인
            bool exists = _repository.ShortcutExists(shortcut.UserId, shortcut.CompanyNo, shortcut.ShortcutOrder);
            if (!exists)
            {
                throw new InvalidOperationException($"바로가기 순서 {shortcut.ShortcutOrder}가 존재하지 않습니다.");
            }

            // 수정
            _repository.UpdateShortcut(shortcut);
        }

        /// <summary>
        /// 바로가기 삭제
        /// </summary>
        public void DeleteShortcut(string userId, string companyNo, int shortcutOrder)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(companyNo))
            {
                throw new ArgumentException("사용자 ID와 회사 번호는 필수입니다.");
            }

            if (shortcutOrder <= 0)
            {
                throw new ArgumentException("바로가기 순서는 1 이상이어야 합니다.");
            }

            // 존재 확인
            bool exists = _repository.ShortcutExists(userId, companyNo, shortcutOrder);
            if (!exists)
            {
                throw new InvalidOperationException($"바로가기 순서 {shortcutOrder}가 존재하지 않습니다.");
            }

            // 삭제
            _repository.DeleteShortcut(userId, companyNo, shortcutOrder);
        }

        /// <summary>
        /// 기본 바로가기 생성
        /// </summary>
        private void CreateDefaultShortcuts(string userId, string companyNo)
        {
            var defaultShortcuts = new List<UserShortcut>
            {
                new UserShortcut
                {
                    ShortcutOrder = 1,
                    MenuName = "계약정보 업로드",
                    MenuUrl = "ContractUpload.aspx",
                    MenuIcon = "fas fa-upload",
                    MenuColor = "#e74c3c",
                    IsLocked = "N"
                },
                new UserShortcut
                {
                    ShortcutOrder = 2,
                    MenuName = "실산 달력터",
                    MenuUrl = "CalendarView.aspx",
                    MenuIcon = "fas fa-calendar",
                    MenuColor = "#3498db",
                    IsLocked = "N"
                },
                new UserShortcut
                {
                    ShortcutOrder = 3,
                    MenuName = "계약관리",
                    MenuUrl = "Default.aspx",
                    MenuIcon = "fas fa-file-contract",
                    MenuColor = "#e67e22",
                    IsLocked = "N"
                },
                new UserShortcut
                {
                    ShortcutOrder = 4,
                    MenuName = "계약정보 관리",
                    MenuUrl = "ContractManagement.aspx",
                    MenuIcon = "fas fa-chart-line",
                    MenuColor = "#9b59b6",
                    IsLocked = "N"
                },
                new UserShortcut
                {
                    ShortcutOrder = 5,
                    MenuName = "프로젝트 ACT...",
                    MenuUrl = "ProjectActivity.aspx",
                    MenuIcon = "fas fa-tasks",
                    MenuColor = "#16a085",
                    IsLocked = "N"
                },
                new UserShortcut
                {
                    ShortcutOrder = 6,
                    MenuName = "마스터플랜",
                    MenuUrl = "MasterPlan.aspx",
                    MenuIcon = "fas fa-project-diagram",
                    MenuColor = "#d35400",
                    IsLocked = "N"
                },
                new UserShortcut
                {
                    ShortcutOrder = 7,
                    MenuName = "프로젝트 부품...",
                    MenuUrl = "ProjectParts.aspx",
                    MenuIcon = "fas fa-cog",
                    MenuColor = "#34495e",
                    IsLocked = "N"
                }
            };

            _repository.InsertDefaultShortcuts(userId, companyNo, defaultShortcuts);
        }

        /// <summary>
        /// 바로가기 유효성 검사
        /// </summary>
        private void ValidateShortcut(UserShortcut shortcut)
        {
            if (shortcut == null)
            {
                throw new ArgumentNullException(nameof(shortcut), "바로가기 정보는 필수입니다.");
            }

            if (string.IsNullOrEmpty(shortcut.UserId))
            {
                throw new ArgumentException("사용자 ID는 필수입니다.", nameof(shortcut.UserId));
            }

            if (string.IsNullOrEmpty(shortcut.CompanyNo))
            {
                throw new ArgumentException("회사 번호는 필수입니다.", nameof(shortcut.CompanyNo));
            }

            if (shortcut.ShortcutOrder <= 0)
            {
                throw new ArgumentException("바로가기 순서는 1 이상이어야 합니다.", nameof(shortcut.ShortcutOrder));
            }

            if (string.IsNullOrEmpty(shortcut.MenuName))
            {
                throw new ArgumentException("메뉴 이름은 필수입니다.", nameof(shortcut.MenuName));
            }

            if (shortcut.MenuName.Length > 100)
            {
                throw new ArgumentException("메뉴 이름은 100자를 초과할 수 없습니다.", nameof(shortcut.MenuName));
            }

            if (!string.IsNullOrEmpty(shortcut.MenuUrl) && shortcut.MenuUrl.Length > 200)
            {
                throw new ArgumentException("메뉴 URL은 200자를 초과할 수 없습니다.", nameof(shortcut.MenuUrl));
            }

            if (!string.IsNullOrEmpty(shortcut.MenuIcon) && shortcut.MenuIcon.Length > 50)
            {
                throw new ArgumentException("메뉴 아이콘은 50자를 초과할 수 없습니다.", nameof(shortcut.MenuIcon));
            }

            if (!string.IsNullOrEmpty(shortcut.MenuColor) && shortcut.MenuColor.Length > 20)
            {
                throw new ArgumentException("메뉴 색상은 20자를 초과할 수 없습니다.", nameof(shortcut.MenuColor));
            }
        }
    }
}
