using System.Collections.Generic;
using KShiftSmartPortalWeb.Models;
using KShiftSmartPortalWeb.Services;

namespace KShiftSmartPortalWeb.Controllers
{
    /// <summary>
    /// 사용자 바로가기 관리 컨트롤러
    /// Service 계층을 사용하는 간단한 래퍼 클래스
    /// </summary>
    public class ShortcutController
    {
        private readonly ShortcutService _service;

        public ShortcutController()
        {
            _service = new ShortcutService();
        }

        /// <summary>
        /// 사용자별 바로가기 목록 조회
        /// </summary>
        /// <param name="userId">사용자 ID</param>
        /// <param name="companyNo">회사 번호</param>
        /// <returns>바로가기 목록</returns>
        public List<UserShortcut> GetUserShortcuts(string userId, string companyNo)
        {
            return _service.GetUserShortcuts(userId, companyNo);
        }

        /// <summary>
        /// 바로가기 저장 (추가 또는 수정)
        /// </summary>
        /// <param name="shortcut">바로가기 정보</param>
        public void SaveShortcut(UserShortcut shortcut)
        {
            _service.SaveShortcut(shortcut);
        }

        /// <summary>
        /// 다음 바로가기 순서 조회
        /// </summary>
        /// <param name="userId">사용자 ID</param>
        /// <param name="companyNo">회사 번호</param>
        /// <returns>다음 순서 번호 (최대값 + 1)</returns>
        public int GetNextShortcutOrder(string userId, string companyNo)
        {
            return _service.GetNextShortcutOrder(userId, companyNo);
        }

        /// <summary>
        /// 바로가기 삭제
        /// </summary>
        /// <param name="userId">사용자 ID</param>
        /// <param name="companyNo">회사 번호</param>
        /// <param name="shortcutOrder">바로가기 순서</param>
        public void DeleteShortcut(string userId, string companyNo, int shortcutOrder)
        {
            _service.DeleteShortcut(userId, companyNo, shortcutOrder);
        }
    }
}
