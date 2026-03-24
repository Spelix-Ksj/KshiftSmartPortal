using System;

namespace KShiftSmartPortal.ViewModels
{
    /// <summary>
    /// 고객사 관리 화면용 ViewModel
    /// STD_COMPANY_MASTER 테이블의 데이터를 담는 클래스
    /// </summary>
    public class CompanyViewModel
    {
        // 기본 정보
        public string CompanyNo { get; set; }
        public string CompanyName { get; set; }
        public string CompanyShName { get; set; }
        public string CompanyEngName { get; set; }
        public string CompanyType { get; set; }
        public string UseYn { get; set; }

        // 상세 정보
        public string Representatives { get; set; }  // 주요생산품
        public string Prop1 { get; set; }             // 제작타입
        public string TelNo { get; set; }
        public string FaxNo { get; set; }
        public string Location { get; set; }
        public string SpecInfo { get; set; }          // 주요 이력

        // 기준 정보 (읽기 전용)
        public string CalId { get; set; }
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
