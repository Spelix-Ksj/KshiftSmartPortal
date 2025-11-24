using System;
using System.Collections.Generic;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;
using KShiftSmartPortalWeb.Models;
using KShiftSmartPortalWeb.Utils;

namespace KShiftSmartPortalWeb.Repositories
{
    /// <summary>
    /// 사용자 바로가기 데이터 접근 계층
    /// </summary>
    public class UserShortcutRepository
    {
        private string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["OracleConnection"].ConnectionString;
            }
        }

        /// <summary>
        /// 사용자별 바로가기 목록 조회
        /// </summary>
        public List<UserShortcut> GetUserShortcuts(string userId, string companyNo)
        {
            List<UserShortcut> shortcuts = new List<UserShortcut>();

            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT
                            USER_ID, COMPANY_NO, SHORTCUT_ORDER, MENU_NAME,
                            MENU_URL, MENU_ICON, MENU_COLOR, IS_ENABLED, IS_LOCKED,
                            IN_USER, IN_DATE, UP_USER, UP_DATE
                        FROM TCM_USER_SHORTCUT
                        WHERE USER_ID = :USER_ID
                          AND COMPANY_NO = :COMPANY_NO
                          AND IS_ENABLED = 'Y'
                        ORDER BY SHORTCUT_ORDER";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("USER_ID", OracleDbType.Varchar2, 50).Value = userId;
                        cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2, 128).Value = companyNo;

                        // SQL 로깅
                        SqlLogger.LogCommand(cmd, "사용자 바로가기 조회");

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                shortcuts.Add(MapReaderToModel(reader));
                            }
                        }

                        SqlLogger.LogResult(shortcuts.Count, $"조회된 바로가기 수");
                    }
                }

                return shortcuts;
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "바로가기 목록 조회 중 오류 발생");
                throw new Exception($"바로가기 목록 조회 중 오류 발생: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 바로가기 존재 여부 확인
        /// </summary>
        public bool ShortcutExists(string userId, string companyNo, int shortcutOrder)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT COUNT(*)
                        FROM TCM_USER_SHORTCUT
                        WHERE USER_ID = :USER_ID
                          AND COMPANY_NO = :COMPANY_NO
                          AND SHORTCUT_ORDER = :SHORTCUT_ORDER";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("USER_ID", OracleDbType.Varchar2, 50).Value = userId;
                        cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2, 128).Value = companyNo;
                        cmd.Parameters.Add("SHORTCUT_ORDER", OracleDbType.Int32).Value = shortcutOrder;

                        // SQL 로깅
                        SqlLogger.LogCommand(cmd, "바로가기 존재 여부 확인");

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        SqlLogger.LogResult(count, "존재 여부");

                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "바로가기 존재 여부 확인 중 오류 발생");
                throw new Exception($"바로가기 존재 여부 확인 중 오류 발생: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 사용자의 최대 바로가기 순서 조회
        /// </summary>
        public int GetMaxShortcutOrder(string userId, string companyNo)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT NVL(MAX(SHORTCUT_ORDER), 0)
                        FROM TCM_USER_SHORTCUT
                        WHERE USER_ID = :USER_ID
                          AND COMPANY_NO = :COMPANY_NO";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("USER_ID", OracleDbType.Varchar2, 50).Value = userId;
                        cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2, 128).Value = companyNo;

                        // SQL 로깅
                        SqlLogger.LogCommand(cmd, "최대 바로가기 순서 조회");

                        int maxOrder = Convert.ToInt32(cmd.ExecuteScalar());

                        SqlLogger.LogResult(maxOrder, $"최대 순서");

                        return maxOrder;
                    }
                }
            }
            catch (Exception ex)
            {
                SqlLogger.LogError(ex, "최대 바로가기 순서 조회 중 오류 발생");
                throw new Exception($"최대 바로가기 순서 조회 중 오류 발생: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 바로가기 추가
        /// </summary>
        public void InsertShortcut(UserShortcut shortcut)
        {
            OracleConnection conn = null;
            OracleTransaction transaction = null;

            try
            {
                conn = new OracleConnection(ConnectionString);
                conn.Open();

                // 트랜잭션 시작
                transaction = conn.BeginTransaction();

                string query = @"
                    INSERT INTO TCM_USER_SHORTCUT
                    (USER_ID, COMPANY_NO, SHORTCUT_ORDER, MENU_NAME, MENU_URL, MENU_ICON, MENU_COLOR, IS_ENABLED, IS_LOCKED, IN_USER, IN_DATE)
                    VALUES
                    (:USER_ID, :COMPANY_NO, :SHORTCUT_ORDER, :MENU_NAME, :MENU_URL, :MENU_ICON, :MENU_COLOR, 'Y', :IS_LOCKED, :IN_USER, SYSDATE)";

                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.Transaction = transaction;

                    cmd.Parameters.Add("USER_ID", OracleDbType.Varchar2, 50).Value = shortcut.UserId;
                    cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2, 128).Value = shortcut.CompanyNo;
                    cmd.Parameters.Add("SHORTCUT_ORDER", OracleDbType.Int32).Value = shortcut.ShortcutOrder;
                    cmd.Parameters.Add("MENU_NAME", OracleDbType.Varchar2, 100).Value = shortcut.MenuName;
                    cmd.Parameters.Add("MENU_URL", OracleDbType.Varchar2, 200).Value = shortcut.MenuUrl ?? (object)DBNull.Value;
                    cmd.Parameters.Add("MENU_ICON", OracleDbType.Varchar2, 50).Value = shortcut.MenuIcon ?? (object)DBNull.Value;
                    cmd.Parameters.Add("MENU_COLOR", OracleDbType.Varchar2, 20).Value = shortcut.MenuColor ?? (object)DBNull.Value;
                    cmd.Parameters.Add("IS_LOCKED", OracleDbType.Char, 1).Value = shortcut.IsLocked ?? "N";
                    cmd.Parameters.Add("IN_USER", OracleDbType.Varchar2, 50).Value = shortcut.UserId;

                    // SQL 로깅
                    SqlLogger.LogCommand(cmd, "바로가기 추가");

                    int affectedRows = cmd.ExecuteNonQuery();

                    SqlLogger.LogResult(affectedRows, "바로가기 추가 완료");
                }

                // 트랜잭션 커밋
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // 트랜잭션 롤백
                transaction?.Rollback();

                SqlLogger.LogError(ex, "바로가기 추가 중 오류 발생");
                throw new Exception($"바로가기 추가 중 오류 발생: {ex.Message}", ex);
            }
            finally
            {
                transaction?.Dispose();
                conn?.Close();
                conn?.Dispose();
            }
        }

        /// <summary>
        /// 바로가기 수정
        /// </summary>
        public void UpdateShortcut(UserShortcut shortcut)
        {
            OracleConnection conn = null;
            OracleTransaction transaction = null;

            try
            {
                conn = new OracleConnection(ConnectionString);
                conn.Open();

                // 트랜잭션 시작
                transaction = conn.BeginTransaction();

                string query = @"
                    UPDATE TCM_USER_SHORTCUT
                    SET MENU_NAME = :MENU_NAME,
                        MENU_URL = :MENU_URL,
                        MENU_ICON = :MENU_ICON,
                        MENU_COLOR = :MENU_COLOR,
                        IS_LOCKED = :IS_LOCKED,
                        UP_USER = :UP_USER,
                        UP_DATE = SYSDATE
                    WHERE USER_ID = :USER_ID
                      AND COMPANY_NO = :COMPANY_NO
                      AND SHORTCUT_ORDER = :SHORTCUT_ORDER";

                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.Transaction = transaction;

                    cmd.Parameters.Add("MENU_NAME", OracleDbType.Varchar2, 100).Value = shortcut.MenuName;
                    cmd.Parameters.Add("MENU_URL", OracleDbType.Varchar2, 200).Value = shortcut.MenuUrl ?? (object)DBNull.Value;
                    cmd.Parameters.Add("MENU_ICON", OracleDbType.Varchar2, 50).Value = shortcut.MenuIcon ?? (object)DBNull.Value;
                    cmd.Parameters.Add("MENU_COLOR", OracleDbType.Varchar2, 20).Value = shortcut.MenuColor ?? (object)DBNull.Value;
                    cmd.Parameters.Add("IS_LOCKED", OracleDbType.Char, 1).Value = shortcut.IsLocked ?? "N";
                    cmd.Parameters.Add("UP_USER", OracleDbType.Varchar2, 50).Value = shortcut.UserId;
                    cmd.Parameters.Add("USER_ID", OracleDbType.Varchar2, 50).Value = shortcut.UserId;
                    cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2, 128).Value = shortcut.CompanyNo;
                    cmd.Parameters.Add("SHORTCUT_ORDER", OracleDbType.Int32).Value = shortcut.ShortcutOrder;

                    // SQL 로깅
                    SqlLogger.LogCommand(cmd, "바로가기 수정");

                    int affectedRows = cmd.ExecuteNonQuery();

                    SqlLogger.LogResult(affectedRows, "바로가기 수정 완료");
                }

                // 트랜잭션 커밋
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // 트랜잭션 롤백
                transaction?.Rollback();

                SqlLogger.LogError(ex, "바로가기 수정 중 오류 발생");
                throw new Exception($"바로가기 수정 중 오류 발생: {ex.Message}", ex);
            }
            finally
            {
                transaction?.Dispose();
                conn?.Close();
                conn?.Dispose();
            }
        }

        /// <summary>
        /// 바로가기 삭제
        /// </summary>
        public void DeleteShortcut(string userId, string companyNo, int shortcutOrder)
        {
            OracleConnection conn = null;
            OracleTransaction transaction = null;

            try
            {
                conn = new OracleConnection(ConnectionString);
                conn.Open();

                // 트랜잭션 시작
                transaction = conn.BeginTransaction();

                string query = @"
                    DELETE FROM TCM_USER_SHORTCUT
                    WHERE USER_ID = :USER_ID
                      AND COMPANY_NO = :COMPANY_NO
                      AND SHORTCUT_ORDER = :SHORTCUT_ORDER";

                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.Transaction = transaction;

                    cmd.Parameters.Add("USER_ID", OracleDbType.Varchar2, 50).Value = userId;
                    cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2, 128).Value = companyNo;
                    cmd.Parameters.Add("SHORTCUT_ORDER", OracleDbType.Int32).Value = shortcutOrder;

                    // SQL 로깅
                    SqlLogger.LogCommand(cmd, "바로가기 삭제");

                    int affectedRows = cmd.ExecuteNonQuery();

                    SqlLogger.LogResult(affectedRows, "바로가기 삭제 완료");
                }

                // 트랜잭션 커밋
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // 트랜잭션 롤백
                transaction?.Rollback();

                SqlLogger.LogError(ex, "바로가기 삭제 중 오류 발생");
                throw new Exception($"바로가기 삭제 중 오류 발생: {ex.Message}", ex);
            }
            finally
            {
                transaction?.Dispose();
                conn?.Close();
                conn?.Dispose();
            }
        }

        /// <summary>
        /// 기본 바로가기 생성 (복수)
        /// </summary>
        public void InsertDefaultShortcuts(string userId, string companyNo, List<UserShortcut> defaultShortcuts)
        {
            OracleConnection conn = null;
            OracleTransaction transaction = null;

            try
            {
                conn = new OracleConnection(ConnectionString);
                conn.Open();

                // 트랜잭션 시작
                transaction = conn.BeginTransaction();

                string query = @"
                    INSERT INTO TCM_USER_SHORTCUT
                    (USER_ID, COMPANY_NO, SHORTCUT_ORDER, MENU_NAME, MENU_URL, MENU_ICON, MENU_COLOR, IS_ENABLED, IS_LOCKED, IN_USER, IN_DATE)
                    VALUES
                    (:USER_ID, :COMPANY_NO, :SHORTCUT_ORDER, :MENU_NAME, :MENU_URL, :MENU_ICON, :MENU_COLOR, 'Y', :IS_LOCKED, :IN_USER, SYSDATE)";

                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.Transaction = transaction;

                    foreach (var shortcut in defaultShortcuts)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("USER_ID", OracleDbType.Varchar2, 50).Value = userId;
                        cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2, 128).Value = companyNo;
                        cmd.Parameters.Add("SHORTCUT_ORDER", OracleDbType.Int32).Value = shortcut.ShortcutOrder;
                        cmd.Parameters.Add("MENU_NAME", OracleDbType.Varchar2, 100).Value = shortcut.MenuName;
                        cmd.Parameters.Add("MENU_URL", OracleDbType.Varchar2, 200).Value = shortcut.MenuUrl;
                        cmd.Parameters.Add("MENU_ICON", OracleDbType.Varchar2, 50).Value = shortcut.MenuIcon;
                        cmd.Parameters.Add("MENU_COLOR", OracleDbType.Varchar2, 20).Value = shortcut.MenuColor;
                        cmd.Parameters.Add("IS_LOCKED", OracleDbType.Char, 1).Value = shortcut.IsLocked;
                        cmd.Parameters.Add("IN_USER", OracleDbType.Varchar2, 50).Value = userId;

                        // SQL 로깅
                        SqlLogger.LogCommand(cmd, $"기본 바로가기 추가 (순서: {shortcut.ShortcutOrder})");

                        int affectedRows = cmd.ExecuteNonQuery();

                        SqlLogger.LogResult(affectedRows, $"바로가기 '{shortcut.MenuName}' 추가 완료");
                    }
                }

                // 트랜잭션 커밋
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // 트랜잭션 롤백
                transaction?.Rollback();

                SqlLogger.LogError(ex, "기본 바로가기 생성 중 오류 발생");
                throw new Exception($"기본 바로가기 생성 중 오류 발생: {ex.Message}", ex);
            }
            finally
            {
                transaction?.Dispose();
                conn?.Close();
                conn?.Dispose();
            }
        }

        /// <summary>
        /// DataReader를 모델로 매핑
        /// </summary>
        private UserShortcut MapReaderToModel(OracleDataReader reader)
        {
            return new UserShortcut
            {
                UserId = GetSafeString(reader, "USER_ID"),
                CompanyNo = GetSafeString(reader, "COMPANY_NO"),
                ShortcutOrder = GetSafeInt(reader, "SHORTCUT_ORDER"),
                MenuName = GetSafeString(reader, "MENU_NAME"),
                MenuUrl = GetSafeString(reader, "MENU_URL"),
                MenuIcon = GetSafeString(reader, "MENU_ICON"),
                MenuColor = GetSafeString(reader, "MENU_COLOR"),
                IsEnabled = GetSafeString(reader, "IS_ENABLED"),
                IsLocked = GetSafeString(reader, "IS_LOCKED"),
                InUser = GetSafeString(reader, "IN_USER"),
                InDate = GetSafeDateTime(reader, "IN_DATE"),
                UpUser = GetSafeString(reader, "UP_USER"),
                UpDate = GetSafeDateTime(reader, "UP_DATE")
            };
        }

        #region Helper Methods

        private string GetSafeString(OracleDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
            }
            catch
            {
                return string.Empty;
            }
        }

        private int GetSafeInt(OracleDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? 0 : reader.GetInt32(ordinal);
            }
            catch
            {
                return 0;
            }
        }

        private DateTime? GetSafeDateTime(OracleDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? (DateTime?)null : reader.GetDateTime(ordinal);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
