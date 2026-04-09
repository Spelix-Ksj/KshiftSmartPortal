using System;

namespace KShiftSmartPortal.ViewModels
{
    /// <summary>
    /// 리소스 관리 화면용 ViewModel
    /// SCM_RESOURCE_MASTER 테이블의 데이터를 담는 클래스
    /// </summary>
    public class ResourceViewModel
    {
        // PK (3-컬럼 복합키)
        public string CaseNo { get; set; }
        public string CompanyNo { get; set; }
        public string ResourceNo { get; set; }

        // 기본 정보
        public string ResourceName { get; set; }
        public string ResourceType { get; set; }
        public string ParentResourceNo { get; set; }
        public string OrgNo { get; set; }
        public int ResLevel { get; set; }
        public string UseYn { get; set; }
        public double ViewOrder { get; set; }

        // 비고
        public string Rmk { get; set; }

        // 트리 표시용 들여쓰기된 이름
        public string DisplayName
        {
            get
            {
                if (ResLevel <= 1) return ResourceName;
                string prefix = new string('\u00A0', (ResLevel - 1) * 4);
                return prefix + "\u2514 " + ResourceName;
            }
        }

        // 감사 정보
        public string InUser { get; set; }
        public DateTime? InDate { get; set; }
        public string UpUser { get; set; }
        public DateTime? UpDate { get; set; }
    }
}
