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
    /// XPO�� �̿��� ������� ���� ���� ��Ʈ�ѷ�
    /// ���� Oracle ��� ��Ʈ�ѷ��� �и��Ǿ� �����մϴ�.
    /// </summary>
    public class ContractManagerXpoController
    {
        private string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["OracleConnection"].ConnectionString; }
        }

        /// <summary>
        /// ���� ��ȸ (������ + ��) - XPO ���, ���� �ּ�ȭ
        /// </summary>
        public List<ScmContractMaster> GetContractFullList(string caseNo, string companyNo, bool selectAll, DateTime? dtStart, DateTime? dtEnd)
        {
            using (var uow = new UnitOfWork())
            {
                // ������ ��ȸ (XPO ��ü �״��)
                var masterQuery = new XPQuery<SCM_CONTRACT_MASTER>(uow)
                    .Where(m => m.CompoundKey1.CASE_NO == caseNo && m.CompoundKey1.COMPANY_NO == companyNo);

                if (!selectAll && dtStart.HasValue && dtEnd.HasValue)
                {
                    DateTime s = dtStart.Value.Date;
                    DateTime e = dtEnd.Value.Date.AddDays(1).AddTicks(-1);
                    masterQuery = masterQuery.Where(m => m.CNTR_DT >= s && m.CNTR_DT <= e);
                }

                var masterXpos = masterQuery.OrderBy(m => m.CompoundKey1.CONTRACT_ID).ToList();

                // �� ��ȸ
                var detailXpos = new XPQuery<SCM_CONTRACT_DETAIL>(uow)
                    .Where(d => d.CompoundKey1.CASE_NO == caseNo && d.CompoundKey1.COMPANY_NO == companyNo)
                    .ToList();

                // �����Ϳ� �󼼸� LINQ�� JOIN (XPO ��ü ���� ���)
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
                                  // �� ���� (ù ��° �׸� - 1:1 ���� ����)
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
        /// ȸ�纰 ���̽� ��� ��ȸ
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

                        SqlLogger.LogCommand(cmd, "���̽� ��� ��ȸ");

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
                SqlLogger.LogError(ex, "���̽� ��� ��ȸ ����");
                throw;
            }
        }

        /// <summary>
        /// ��� ���� ���� (�߰� �� ����) - XPO ���
        /// </summary>
        public bool SaveContract(ScmContractMaster contract, string userId)
        {
            using (var uow = new UnitOfWork())
            {
                try
                {
                    // ��� ������ ��ȸ
                    var existing = uow.Query<SCM_CONTRACT_MASTER>().FirstOrDefault(m =>
                        m.CompoundKey1.CASE_NO == contract.CASE_NO &&
                        m.CompoundKey1.COMPANY_NO == contract.COMPANY_NO &&
                        m.CompoundKey1.CONTRACT_ID == contract.CONTRACT_ID);

                    SCM_CONTRACT_MASTER masterEntity;
                    bool isNew = existing == null;

                    if (isNew)
                    {
                        // �ű� ����
                        masterEntity = new SCM_CONTRACT_MASTER(uow);
                        masterEntity.CompoundKey1 = new SCM_CONTRACT_MASTER.CompoundKey1Struct
                        {
                            CASE_NO = contract.CASE_NO,
                            COMPANY_NO = contract.COMPANY_NO,
                            CONTRACT_ID = contract.CONTRACT_ID
                        };
                        masterEntity.IN_USER = userId;
                        masterEntity.IN_DATE = DateTime.Now;
                    }
                    else
                    {
                        // ��� ������ ����
                        masterEntity = existing;
                        masterEntity.UP_USER = userId;
                        masterEntity.UP_DATE = DateTime.Now;
                    }

                    // ������ ����
                    masterEntity.STD_CASE_NO = contract.STD_CASE_NO ?? "MASTER";
                    masterEntity.STD_CONTRACT_ID = contract.STD_CONTRACT_ID;
                    masterEntity.PROP1 = contract.PROP1;
                    masterEntity.PROP2 = contract.PROP2;
                    masterEntity.CONTRACT_NAME = contract.CONTRACT_NAME;
                    masterEntity.CONTRACT_CATEGORY = contract.CONTRACT_CATEGORY ?? "ING";
                    masterEntity.CONTRACT_TYPE = contract.CONTRACT_TYPE ?? "CONTRACTED";
                    masterEntity.CONTRACT_NO = contract.CONTRACT_NO;
                    masterEntity.POR_NO = contract.POR_NO;
                    masterEntity.SEQ_NO = contract.SEQ_NO;
                    masterEntity.POR_DT = contract.POR_DT ?? DateTime.Now;
                    masterEntity.CNTR_DT = contract.CNTR_DT ?? DateTime.Now;
                    masterEntity.CNTR_INIT_DT = contract.CNTR_INIT_DT ?? contract.CNTR_DT ?? DateTime.Now;
                    masterEntity.MP_DT = contract.MP_DT ?? DateTime.Now;
                    masterEntity.MP_INIT_DT = contract.MP_INIT_DT ?? contract.MP_DT ?? DateTime.Now;
                    masterEntity.PRODUCT_TYPE = contract.PRODUCT_TYPE;
                    masterEntity.PRODUCT_DESC = contract.PRODUCT_DESC;
                    masterEntity.CNTR_EA = contract.CNTR_EA ?? 0;
                    masterEntity.CNTR_PIECE_WGT = contract.CNTR_PIECE_WGT ?? 0;
                    masterEntity.CNTR_WGT = contract.CNTR_WGT ?? 0;
                    masterEntity.PROJECT_NO = contract.PROJECT_NO;
                    masterEntity.BLOCK_NO = contract.BLOCK_NO;
                    masterEntity.MARK_NO = contract.MARK_NO;
                    masterEntity.OWNER = contract.OWNER;
                    masterEntity.TAG1 = contract.TAG1;
                    masterEntity.EST_ST_DT = contract.EST_ST_DT ?? DateTime.Now;
                    masterEntity.EST_FI_DT = contract.EST_FI_DT ?? DateTime.Now;
                    masterEntity.PLAN_ST_DT = contract.PLAN_ST_DT ?? DateTime.Now;
                    masterEntity.PLAN_FI_DT = contract.PLAN_FI_DT ?? DateTime.Now;
                    masterEntity.RESULT_ST_DT = contract.RESULT_ST_DT ?? DateTime.Now;
                    masterEntity.RESULT_FI_DT = contract.RESULT_FI_DT ?? DateTime.Now;
                    masterEntity.RMK = contract.RMK;

                    // Detail ������ ���� ����
                    if (contract.DetailInfo != null)
                    {
                        var existingDetail = uow.Query<SCM_CONTRACT_DETAIL>().FirstOrDefault(d =>
                            d.CompoundKey1.CASE_NO == contract.CASE_NO &&
                            d.CompoundKey1.COMPANY_NO == contract.COMPANY_NO &&
                            d.CompoundKey1.CONTRACT_ID == contract.CONTRACT_ID);

                        SCM_CONTRACT_DETAIL detailEntity;
                        if (existingDetail == null)
                        {
                            detailEntity = new SCM_CONTRACT_DETAIL(uow);
                            detailEntity.CompoundKey1 = new SCM_CONTRACT_DETAIL.CompoundKey1Struct
                            {
                                CASE_NO = contract.CASE_NO,
                                COMPANY_NO = contract.COMPANY_NO,
                                CONTRACT_ID = contract.CONTRACT_ID
                            };
                        }
                        else
                        {
                            detailEntity = existingDetail;
                        }

                        // Detail ������ ����
                        detailEntity.ACT_NO = contract.DetailInfo.ACT_NO;
                        detailEntity.OWNER_DEPT = contract.DetailInfo.OWNER_DEPT;
                        detailEntity.MAIN_CONTRACTOR = contract.DetailInfo.MAIN_CONTRACTOR;
                        detailEntity.SUB_CONTRACTOR = contract.DetailInfo.SUB_CONTRACTOR;
                        detailEntity.MS_NO = contract.DetailInfo.MS_NO;
                        detailEntity.MS_DT = contract.DetailInfo.MS_DT ?? DateTime.Now;
                        detailEntity.MS_CONTRACTOR = contract.DetailInfo.MS_CONTRACTOR;
                        detailEntity.PAINT_MS_NO = contract.DetailInfo.PAINT_MS_NO;
                        detailEntity.PAINT_MS_DT = contract.DetailInfo.PAINT_MS_DT ?? DateTime.Now;
                        detailEntity.PAINT_MS_CONTRACTOR = contract.DetailInfo.PAINT_MS_CONTRACTOR;
                        detailEntity.DRAWING_DT = contract.DetailInfo.DRAWING_DT ?? DateTime.Now;
                        detailEntity.DRAWING_INIT_DT = contract.DetailInfo.DRAWING_INIT_DT ?? DateTime.Now;
                        detailEntity.MAT_LOG_DT = contract.DetailInfo.MAT_LOG_DT ?? DateTime.Now;
                        detailEntity.MAT_LOG_REQ_NO = contract.DetailInfo.MAT_LOG_REQ_NO;
                        detailEntity.MAT_LOG_POS_NM = contract.DetailInfo.MAT_LOG_POS_NM;
                        detailEntity.MAT_LOG_REQ_PER = contract.DetailInfo.MAT_LOG_REQ_PER;
                        detailEntity.MAT_LOG_REQ_DEPT = contract.DetailInfo.MAT_LOG_REQ_DEPT;
                        detailEntity.MAT_LOG_REQ_TEL = contract.DetailInfo.MAT_LOG_REQ_TEL;
                        detailEntity.MAT_LOG_PER = contract.DetailInfo.MAT_LOG_PER;
                        detailEntity.MPPL_PROJ = contract.DetailInfo.MPPL_PROJ;
                        detailEntity.MPPL_NO = contract.DetailInfo.MPPL_NO;
                        detailEntity.MPPL_SEQ = contract.DetailInfo.MPPL_SEQ;
                        detailEntity.MP_RES_DT = contract.DetailInfo.MP_RES_DT ?? DateTime.Now;
                        detailEntity.MAKING_DT = contract.DetailInfo.MAKING_DT ?? DateTime.Now;
                        detailEntity.MAKING_RES_DT = contract.DetailInfo.MAKING_RES_DT ?? DateTime.Now;
                        detailEntity.PAINTING_DT = contract.DetailInfo.PAINTING_DT ?? DateTime.Now;
                        detailEntity.PAINTING_RES_DT = contract.DetailInfo.PAINTING_RES_DT ?? DateTime.Now;
                        detailEntity.INSPECTION_DT = contract.DetailInfo.INSPECTION_DT ?? DateTime.Now;
                        detailEntity.INSPECTION_RES_DT = contract.DetailInfo.INSPECTION_RES_DT ?? DateTime.Now;
                    }

                    uow.CommitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    SqlLogger.LogError(ex, "��� ���� ���� ����");
                    throw;
                }
            }
        }

        /// <summary>
        /// ��� ���� ���� - XPO ���
        /// </summary>
        public bool DeleteContract(string caseNo, string companyNo, string contractId)
        {
            using (var uow = new UnitOfWork())
            {
                try
                {
                    // Master ��ȸ
                    var master = uow.Query<SCM_CONTRACT_MASTER>().FirstOrDefault(m =>
                        m.CompoundKey1.CASE_NO == caseNo &&
                        m.CompoundKey1.COMPANY_NO == companyNo &&
                        m.CompoundKey1.CONTRACT_ID == contractId);

                    if (master == null)
                    {
                        return false; // �̹� ����Ǿ����ų� �������� ����
                    }

                    // Detail ��ȸ �� ����
                    var detail = uow.Query<SCM_CONTRACT_DETAIL>().FirstOrDefault(d =>
                        d.CompoundKey1.CASE_NO == caseNo &&
                        d.CompoundKey1.COMPANY_NO == companyNo &&
                        d.CompoundKey1.CONTRACT_ID == contractId);

                    if (detail != null)
                    {
                        uow.Delete(detail);
                    }

                    // Master ����
                    uow.Delete(master);
                    uow.CommitChanges();

                    return true;
                }
                catch (Exception ex)
                {
                    SqlLogger.LogError(ex, "��� ���� ���� ����");
                    throw;
                }
            }
        }

        /// <summary>
        /// �ܿ� ��� ���� ��ȸ - XPO ���
        /// </summary>
        public ScmContractMaster GetContractById(string caseNo, string companyNo, string contractId)
        {
            using (var uow = new UnitOfWork())
            {
                try
                {
                    var master = uow.Query<SCM_CONTRACT_MASTER>().FirstOrDefault(m =>
                        m.CompoundKey1.CASE_NO == caseNo &&
                        m.CompoundKey1.COMPANY_NO == companyNo &&
                        m.CompoundKey1.CONTRACT_ID == contractId);

                    if (master == null)
                    {
                        return null;
                    }

                    var detail = uow.Query<SCM_CONTRACT_DETAIL>().FirstOrDefault(d =>
                        d.CompoundKey1.CASE_NO == caseNo &&
                        d.CompoundKey1.COMPANY_NO == companyNo &&
                        d.CompoundKey1.CONTRACT_ID == contractId);

                    var result = new ScmContractMaster
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
                        DetailInfo = detail != null ? new ScmContractDetail
                        {
                            CASE_NO = detail.CompoundKey1.CASE_NO,
                            COMPANY_NO = detail.CompoundKey1.COMPANY_NO,
                            CONTRACT_ID = detail.CompoundKey1.CONTRACT_ID,
                            ACT_NO = detail.ACT_NO,
                            OWNER_DEPT = detail.OWNER_DEPT,
                            MAIN_CONTRACTOR = detail.MAIN_CONTRACTOR,
                            SUB_CONTRACTOR = detail.SUB_CONTRACTOR,
                            MS_NO = detail.MS_NO,
                            MS_DT = detail.MS_DT,
                            MS_CONTRACTOR = detail.MS_CONTRACTOR,
                            PAINT_MS_NO = detail.PAINT_MS_NO,
                            PAINT_MS_DT = detail.PAINT_MS_DT,
                            PAINT_MS_CONTRACTOR = detail.PAINT_MS_CONTRACTOR,
                            DRAWING_DT = detail.DRAWING_DT,
                            DRAWING_INIT_DT = detail.DRAWING_INIT_DT,
                            MAT_LOG_DT = detail.MAT_LOG_DT,
                            MAT_LOG_REQ_NO = detail.MAT_LOG_REQ_NO,
                            MAT_LOG_POS_NM = detail.MAT_LOG_POS_NM,
                            MAT_LOG_REQ_PER = detail.MAT_LOG_REQ_PER,
                            MAT_LOG_REQ_DEPT = detail.MAT_LOG_REQ_DEPT,
                            MAT_LOG_REQ_TEL = detail.MAT_LOG_REQ_TEL,
                            MAT_LOG_PER = detail.MAT_LOG_PER,
                            MPPL_PROJ = detail.MPPL_PROJ,
                            MPPL_NO = detail.MPPL_NO,
                            MPPL_SEQ = detail.MPPL_SEQ,
                            MP_RES_DT = detail.MP_RES_DT,
                            MAKING_DT = detail.MAKING_DT,
                            MAKING_RES_DT = detail.MAKING_RES_DT,
                            PAINTING_DT = detail.PAINTING_DT,
                            PAINTING_RES_DT = detail.PAINTING_RES_DT,
                            INSPECTION_DT = detail.INSPECTION_DT,
                            INSPECTION_RES_DT = detail.INSPECTION_RES_DT,
                        } : null
                    };

                    return result;
                }
                catch (Exception ex)
                {
                    SqlLogger.LogError(ex, "��� ���� ��ȸ ����");
                    throw;
                }
            }
        }
    }
}