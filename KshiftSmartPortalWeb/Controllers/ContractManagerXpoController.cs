using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using KShiftSmartPortal.Database;
using KShiftSmartPortalWeb.Models;
using KShiftSmartPortalWeb.Utils;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace KShiftSmartPortalWeb.Controllers
{
    /// <summary>
    /// XPO를 이용한 계약정보 관리 전용 컨트롤러
    /// 기존 Oracle 기반 컨트롤러와 분리되어 동작합니다.
    /// </summary>
    public class ContractManagerXpoController
    {
        private string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["OracleConnection"].ConnectionString; }
        }

        /// <summary>
        /// 통합 조회 (마스터 + 상세) - XPO 사용, 매핑 최소화
        /// </summary>
        public List<ScmContractMaster> GetContractFullList(string caseNo, string companyNo, bool selectAll, DateTime? dtStart, DateTime? dtEnd)
        {
            using (var uow = new UnitOfWork())
            {
                // 마스터 조회 (XPO 객체 그대로)
                var masterQuery = new XPQuery<SCM_CONTRACT_MASTER>(uow)
                    .Where(m => m.CompoundKey1.CASE_NO == caseNo && m.CompoundKey1.COMPANY_NO == companyNo);

                if (!selectAll && dtStart.HasValue && dtEnd.HasValue)
                {
                    DateTime s = dtStart.Value.Date;
                    DateTime e = dtEnd.Value.Date.AddDays(1).AddTicks(-1);
                    masterQuery = masterQuery.Where(m => m.CNTR_DT >= s && m.CNTR_DT <= e);
                }

                var masterXpos = masterQuery.OrderBy(m => m.CompoundKey1.CONTRACT_ID).ToList();

                // 상세 조회
                var detailXpos = new XPQuery<SCM_CONTRACT_DETAIL>(uow)
                    .Where(d => d.CompoundKey1.CASE_NO == caseNo && d.CompoundKey1.COMPANY_NO == companyNo)
                    .ToList();

                // 마스터와 상세를 LINQ로 JOIN (XPO 객체 직접 사용)
                var result = (from master in masterXpos
                              join detail in detailXpos
                                on master.CompoundKey1.CONTRACT_ID
                                equals detail.CompoundKey1.CONTRACT_ID into detailGroup
                              select new ScmContractMaster
                              {
                                  CASE_NO = master.CompoundKey1.CASE_NO,
                                  COMPANY_NO = master.CompoundKey1.COMPANY_NO,
                                  CONTRACT_ID = master.CompoundKey1.CONTRACT_ID,
                                  STD_CASE_NO = master.STD_CASE_NO,
                                  STD_CONTRACT_ID = master.STD_CONTRACT_ID,
                                  PROP1 = master.PROP1,
                                  PROP2 = master.PROP2,
                                  CONTRACT_NAME = master.CONTRACT_NAME,
                                  CONTRACT_CATEGORY = master.CONTRACT_CATEGORY,
                                  CONTRACT_TYPE = master.CONTRACT_TYPE,
                                  CONTRACT_NO = master.CONTRACT_NO,
                                  POR_NO = master.POR_NO,
                                  SEQ_NO = master.SEQ_NO,
                                  POR_DT = master.POR_DT,
                                  CNTR_DT = master.CNTR_DT,
                                  CNTR_INIT_DT = master.CNTR_INIT_DT,
                                  MP_DT = master.MP_DT,
                                  MP_INIT_DT = master.MP_INIT_DT,
                                  PRODUCT_TYPE = master.PRODUCT_TYPE,
                                  PRODUCT_DESC = master.PRODUCT_DESC,
                                  CNTR_EA = master.CNTR_EA,
                                  CNTR_PIECE_WGT = master.CNTR_PIECE_WGT,
                                  CNTR_WGT = master.CNTR_WGT,
                                  PROJECT_NO = master.PROJECT_NO,
                                  BLOCK_NO = master.BLOCK_NO,
                                  MARK_NO = master.MARK_NO,
                                  OWNER = master.OWNER,
                                  TAG1 = master.TAG1,
                                  EST_ST_DT = master.EST_ST_DT,
                                  EST_FI_DT = master.EST_FI_DT,
                                  PLAN_ST_DT = master.PLAN_ST_DT,
                                  PLAN_FI_DT = master.PLAN_FI_DT,
                                  RESULT_ST_DT = master.RESULT_ST_DT,
                                  RESULT_FI_DT = master.RESULT_FI_DT,
                                  RMK = master.RMK,
                                  IN_USER = master.IN_USER,
                                  IN_DATE = master.IN_DATE,
                                  UP_USER = master.UP_USER,
                                  UP_DATE = master.UP_DATE,
                                  // 상세 정보 (첫 번째 항목만 - 1:1 매핑 가정)
                                  DetailInfo = detailGroup.FirstOrDefault() != null ? new ScmContractDetail
                                  {
                                      CASE_NO = detailGroup.First().CompoundKey1.CASE_NO,
                                      COMPANY_NO = detailGroup.First().CompoundKey1.COMPANY_NO,
                                      CONTRACT_ID = detailGroup.First().CompoundKey1.CONTRACT_ID,
                                      ACT_NO = detailGroup.First().ACT_NO,
                                      OWNER_DEPT = detailGroup.First().OWNER_DEPT,
                                      MAIN_CONTRACTOR = detailGroup.First().MAIN_CONTRACTOR,
                                      SUB_CONTRACTOR = detailGroup.First().SUB_CONTRACTOR,
                                      MS_NO = detailGroup.First().MS_NO,
                                      MS_DT = detailGroup.First().MS_DT,
                                      MS_CONTRACTOR = detailGroup.First().MS_CONTRACTOR,
                                      PAINT_MS_NO = detailGroup.First().PAINT_MS_NO,
                                      PAINT_MS_DT = detailGroup.First().PAINT_MS_DT,
                                      PAINT_MS_CONTRACTOR = detailGroup.First().PAINT_MS_CONTRACTOR,
                                      DRAWING_DT = detailGroup.First().DRAWING_DT,
                                      DRAWING_INIT_DT = detailGroup.First().DRAWING_INIT_DT,
                                      MAT_LOG_DT = detailGroup.First().MAT_LOG_DT,
                                      MAT_LOG_REQ_NO = detailGroup.First().MAT_LOG_REQ_NO,
                                      MAT_LOG_POS_NM = detailGroup.First().MAT_LOG_POS_NM,
                                      MAT_LOG_REQ_PER = detailGroup.First().MAT_LOG_REQ_PER,
                                      MAT_LOG_REQ_DEPT = detailGroup.First().MAT_LOG_REQ_DEPT,
                                      MAT_LOG_REQ_TEL = detailGroup.First().MAT_LOG_REQ_TEL,
                                      MAT_LOG_PER = detailGroup.First().MAT_LOG_PER,
                                      MPPL_PROJ = detailGroup.First().MPPL_PROJ,
                                      MPPL_NO = detailGroup.First().MPPL_NO,
                                      MPPL_SEQ = detailGroup.First().MPPL_SEQ,
                                      MP_RES_DT = detailGroup.First().MP_RES_DT,
                                      MAKING_DT = detailGroup.First().MAKING_DT,
                                      MAKING_RES_DT = detailGroup.First().MAKING_RES_DT,
                                      PAINTING_DT = detailGroup.First().PAINTING_DT,
                                      PAINTING_RES_DT = detailGroup.First().PAINTING_RES_DT,
                                      INSPECTION_DT = detailGroup.First().INSPECTION_DT,
                                      INSPECTION_RES_DT = detailGroup.First().INSPECTION_RES_DT,
                                      TAG1 = detailGroup.First().TAG1,
                                      TAG2 = detailGroup.First().TAG2,
                                      TAG3 = detailGroup.First().TAG3,
                                      TAG4 = detailGroup.First().TAG4,
                                      TAG5 = detailGroup.First().TAG5,
                                  } : null
                              }).ToList();

                return result;
            }
        }


        /// <summary>
        /// 회사별 케이스 목록 조회
        /// </summary>
        public DataTable GetCaseList(string companyNo)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT CASE_NO, CASE_NAME, COMPANY_NO, VIEW_ORDER
                        FROM SCM_CASE_MASTER
                        WHERE COMPANY_NO = :COMPANY_NO
                        ORDER BY VIEW_ORDER";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2).Value = companyNo;

                        SqlLogger.LogCommand(cmd, "케이스 목록 조회");

                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "케이스 목록 조회 실패");
                throw;
            }
        }
    }
}