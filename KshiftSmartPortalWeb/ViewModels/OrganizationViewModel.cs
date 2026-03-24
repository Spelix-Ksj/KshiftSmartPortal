using System;

namespace KShiftSmartPortal.ViewModels
{
    /// <summary>
    /// 조직 관리 화면용 ViewModel
    /// STD_OBS_MASTER 테이블의 데이터를 담는 클래스
    /// </summary>
    public class OrganizationViewModel
    {
        // PK
        public string CompanyNo { get; set; }
        public string OrgNo { get; set; }

        // 기본 정보
        public string OrgName { get; set; }
        public string ParentObsNo { get; set; }
        public string ObsType { get; set; }
        public int ObsLevel { get; set; }
        public string UseYn { get; set; }
        public double ViewOrder { get; set; }

        // 비고
        public string Rmk { get; set; }

        // 감사 정보
        public string InUser { get; set; }
        public DateTime? InDate { get; set; }
        public string UpUser { get; set; }
        public DateTime? UpDate { get; set; }
    }
}
