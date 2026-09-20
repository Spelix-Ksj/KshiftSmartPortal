using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using DevExpress.Spreadsheet;
using DevExpress.Web.Bootstrap;
using DevExpress.Web.Office;
using KShiftSmartPortalWeb.Controllers;
using KShiftSmartPortalWeb.Utils;

namespace KShiftSmartPortalWeb
{
    /// <summary>
    /// 블록제작 일정 조회 페이지 (ALIS ScmBlock*PlanMan 9탭 공용, 조회 전용)
    /// 그리드 컬럼은 SCM_EXCEL_TEMP_DETAIL 정의로 매 요청 Page_Init에서 재생성한다(그리드 ViewState 비활성).
    /// </summary>
    public partial class BlockSchedule : BasePage
    {
        /// <summary>탭 정의(뷰명, 캡션, 활성). URL view 파라미터 허용목록 겸용. 검증된 템플릿을 배포한 탭만 활성화한다.</summary>
        private static readonly (string View, string Caption, bool Enabled)[] ViewDefs =
        {
            ("ScmBlockMasterPlanMan",   "메인(공유)",    true),
            ("ScmBlockCuttingPlanMan",  "가공",          true),
            ("ScmBlockUnitPlanMan",     "소조",          false),
            ("ScmBlockAssPlanMan",      "조립",          true),
            ("ScmBlockPEPlanMan",       "PE",            true),
            ("ScmBlockPaintPlanMan",    "도장",          false),
            ("ScmBlockShippingPlanMan", "선적",          false),
            ("ScmBlockMainPlanMan",     "메인입력(SCM)", false),
            ("ScmBlockCGPlanMan",       "셀가이드",      false),
        };

        private readonly BlockScheduleController _controller = new BlockScheduleController();

        /// <summary>URL view 파라미터가 허용목록에 있고 활성이면 채택, 아니면 메인(공유)으로 폴백</summary>
        private string ViewName
        {
            get
            {
                string requested = Request.QueryString["view"];
                var def = ViewDefs.FirstOrDefault(d => d.Enabled && string.Equals(d.View, requested, StringComparison.OrdinalIgnoreCase));
                return def.View ?? ViewDefs[0].View;
            }
        }

        /// <summary>표시 방식 라디오가 "스프레드시트"인지 (값은 컨트롤 ViewState에 보존)</summary>
        private bool IsSheetMode => rblViewMode.Value?.ToString() == "sheet";

        /// <summary>페이지 인스턴스 GUID(ViewState 보존). 같은 세션의 다중 탭이 문서를 공유하지 않도록 DocId에 포함한다(탭별 문서 분리).</summary>
        private string PageInstanceId
        {
            get
            {
                if (ViewState["PageInstanceId"] == null) ViewState["PageInstanceId"] = Guid.NewGuid().ToString("N");
                return (string)ViewState["PageInstanceId"];
            }
        }

        /// <summary>
        /// 스프레드시트 문서 ID. DocumentManager는 앱 전역 캐시이므로 세션ID를 반드시 포함하고,
        /// 탭별 문서 분리를 위해 페이지 인스턴스 GUID를 덧붙인다. Session_End의 접두(BlockSchedule_{SessionID}_) 정리는 그대로 유효.
        /// </summary>
        private string DocId => $"BlockSchedule_{Session.SessionID}_{ViewName}_{PageInstanceId}";

        #region 세션 캐시

        // ponytail: 세션 DataTable 캐시, 동시 사용자 늘면 HttpRuntime.Cache 슬라이딩 만료로 전환
        private DataTable GridData
        {
            get { return Session[$"BlockSchedule_{ViewName}_Data"] as DataTable; }
            set { Session[$"BlockSchedule_{ViewName}_Data"] = value; }
        }

        private DataTable ColumnDefs
        {
            get { return Session[$"BlockSchedule_{ViewName}_Cols"] as DataTable; }
            set { Session[$"BlockSchedule_{ViewName}_Cols"] = value; }
        }

