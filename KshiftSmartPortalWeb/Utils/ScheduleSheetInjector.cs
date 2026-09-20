using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using DevExpress.Spreadsheet;

namespace KShiftSmartPortalWeb.Utils
{
    /// <summary>
    /// 블록제작 엑셀 템플릿(SCHEDULE 시트)에 조회 데이터를 셀 단위로 주입한다 (ALIS MakeScheduleSheet 이식).
    /// DevExpress.Spreadsheet(IWorkbook)만 의존하며 웹 컨트롤은 모른다.
    /// </summary>
    public static class ScheduleSheetInjector
    {
        /// <summary>블록 계열 ALIS는 TAG3를 읽지 않고 시트명을 상수로 사용</summary>
        public const string SheetName = "SCHEDULE";

#if DEBUG
        // 열문자 변환 자기검사 (DEBUG 빌드 최초 사용 시 1회)
        static ScheduleSheetInjector()
        {
            Debug.Assert(ColumnLetterToIndex("A") == 0 && ColumnLetterToIndex("Z") == 25
                && ColumnLetterToIndex("AA") == 26 && ColumnLetterToIndex("DD") == 107
                && ColumnLetterToIndex("") == -1, "ColumnLetterToIndex 자기검사 실패");
        }
#endif

        /// <summary>엑셀 열문자 → 0-based 인덱스 (A=0 … Z=25, AA=26). 빈값·영문자 외 문자가 있으면 -1</summary>
        public static int ColumnLetterToIndex(string letters)
        {
            if (string.IsNullOrWhiteSpace(letters)) return -1;
            int index = 0;
            foreach (char ch in letters.Trim().ToUpperInvariant())
            {
                if (ch < 'A' || ch > 'Z') return -1;
                index = index * 26 + (ch - 'A' + 1);   // 26진(1-based)
            }
            return index - 1;
        }

        /// <summary>
        /// data 1행 = 시트 1행(stRow부터). OBJ_NAME='MASTER' 정의만 주입, 열은 COLUMN_CELL.
        /// 남는 템플릿 행은 손대지 않는다(ALIS 동일). hidden 시트는 제거하고 SCHEDULE을 활성화한다.
        /// </summary>
        public static void Inject(IWorkbook wb, DataTable data, DataTable defs, int stRow)
        {
            // 주입 중 셀 변경마다 일어나는 자동 재계산·변경 알림을 중단(IBatchUpdateable)하고 끝나고 1회만 재계산 → 첫 렌더 지연 완화
            wb.BeginUpdate();
            try
            {
                // hidden 시트 제거 (2010 실데이터 비노출; SCHEDULE 수식이 참조하지 않음을 확인함)
                foreach (Worksheet hidden in wb.Worksheets.Where(s => !s.Visible).ToList())
                    wb.Worksheets.Remove(hidden);

                // 다운로드 유출 방지: 외부 참조 이름·작성자·메모 제거 (외부 링크 파트 자체는 배포 전 Tools\Sanitize-ExcelTemplate.ps1)
                Sanitize(wb);

                if (!wb.Worksheets.Contains(SheetName))
                    throw new InvalidOperationException($"템플릿에 '{SheetName}' 시트가 없습니다.");
                Worksheet ws = wb.Worksheets[SheetName];
                wb.Worksheets.ActiveWorksheet = ws;

                var cols = defs.AsEnumerable()
                    .Select(d => (Prop: d["PROP_NAME"].ToString(), Type: d["TAG2"]?.ToString(), Obj: d["OBJ_NAME"]?.ToString(),
                                  Col: ColumnLetterToIndex(d["COLUMN_CELL"]?.ToString())))
                    .Where(c => c.Col >= 0).ToList();
                var targets = cols.Where(c => c.Obj == "MASTER").ToList();
                if (data.Rows.Count == 0 || targets.Count == 0) return;

                // 템플릿 서식 행(수식·조건부서식) 초과분은 ALIS와 동일하게 미처리, 로그만 남김
                if (stRow + data.Rows.Count - 1 > ws.GetUsedRange().BottomRowIndex)
                    SqlLogger.LogWarning($"데이터 {data.Rows.Count}건이 템플릿 서식 행을 초과 (ALIS와 동일하게 미처리)");

                for (int r = 0; r < data.Rows.Count; r++)
                    foreach (var t in targets)
                        SetCell(ws.Cells[stRow + r, t.Col], data.Rows[r][t.Prop], t.Type);

                // ALIS: 정의 전체(OBJ_NAME 무관) min~max 열, ST_ROW~마지막 행에 Thin 검정 테두리
                ws.Range.FromLTRB(cols.Min(c => c.Col), stRow, cols.Max(c => c.Col), stRow + data.Rows.Count - 1)
                    .Borders.SetAllBorders(Color.Black, BorderLineStyle.Thin);
            }
            finally
            {
                wb.EndUpdate();
            }

            wb.Calculate();   // $G$4=TODAY() 의존 수식·조건부서식 재계산
        }

