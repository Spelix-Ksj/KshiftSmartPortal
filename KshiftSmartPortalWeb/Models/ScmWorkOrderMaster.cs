using System;

namespace KShiftSmartPortalWeb.Models
{
    /// <summary>
    /// SCM_WORK_ORDER_MASTER 테이블 모델
    /// 작업지시 마스터 정보를 담는 엔티티 클래스
    /// </summary>
    public class ScmWorkOrderMaster
    {
        #region 기본 정보

        /// <summary>
        /// 회사 번호
        /// </summary>
        public string COMPANY_NO { get; set; }

        /// <summary>
        /// 케이스 번호
        /// </summary>
        public string CASE_NO { get; set; }

        /// <summary>
        /// 프로젝트 번호
        /// </summary>
        public string PROJECT_NO { get; set; }

        /// <summary>
        /// 속성1 (호선번호 등)
        /// </summary>
        public string PROP01 { get; set; }

        /// <summary>
        /// 지시 번호 (Key)
        /// </summary>
        public string ORDER_NO { get; set; }

        /// <summary>
        /// 지시 명칭
        /// </summary>
        public string ORDER_NAME { get; set; }

        /// <summary>
        /// 속성2 (블록번호 등)
        /// </summary>
        public string PROP02 { get; set; }

        /// <summary>
        /// 작업 목록
        /// </summary>
        public string WORK_LIST { get; set; }

        #endregion

        #region 일정 정보

        /// <summary>
        /// 작업 시작일
        /// </summary>
        public DateTime? WORK_ST { get; set; }

        /// <summary>
        /// 작업 종료일
        /// </summary>
        public DateTime? WORK_FI { get; set; }

        /// <summary>
        /// 마감일
        /// </summary>
        public DateTime? DUE_DATE { get; set; }

        /// <summary>
        /// 품질관리일
        /// </summary>
        public DateTime? QM_DATE { get; set; }

        /// <summary>
        /// 완료일 (수정 가능)
        /// </summary>
        public DateTime? COMP_DATE { get; set; }

        /// <summary>
        /// 품질관리 완료일
        /// </summary>
        public DateTime? QM_COMP_DATE { get; set; }

        #endregion

        #region 공수 정보 (수정 가능)

        /// <summary>
        /// 계획 M/H
        /// </summary>
        public decimal? PLAN_MHR { get; set; }

        /// <summary>
        /// 실적 M/H
        /// </summary>
        public decimal? REAL_MHR { get; set; }

        /// <summary>
        /// 계획 인원
        /// </summary>
        public decimal? PLAN_MP { get; set; }

        /// <summary>
        /// 실적 인원
        /// </summary>
        public decimal? REAL_MP { get; set; }

        #endregion

        #region 담당자 정보

        /// <summary>
        /// 담당자 사번
        /// </summary>
        public string EMP_NO { get; set; }

        #endregion
    }
}