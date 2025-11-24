using System;

namespace KShiftSmartPortalWeb.Models
{
    /// <summary>
    /// SCM_CONTRACT_DETAIL 테이블 모델 클래스
    /// </summary>
    public class ScmContractDetail
    {
        // 기본 정보
        public string CASE_NO { get; set; }
        public string COMPANY_NO { get; set; }
        public string CONTRACT_ID { get; set; }
        public string ACT_NO { get; set; }

        // 계약 담당 정보
        public string OWNER_DEPT { get; set; }
        public string MAIN_CONTRACTOR { get; set; }
        public string SUB_CONTRACTOR { get; set; }
        public string SUB_CONTRACTOR2 { get; set; }
        public string SUB_CONTRACTOR2_DESC { get; set; }

        // 도급 정보
        public string MS_NO { get; set; }
        public DateTime? MS_DT { get; set; }
        public string MS_CONTRACTOR { get; set; }
        public string PAINT_MS_NO { get; set; }
        public DateTime? PAINT_MS_DT { get; set; }
        public string PAINT_MS_CONTRACTOR { get; set; }

        // 도면 정보
        public DateTime? DRAWING_DT { get; set; }
        public DateTime? DRAWING_INIT_DT { get; set; }

        // 자재 배송 정보
        public DateTime? MAT_LOG_REQ_DT { get; set; }
        public DateTime? MAT_LOG_DT { get; set; }
        public string MAT_LOG_REQ_NO { get; set; }
        public string MAT_LOG_POS_NM { get; set; }
        public string MAT_LOG_REQ_PER { get; set; }
        public string MAT_LOG_REQ_DEPT { get; set; }
        public string MAT_LOG_REQ_TEL { get; set; }
        public string MAT_LOG_PER { get; set; }

        // MPPL 정보
        public string MPPL_PROJ { get; set; }
        public string MPPL_NO { get; set; }
        public string MPPL_SEQ { get; set; }

        // 생산 공정 진행 현황
        public DateTime? MP_RES_DT { get; set; }
        public DateTime? MAKING_DT { get; set; }
        public DateTime? MAKING_RES_DT { get; set; }
        public DateTime? PAINTING_DT { get; set; }
        public DateTime? PAINTING_RES_DT { get; set; }
        public DateTime? INSPECTION_DT { get; set; }
        public DateTime? INSPECTION_RES_DT { get; set; }

        // TAG 정보
        public string TAG1 { get; set; }
        public string TAG2 { get; set; }
        public string TAG3 { get; set; }
        public string TAG4 { get; set; }
        public string TAG5 { get; set; }

        // 시스템 정보
        public string IN_USER { get; set; }
        public DateTime? IN_DATE { get; set; }
        public string IN_IP { get; set; }
        public string IN_PID { get; set; }
        public string UP_USER { get; set; }
        public DateTime? UP_DATE { get; set; }
        public string UP_IP { get; set; }
        public string UP_PID { get; set; }
    }
}