using System;

namespace ScmBlockContractWeb.Models
{
    /// <summary>
    /// 사용자 바로가기 설정 모델 클래스
    /// </summary>
    public class UserShortcut
    {
        /// <summary>
        /// 사용자 ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 회사 번호
        /// </summary>
        public string CompanyNo { get; set; }

        /// <summary>
        /// 바로가기 순서 (1-8)
        /// </summary>
        public int ShortcutOrder { get; set; }

        /// <summary>
        /// 메뉴 이름
        /// </summary>
        public string MenuName { get; set; }

        /// <summary>
        /// 메뉴 URL (상대 경로)
        /// </summary>
        public string MenuUrl { get; set; }

        /// <summary>
        /// 아이콘 클래스명 (Font Awesome)
        /// </summary>
        public string MenuIcon { get; set; }

        /// <summary>
        /// 타일 배경 색상
        /// </summary>
        public string MenuColor { get; set; }

        /// <summary>
        /// 사용 여부 (Y/N)
        /// </summary>
        public string IsEnabled { get; set; }

        /// <summary>
        /// 잠김 여부 (Y/N)
        /// </summary>
        public string IsLocked { get; set; }

        /// <summary>
        /// 등록자
        /// </summary>
        public string InUser { get; set; }

        /// <summary>
        /// 등록일
        /// </summary>
        public DateTime? InDate { get; set; }

        /// <summary>
        /// 수정자
        /// </summary>
        public string UpUser { get; set; }

        /// <summary>
        /// 수정일
        /// </summary>
        public DateTime? UpDate { get; set; }
    }
}