        /// <summary>TempNo/TableName/SheetName/ExcelName/StRow/StCol — 2단계(스프레드시트 셀 주입)용</summary>
        private BlockScheduleResult Meta
        {
            get { return Session[$"BlockSchedule_{ViewName}_Meta"] as BlockScheduleResult; }
            set { Session[$"BlockSchedule_{ViewName}_Meta"] = value; }
        }

        #endregion

        #region Page Events

        protected void Page_Init(object sender, EventArgs e)
        {
            // 그리드 ViewState가 꺼져 있으므로 매 요청 컬럼 재생성 (콜백/포스트백/엑셀에서 컬럼 소실 방지)
            if (ColumnDefs != null) BuildGridColumns(ColumnDefs);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!CheckSession()) return;

            if (!IsPostBack && !IsCallback)
                InitializePage();
        }

        /// <summary>
        /// 모든 포스트백 이벤트 핸들러(콤보 변경·조회·라디오) 이후 최종 상태로 1회만 렌더한다 (AutoEventWireup="true"로 자동 연결).
        /// 첫 GET에도 실행되어 GridData null → 그리드 표시/시트 숨김 처리. Response.End 경로(엑셀)는 여기 도달하지 않는다.
        /// </summary>
        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            // 세션 만료로 CheckSession이 리다이렉트한 경우는 렌더하지 않음
            if (Session["UserID"] == null) return;
            Guarded(RenderResult, "결과 표시");
        }

        private void InitializePage()
        {
            try
            {
                ComboBoxHelper.InitializeCompanyTypeCombo(cmbCompanyType);
                LoadCompanyCombo();
                LoadCaseList();
                LoadDateTypeList();
                dtStart.Date = DateTime.Today.AddMonths(-1);
                dtEnd.Date = DateTime.Today;

                rptTabs.DataSource = ViewDefs.Select(d => new { d.View, d.Caption, d.Enabled, Active = d.View == ViewName });
                rptTabs.DataBind();

                ClearResult();
            }
            catch (Exception ex)
            {
                ShowMessage($"페이지 초기화 오류: {ex.Message}");
            }
        }

        #endregion

        #region 콤보박스 (구분 → Company → 케이스/기준일정 캐스케이드)

        private void LoadCompanyCombo()
        {
            string companyType = cmbCompanyType.Value?.ToString() ?? "*";
            ComboBoxHelper.LoadCompanyCombo(cmbCompany, _controller.GetCompanyList(), companyType, CurrentCompanyNo);
        }

        private void LoadCaseList()
        {
            cmbCase.Items.Clear();
            string companyNo = cmbCompany.Value?.ToString();
            if (string.IsNullOrEmpty(companyNo)) return;

            foreach (DataRow row in _controller.GetCaseList(companyNo).Rows)
                cmbCase.Items.Add(row["CASE_NAME"]?.ToString(), row["CASE_NO"]?.ToString());

            if (cmbCase.Items.Count > 0) cmbCase.SelectedIndex = 0;
        }

        /// <summary>기준일정 콤보: TEMP_DETAIL 중 PROP2='Y' 행 (Text=TAG1, Value=PROP_NAME), 맨 앞 "전체"="*"</summary>
        private void LoadDateTypeList()
        {
            cmbDateType.Items.Clear();
            cmbDateType.Items.Add("전체", "*");

            string companyNo = cmbCompany.Value?.ToString();
            if (!string.IsNullOrEmpty(companyNo))
            {
                foreach (DataRow r in _controller.GetTemplateDetail(companyNo, ViewName).Rows)
                {
                    if (r["PROP2"]?.ToString() != "Y") continue;
                    string prop = r["PROP_NAME"]?.ToString();
                    string text = string.IsNullOrEmpty(r["TAG1"]?.ToString()) ? prop : r["TAG1"].ToString();
                    cmbDateType.Items.Add(text, prop);
                }
            }
            cmbDateType.SelectedIndex = 0;
        }

        // 조회 조건이 바뀌면 이전 조회 결과(세션·그리드·건수)를 비운다
        protected void cmbCompanyType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Guarded(() => { ClearResult(); LoadCompanyCombo(); LoadCaseList(); LoadDateTypeList(); }, "Company 목록 로드");
        }

        protected void cmbCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            Guarded(() => { ClearResult(); LoadCaseList(); LoadDateTypeList(); }, "케이스/기준일정 목록 로드");
        }

        #endregion

        #region Button Events

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Guarded(LoadData, "데이터 조회");
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            if (GridData == null)
            {
                ShowMessage("조회 후 이용하세요.");
                return;
            }
            Guarded(() =>
            {
                string fileName = $"{ViewName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                if (IsSheetMode)
                {
                    // 시트 모드: 주입된 양식 문서 그대로(hidden 시트 제거 상태) 내려받기
                    ShowSpreadsheet();
                    byte[] bytes = spreadsheet.SaveCopy(DocumentFormat.Xlsx);
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", $"attachment; filename=\"{fileName}\"");
                    Response.BinaryWrite(bytes);
                    Response.End();
                    return;
                }
                gridSchedule.DataSource = GridData;
                gridSchedule.DataBind();
                gridExporter.WriteXlsxToResponse(fileName);
            }, "엑셀 내보내기");
        }

        /// <summary>핸들러 공통 예외 처리 (Response.End의 ThreadAbort는 통과)</summary>
        private void Guarded(Action action, string context)
        {
            try
            {
                action();
            }
            catch (Exception ex) when (!(ex is System.Threading.ThreadAbortException))
            {
                Notify($"{context} 오류: {ex.Message}");
            }
        }

        /// <summary>포스트백은 alert 스크립트, 그리드 콜백(페이징·정렬 등)은 JSProperties(cpMessage)로 메시지 전달 (aspx EndCallback에서 표시)</summary>
        private void Notify(string msg)
        {
            if (IsCallback) ShowMessageCallback(gridSchedule, msg); else ShowMessage(msg);
        }

        #endregion

        #region Data

        private void LoadData()
        {
            // 조기 return(미선택·기간 오류·조회 실패) 시 이전 결과가 남지 않도록 먼저 비운다
            ClearResult();

            string companyNo = cmbCompany.Value?.ToString();
            string caseNo = cmbCase.Value?.ToString();
            if (string.IsNullOrEmpty(companyNo)) { ShowMessage("회사를 선택하세요."); return; }
            if (string.IsNullOrEmpty(caseNo)) { ShowMessage("케이스를 선택하세요."); return; }

            string dateCol = cmbDateType.Value?.ToString() ?? "*";
            DateTime start = dtStart.Value is DateTime s ? s : DateTime.Today.AddMonths(-1);
            DateTime end = dtEnd.Value is DateTime t ? t : DateTime.Today;
            if (dateCol != "*" && start > end) { ShowMessage("조회기간 시작일이 종료일보다 늦습니다."); return; }

            BlockScheduleResult result = _controller.GetSchedule(ViewName, companyNo, caseNo, dateCol, start, end);
            if (result.Error != null)
            {
                ShowMessage(result.Error);
                return;
            }

            // 세션 저장: 테이블 2개는 별도 키, 메타는 Data/ColumnDefs를 비운 결과 객체로
            GridData = result.Data;
            ColumnDefs = result.ColumnDefs;
            result.Data = null;
            result.ColumnDefs = null;
            Meta = result;

            BuildGridColumns(ColumnDefs);
            // 렌더는 Page_LoadComplete의 RenderResult가 담당
        }

        /// <summary>세션 결과·스프레드시트 문서·그리드·건수 라벨 초기화</summary>
        private void ClearResult()
        {
            CloseSheetDocument();
            spreadsheet.Visible = false;   // 닫힌 문서가 렌더되지 않도록 안전장치 (최종 표시 여부는 RenderResult가 결정)
            GridData = null;
            ColumnDefs = null;
            Meta = null;
            gridSchedule.Columns.Clear();
            gridSchedule.DataSource = null;
            gridSchedule.DataBind();
            lblRecordCount.Text = "조회된 데이터가 없습니다.";
        }

        /// <summary>라디오 값에 따라 그리드 또는 스프레드시트 한쪽만 표시하고 세션 결과를 바인딩한다</summary>
        private void RenderResult()
        {
            bool sheet = IsSheetMode;
            gridSchedule.Visible = !sheet;
            // 문서를 열지 않은 채 보이면 컨트롤이 빈 문서를 자동 생성·캐시하므로 결과가 있을 때만 표시
            spreadsheet.Visible = sheet && GridData != null;
            if (GridData == null) return;

            lblRecordCount.Text = $"총 <strong>{GridData.Rows.Count}</strong>건의 데이터가 조회되었습니다.";
            if (!sheet)
            {
                gridSchedule.DataSource = GridData;
                gridSchedule.DataBind();
                return;
            }
            if (GridData.Rows.Count == 0)
            {
                spreadsheet.Visible = false;
                Notify("조회된 데이터가 없습니다.");
                return;
            }
            ShowSpreadsheet();
        }

        /// <summary>
        /// 캐시에 문서가 없으면(첫 전환·재조회 후·앱풀 재시작) 템플릿 로드 + 셀 주입, 있으면 재활성화만.
        /// Open은 항상 호출: 같은 DocumentId가 캐시에 있으면 바이트는 무시되고 기존 문서가 재활성화된다.
        /// </summary>
        private void ShowSpreadsheet()
        {
            string path = ResolveTemplatePath();
            if (path == null)
            {
                spreadsheet.Visible = false;
                throw new FileNotFoundException($"엑셀 템플릿 파일이 없습니다: {Path.GetFileName(Meta?.ExcelName ?? "")}");
            }

            bool cached = DocumentManager.FindDocument(DocId) != null;
            spreadsheet.Open(DocId, DocumentFormat.Xlsx, () => File.ReadAllBytes(path));
            if (cached) return;

            try
            {
                ScheduleSheetInjector.Inject(spreadsheet.Document, GridData, ColumnDefs, Meta.StRow);
            }
            catch
            {
                // 주입 실패 시 손상된 문서가 캐시에 남지 않도록 닫고(표시도 끄고) 다시 던진다
                spreadsheet.Visible = false;
                DocumentManager.CloseDocument(DocId);
                throw;
            }
        }

        /// <summary>EXCEL_NAME의 파일명만 취해 ~/App_Data/ExcelTemplates/와 결합(경로 조작 방지). 없으면 null</summary>
        private string ResolveTemplatePath()
        {
            string name = Path.GetFileName(Meta?.ExcelName ?? "");
            if (name.Length == 0) return null;
            string path = Path.Combine(Server.MapPath("~/App_Data/ExcelTemplates/"), name);
            return File.Exists(path) ? path : null;
        }

        /// <summary>이 세션·뷰의 스프레드시트 문서를 앱 전역 캐시에서 닫는다 (재조회 시 템플릿 재로드 보장)</summary>
        private void CloseSheetDocument()
        {
            if (DocumentManager.FindDocument(DocId) != null)
                DocumentManager.CloseDocument(DocId);
        }

        #endregion

        #region 동적 컬럼

        /// <summary>ALIS XAML 표준 밴드 선언 순서 (gb_BASIC → gb_KEY → gb_TEXT → gb_DATE → gb_NUMBER)</summary>
        private static readonly string[] StandardBands = { "기본정보", "키정보", "텍스트정보", "일정정보", "수치정보" };

        /// <summary>
        /// TEMP_DETAIL 정의(VIEW_ORDER 순)로 밴드/컬럼 생성. 표준 밴드 5개는 ALIS 순서로 먼저 생성하고
        /// TAG3 커스텀 밴드는 첫 등장 시 뒤에 추가. 기본정보 밴드(케이스/프로젝트)는 고정, 컬럼 없는 밴드는 제거.
        /// </summary>
        private void BuildGridColumns(DataTable defs)
        {
            gridSchedule.Columns.Clear();
            var added = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "CASE_NO", "PROJECT_NO" };

            foreach (string band in StandardBands) GetOrCreateBand(band);
            var basic = GetOrCreateBand("기본정보");
            basic.Columns.Add(CreateColumn("CASE_NO", "케이스", null));
            basic.Columns.Add(CreateColumn("PROJECT_NO", "프로젝트", null));

            foreach (DataRow d in defs.Rows)
            {
                string prop = d["PROP_NAME"]?.ToString() ?? "";
                if (prop.Length == 0 || !added.Add(prop)) continue;   // 빈 컬럼명 skip, 중복 정의는 첫 행만
                string caption = string.IsNullOrEmpty(d["TAG1"]?.ToString()) ? prop : d["TAG1"].ToString();
                GetOrCreateBand(ResolveBandName(d)).Columns.Add(CreateColumn(prop, caption, d["TAG2"]?.ToString()));
            }

            // 컬럼이 하나도 없는 밴드는 제거 (빈 밴드 헤더 방지)
            foreach (var empty in gridSchedule.Columns.OfType<BootstrapGridViewBandColumn>().Where(b => b.Columns.Count == 0).ToList())
                gridSchedule.Columns.Remove(empty);
        }

        /// <summary>밴드명: TAG3 우선 → TAG2(DATE=일정정보, NUMBER=수치정보) → KEY_FIELD(Y=키정보, 그 외 텍스트정보)</summary>
        private static string ResolveBandName(DataRow d)
        {
            // ALIS gb_DATE 헤더 오타("수치정보")는 복제하지 않음
            if (!string.IsNullOrEmpty(d["TAG3"]?.ToString())) return d["TAG3"].ToString();
            string t = d["TAG2"]?.ToString();
            if (t == "DATE") return "일정정보";
            if (t == "NUMBER") return "수치정보";
            return d["KEY_FIELD"]?.ToString() == "Y" ? "키정보" : "텍스트정보";
        }

        /// <summary>캡션이 같은 밴드가 있으면 재사용, 없으면 생성</summary>
        private BootstrapGridViewBandColumn GetOrCreateBand(string caption)
        {
            var band = gridSchedule.Columns.OfType<BootstrapGridViewBandColumn>().FirstOrDefault(b => b.Caption == caption);
            if (band != null) return band;

            band = new BootstrapGridViewBandColumn { Caption = caption, Name = "band" + gridSchedule.Columns.Count };
            band.CssClasses.HeaderCell = "header-band";
            gridSchedule.Columns.Add(band);
            return band;
        }

        /// <summary>DATE → DateColumn(yyyy-MM-dd) / NUMBER → TextColumn(#,##0.##) / 그 외 TextColumn. 읽기 전용, Name=FieldName.</summary>
        private static BootstrapGridViewEditDataColumn CreateColumn(string field, string caption, string type)
        {
            BootstrapGridViewEditDataColumn col;
            if (type == "DATE")
            {
                var dateCol = new BootstrapGridViewDateColumn { Width = Unit.Pixel(100) };
                dateCol.PropertiesDateEdit.DisplayFormatString = "yyyy-MM-dd";
                col = dateCol;
            }
            else
            {
                var textCol = new BootstrapGridViewTextColumn { Width = Unit.Pixel(type == "NUMBER" ? 90 : 120) };
                if (type == "NUMBER") textCol.PropertiesTextEdit.DisplayFormatString = "#,##0.##";
                col = textCol;
            }
            col.FieldName = field;
            col.Name = field;
            col.Caption = caption;
            col.ReadOnly = true;
            col.CssClasses.HeaderCell = "header-basic";
            return col;
        }

        #endregion
    }
}
