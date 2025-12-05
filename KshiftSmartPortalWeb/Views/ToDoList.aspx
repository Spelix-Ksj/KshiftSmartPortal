<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Views/Site.Master" 
    CodeBehind="ToDoList.aspx.cs" Inherits="KShiftSmartPortalWeb.ToDoList" %>
<%@ Register Assembly="DevExpress.Web.v25.1, Version=25.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" 
    Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    To-Do List - K-SHIFT Portal
</asp:Content>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        /* 조회 조건 패널 스타일 */
        .search-panel { 
            background: #fff; 
            border-radius: 8px; 
            padding: 20px; 
            margin-bottom: 20px; 
            box-shadow: 0 2px 4px rgba(0,0,0,0.1); 
        }
        .search-panel-header { 
            font-size: 16px; 
            font-weight: 600; 
            color: #333; 
            margin-bottom: 15px; 
            padding-bottom: 10px; 
            border-bottom: 2px solid #17a2b8; 
        }
        .search-row { 
            display: flex; 
            flex-wrap: wrap; 
            gap: 20px; 
            align-items: flex-end; 
        }
        .search-item { 
            display: flex; 
            flex-direction: column; 
            gap: 5px; 
        }
        .search-item label { 
            font-size: 12px; 
            font-weight: 600; 
            color: #666; 
        }
        .button-group { 
            display: flex; 
            gap: 10px; 
            margin-top: 15px; 
            padding-top: 15px; 
            border-top: 1px solid #eee; 
        }
        .grid-container { 
            background: #fff; 
            border-radius: 8px; 
            padding: 15px; 
            box-shadow: 0 2px 4px rgba(0,0,0,0.1); 
        }
        .grid-header { 
            font-size: 16px; 
            text-align: center; 
            font-weight: 600; 
            color: #333; 
            margin-bottom: 15px; 
            padding-bottom: 10px; 
            border-bottom: 2px solid #17a2b8; 
        }
        .status-bar { 
            display: flex; 
            justify-content: space-between; 
            align-items: center; 
            padding: 15px; 
            background: #e8f4f8; 
            border-radius: 6px; 
            margin-top: 15px; 
            border-left: 4px solid #17a2b8; 
        }
        .record-count { 
            font-size: 14px; 
            color: #333; 
        }
        .record-count strong { 
            color: #17a2b8; 
            font-size: 16px; 
        }
        
        /* 그리드 컬럼 그룹별 헤더 색상 */
        .col-basic { background-color: #e3f2fd !important; }      /* 기본정보 - 파랑 */
        .col-schedule { background-color: #fff3e0 !important; }   /* 일정정보 - 주황 */
        .col-editable { background-color: #e8f5e9 !important; }   /* 수정가능 - 녹색 */
        .col-status { background-color: #fce4ec !important; }     /* 상태 - 분홍 */
        
        /* 수정 가능 컬럼 셀 배경색 */
        .cell-editable { background-color: #f1f8e9 !important; }

        /* 선택된 행에서 수정 가능 셀 스타일 - 글자가 보이도록 */
        .dxgvFocusedRow .cell-editable,
        .dxgvSelectedRow .cell-editable,
        .dxgvFocusedRow_Office2003Blue .cell-editable,
        .dxgvSelectedRow_Office2003Blue .cell-editable {
            background-color: #1976d2 !important;
            color: #fff !important;
        }

        /* 상태별 셀 색상 */
        .status-complete { background-color: #c8e6c9 !important; color: #2e7d32 !important; font-weight: bold; }
        .status-inprogress { background-color: #fff9c4 !important; color: #f9a825 !important; font-weight: bold; }
        .status-pending { background-color: #ffccbc !important; color: #e64a19 !important; font-weight: bold; }
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
                <dx:ASPxComboBox ID="cmbCompanyType" runat="server" 
                    Width="150px" 
                    OnSelectedIndexChanged="cmbCompanyType_SelectedIndexChanged" 
                    AutoPostBack="true" />
            </div>
            <div class="search-item">
                <label>Company:</label>
                <dx:ASPxComboBox ID="cmbCompany" runat="server" 
                    Width="250px" 
                    AutoPostBack="false" />
            </div>
            <div class="search-item">
                <label>기준일:</label>
                <dx:ASPxDateEdit ID="dtBaseDate" runat="server" 
                    Width="150px" 
                    DisplayFormatString="yyyy-MM-dd" 
                    EditFormat="Date" 
                    AllowNull="false" />
            </div>
        </div>
        
        <!-- 버튼 그룹 -->
        <div class="button-group">
            <dx:ASPxButton ID="btnSearch" runat="server" 
                Text="조회" Width="100px" 
                OnClick="btnSearch_Click">
                <Image IconID="actions_search_16x16office2013" />
            </dx:ASPxButton>
            
            <dx:ASPxButton ID="btnReset" runat="server" 
                Text="초기화" Width="100px" 
                OnClick="btnReset_Click">
                <Image IconID="actions_refresh_16x16office2013" />
            </dx:ASPxButton>
            
            <dx:ASPxButton ID="btnDelete" runat="server" 
                Text="삭제" Width="100px" 
                OnClick="btnDelete_Click">
                <Image IconID="edit_delete_16x16office2013" />
                <ClientSideEvents Click="function(s, e) {
                    e.processOnServer = confirm('선택한 데이터를 삭제하시겠습니까?');
                }" />
            </dx:ASPxButton>
            
            <dx:ASPxButton ID="btnExcel" runat="server" 
                Text="엑셀 다운로드" Width="130px" 
                OnClick="btnExcel_Click">
                <Image IconID="export_exporttoxlsx_16x16office2013" />
            </dx:ASPxButton>
        </div>
    </div>

    <!-- 그리드 컨테이너 -->
    <div class="grid-container">
        <div class="grid-header">
            <i class="fas fa-tasks"></i> 작업 목록
        </div>
        
        <dx:ASPxGridView ID="gridToDoList" runat="server" 
            Width="100%" 
            KeyFieldName="COMPANY_NO;CASE_NO;PROJECT_NO;ORDER_NO"
            AutoGenerateColumns="False" 
            EnableCallBacks="true"
            OnPageIndexChanged="gridToDoList_PageIndexChanged"
            OnBeforeColumnSortingGrouping="gridToDoList_BeforeColumnSortingGrouping"
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
            
            <SettingsPager Mode="ShowPager" PageSize="50" Position="Bottom">
                <PageSizeItemSettings Visible="true" Items="50,100,200,500" />
            </SettingsPager>

            <%-- EditForm 레이아웃 설정 --%>
            <EditFormLayoutProperties ColumnCount="2">
                <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="600" />
            </EditFormLayoutProperties>

            <Columns>
                <%-- 수정 버튼 컬럼 (첫 번째 컬럼) --%>
                <dx:GridViewCommandColumn Width="60px" ShowEditButton="True" VisibleIndex="0" Caption=" ">
                    <HeaderStyle HorizontalAlign="Center" />
                    <CellStyle HorizontalAlign="Center" />
                </dx:GridViewCommandColumn>

                <%-- 기본 정보 컬럼 그룹 (ReadOnly) --%>
                <dx:GridViewDataTextColumn FieldName="PROJECT_NO" Caption="프로젝트 번호" Width="120px" 
                    ReadOnly="true" HeaderStyle-CssClass="col-basic">
                    <EditFormSettings Visible="False" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="PROP01" Caption="호선번호" Width="100px" 
                    ReadOnly="true" HeaderStyle-CssClass="col-basic">
                    <EditFormSettings Visible="False" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="ORDER_NO" Caption="지시번호" Width="120px" 
                    ReadOnly="true" HeaderStyle-CssClass="col-basic">
                    <EditFormSettings Visible="False" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="ORDER_NAME" Caption="지시명칭" Width="200px" 
                    ReadOnly="true" HeaderStyle-CssClass="col-basic">
                    <EditFormSettings Visible="False" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="PROP02" Caption="블록번호" Width="100px" 
                    ReadOnly="true" HeaderStyle-CssClass="col-basic">
                    <EditFormSettings Visible="False" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="WORK_LIST" Caption="작업목록" Width="150px" 
                    ReadOnly="true" HeaderStyle-CssClass="col-basic">
                    <EditFormSettings Visible="False" />
                </dx:GridViewDataTextColumn>
                
                <%-- 수정 가능 컬럼 그룹 (PopupEditForm에 표시) --%>
                <dx:GridViewDataDateColumn FieldName="COMP_DATE" Caption="완료일" Width="120px" 
                    HeaderStyle-CssClass="col-editable" CellStyle-CssClass="cell-editable">
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" AllowNull="true">
                        <ValidationSettings>
                            <RequiredField IsRequired="false" />
                        </ValidationSettings>
                    </PropertiesDateEdit>
                    <EditFormSettings Caption="완료일" VisibleIndex="0" />
                </dx:GridViewDataDateColumn>
                
                <dx:GridViewDataSpinEditColumn FieldName="PLAN_MHR" Caption="계획 M/H" Width="100px" 
                    HeaderStyle-CssClass="col-editable" CellStyle-CssClass="cell-editable">
                    <PropertiesSpinEdit DisplayFormatString="N1" MinValue="0" MaxValue="99999" 
                        SpinButtons-ShowIncrementButtons="false" AllowNull="true" />
                    <EditFormSettings Caption="계획 M/H" VisibleIndex="1" />
                </dx:GridViewDataSpinEditColumn>
                
                <dx:GridViewDataSpinEditColumn FieldName="REAL_MHR" Caption="실적 M/H" Width="100px" 
                    HeaderStyle-CssClass="col-editable" CellStyle-CssClass="cell-editable">
                    <PropertiesSpinEdit DisplayFormatString="N1" MinValue="0" MaxValue="99999" 
                        SpinButtons-ShowIncrementButtons="false" AllowNull="true" />
                    <EditFormSettings Caption="실적 M/H" VisibleIndex="2" />
                </dx:GridViewDataSpinEditColumn>
                
                <dx:GridViewDataSpinEditColumn FieldName="PLAN_MP" Caption="계획 인원" Width="100px" 
                    HeaderStyle-CssClass="col-editable" CellStyle-CssClass="cell-editable">
                    <PropertiesSpinEdit DisplayFormatString="N1" MinValue="0" MaxValue="9999" 
                        SpinButtons-ShowIncrementButtons="false" AllowNull="true" />
                    <EditFormSettings Caption="계획 인원" VisibleIndex="3" />
                </dx:GridViewDataSpinEditColumn>
                
                <dx:GridViewDataSpinEditColumn FieldName="REAL_MP" Caption="실적 인원" Width="100px" 
                    HeaderStyle-CssClass="col-editable" CellStyle-CssClass="cell-editable">
                    <PropertiesSpinEdit DisplayFormatString="N1" MinValue="0" MaxValue="9999" 
                        SpinButtons-ShowIncrementButtons="false" AllowNull="true" />
                    <EditFormSettings Caption="실적 인원" VisibleIndex="4" />
                </dx:GridViewDataSpinEditColumn>

                <%-- 일정 정보 컬럼 그룹 (ReadOnly) --%>
                <dx:GridViewDataDateColumn FieldName="WORK_ST" Caption="작업시작일" Width="110px" 
                    ReadOnly="true" HeaderStyle-CssClass="col-schedule">
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                    <EditFormSettings Visible="False" />
                </dx:GridViewDataDateColumn>
                <dx:GridViewDataDateColumn FieldName="WORK_FI" Caption="작업종료일" Width="110px" 
                    ReadOnly="true" HeaderStyle-CssClass="col-schedule">
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                    <EditFormSettings Visible="False" />
                </dx:GridViewDataDateColumn>
                <dx:GridViewDataDateColumn FieldName="DUE_DATE" Caption="마감일" Width="110px" 
                    ReadOnly="true" HeaderStyle-CssClass="col-schedule">
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                    <EditFormSettings Visible="False" />
                </dx:GridViewDataDateColumn>
                <dx:GridViewDataDateColumn FieldName="QM_DATE" Caption="품질관리일" Width="110px" 
                    ReadOnly="true" HeaderStyle-CssClass="col-schedule">
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                    <EditFormSettings Visible="False" />
                </dx:GridViewDataDateColumn>
                <dx:GridViewDataDateColumn FieldName="QM_COMP_DATE" Caption="QM완료일" Width="110px" 
                    ReadOnly="true" HeaderStyle-CssClass="col-schedule">
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                    <EditFormSettings Visible="False" />
                </dx:GridViewDataDateColumn>
                

                <%-- 계산/상태 컬럼 (ReadOnly) --%>
                <dx:GridViewDataTextColumn FieldName="STATUS" Caption="상태" Width="80px" 
                    ReadOnly="true" HeaderStyle-CssClass="col-status">
                    <EditFormSettings Visible="False" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="MHR_RATE" Caption="달성률(%)" Width="90px" 
                    ReadOnly="true" HeaderStyle-CssClass="col-status">
                    <PropertiesTextEdit DisplayFormatString="N1" />
                    <EditFormSettings Visible="False" />
                </dx:GridViewDataTextColumn>

                <%-- 숨김 Key 컬럼 (4개 복합키) --%>
                <dx:GridViewDataTextColumn FieldName="COMPANY_NO" Visible="false">
                    <EditFormSettings Visible="False" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="CASE_NO" Visible="false">
                    <EditFormSettings Visible="False" />
                </dx:GridViewDataTextColumn>
            </Columns>
            
            <%-- Styles 섹션: 테마 색상 사용 (직접 색상 지정 제거) --%>
            <Styles>
                <Header BackColor="#f5f5f5" Font-Bold="true" />
            </Styles>
        </dx:ASPxGridView>
        
        <%-- 엑셀 Exporter --%>
        <dx:ASPxGridViewExporter ID="gridExporter" runat="server" GridViewID="gridToDoList" />
        
        <%-- 상태 바 --%>
        <div class="status-bar">
            <div class="record-count">
                <asp:Label ID="lblRecordCount" runat="server" Text="조회된 데이터가 없습니다." />
            </div>
            <div>
                <span style="margin-right:15px; color:#4caf50;">■ 수정가능 필드</span>
                <span style="margin-right:15px; color:#2196f3;">■ 기본정보</span>
                <span style="margin-right:15px; color:#ff9800;">■ 일정정보</span>
            </div>
        </div>
    </div>
</asp:Content>