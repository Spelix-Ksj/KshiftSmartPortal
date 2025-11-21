using System;
using System.Collections.Generic;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;
using ScmBlockContractWeb.Models;

namespace ScmBlockContractWeb.Controllers
{
    /// <summary>
    /// 사용자 바로가기 관리 컨트롤러
    /// </summary>
    public class ShortcutController
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
        /// <param name="userId">사용자 ID</param>
        /// <param name="companyNo">회사 번호</param>
        /// <returns>바로가기 목록 (최대 8개)</returns>
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

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                shortcuts.Add(MapReaderToModel(reader));
                            }
                        }
                    }
                }

                // 바로가기가 없으면 기본 바로가기 생성
                if (shortcuts.Count == 0)
                {
                    CreateDefaultShortcuts(userId, companyNo);
                    shortcuts = GetUserShortcuts(userId, companyNo); // 재조회
                }

                return shortcuts;
            }
            catch (Exception ex)
            {
                throw new Exception($"바로가기 목록 조회 중 오류 발생: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 사용자별 기본 바로가기 생성
        /// </summary>
        /// <param name="userId">사용자 ID</param>
        /// <param name="companyNo">회사 번호</param>
        private void CreateDefaultShortcuts(string userId, string companyNo)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    // 기본 바로가기 데이터
                    var defaultShortcuts = new List<UserShortcut>
                    {
                        new UserShortcut { ShortcutOrder = 1, MenuName = "계약정보 업로드", MenuUrl = "ContractUpload.aspx", MenuIcon = "fa-upload", MenuColor = "#e74c3c", IsLocked = "N" },
                        new UserShortcut { ShortcutOrder = 2, MenuName = "실산 달력터", MenuUrl = "CalendarView.aspx", MenuIcon = "fa-calendar", MenuColor = "#3498db", IsLocked = "N" },
                        new UserShortcut { ShortcutOrder = 3, MenuName = "계약관리", MenuUrl = "Default.aspx", MenuIcon = "fa-file-contract", MenuColor = "#e67e22", IsLocked = "N" },
                        new UserShortcut { ShortcutOrder = 4, MenuName = "계약정보 관리", MenuUrl = "ContractManagement.aspx", MenuIcon = "fa-chart-line", MenuColor = "#9b59b6", IsLocked = "N" },
                        new UserShortcut { ShortcutOrder = 5, MenuName = "프로젝트 ACT...", MenuUrl = "ProjectActivity.aspx", MenuIcon = "fa-tasks", MenuColor = "#16a085", IsLocked = "N" },
                        new UserShortcut { ShortcutOrder = 6, MenuName = "마스터플랜", MenuUrl = "MasterPlan.aspx", MenuIcon = "fa-project-diagram", MenuColor = "#d35400", IsLocked = "N" },
                        new UserShortcut { ShortcutOrder = 7, MenuName = "프로젝트 부품...", MenuUrl = "ProjectParts.aspx", MenuIcon = "fa-cog", MenuColor = "#34495e", IsLocked = "N" }
                    };

                    string insertQuery = @"
                        INSERT INTO TCM_USER_SHORTCUT 
                        (USER_ID, COMPANY_NO, SHORTCUT_ORDER, MENU_NAME, MENU_URL, MENU_ICON, MENU_COLOR, IS_ENABLED, IS_LOCKED, IN_USER, IN_DATE)
                        VALUES 
                        (:USER_ID, :COMPANY_NO, :SHORTCUT_ORDER, :MENU_NAME, :MENU_URL, :MENU_ICON, :MENU_COLOR, 'Y', :IS_LOCKED, :IN_USER, SYSDATE)";

                    using (OracleCommand cmd = new OracleCommand(insertQuery, conn))
                    {
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

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"기본 바로가기 생성 중 오류 발생: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 바로가기 저장 (추가 또는 수정)
        /// </summary>
        /// <param name="shortcut">바로가기 정보</param>
        public void SaveShortcut(UserShortcut shortcut)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    // 기존 데이터 확인
                    string checkQuery = @"
                        SELECT COUNT(*) 
                        FROM TCM_USER_SHORTCUT 
                        WHERE USER_ID = :USER_ID 
                          AND COMPANY_NO = :COMPANY_NO 
                          AND SHORTCUT_ORDER = :SHORTCUT_ORDER";

                    int count = 0;
                    using (OracleCommand checkCmd = new OracleCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.Add("USER_ID", OracleDbType.Varchar2, 50).Value = shortcut.UserId;
                        checkCmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2, 128).Value = shortcut.CompanyNo;
                        checkCmd.Parameters.Add("SHORTCUT_ORDER", OracleDbType.Int32).Value = shortcut.ShortcutOrder;

                        count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    }

                    string query;
                    if (count > 0)
                    {
                        // 수정
                        query = @"
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
                    }
                    else
                    {
                        // 추가
                        query = @"
                            INSERT INTO TCM_USER_SHORTCUT 
                            (USER_ID, COMPANY_NO, SHORTCUT_ORDER, MENU_NAME, MENU_URL, MENU_ICON, MENU_COLOR, IS_ENABLED, IS_LOCKED, IN_USER, IN_DATE)
                            VALUES 
                            (:USER_ID, :COMPANY_NO, :SHORTCUT_ORDER, :MENU_NAME, :MENU_URL, :MENU_ICON, :MENU_COLOR, 'Y', :IS_LOCKED, :IN_USER, SYSDATE)";
                    }

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("MENU_NAME", OracleDbType.Varchar2, 100).Value = shortcut.MenuName;
                        cmd.Parameters.Add("MENU_URL", OracleDbType.Varchar2, 200).Value = shortcut.MenuUrl ?? (object)DBNull.Value;
                        cmd.Parameters.Add("MENU_ICON", OracleDbType.Varchar2, 50).Value = shortcut.MenuIcon ?? (object)DBNull.Value;
                        cmd.Parameters.Add("MENU_COLOR", OracleDbType.Varchar2, 20).Value = shortcut.MenuColor ?? (object)DBNull.Value;
                        cmd.Parameters.Add("IS_LOCKED", OracleDbType.Char, 1).Value = shortcut.IsLocked ?? "N";
                        cmd.Parameters.Add("USER_ID", OracleDbType.Varchar2, 50).Value = shortcut.UserId;
                        cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2, 128).Value = shortcut.CompanyNo;
                        cmd.Parameters.Add("SHORTCUT_ORDER", OracleDbType.Int32).Value = shortcut.ShortcutOrder;

                        if (count > 0)
                        {
                            cmd.Parameters.Add("UP_USER", OracleDbType.Varchar2, 50).Value = shortcut.UserId;
                        }
                        else
                        {
                            cmd.Parameters.Add("IN_USER", OracleDbType.Varchar2, 50).Value = shortcut.UserId;
                        }

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"바로가기 저장 중 오류 발생: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 바로가기 삭제
        /// </summary>
        /// <param name="userId">사용자 ID</param>
        /// <param name="companyNo">회사 번호</param>
        /// <param name="shortcutOrder">바로가기 순서</param>
        public void DeleteShortcut(string userId, string companyNo, int shortcutOrder)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        DELETE FROM TCM_USER_SHORTCUT 
                        WHERE USER_ID = :USER_ID 
                          AND COMPANY_NO = :COMPANY_NO 
                          AND SHORTCUT_ORDER = :SHORTCUT_ORDER";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("USER_ID", OracleDbType.Varchar2, 50).Value = userId;
                        cmd.Parameters.Add("COMPANY_NO", OracleDbType.Varchar2, 128).Value = companyNo;
                        cmd.Parameters.Add("SHORTCUT_ORDER", OracleDbType.Int32).Value = shortcutOrder;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"바로가기 삭제 중 오류 발생: {ex.Message}", ex);
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

        /// <summary>
        /// 안전한 문자열 읽기
        /// </summary>
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

        /// <summary>
        /// 안전한 정수 읽기
        /// </summary>
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

        /// <summary>
        /// 안전한 DateTime 읽기
        /// </summary>
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