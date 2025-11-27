<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContractManagerXpo.aspx.cs" Inherits="KShiftSmartPortalWeb.ContractManagerXpo" %>
<%@ Register Assembly="DevExpress.Web.v25.1, Version=25.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" 
    Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    계약정보 관리 (XPO) - K-SHIFT Portal
</asp:Content>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        /* 기존 ContractManager.aspx의 스타일을 그대로 사용합니다. */
        /* 조회 조건 패널 스타일 */
        .search-panel { background: #fff; border-radius: 8px; padding: 20px; margin-bottom: 20px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
        .search-panel-header { font-size: 16px; font-weight: 600; color: #333; margin-bottom: 15px; padding-bottom: 10px; border-bottom: 2px solid #0078d4; }
        .search-row { display: flex; flex-wrap: wrap; gap: 20px; align-items: flex-end; }
        .search-item { display: flex; flex-direction: column; gap: 5px; }
        .search-item label { font-size: 12px; font-weight: 600; color: #666; }
        .button-group { display: flex; gap: 10px; margin-top: 15px; padding-top: 15px; border-top: 1px solid #eee; }
        .grid-container { background: #fff; border-radius: 8px; padding: 15px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
        .grid-header { font-size: 16px; font-weight: 600; color: #333; margin-bottom: 15px; padding-bottom: 10px; border-bottom: 2px solid #0078d4; }
        .status-bar { display: flex; justify-content: space-between; align-items: center; padding: 15px; background: #e3f2fd; border-radius: 6px; margin-top: 15px; border-left: 4px solid #0078d4; }
        .record-count { font-size: 14px; color: #333; }
        .record-count strong { color: #0078d4; font-size: 16px; }
    </style>
</asp:Content>

<asp:Content ID="PageTitleContent" ContentPlaceHolderID="PageTitleContent" runat="server">
    계약정보 관리 (XPO)
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- UI는 기존 ContractManager.aspx와 동일하게 복사했습니다. -->
    <div class="search-panel">
        <div class="search-panel-header"><i class="fas fa-search"></i> 조회 조건 (XPO)</div>
        <div class="search-row">
            <div class="search-item">
                <dx:ASPxCheckBox ID="chkSelectAll" runat="server" Text="전체 조회" Checked="true" />
            </div>
        </div>

        <div class="search-row" style="margin-top:15px;">
            <div class="search-item">
                <label>Company 구분:</label>
                <dx:ASPxComboBox ID="cmbCompanyType" runat="server" Width="150px" OnSelectedIndexChanged="cmbCompanyType_SelectedIndexChanged" AutoPostBack="true" />
            </div>
            <div class="search-item">
                <label>Company:</label>
                <dx:ASPxComboBox ID="cmbCompany" runat="server" Width="250px" OnSelectedIndexChanged="cmbCompany_SelectedIndexChanged" AutoPostBack="true" />
            </div>
            <div class="search-item">
                <label>케이스:</label>
                <dx:ASPxComboBox ID="cmbCase" runat="server" Width="200px" />
            </div>
        </div>

        <div class="search-row" style="margin-top:15px;">
            <div class="search-item">
                <label>계약일 (시작):</label>
                <dx:ASPxDateEdit ID="dtStartDate" runat="server" Width="150px" DisplayFormatString="yyyy-MM-dd" EditFormat="Date" AllowNull="false" />
            </div>
            <div class="search-item">
                <label>계약일 (종료):</label>
                <dx:ASPxDateEdit ID="dtEndDate" runat="server" Width="150px" DisplayFormatString="yyyy-MM-dd" EditFormat="Date" AllowNull="false" />
            </div>
        </div>

        <div class="button-group">
            <dx:ASPxButton ID="btnSearch" runat="server" Text="조회" Width="100px" OnClick="btnSearch_Click"><Image IconID="actions_search_16x16office2013" /></dx:ASPxButton>
            <dx:ASPxButton ID="btnReset" runat="server" Text="초기화" Width="100px" OnClick="btnReset_Click"><Image IconID="actions_refresh_16x16office2013" /></dx:ASPxButton>
            <dx:ASPxButton ID="btnExcel" runat="server" Text="엑셀 내보내기" Width="130px" OnClick="btnExcel_Click"><Image IconID="export_exporttoxlsx_16x16office2013" /></dx:ASPxButton>
        </div>
    </div>

    <div class="grid-container">
        <div class="grid-header"><i class="fas fa-table"></i> 계약 목록 (XPO)</div>
        <dx:ASPxGridView ID="gridContracts" runat="server" Width="100%" KeyFieldName="CONTRACT_ID" AutoGenerateColumns="False" EnableCallBacks="true" OnPageIndexChanged="gridContracts_PageIndexChanged" OnBeforeColumnSortingGrouping="gridContracts_BeforeColumnSortingGrouping" OnCustomCallback="gridContracts_CustomCallback">
            <Settings ShowFilterRow="True" ShowFilterRowMenu="True" VerticalScrollBarMode="Visible" VerticalScrollableHeight="450" HorizontalScrollBarMode="Auto" />
            <SettingsBehavior AllowFocusedRow="True" AllowSelectByRowClick="True" />
            <SettingsPager Mode="ShowPager" PageSize="200" Position="Bottom">
                <PageSizeItemSettings Visible="true" Items="50,100,200,500" />
            </SettingsPager>
            <Columns>
                <dx:GridViewDataTextColumn FieldName="COMPANY_NO" Caption="기업" Width="80" ReadOnly="true" />
                <dx:GridViewDataTextColumn FieldName="CASE_NO" Caption="케이스" Width="100" ReadOnly="true" />
                <dx:GridViewDataTextColumn FieldName="CONTRACT_ID" Caption="계약ID" Width="100" ReadOnly="true" />
                <dx:GridViewDataTextColumn FieldName="CONTRACT_NAME" Caption="계약명" Width="250" />
                <dx:GridViewDataDateColumn FieldName="CNTR_DT" Caption="계약일" Width="100"><PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" /></dx:GridViewDataDateColumn>
            </Columns>
            <TotalSummary>
                <dx:ASPxSummaryItem FieldName="CONTRACT_ID" SummaryType="Count" DisplayFormat="총 건" />
            </TotalSummary>
        </dx:ASPxGridView>

        <dx:ASPxGridViewExporter ID="gridExporter" runat="server" GridViewID="gridContracts" />
    </div>

    <div class="status-bar">
        <span class="record-count">
            <i class="fas fa-info-circle"></i>
            <asp:Label ID="lblRecordCount" runat="server" Text="조회된 데이터가 없습니다."></asp:Label>
        </span>
    </div>
</asp:Content>

<asp:Content ID="ScriptContent" ContentPlaceHolderID="ScriptContent" runat="server">
    <%-- 필요 시 스크립트 추가 --%>
</asp:Content>