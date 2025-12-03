using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KShiftSmartPortal.ViewModels
{

    [Serializable]
    public class ContractViewModel
    {
        public string COMPANY_NO { get; set; }
        public string CASE_NO { get; set; }
        public string CONTRACT_ID { get; set; }
        public string STD_CASE_NO { get; set; }
        public string STD_CONTRACT_ID { get; set; }
        public string PROP1 { get; set; }
        public string PROP2 { get; set; }
        public string CONTRACT_NAME { get; set; }
        public string CONTRACT_CATEGORY { get; set; }
        public string CONTRACT_TYPE { get; set; }
        public string CONTRACT_NO { get; set; }
        public string POR_NO { get; set; }
        public string SEQ_NO { get; set; }
        public DateTime? POR_DT { get; set; }
        public DateTime? CNTR_DT { get; set; }
        public DateTime? CNTR_INIT_DT { get; set; }
        public DateTime? MP_DT { get; set; }
        public DateTime? MP_INIT_DT { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public string PRODUCT_DESC { get; set; }
        public int? CNTR_EA { get; set; }
        public decimal? CNTR_PIECE_WGT { get; set; }
        public decimal? CNTR_WGT { get; set; }
        public string PROJECT_NO { get; set; }
        public string BLOCK_NO { get; set; }
        public string MARK_NO { get; set; }
        public string OWNER { get; set; }
        public string TAG1 { get; set; }
        public string OWNER_DEPT { get; set; }
        public string MAIN_CONTRACTOR { get; set; }
        public string SUB_CONTRACTOR { get; set; }
        public string MS_NO { get; set; }
        public DateTime? MS_DT { get; set; }
        public string MS_CONTRACTOR { get; set; }
        public string PAINT_MS_NO { get; set; }
        public DateTime? PAINT_MS_DT { get; set; }
        public string PAINT_MS_CONTRACTOR { get; set; }
        public DateTime? DRAWING_DT { get; set; }
        public DateTime? DRAWING_INIT_DT { get; set; }
        public DateTime? MAT_LOG_DT { get; set; }
        public string MAT_LOG_REQ_NO { get; set; }
        public string MAT_LOG_POS_NM { get; set; }
        public string MAT_LOG_REQ_PER { get; set; }
        public string MAT_LOG_REQ_DEPT { get; set; }
        public string MAT_LOG_REQ_TEL { get; set; }
        public string MAT_LOG_PER { get; set; }
        public DateTime? MP_RES_DT { get; set; }
        public DateTime? MAKING_DT { get; set; }
        public DateTime? MAKING_RES_DT { get; set; }
        public DateTime? PAINTING_DT { get; set; }
        public DateTime? PAINTING_RES_DT { get; set; }
        public DateTime? INSPECTION_DT { get; set; }
        public DateTime? INSPECTION_RES_DT { get; set; }
        public string MPPL_PROJ { get; set; }
        public string MPPL_NO { get; set; }
        public string MPPL_SEQ { get; set; }
        public DateTime? EST_ST_DT { get; set; }
        public DateTime? EST_FI_DT { get; set; }
        public DateTime? PLAN_ST_DT { get; set; }
        public DateTime? PLAN_FI_DT { get; set; }
        public DateTime? RESULT_ST_DT { get; set; }
        public DateTime? RESULT_FI_DT { get; set; }
        public string RMK { get; set; }
    }
}