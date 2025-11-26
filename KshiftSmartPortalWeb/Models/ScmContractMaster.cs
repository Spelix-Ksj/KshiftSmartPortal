using System;

namespace KShiftSmartPortalWeb.Models
{
    /// <summary>
    /// SCM_CONTRACT_MASTER 테이블 모델 클래스
    /// </summary>
    public class ScmContractMaster
    {
        // 기본 정보
        public string CASE_NO { get; set; }
        public string COMPANY_NO { get; set; }
        public string CONTRACT_ID { get; set; }
        public string STD_CASE_NO { get; set; }
        public string STD_CONTRACT_ID { get; set; }
        public string PROP1 { get; set; }  // 스케줄방식
        public string PROP2 { get; set; }  // 작업난이도

        // 계약일반
        public string CONTRACT_NAME { get; set; }
        public string CONTRACT_CATEGORY { get; set; }
        public string CONTRACT_TYPE { get; set; }
        public string CONTRACT_NO { get; set; }
        public string POR_NO { get; set; }
        public string SEQ_NO { get; set; }

        // 계약일정
        public DateTime? POR_DT { get; set; }
        public DateTime? CNTR_DT { get; set; }
        public DateTime? CNTR_INIT_DT { get; set; }
        public DateTime? MP_DT { get; set; }
        public DateTime? MP_INIT_DT { get; set; }

        // 제품정보
        public string PRODUCT_TYPE { get; set; }
        public string PRODUCT_DESC { get; set; }
        public int? CNTR_EA { get; set; }
        public decimal? CNTR_PIECE_WGT { get; set; }
        public decimal? CNTR_WGT { get; set; }
        public string PROJECT_NO { get; set; }
        public string BLOCK_NO { get; set; }
        public string MARK_NO { get; set; }

        // 계약담당
        public string OWNER { get; set; }
        public string TAG1 { get; set; }  // 계약담당

        // 기준일정정보
        public DateTime? EST_ST_DT { get; set; }
        public DateTime? EST_FI_DT { get; set; }
        public DateTime? PLAN_ST_DT { get; set; }
        public DateTime? PLAN_FI_DT { get; set; }
        public DateTime? RESULT_ST_DT { get; set; }
        public DateTime? RESULT_FI_DT { get; set; }

        // 기타
        public string RMK { get; set; }

        // 시스템 정보
        public string IN_USER { get; set; }
        public DateTime? IN_DATE { get; set; }
        public string UP_USER { get; set; }
        public DateTime? UP_DATE { get; set; }

        // 상세 정보 (JOIN 결과)
        public ScmContractDetail DetailInfo { get; set; }
    }
}