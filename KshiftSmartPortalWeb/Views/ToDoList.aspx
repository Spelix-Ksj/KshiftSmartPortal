<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Views/Site.Master" 
    CodeBehind="ToDoList.aspx.cs" Inherits="KShiftSmartPortalWeb.ToDoList" %>
<%@ Register Assembly="DevExpress.Web.Bootstrap.v25.1, Version=25.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.Bootstrap" TagPrefix="dx" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    To-Do List - K-SHIFT Portal
</asp:Content>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        /* 전체 폰트 사이즈 */
        .search-panel, .grid-container { font-size: 13px; }

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
        }
        .search-item label {
            font-size: 12px;
            font-weight: 600;
            color: #666;
        }
        .button-group {
            display: flex;
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
            font-size: 14px;
            text-align: center;
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

        /* 컴보박스/DateEdit 높이 */
        .search-item .dxbs-combobox,
        .search-item .dxbs-date-edit,
        .search-item .dxbs-textbox {
            height: 32px !important;
            min-height: 32px !important;
        }
        .search-item .dxbs-combobox .form-control,
        .search-item .dxbs-date-edit .form-control {
            height: 32px !important;
            padding: 2px 8px !important;
            font-size: 13px !important;
        }

        /* 버튼 크기 조정 */
        .button-group .btn {
            padding: 5px 14px !important;
            width: 120px !important;
            font-size: 13px !important;
        }

        /* BootstrapGridView 스타일 */
        #gridToDoList {
            font-size: 12px !important;
        }

        /* 그리드 헤더 가운데 정렬 */
        #gridToDoList th,
        #gridToDoList th a,
        #gridToDoList th span,
        #gridToDoList thead th,
        #gridToDoList thead th a,
        #gridToDoList thead th span,
        .dxbs-gridview th,
        .dxbs-gridview th a,
        .dxbs-gridview th span {
            text-align: center !important;
            vertical-align: middle !important;
            font-size: 12px !important;
            padding: 6px 4px !important;
            color: #fff !important;
        }
        #gridToDoList th a:hover,
        .dxbs-gridview th a:hover {
            color: #fff !important;
            text-decoration: none !important;
        }

        /* 그리드 셀 스타일 */
        #gridToDoList td {
            font-size: 12px !important;
            padding: 4px !important;
            white-space: nowrap;
        }

        /* 헤더 색상 클래스 - CssClasses HeaderCell용 */
        .header-basic {
            background-color: #2196f3 !important;
            color: #fff !important;
        }
        .header-editable {
            background-color: #4caf50 !important;
            color: #fff !important;
        }
        .header-schedule {
            background-color: #ff9800 !important;
            color: #fff !important;
        }
        .header-status {
            background-color: #607d8b !important;
            color: #fff !important;
        }

        /* 수정 가능 컬럼 셀 배경색 */
        .cell-editable {
            background-color: #e8f5e9 !important;
        }

        /* 상태별 셀 색상 */
        .status-complete { background-color: #c8e6c9 !important; color: #2e7d32 !important; font-weight: bold; }
        .status-inprogress { background-color: #fff9c4 !important; color: #f9a825 !important; font-weight: bold; }
        .status-pending { background-color: #ffccbc !important; color: #e64a19 !important; font-weight: bold; }

        /* 필터 행 */
        .dxbs-gridview .dxgvFilterRow td {
            padding: 2px !important;
        }
        .dxbs-gridview .dxgvFilterRow input {
            height: 20px !important;
            font-size: 12px !important;
        }

    </style>
</asp:Content>

<asp:Content ID="PageTitleContent" ContentPlaceHolderID="PageTitleContent" runat="server">
    To-Do List (작업지시 관리)
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- 조회 조건 패널 -->
    <div class="search-panel">
        <div class="search-panel-header">
            <i class="fas fa-search"></i> 조회 조건
        </div>
        
        <div class="search-row">
            <div class="search-item">
                <label>Company 유형:</label>
                <dx:BootstrapComboBox ID="cmbCompanyType" runat="server" 
                    Width="150px" 
                    OnSelectedIndexChanged="cmbCompanyType_SelectedIndexChanged" 
                    AutoPostBack="true" />
            </div>
            <div class="search-item">
                <label>Company:</label>
                <dx:BootstrapComboBox ID="cmbCompany" runat="server" 
                    Width="250px" 
                    AutoPostBack="false" />
            </div>
            <div class="search-item">
                <label>기준일:</label>
                <dx:BootstrapDateEdit  ID="dtBaseDate" runat="server" 
                    Width="150px" 
                    DisplayFormatString="yyyy-MM-dd" 
                    EditFormat="Date" 
                    AllowNull="false" />
            </div>
        </div>
        
        <!-- 버튼 그룹 -->
        <div class="button-group">
            <dx:BootstrapButton ID="btnSearch" runat="server"
                Text="<i class='fas fa-search'></i> 조회"
                EncodeHtml="false"
                OnClick="btnSearch_Click">
                <SettingsBootstrap RenderOption="Primary" />
            </dx:BootstrapButton>

            <dx:BootstrapButton ID="btnReset" runat="server"
                Text="<i class='fas fa-sync-alt'></i> 초기화"
                EncodeHtml="false"
                OnClick="btnReset_Click">
                <SettingsBootstrap RenderOption="Secondary" />
            </dx:BootstrapButton>

            <dx:BootstrapButton ID="btnDelete" runat="server"
                Text="<i class='fas fa-times'></i> 삭제"
                EncodeHtml="false"
                OnClick="btnDelete_Click">
                <SettingsBootstrap RenderOption="Danger" />
                <ClientSideEvents Click="function(s, e) {
                    e.processOnServer = confirm('선택한 데이터를 삭제하시겠습니까?');
                }" />
            </dx:BootstrapButton>

            <dx:BootstrapButton ID="btnExcel" runat="server"
                Text="<i class='fas fa-file-excel'></i> 엑셀 다운로드"
                EncodeHtml="false"
                OnClick="btnExcel_Click">
                <SettingsBootstrap RenderOption="Success" />
            </dx:BootstrapButton>
        </div>
    </div>

    <!-- 그리드 컨테이너 -->
    <div class="grid-container">
        <div class="grid-header">
            <i class="fas fa-tasks"></i> 작업 목록
        </div>
        
        <dx:BootstrapGridView ID="gridToDoList" runat="server"
            ClientInstanceName="gridToDoList"
            Width="100%"
            KeyFieldName="COMPANY_NO;CASE_NO;PROJECT_NO;ORDER_NO"
            AutoGenerateColumns="False"
            EnableCallBacks="true"
            OnPageIndexChanged="gridToDoList_PageIndexChanged"
            OnCustomCallback="gridToDoList_CustomCallback"
            OnRowUpdating="gridToDoList_RowUpdating"
            OnHtmlRowPrepared="gridToDoList_HtmlRowPrepared">

            <Settings
                ShowFilterRow="True"
                ShowFilterRowMenu="True"
                VerticalScrollBarMode="Visible"
                VerticalScrollableHeight="450"
                HorizontalScrollBarMode="Auto" />

            <SettingsBehavior
                AllowFocusedRow="True"
                AllowSelectByRowClick="True" />

            <SettingsDataSecurity
                AllowEdit="True"
                AllowInsert="False"
                AllowDelete="False" />

            <ClientSideEvents EndCallback="function(s, e) {
                    if (s.cpMessage) {
                        alert(s.cpMessage);
                        s.cpMessage = null;
                    }
                }" />
            
            <%-- PopupEditForm 방식 설정 --%>
            <SettingsEditing Mode="PopupEditForm" />
            <SettingsPopup>
                <EditForm 
                    HorizontalAlign="WindowCenter" 
                    VerticalAlign="WindowCenter" 
                    Modal="True" 
                    PopupAnimationType="Auto" />
            </SettingsPopup>
            <SettingsText
                CommandEdit="수정"
                CommandUpdate="저장"
                CommandCancel="취소"
                PopupEditFormCaption="작업지시 수정" />

            <%-- EditForm 템플릿 - 수정 가능한 필드만 표시 --%>
            <Templates>
                <EditForm>
                    <div style="padding: 20px; min-width: 350px;">
                        <table class="table" style="margin-bottom: 15px;">
                            <tr>
                                <td style="width: 120px; font-weight: bold; vertical-align: middle;">완료일</td>
                                <td>
                                    <dx:BootstrapDateEdit ID="edtCompDate" runat="server"
                                        Width="100%"
                                        DisplayFormatString="yyyy-MM-dd"
                                        EditFormat="Date"
                                        AllowNull="true"
                                        Value='<%# Bind("COMP_DATE") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="font-weight: bold; vertical-align: middle;">계획 M/H</td>
                                <td>
                                    <dx:BootstrapSpinEdit ID="edtPlanMhr" runat="server"
                                        Width="100%"
                                        DisplayFormatString="N1"
                                        MinValue="0" MaxValue="99999"
                                        AllowNull="true"
                                        Value='<%# Bind("PLAN_MHR") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="font-weight: bold; vertical-align: middle;">실적 M/H</td>
                                <td>
                                    <dx:BootstrapSpinEdit ID="edtRealMhr" runat="server"
                                        Width="100%"
                                        DisplayFormatString="N1"
                                        MinValue="0" MaxValue="99999"
                                        AllowNull="true"
                                        Value='<%# Bind("REAL_MHR") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="font-weight: bold; vertical-align: middle;">계획 인원</td>
                                <td>
                                    <dx:BootstrapSpinEdit ID="edtPlanMp" runat="server"
                                        Width="100%"
                                        DisplayFormatString="N1"
                                        MinValue="0" MaxValue="9999"
                                        AllowNull="true"
                                        Value='<%# Bind("PLAN_MP") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="font-weight: bold; vertical-align: middle;">실적 인원</td>
                                <td>
                                    <dx:BootstrapSpinEdit ID="edtRealMp" runat="server"
                                        Width="100%"
                                        DisplayFormatString="N1"
                                        MinValue="0" MaxValue="9999"
                                        AllowNull="true"
                                        Value='<%# Bind("REAL_MP") %>' />
                                </td>
                            </tr>
                        </table>
                        <div style="text-align: right; padding-top: 10px; border-top: 1px solid #ddd;">
                            <dx:BootstrapButton ID="btnUpdate" runat="server" Text="저장" AutoPostBack="false">
                                <SettingsBootstrap RenderOption="Primary" />
                                <ClientSideEvents Click="function(s, e) { gridToDoList.UpdateEdit(); }" />
                            </dx:BootstrapButton>
                            <dx:BootstrapButton ID="btnCancel" runat="server" Text="취소" AutoPostBack="false" CssClasses-Control="ml-2">
                                <SettingsBootstrap RenderOption="Secondary" />
                                <ClientSideEvents Click="function(s, e) { gridToDoList.CancelEdit(); }" />
                            </dx:BootstrapButton>
                        </div>
                    </div>
                </EditForm>
            </Templates>

            <SettingsPager Mode="ShowPager" PageSize="50" Position="Bottom">
                <PageSizeItemSettings Visible="true" Items="50,100,200,500" />
            </SettingsPager>

            <Columns>
                <%-- 수정 버튼 컬럼 --%>
                <dx:BootstrapGridViewCommandColumn Width="60px" ShowEditButton="True" VisibleIndex="0" Caption=" ">
                    <CssClasses HeaderCell="text-center" />
                </dx:BootstrapGridViewCommandColumn>

                

                <%-- 기본 정보 컬럼 (ReadOnly) - 파란색 #2196f3 --%>
                <dx:BootstrapGridViewTextColumn FieldName="PROJECT_NO" Caption="프로젝트 번호" Width="120px" ReadOnly="true" VisibleIndex="1">
                    <CssClasses HeaderCell="header-basic text-center" />
                </dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewTextColumn FieldName="PROP01" Caption="호선번호" Width="100px" ReadOnly="true" VisibleIndex="2">
                    <CssClasses HeaderCell="header-basic text-center" />
                </dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewTextColumn FieldName="ORDER_NAME" Caption="지시명칭" Width="200px" ReadOnly="true" VisibleIndex="3">
                    <CssClasses HeaderCell="header-basic text-center" />
                </dx:BootstrapGridViewTextColumn>
                
                <%-- 수정 가능 컬럼 - 녹색 #4caf50 --%>
                <dx:BootstrapGridViewDateColumn FieldName="COMP_DATE" Caption="완료일" Width="120px" VisibleIndex="4">
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" AllowNull="true" />
                    <CssClasses HeaderCell="header-editable text-center" />
                </dx:BootstrapGridViewDateColumn>

                <dx:BootstrapGridViewSpinEditColumn FieldName="PLAN_MHR" Caption="계획 M/H" Width="100px" VisibleIndex="5">
                    <PropertiesSpinEdit DisplayFormatString="N1" MinValue="0" MaxValue="99999" AllowNull="true" />
                    <CssClasses HeaderCell="header-editable text-center" />
                </dx:BootstrapGridViewSpinEditColumn>

                <dx:BootstrapGridViewSpinEditColumn FieldName="REAL_MHR" Caption="실적 M/H" Width="100px" VisibleIndex="6">
                    <PropertiesSpinEdit DisplayFormatString="N1" MinValue="0" MaxValue="99999" AllowNull="true" />
                    <CssClasses HeaderCell="header-editable text-center" />
                </dx:BootstrapGridViewSpinEditColumn>

                <dx:BootstrapGridViewSpinEditColumn FieldName="PLAN_MP" Caption="계획 인원" Width="100px" VisibleIndex="7">
                    <PropertiesSpinEdit DisplayFormatString="N1" MinValue="0" MaxValue="9999" AllowNull="true" />
                    <CssClasses HeaderCell="header-editable text-center" />
                </dx:BootstrapGridViewSpinEditColumn>

                <dx:BootstrapGridViewSpinEditColumn FieldName="REAL_MP" Caption="실적 인원" Width="100px" VisibleIndex="8">
                    <PropertiesSpinEdit DisplayFormatString="N1" MinValue="0" MaxValue="9999" AllowNull="true" />
                    <CssClasses HeaderCell="header-editable text-center" />
                </dx:BootstrapGridViewSpinEditColumn>

                 <%-- 기본 정보 컬럼 (ReadOnly) - 파란색 #2196f3 --%>
                <dx:BootstrapGridViewTextColumn FieldName="ORDER_NO" Caption="지시번호" Width="120px" ReadOnly="true" VisibleIndex="9">
                    <CssClasses HeaderCell="header-basic text-center" />
                </dx:BootstrapGridViewTextColumn>                
                <dx:BootstrapGridViewTextColumn FieldName="PROP02" Caption="블록번호" Width="100px" ReadOnly="true" VisibleIndex="10">
                    <CssClasses HeaderCell="header-basic text-center" />
                </dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewTextColumn FieldName="WORK_LIST" Caption="작업목록" Width="150px" ReadOnly="true" VisibleIndex="11">
                    <CssClasses HeaderCell="header-basic text-center" />
                </dx:BootstrapGridViewTextColumn>

                <%-- 일정 정보 컬럼 (ReadOnly) - 주황색 #ff9800 --%>
                <dx:BootstrapGridViewDateColumn FieldName="WORK_ST" Caption="작업시작일" Width="110px" ReadOnly="true" VisibleIndex="12">
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                    <CssClasses HeaderCell="header-schedule text-center" />
                </dx:BootstrapGridViewDateColumn>
                <dx:BootstrapGridViewDateColumn FieldName="WORK_FI" Caption="작업종료일" Width="110px" ReadOnly="true" VisibleIndex="13">
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                    <CssClasses HeaderCell="header-schedule text-center" />
                </dx:BootstrapGridViewDateColumn>
                <dx:BootstrapGridViewDateColumn FieldName="DUE_DATE" Caption="마감일" Width="110px" ReadOnly="true" VisibleIndex="14">
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                    <CssClasses HeaderCell="header-schedule text-center" />
                </dx:BootstrapGridViewDateColumn>
                <dx:BootstrapGridViewDateColumn FieldName="QM_DATE" Caption="품질관리일" Width="110px" ReadOnly="true" VisibleIndex="15">
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                    <CssClasses HeaderCell="header-schedule text-center" />
                </dx:BootstrapGridViewDateColumn>
                <dx:BootstrapGridViewDateColumn FieldName="QM_COMP_DATE" Caption="QM완료일" Width="110px" ReadOnly="true" VisibleIndex="16">
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                    <CssClasses HeaderCell="header-schedule text-center" />
                </dx:BootstrapGridViewDateColumn>

                <%-- 상태 컬럼 (ReadOnly) --%>
                <dx:BootstrapGridViewTextColumn FieldName="STATUS" Caption="상태" Width="80px" ReadOnly="true" VisibleIndex="17">
                    <CssClasses HeaderCell="header-status text-center" />
                </dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewTextColumn FieldName="MHR_RATE" Caption="달성률(%)" Width="90px" ReadOnly="true" VisibleIndex="18">
                    <CssClasses HeaderCell="header-status text-center" />
                </dx:BootstrapGridViewTextColumn>

                <%-- 숨김 Key 컬럼 --%>
                <dx:BootstrapGridViewTextColumn FieldName="COMPANY_NO" Visible="false" />
                <dx:BootstrapGridViewTextColumn FieldName="CASE_NO" Visible="false" />
            </Columns>
        </dx:BootstrapGridView>
        
        <%-- 엑셀 Exporter --%>
        <dx:ASPxGridViewExporter ID="gridExporter" runat="server" GridViewID="gridToDoList" />
        
        <%-- 상태 바 --%>
        <div class="status-bar">
            <div class="record-count">
                <asp:Label ID="lblRecordCount" runat="server" Text="조회된 데이터가 없습니다." />
            </div>
            <div>
                <span style="margin-right:15px; color:#4caf50;">■ 수정가능</span>
                <span style="margin-right:15px; color:#2196f3;">■ 기본정보</span>
                <span style="margin-right:15px; color:#ff9800;">■ 일정정보</span>
            </div>
        </div>
    </div>
</asp:Content>