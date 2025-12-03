using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using KShiftSmartPortalWeb.Models;
using KShiftSmartPortalWeb.Utils;
using KShiftSmartPortal.ViewModels;

namespace KShiftSmartPortalWeb.Controllers
{
    /// <summary>
    /// To-Do List 화면 컨트롤러
    /// 작업지시 목록 조회 및 수정 기능을 제공합니다.
    /// </summary>
    public class ToDoListController
    {
        private string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["OracleConnection"].ConnectionString; }
        }

        #region 조회 메서드

        /// <summary>
        /// To-Do List 조회
        /// 로그인한 사용자의 작업지시 목록을 조회합니다.
        /// </summary>
        /// <param name="companyNo">회사 번호</param>
        /// <param name="userId">로그인 사용자 ID</param>
        /// <param name="baseDate">기준일</param>
        /// <returns>작업지시 목록</returns>
        public List<ToDoListViewModel> GetToDoList(string companyNo, string userId, DateTime baseDate)
        {
            List<ToDoListViewModel> result = new List<ToDoListViewModel>();

            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    // 원본 쿼리를 기반으로 구성
                    string query = @"
                        SELECT 
                            B.COMPANY_NO,
                            B.CASE_NO,
                            B.PROJECT_NO, 
                            B.PROP01, 
                            B.ORDER_NO, 
                            B.ORDER_NAME, 
                            B.PROP02,
                            B.WORK_LIST, 
                            B.WORK_ST, 
                            B.WORK_FI, 
                            B.DUE_DATE, 
                            B.QM_DATE, 
                            B.COMP_DATE, 
                            B.QM_COMP_DATE,
                            B.PLAN_MHR, 
                            B.REAL_MHR, 
                            B.PLAN_MP, 
                            B.REAL_MP
                        FROM SCM_WORK_ORDER_DETAIL A, 
                             SCM_WORK_ORDER_MASTER B, 
                             STD_PERSONNEL_INFO C, 
                             SCM_CASE_MASTER D
                        WHERE D.COMPANY_NO = :COMPANY_NO 
                          AND D.PROP1 = 'Y'
                          AND B.COMPANY_NO = D.COMPANY_NO
                          AND B.CASE_NO = D.CASE_NO
                          AND (B.COMP_DATE IS NULL 
                               OR TO_DATE(:BASE_DATE,'YYYYMMDD') BETWEEN B.WORK_ST AND B.WORK_FI)
                          AND A.COMPANY_NO(+) = D.COMPANY_NO
                          AND A.CASE_NO(+) = D.CASE_NO    
                          AND A.ORDER_NO(+) = B.ORDER_NO
                          AND C.COMPANY_NO = D.COMPANY_NO    
                          AND C.USER_ID = :USER_ID
                          AND B.EMP_NO = C.EMP_NO    
                        ORDER BY B.WORK_ST";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2).Value = companyNo;
                        cmd.Parameters.Add("BASE_DATE", OracleDbType.Varchar2).Value = baseDate.ToString("yyyyMMdd");
                        cmd.Parameters.Add("USER_ID", OracleDbType.Varchar2).Value = userId;

                        SqlLogger.LogCommand(cmd, "To-Do List 조회");

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(MapReaderToViewModel(reader));
                            }
                        }

                        SqlLogger.LogResult(result.Count, "To-Do List 조회 완료");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "To-Do List 조회 실패");
                throw;
            }
        }

        /// <summary>
        /// DataReader를 ViewModel로 매핑
        /// </summary>
        private ToDoListViewModel MapReaderToViewModel(OracleDataReader reader)
        {
            return new ToDoListViewModel
            {
                COMPANY_NO = reader["COMPANY_NO"]?.ToString(),
                CASE_NO = reader["CASE_NO"]?.ToString(),
                PROJECT_NO = reader["PROJECT_NO"]?.ToString(),
                PROP01 = reader["PROP01"]?.ToString(),
                ORDER_NO = reader["ORDER_NO"]?.ToString(),
                ORDER_NAME = reader["ORDER_NAME"]?.ToString(),
                PROP02 = reader["PROP02"]?.ToString(),
                WORK_LIST = reader["WORK_LIST"]?.ToString(),
                WORK_ST = reader["WORK_ST"] as DateTime?,
                WORK_FI = reader["WORK_FI"] as DateTime?,
                DUE_DATE = reader["DUE_DATE"] as DateTime?,
                QM_DATE = reader["QM_DATE"] as DateTime?,
                COMP_DATE = reader["COMP_DATE"] as DateTime?,
                QM_COMP_DATE = reader["QM_COMP_DATE"] as DateTime?,
                PLAN_MHR = reader["PLAN_MHR"] as decimal?,
                REAL_MHR = reader["REAL_MHR"] as decimal?,
                PLAN_MP = reader["PLAN_MP"] as decimal?,
                REAL_MP = reader["REAL_MP"] as decimal?
            };
        }

        #endregion

        #region 저장 메서드

        /// <summary>
        /// 작업지시 수정 저장
        /// COMP_DATE, PLAN_MHR, REAL_MHR, PLAN_MP, REAL_MP 필드만 수정 가능
        /// </summary>
        public bool UpdateWorkOrder(string caseNo, string companyNo, string orderNo,
            DateTime? compDate, decimal? planMhr, decimal? realMhr, decimal? planMp, decimal? realMp,
            string userId)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        UPDATE SCM_WORK_ORDER_MASTER 
                        SET COMP_DATE = :COMP_DATE,
                            PLAN_MHR = :PLAN_MHR,
                            REAL_MHR = :REAL_MHR,
                            PLAN_MP = :PLAN_MP,
                            REAL_MP = :REAL_MP,
                            UP_USER = :UP_USER,
                            UP_DATE = SYSDATE
                        WHERE CASE_NO = :CASE_NO
                          AND COMPANY_NO = :COMPANY_NO
                          AND ORDER_NO = :ORDER_NO";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        // 수정 가능 필드
                        cmd.Parameters.Add("COMP_DATE", OracleDbType.Date).Value =
                            compDate.HasValue ? (object)compDate.Value : DBNull.Value;
                        cmd.Parameters.Add("PLAN_MHR", OracleDbType.Decimal).Value =
                            planMhr.HasValue ? (object)planMhr.Value : DBNull.Value;
                        cmd.Parameters.Add("REAL_MHR", OracleDbType.Decimal).Value =
                            realMhr.HasValue ? (object)realMhr.Value : DBNull.Value;
                        cmd.Parameters.Add("PLAN_MP", OracleDbType.Decimal).Value =
                            planMp.HasValue ? (object)planMp.Value : DBNull.Value;
                        cmd.Parameters.Add("REAL_MP", OracleDbType.Decimal).Value =
                            realMp.HasValue ? (object)realMp.Value : DBNull.Value;
                        cmd.Parameters.Add("UP_USER", OracleDbType.Varchar2).Value = userId;

                        // Key 필드
                        cmd.Parameters.Add("CASE_NO", OracleDbType.Varchar2).Value = caseNo;
                        cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2).Value = companyNo;
                        cmd.Parameters.Add("ORDER_NO", OracleDbType.Varchar2).Value = orderNo;

                        SqlLogger.LogCommand(cmd, "작업지시 수정");

                        int rowsAffected = cmd.ExecuteNonQuery();

                        SqlLogger.LogResult(rowsAffected, "작업지시 수정 완료");

                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "작업지시 수정 실패");
                throw;
            }
        }

        /// <summary>
        /// 일괄 저장 (BatchEdit 모드용)
        /// </summary>
        public int BatchUpdateWorkOrders(List<ToDoListViewModel> items, string userId)
        {
            int successCount = 0;

            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    using (OracleTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (var item in items)
                            {
                                string query = @"
                                    UPDATE SCM_WORK_ORDER_MASTER 
                                    SET COMP_DATE = :COMP_DATE,
                                        PLAN_MHR = :PLAN_MHR,
                                        REAL_MHR = :REAL_MHR,
                                        PLAN_MP = :PLAN_MP,
                                        REAL_MP = :REAL_MP,
                                        UP_USER = :UP_USER,
                                        UP_DATE = SYSDATE
                                    WHERE CASE_NO = :CASE_NO
                                      AND COMPANY_NO = :COMPANY_NO
                                      AND ORDER_NO = :ORDER_NO";

                                using (OracleCommand cmd = new OracleCommand(query, conn))
                                {
                                    cmd.Transaction = trans;

                                    cmd.Parameters.Add("COMP_DATE", OracleDbType.Date).Value =
                                        item.COMP_DATE.HasValue ? (object)item.COMP_DATE.Value : DBNull.Value;
                                    cmd.Parameters.Add("PLAN_MHR", OracleDbType.Decimal).Value =
                                        item.PLAN_MHR.HasValue ? (object)item.PLAN_MHR.Value : DBNull.Value;
                                    cmd.Parameters.Add("REAL_MHR", OracleDbType.Decimal).Value =
                                        item.REAL_MHR.HasValue ? (object)item.REAL_MHR.Value : DBNull.Value;
                                    cmd.Parameters.Add("PLAN_MP", OracleDbType.Decimal).Value =
                                        item.PLAN_MP.HasValue ? (object)item.PLAN_MP.Value : DBNull.Value;
                                    cmd.Parameters.Add("REAL_MP", OracleDbType.Decimal).Value =
                                        item.REAL_MP.HasValue ? (object)item.REAL_MP.Value : DBNull.Value;
                                    cmd.Parameters.Add("UP_USER", OracleDbType.Varchar2).Value = userId;
                                    cmd.Parameters.Add("CASE_NO", OracleDbType.Varchar2).Value = item.CASE_NO;
                                    cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2).Value = item.COMPANY_NO;
                                    cmd.Parameters.Add("ORDER_NO", OracleDbType.Varchar2).Value = item.ORDER_NO;

                                    if (cmd.ExecuteNonQuery() > 0)
                                        successCount++;
                                }
                            }

                            trans.Commit();
                            SqlLogger.LogResult(successCount, "일괄 저장 완료");
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }

                return successCount;
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "일괄 저장 실패");
                throw;
            }
        }

        #endregion

        #region 삭제 메서드

        /// <summary>
        /// 작업지시 삭제
        /// </summary>
        public bool DeleteWorkOrder(string caseNo, string companyNo, string orderNo)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    using (OracleTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            // 상세 먼저 삭제
                            string deleteDetailQuery = @"
                                DELETE FROM SCM_WORK_ORDER_DETAIL 
                                WHERE CASE_NO = :CASE_NO
                                  AND COMPANY_NO = :COMPANY_NO
                                  AND ORDER_NO = :ORDER_NO";

                            using (OracleCommand cmd = new OracleCommand(deleteDetailQuery, conn))
                            {
                                cmd.Transaction = trans;
                                cmd.Parameters.Add("CASE_NO", OracleDbType.Varchar2).Value = caseNo;
                                cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2).Value = companyNo;
                                cmd.Parameters.Add("ORDER_NO", OracleDbType.Varchar2).Value = orderNo;
                                cmd.ExecuteNonQuery();
                            }

                            // 마스터 삭제
                            string deleteMasterQuery = @"
                                DELETE FROM SCM_WORK_ORDER_MASTER 
                                WHERE CASE_NO = :CASE_NO
                                  AND COMPANY_NO = :COMPANY_NO
                                  AND ORDER_NO = :ORDER_NO";

                            using (OracleCommand cmd = new OracleCommand(deleteMasterQuery, conn))
                            {
                                cmd.Transaction = trans;
                                cmd.Parameters.Add("CASE_NO", OracleDbType.Varchar2).Value = caseNo;
                                cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2).Value = companyNo;
                                cmd.Parameters.Add("ORDER_NO", OracleDbType.Varchar2).Value = orderNo;

                                int rowsAffected = cmd.ExecuteNonQuery();
                                trans.Commit();

                                SqlLogger.LogResult(rowsAffected, "작업지시 삭제 완료");
                                return rowsAffected > 0;
                            }
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "작업지시 삭제 실패");
                throw;
            }
        }

        #endregion

        #region 공통 메서드

        /// <summary>
        /// 회사별 Company 목록 조회
        /// </summary>
        public DataTable GetCompanyList()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT COMPANY_NO, COMPANY_NAME, COMPANY_TYPE
                        FROM STD_COMPANY_INFO
                        WHERE USE_YN = 'Y'
                        ORDER BY COMPANY_NO";

                    using (OracleDataAdapter adapter = new OracleDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "Company 목록 조회 실패");
                throw;
            }
        }

        #endregion
    }
}