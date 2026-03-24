using System.Data;
using DevExpress.Web;

namespace KShiftSmartPortalWeb.Utils
{
    /// <summary>
    /// DevExpress ComboBox 초기화 공통 헬퍼.
    /// Company 유형, Company 목록 등 반복적으로 사용되는 콤보박스 로직을 제공합니다.
    /// </summary>
    public static class ComboBoxHelper
    {
        /// <summary>
        /// CompanyType 콤보박스를 초기화합니다. (전체/협력사/MASTER)
        /// </summary>
        public static void InitializeCompanyTypeCombo(ASPxComboBox comboBox)
        {
            comboBox.Items.Clear();
            comboBox.Items.Add("전체", "*");
            comboBox.Items.Add("협력사", "VENDOR");
            comboBox.Items.Add("MASTER", "MASTER");
            comboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Company 콤보박스를 로드합니다.
        /// CompanyType 필터링 및 기본값 설정을 지원합니다.
        /// </summary>
        /// <param name="comboBox">대상 콤보박스</param>
        /// <param name="companies">회사 목록 DataTable (COMPANY_NO, COMPANY_NAME, COMPANY_TYPE)</param>
        /// <param name="selectedCompanyType">필터할 회사 유형 ("*"이면 전체)</param>
        /// <param name="defaultCompanyNo">기본 선택할 회사번호 (null이면 첫 번째 항목 선택)</param>
        public static void LoadCompanyCombo(
            ASPxComboBox comboBox,
            DataTable companies,
            string selectedCompanyType = "*",
            string defaultCompanyNo = null)
        {
            comboBox.Items.Clear();

            foreach (DataRow row in companies.Rows)
            {
                if (selectedCompanyType == "*" ||
                    row["COMPANY_TYPE"]?.ToString() == selectedCompanyType)
                {
                    string text = $"{row["COMPANY_NO"]} - {row["COMPANY_NAME"]}";
                    comboBox.Items.Add(text, row["COMPANY_NO"].ToString());
                }
            }

            if (!string.IsNullOrEmpty(defaultCompanyNo) &&
                comboBox.Items.FindByValue(defaultCompanyNo) != null)
            {
                comboBox.Value = defaultCompanyNo;
            }
            else if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }
        }
    }
}
