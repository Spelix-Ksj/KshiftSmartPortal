<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Views/Site.Master" CodeBehind="ContractManagerXpo.aspx.cs" Inherits="KShiftSmartPortalWeb.ContractManagerXpo" %>
<%@ Register Assembly="DevExpress.Web.v25.1, Version=25.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" 
    Namespace="DevExpress.Web" TagPrefix="dx" %>


<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    계약정보 관리 - K-SHIFT Portal
</asp:Content>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        /* 기존 ContractManager.aspx의 스타일을 그대로 유지합니다. */
        /* 조회 조건 패널 스타일 */
        .search-panel { background: #fff; border-radius: 8px; padding: 20px; margin-bottom: 20px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
        .search-panel-header { font-size: 16px; font-weight: 600; color: #333; margin-bottom: 15px; padding-bottom: 10px; border-bottom: 2px solid #0078d4; }
        .search-row { display: flex; flex-wrap: wrap; gap: 20px; align-items: flex-end; }
        .search-item { display: flex; flex-direction: column; gap: 5px; }
        .search-item label { font-size: 12px; font-weight: 600; color: #666; }
        .button-group { display: flex; gap: 10px; margin-top: 15px; padding-top: 15px; border-top: 1px solid #eee; }
        .grid-container { background: #fff; border-radius: 8px; padding: 15px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
        .grid-header { font-size: 16px; text-align:center; font-weight: 600; color: #333; margin-bottom: 15px; padding-bottom: 10px; border-bottom: 2px solid #0078d4; }
        .status-bar { display: flex; justify-content: space-between; align-items: center; padding: 15px; background: #e3f2fd; border-radius: 6px; margin-top: 15px; border-left: 4px solid #0078d4; }
        .record-count { font-size: 14px; color: #333; }
        .record-count strong { color: #0078d4; font-size: 16px; }
    </style>
</asp:Content>

<asp:Content ID="PageTitleContent" ContentPlaceHolderID="PageTitleContent" runat="server">
    계약정보 관리
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- UI는 기존 ContractManager.aspx를 참고하여 작성했습니다. -->
    <div class="search-panel">
        <div class="search-panel-header"><i class="fas fa-search"></i> 조회 조건</div>
        <div class="search-row">
            <div class="search-item">
                <dx:ASPxCheckBox ID="chkSelectAll" runat="server" Text="전체 조회" Checked="true" />
            </div>
        </div>

        <div class="search-row" style="margin-top:15px;">
            <div class="search-item">
                <label>Company 유형:</label>
                <dx:ASPxComboBox ID="cmbCompanyType" runat="server" Width="150px" OnSelectedIndexChanged="cmbCompanyType_SelectedIndexChanged" AutoPostBack="true" />
            </div>
            <div class="search-item">
                <label>Company:</label>
                <dx:ASPxComboBox ID="cmbCompany" runat="server" Width="250px" OnSelectedIndexChanged="cmbCompany_SelectedIndexChanged" AutoPostBack="true" />
            </div>
            <div class="search-item">
                <label>케이스:</label>
                <dx:ASPxComboBox ID="cmbCase" runat="server"
                    Width="200px"
                    ValueType="System.String"
                    TextField="CASE_NAME"
                    ValueField="CASE_NO"
                    NullText="선택">
                </dx:ASPxComboBox>
            </div>
        </div>

        <div class="search-row" style="margin-top:15px;">
            <div class="search-item">
                <label>기준일 (시작):</label>
                <dx:ASPxDateEdit ID="dtStartDate" runat="server" Width="150px" DisplayFormatString="yyyy-MM-dd" EditFormat="Date" AllowNull="false" />
            </div>
            <div class="search-item">
                <label>기준일 (종료):</label>
                <dx:ASPxDateEdit ID="dtEndDate" runat="server" Width="150px" DisplayFormatString="yyyy-MM-dd" EditFormat="Date" AllowNull="false" />
            </div>
        </div>
        
        <div class="button-group">
            <dx:ASPxButton ID="btnSearch" runat="server" Text="조회" Width="100px" OnClick="btnSearch_Click"><Image
        IconID="actions_search_16x16office2013" /></dx:ASPxButton>
            <dx:ASPxButton ID="btnReset" runat="server" Text="초기화" Width="100px" OnClick="btnReset_Click"><Image
        IconID="actions_refresh_16x16office2013" /></dx:ASPxButton>
            <dx:ASPxButton ID="btnSave" runat="server" Text="저장" Width="100px" OnClick="btnSave_Click"><Image
        IconID="actions_apply_16x16office2013" /></dx:ASPxButton>
            <dx:ASPxButton ID="btnDelete" runat="server" Text="삭제" Width="100px" OnClick="btnDelete_Click">
                <Image IconID="edit_delete_16x16office2013" />
                <ClientSideEvents Click="function(s, e) {
                    e.processOnServer = confirm('선택한 데이터를 삭제하시겠습니까?');
                }" />
            </dx:ASPxButton>
            <dx:ASPxButton ID="btnExcel" runat="server" Text="엑셀 다운로드" Width="130px" OnClick="btnExcel_Click"><Image
        IconID="export_exporttoxlsx_16x16office2013" /></dx:ASPxButton>
        </div>
    </div>

    <div class="grid-container">
        <div class="grid-header"><i class="fas fa-table"></i> 조회 결과</div>
        <dx:ASPxGridView ID="gridContracts" runat="server" Width="100%" KeyFieldName="CONTRACT_ID" AutoGenerateColumns="False" EnableCallBacks="true"
              OnPageIndexChanged="gridContracts_PageIndexChanged"
              OnBeforeColumnSortingGrouping="gridContracts_BeforeColumnSortingGrouping"
              OnCustomCallback="gridContracts_CustomCallback"
              OnRowUpdating="gridContracts_RowUpdating">
            <Settings ShowFilterRow="True" ShowFilterRowMenu="True" VerticalScrollBarMode="Visible" VerticalScrollableHeight="450" HorizontalScrollBarMode="Auto" />
            <SettingsBehavior AllowFocusedRow="True" AllowSelectByRowClick="True" />
            <SettingsEditing Mode="EditForm" />
            <SettingsPager Mode="ShowPager" PageSize="50" Position="Bottom">
                <PageSizeItemSettings Visible="true" Items="50,100,200,500" />
            </SettingsPager>
            <Columns>
                <%-- 수정 버튼 컬럼 추가 --%>
                <dx:GridViewCommandColumn ShowEditButton="true" Caption="수정" Width="50">
                    <HeaderStyle BackColor="#EEEEEE" Font-Bold="true" />                    
                </dx:GridViewCommandColumn>

                <%-- ========== 기본정보 (파란색 계열) ========== --%>
                <dx:GridViewDataTextColumn FieldName="COMPANY_NO" Caption="회사" Width="80" ReadOnly="true">
                    <HeaderStyle BackColor="#E3F2FD" ForeColor="#1565C0" Font-Bold="true" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="CASE_NO" Caption="케이스" Width="100" ReadOnly="true">
                    <HeaderStyle BackColor="#E3F2FD" ForeColor="#1565C0" Font-Bold="true" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="CONTRACT_ID" Caption="계약ID" Width="100" ReadOnly="true">
                    <HeaderStyle BackColor="#E3F2FD" ForeColor="#1565C0" Font-Bold="true" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="STD_CASE_NO" Caption="표준케이스" Width="90">
                    <HeaderStyle BackColor="#E3F2FD" ForeColor="#1565C0" Font-Bold="true" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="STD_CONTRACT_ID" Caption="표준계약" Width="90">
                    <HeaderStyle BackColor="#E3F2FD" ForeColor="#1565C0" Font-Bold="true" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="PROP1" Caption="프로타입번호" Width="90">
                    <HeaderStyle BackColor="#E3F2FD" ForeColor="#1565C0" Font-Bold="true" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="PROP2" Caption="작업가이드" Width="90">
                    <HeaderStyle BackColor="#E3F2FD" ForeColor="#1565C0" Font-Bold="true" />
                </dx:GridViewDataTextColumn>

                <%-- ========== 계약일반 (주황색 계열) ========== --%>
                <dx:GridViewDataTextColumn FieldName="CONTRACT_NAME" Caption="계약명" Width="200">
                    <HeaderStyle BackColor="#FFF3E0" ForeColor="#E65100" Font-Bold="true" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="CONTRACT_CATEGORY" Caption="계약카테고리" Width="100">
                    <HeaderStyle BackColor="#FFF3E0" ForeColor="#E65100" Font-Bold="true" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="CONTRACT_TYPE" Caption="계약구분" Width="80">
                    <HeaderStyle BackColor="#FFF3E0" ForeColor="#E65100" Font-Bold="true" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="CONTRACT_NO" Caption="계약번호" Width="100">
                    <HeaderStyle BackColor="#FFF3E0" ForeColor="#E65100" Font-Bold="true" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="POR_NO" Caption="POR" Width="80">
                    <HeaderStyle BackColor="#FFF3E0" ForeColor="#E65100" Font-Bold="true" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="SEQ_NO" Caption="Seq" Width="60">
                    <HeaderStyle BackColor="#FFF3E0" ForeColor="#E65100" Font-Bold="true" />
                </dx:GridViewDataTextColumn>
                
                <%-- ========== 날짜정보 (초록색 계열) ========== --%>
                <dx:GridViewDataDateColumn FieldName="POR_DT" Caption="POR발행일" Width="100">
                    <HeaderStyle BackColor="#E8F5E9" ForeColor="#2E7D32" Font-Bold="true" />
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                </dx:GridViewDataDateColumn>
                <dx:GridViewDataDateColumn FieldName="CNTR_DT" Caption="계약일" Width="100">
                    <HeaderStyle BackColor="#E8F5E9" ForeColor="#2E7D32" Font-Bold="true" />
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                </dx:GridViewDataDateColumn>
                <dx:GridViewDataDateColumn FieldName="CNTR_INIT_DT" Caption="초기계약일" Width="100" ReadOnly="true">
                    <HeaderStyle BackColor="#E8F5E9" ForeColor="#2E7D32" Font-Bold="true" />
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                </dx:GridViewDataDateColumn>
                <dx:GridViewDataDateColumn FieldName="MP_DT" Caption="납기일" Width="100">
                    <HeaderStyle BackColor="#E8F5E9" ForeColor="#2E7D32" Font-Bold="true" />
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                </dx:GridViewDataDateColumn>
                <dx:GridViewDataDateColumn FieldName="MP_INIT_DT" Caption="초기납기일" Width="100" ReadOnly="true">
                    <HeaderStyle BackColor="#E8F5E9" ForeColor="#2E7D32" Font-Bold="true" />
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                </dx:GridViewDataDateColumn>

                <%-- ========== 제품정보 (분홍색 계열) ========== --%>
                <dx:GridViewDataTextColumn FieldName="PRODUCT_TYPE" Caption="제품타입" Width="100">
                    <HeaderStyle BackColor="#FCE4EC" ForeColor="#C2185B" Font-Bold="true" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="PRODUCT_DESC" Caption="제품설명" Width="150">
                    <HeaderStyle BackColor="#FCE4EC" ForeColor="#C2185B" Font-Bold="true" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataSpinEditColumn FieldName="CNTR_EA" Caption="수량" Width="70">
                    <HeaderStyle BackColor="#FCE4EC" ForeColor="#C2185B" Font-Bold="true" />
                    <PropertiesSpinEdit DisplayFormatString="N0" />
                </dx:GridViewDataSpinEditColumn>
                <dx:GridViewDataSpinEditColumn FieldName="CNTR_PIECE_WGT" Caption="단중" Width="80">
                    <HeaderStyle BackColor="#FCE4EC" ForeColor="#C2185B" Font-Bold="true" />
                    <PropertiesSpinEdit DisplayFormatString="N2" />
                </dx:GridViewDataSpinEditColumn>
                <dx:GridViewDataSpinEditColumn FieldName="CNTR_WGT" Caption="중량" Width="80">
                    <HeaderStyle BackColor="#FCE4EC" ForeColor="#C2185B" Font-Bold="true" />
                    <PropertiesSpinEdit DisplayFormatString="N2" />
                </dx:GridViewDataSpinEditColumn>
                <dx:GridViewDataTextColumn FieldName="PROJECT_NO" Caption="프로젝트번호" Width="100">
                    <HeaderStyle BackColor="#FCE4EC" ForeColor="#C2185B" Font-Bold="true" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="BLOCK_NO" Caption="블록번호" Width="80">
                    <HeaderStyle BackColor="#FCE4EC" ForeColor="#C2185B" Font-Bold="true" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="MARK_NO" Caption="마크번호" Width="80">
                    <HeaderStyle BackColor="#FCE4EC" ForeColor="#C2185B" Font-Bold="true" />
                </dx:GridViewDataTextColumn>

                <%-- ========== 업체정보 (보라색 계열) ========== --%>
                <dx:GridViewDataTextColumn FieldName="OWNER" Caption="발주사" Width="100">
                    <HeaderStyle BackColor="#F3E5F5" ForeColor="#7B1FA2" Font-Bold="true" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="TAG1" Caption="비고란" Width="100">
                    <HeaderStyle BackColor="#F3E5F5" ForeColor="#7B1FA2" Font-Bold="true" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="OWNER_DEPT" Caption="발주처" Width="100">
                    <HeaderStyle BackColor="#F3E5F5" ForeColor="#7B1FA2" Font-Bold="true" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="MAIN_CONTRACTOR" Caption="원청" Width="100">
                    <HeaderStyle BackColor="#F3E5F5" ForeColor="#7B1FA2" Font-Bold="true" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="SUB_CONTRACTOR" Caption="하청업체" Width="100">
                    <HeaderStyle BackColor="#F3E5F5" ForeColor="#7B1FA2" Font-Bold="true" />
                </dx:GridViewDataTextColumn>

                <%-- ========== 자재정보 (청록색 계열) ========== --%>
                <dx:GridViewDataTextColumn FieldName="MS_NO" Caption="자재번호" Width="100">
                    <HeaderStyle BackColor="#E0F7FA" ForeColor="#00838F" Font-Bold="true" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataDateColumn FieldName="MS_DT" Caption="자재계약일" Width="100">
                    <HeaderStyle BackColor="#E0F7FA" ForeColor="#00838F" Font-Bold="true" />
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                </dx:GridViewDataDateColumn>
                <dx:GridViewDataTextColumn FieldName="MS_CONTRACTOR" Caption="자재업체" Width="100">
                    <HeaderStyle BackColor="#E0F7FA" ForeColor="#00838F" Font-Bold="true" />
                </dx:GridViewDataTextColumn>

                <%-- ========== 일정정보 (남색 계열) ========== --%>
                <dx:GridViewDataDateColumn FieldName="MAKING_DT" Caption="제작예정일" Width="100">
                    <HeaderStyle BackColor="#E8EAF6" ForeColor="#3949AB" Font-Bold="true" />
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                </dx:GridViewDataDateColumn>
                <dx:GridViewDataDateColumn FieldName="MAKING_RES_DT" Caption="제작실적일" Width="100">
                    <HeaderStyle BackColor="#E8EAF6" ForeColor="#3949AB" Font-Bold="true" />
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                </dx:GridViewDataDateColumn>
                <dx:GridViewDataDateColumn FieldName="PAINTING_DT" Caption="도장예정일" Width="100">
                    <HeaderStyle BackColor="#E8EAF6" ForeColor="#3949AB" Font-Bold="true" />
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                </dx:GridViewDataDateColumn>
                <dx:GridViewDataDateColumn FieldName="PAINTING_RES_DT" Caption="도장실적일" Width="100">
                    <HeaderStyle BackColor="#E8EAF6" ForeColor="#3949AB" Font-Bold="true" />
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                </dx:GridViewDataDateColumn>
                
                <%-- ========== 기타 (회색 계열) ========== --%>
                <dx:GridViewDataTextColumn FieldName="RMK" Caption="REMARK" Width="200">
                    <HeaderStyle BackColor="#ECEFF1" ForeColor="#455A64" Font-Bold="true" />
                </dx:GridViewDataTextColumn>
                
            </Columns>
            <TotalSummary>
                <dx:ASPxSummaryItem FieldName="CONTRACT_ID" SummaryType="Count" DisplayFormat="총 건" />
                <dx:ASPxSummaryItem FieldName="CNTR_EA" SummaryType="Sum" DisplayFormat="{0:N0}" />
                <dx:ASPxSummaryItem FieldName="CNTR_WGT" SummaryType="Sum" DisplayFormat="{0:N2}" />
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
    <%-- 필요시 스크립트 추가 --%>
</asp:Content>