        /// <summary>
        /// 다운로드 유출 방지: 템플릿에 남은 외부 워크북 참조 이름([n]…)·깨진 이름(#REF!)·문서 속성(작성자 실명 등)·셀 메모를 제거한다.
        /// 외부 링크 파트(xl/externalLinks, 거래처 캐시·내부 경로)는 ExternalWorkbooks에 제거 API가 없어 배포 시 Tools\Sanitize-ExcelTemplate.ps1로 정리.
        /// 예외는 삼키지 않고 전파한다(주입 실패 경로가 문서를 닫음).
        /// </summary>
        public static void Sanitize(IWorkbook wb)
        {
            // 1) 정의 이름: 워크북 범위 + 시트 범위(Print_Area 등) 모두 검사. 외부 참조([)·#REF! 포함만 제거,
            //    _xlnm._FilterDatabase·Print_Titles 같은 정상 참조는 유지. 열거 중 제거 금지 → 목록화 후 Remove
            foreach (DefinedNameCollection names in new[] { wb.DefinedNames }.Concat(wb.Worksheets.Select(s => s.DefinedNames)))
                foreach (DefinedName bad in names.Where(n => (n.RefersTo ?? "").Contains("[") || (n.RefersTo ?? "").Contains("#REF!")).ToList())
                    names.Remove(bad);

            // 2) 문서 속성: 작성자·수정자·회사 등 식별 정보 비움(사용자 정의 속성 포함)
            DocumentProperties props = wb.DocumentProperties;
            props.Author = props.LastModifiedBy = props.Company = props.Manager = props.Title
                = props.Subject = props.Keywords = props.Description = props.Category = "";
            props.Custom.Clear();

            // 3) 셀 메모(일반·스레드) 전부 제거 — 작성자명·메모 본문 유출 차단
            foreach (Worksheet ws in wb.Worksheets)
            {
                ws.Comments.Clear();
                ws.ThreadedComments.Clear();
            }
        }

        /// <summary>
        /// 서식은 템플릿 것을 그대로 사용(NumberFormat 미지정). 문자열은 cell.Value = string으로 텍스트 강제("2676"도 텍스트).
        /// SetValueFromText 금지(자동 형변환). 변환 실패는 로그 후 건너뜀.
        /// </summary>
        private static void SetCell(Cell cell, object value, string type)
        {
            if (value == null || value == DBNull.Value) return;
            try
            {
                if (type == "DATE") cell.Value = Convert.ToDateTime(value);
                else if (type == "NUMBER") cell.Value = Convert.ToDouble(value);
                else cell.Value = Convert.ToString(value);
            }
            catch (Exception ex) when (ex is FormatException || ex is InvalidCastException || ex is OverflowException)
            {
                SqlLogger.LogError(ex, $"셀 주입 변환 실패 {cell.GetReferenceA1()} type={type}");
            }
        }
    }
}
