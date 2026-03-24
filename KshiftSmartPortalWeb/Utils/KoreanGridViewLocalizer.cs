using DevExpress.Web.Localization;

namespace KShiftSmartPortalWeb.Utils
{
    /// <summary>
    /// DevExpress ASPxGridView 필터 메뉴 한글화 로컬라이저
    /// </summary>
    public class KoreanGridViewLocalizer : ASPxGridViewLocalizer
    {
        public override string GetLocalizedString(ASPxGridViewStringId id)
        {
            switch (id)
            {
                // 자동 필터 조건
                case ASPxGridViewStringId.AutoFilterBeginsWith: return "시작 문자";
                case ASPxGridViewStringId.AutoFilterContains: return "포함";
                case ASPxGridViewStringId.AutoFilterDoesNotContain: return "미포함";
                case ASPxGridViewStringId.AutoFilterEndsWith: return "끝 문자";
                case ASPxGridViewStringId.AutoFilterEquals: return "같음";
                case ASPxGridViewStringId.AutoFilterNotEqual: return "같지 않음";
                case ASPxGridViewStringId.AutoFilterGreater: return "초과";
                case ASPxGridViewStringId.AutoFilterGreaterOrEqual: return "이상";
                case ASPxGridViewStringId.AutoFilterLess: return "미만";
                case ASPxGridViewStringId.AutoFilterLessOrEqual: return "이하";
                case ASPxGridViewStringId.AutoFilterLike: return "유사";

                // 그리드 공통 메시지
                case ASPxGridViewStringId.ConfirmDelete: return "이 데이터를 삭제하시겠습니까?";
                case ASPxGridViewStringId.EmptyDataRow: return "표시할 데이터가 없습니다.";
                case ASPxGridViewStringId.PopupEditFormCaption: return "편집";

                default: return base.GetLocalizedString(id);
            }
        }
    }
}
