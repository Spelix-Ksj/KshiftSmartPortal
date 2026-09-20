<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Views/Site.Master"
    CodeBehind="BlockSchedule.aspx.cs" Inherits="KShiftSmartPortalWeb.BlockSchedule" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    블록제작 - K-SHIFT Portal
</asp:Content>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        /* 전체 폰트 사이즈 */
        .search-panel, .grid-container { font-size: 13px; }

        /* 뷰 전환 탭 */
        .view-tabs { margin-bottom: 12px; }
        .view-tabs .nav-link { font-size: 13px; padding: 6px 14px; }
        .view-tabs .nav-link.disabled { color: #aaa; }

        /* 조회 조건 패널 스타일 */
        .search-panel {
            background: #fff;
            border-radius: 8px;
            padding: 15px;
            margin-bottom: 15px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .search-panel-header {
            font-size: 14px;
            font-weight: 600;
            color: #333;
            margin-bottom: 10px;
            padding-bottom: 8px;
            border-bottom: 2px solid #17a2b8;
        }
        .search-row {
            display: flex;
            flex-wrap: wrap;
            gap: 15px;
            align-items: flex-end;
        }
        .search-item {
            display: flex;
            flex-direction: column;
            gap: 3px;
            min-width: 150px;
        }
        .search-item label {
            font-size: 12px;
            font-weight: 600;
            color: #666;
        }
        .date-range {
            display: flex;
            align-items: center;
            gap: 6px;
        }
        .button-group {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
            margin-top: 12px;
            padding-top: 12px;
            border-top: 1px solid #eee;
        }
        .grid-container {
            background: #fff;
            border-radius: 8px;
            padding: 12px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .grid-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            font-size: 14px;
            font-weight: 600;
            color: #333;
            margin-bottom: 12px;
            padding-bottom: 8px;
            border-bottom: 2px solid #17a2b8;
        }
        .status-bar {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 10px 15px;
            background: #e8f4f8;
            border-radius: 6px;
            margin-top: 12px;
            border-left: 4px solid #17a2b8;
        }
        .record-count {
            font-size: 13px;
            color: #333;
        }
        .record-count strong {
            color: #17a2b8;
            font-size: 14px;
        }

        /* 컴보박스/날짜 편집기 높이 */
        .search-item .dxbs-combobox,
        .search-item .dxbs-dateedit {
            height: 32px !important;
            min-height: 32px !important;
        }
        .search-item .dxbs-combobox .form-control,
        .search-item .dxbs-dateedit .form-control {
            height: 32px !important;
            padding: 2px 8px !important;
            font-size: 13px !important;
        }

        /* 버튼 크기 조정 */
        .button-group .btn {
            padding: 5px 14px !important;
            font-size: 13px !important;
            min-width: 100px;
        }

        /* BootstrapGridView 스타일 */
        #gridSchedule {
            font-size: 12px !important;
        }

        /* 그리드 헤더 가운데 정렬 */
        #gridSchedule th,
        #gridSchedule th a,
        #gridSchedule th span,
        .dxbs-gridview th,
        .dxbs-gridview th a,
        .dxbs-gridview th span {
            text-align: center !important;
            vertical-align: middle !important;
            font-size: 12px !important;
            padding: 6px 4px !important;
            color: #fff !important;
        }
        #gridSchedule th a:hover,
        .dxbs-gridview th a:hover {
            color: #fff !important;
            text-decoration: none !important;
        }

        /* 그리드 셀 스타일 */
        #gridSchedule td {
            font-size: 12px !important;
            padding: 4px !important;
            white-space: nowrap;
        }

        /* 헤더 색상 클래스 (밴드 / 컬럼) */
        .header-band {
            background-color: #1565c0 !important;
            color: #fff !important;
        }
        .header-basic {
            background-color: #2196f3 !important;
            color: #fff !important;
        }

        /* 필터 행 */
        .dxbs-gridview .dxgvFilterRow td {
            padding: 2px !important;
        }
        .dxbs-gridview .dxgvFilterRow input {
            height: 20px !important;
            font-size: 12px !important;
        }

        /* ===== 모바일 반응형 (카드 뷰 없음, 그리드 가로 스크롤만) ===== */
        @media (max-width: 768px) {
            .search-panel {
                padding: 12px;
            }
            .search-row {
                flex-direction: column;
                gap: 10px;
            }
            .search-item {
                width: 100%;
                min-width: unset;
            }
            .search-item .dxbs-combobox {
                width: 100% !important;
            }
            .button-group {
                display: grid;
                grid-template-columns: 1fr 1fr;
                gap: 8px;
            }
            .button-group .btn {
                width: 100% !important;
                min-width: unset;
            }
            .status-bar {
                flex-direction: column;
                gap: 8px;
                text-align: center;
            }
            /* 헤더 제목·표시방식 라디오를 세로 배치 */
            .grid-header {
                flex-direction: column;
                gap: 6px;
            }
            .grid-header .form-check {
                margin-bottom: 0;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="PageTitleContent" ContentPlaceHolderID="PageTitleContent" runat="server">
    블록제작 (Block Schedule)
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <%-- 뷰 전환 탭 (활성 여부는 코드비하인드 ViewDefs 상수표로 관리, Enabled=false 탭은 disabled 렌더) --%>
    <ul class="nav nav-tabs view-tabs">
        <asp:Repeater ID="rptTabs" runat="server">
            <ItemTemplate>
                <li class="nav-item">
                    <a class='nav-link<%# (bool)Eval("Active") ? " active" : "" %><%# (bool)Eval("Enabled") ? "" : " disabled" %>'
                       href='<%#: (bool)Eval("Enabled") ? "BlockSchedule.aspx?view=" + Eval("View") : "#" %>'><%#: Eval("Caption") %></a>
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>

    <%-- 조회 조건 패널 --%>
    <div class="search-panel">
        <div class="search-panel-header">
            <i class="fas fa-search"></i> 조회 조건
        </div>

        <div class="search-row">
            <div class="search-item">
                <label>구분:</label>
                <dx:BootstrapComboBox ID="cmbCompanyType" runat="server"
                    ClientInstanceName="cmbCompanyType"
                    Width="150px"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="cmbCompanyType_SelectedIndexChanged" />
            </div>
            <div class="search-item">
                <label>Company:</label>
                <dx:BootstrapComboBox ID="cmbCompany" runat="server"
                    ClientInstanceName="cmbCompany"
                    Width="250px"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="cmbCompany_SelectedIndexChanged" />
            </div>
            <div class="search-item">
                <label>케이스:</label>
                <dx:BootstrapComboBox ID="cmbCase" runat="server"
                    ClientInstanceName="cmbCase"
                    Width="250px"
                    AutoPostBack="false" />
            </div>
            <div class="search-item">
                <label>기준일정:</label>
                <dx:BootstrapComboBox ID="cmbDateType" runat="server"
                    ClientInstanceName="cmbDateType"
                    Width="180px"
                    AutoPostBack="false" />
            </div>
            <div class="search-item">
                <label>조회기간:</label>
                <div class="date-range">
                    <dx:BootstrapDateEdit ID="dtStart" runat="server"
                        ClientInstanceName="dtStart"
                        Width="140px"
                        DisplayFormatString="yyyy-MM-dd"
                        EditFormat="Date"
                        AllowNull="false" />
                    <span>~</span>
                    <dx:BootstrapDateEdit ID="dtEnd" runat="server"
                        ClientInstanceName="dtEnd"
                        Width="140px"
                        DisplayFormatString="yyyy-MM-dd"
                        EditFormat="Date"
                        AllowNull="false" />
                </div>
            </div>
        </div>

        <%-- 버튼 그룹 (조회 / 엑셀만) --%>
        <div class="button-group">
            <dx:BootstrapButton ID="btnSearch" runat="server"
                ClientInstanceName="btnSearch"
                Text="<i class='fas fa-search'></i> 조회"
                EncodeHtml="false"
                OnClick="btnSearch_Click">
                <SettingsBootstrap RenderOption="Primary" />
            </dx:BootstrapButton>

            <dx:BootstrapButton ID="btnExcel" runat="server"
                Text="<i class='fas fa-file-excel'></i> 엑셀 다운로드"
                EncodeHtml="false"
                OnClick="btnExcel_Click">
                <SettingsBootstrap RenderOption="Success" />
            </dx:BootstrapButton>
        </div>
    </div>

    <%-- 그리드 컨테이너 --%>
    <div class="grid-container">
        <div class="grid-header">
            <span><i class="fas fa-cubes"></i> 블록제작 일정</span>
            <%-- 표시 방식 전환. 전용 핸들러 없이 AutoPostBack 포스트백에서 Page_LoadComplete의 RenderResult가 처리 --%>
            <dx:BootstrapRadioButtonList ID="rblViewMode" runat="server" RepeatDirection="Horizontal" AutoPostBack="true">
                <Items>
                    <dx:BootstrapListEditItem Text="그리드" Value="grid" Selected="true" />
                    <dx:BootstrapListEditItem Text="스프레드시트" Value="sheet" />
                </Items>
            </dx:BootstrapRadioButtonList>
        </div>

        <%-- 컬럼은 SCM_EXCEL_TEMP_DETAIL 정의로 코드비하인드(Page_Init)에서 매 요청 생성 --%>
        <dx:BootstrapGridView ID="gridSchedule" runat="server"
            ClientInstanceName="gridSchedule"
            Width="100%"
            AutoGenerateColumns="False"
            EnableCallBacks="true"
            EnableViewState="false">

            <Settings
                ShowFilterRow="True"
                ShowFilterRowMenu="True"
                VerticalScrollBarMode="Visible"
                VerticalScrollableHeight="450"
                HorizontalScrollBarMode="Auto" />

            <SettingsBehavior
                AllowFocusedRow="False"
                AllowSelectByRowClick="False" />

            <SettingsDataSecurity
                AllowEdit="False"
                AllowInsert="False"
                AllowDelete="False" />

            <SettingsPager Mode="ShowPager" PageSize="50" Position="Bottom">
                <PageSizeItemSettings Visible="true" Items="50,100,200,500" />
            </SettingsPager>

            <%-- 콜백(페이징·정렬·필터) 중 서버 메시지는 JSProperties(cpMessage)로 전달되므로 콜백 종료 시 표시 --%>
            <ClientSideEvents EndCallback="function(s, e) { if (s.cpMessage) { alert(s.cpMessage); s.cpMessage = null; } }" />
        </dx:BootstrapGridView>

        <%-- 엑셀 양식 그대로 읽기전용 표시. 문서는 세션별 DocumentId로 서버 캐시(DocumentManager) --%>
        <dx:ASPxSpreadsheet ID="spreadsheet" runat="server"
            ClientInstanceName="spreadsheet"
            Visible="false"
            Width="100%"
            Height="650px"
            ReadOnly="True"
            RibbonMode="None"
            ShowFormulaBar="False"
            ShowSheetTabs="False"
            ShowConfirmOnLosingChanges="False">
            <SettingsView Mode="Reading" />
        </dx:ASPxSpreadsheet>

        <%-- 엑셀 Exporter --%>
        <dx:ASPxGridViewExporter ID="gridExporter" runat="server" GridViewID="gridSchedule" />

        <%-- 상태 바 --%>
        <div class="status-bar">
            <div class="record-count">
                <asp:Label ID="lblRecordCount" runat="server" Text="조회된 데이터가 없습니다." />
            </div>
        </div>
    </div>
</asp:Content>
