﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ScmBlockContractWeb.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>SCM Block Contract Manager</title>
    <style type="text/css">
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f5f5f5;
        }
        .page-container {
            max-width: 1600px;
            margin: 0 auto;
            padding: 20px;
        }
        .page-header {
            background-color: #0078d4;
            color: white;
            padding: 20px;
            margin-bottom: 20px;
            border-radius: 4px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        .page-header h1 {
            margin: 0;
            font-size: 24px;
            font-weight: 600;
        }
        .user-info {
            display: flex;
            align-items: center;
            gap: 15px;
            font-size: 14px;
        }
        .user-info .username {
            font-weight: 600;
        }
        .btn-logout {
            background-color: rgba(255,255,255,0.2);
            color: white;
            padding: 8px 16px;
            border: 1px solid rgba(255,255,255,0.3);
            border-radius: 4px;
            cursor: pointer;
            transition: all 0.3s;
            text-decoration: none;
            display: inline-block;
        }
        .btn-logout:hover {
            background-color: rgba(255,255,255,0.3);
        }
        .search-panel {
            background-color: white;
            padding: 20px;
            margin-bottom: 20px;
            border-radius: 4px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .search-panel h2 {
            margin: 0 0 15px 0;
            font-size: 18px;
            color: #333;
            border-bottom: 2px solid #0078d4;
            padding-bottom: 10px;
        }
        .search-row {
            display: flex;
            align-items: center;
            margin-bottom: 15px;
            flex-wrap: wrap;
            gap: 10px;
        }
        .search-item {
            display: flex;
            align-items: center;
            margin-right: 20px;
        }
        .search-item label {
            margin-right: 8px;
            font-weight: 500;
            color: #555;
            min-width: 100px;
        }
        .button-group {
            margin-top: 15px;
            padding-top: 15px;
            border-top: 1px solid #e0e0e0;
        }
        .grid-container {
            background-color: white;
            padding: 20px;
            border-radius: 4px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .info-message {
            padding: 10px;
            background-color: #e3f2fd;
            border-left: 4px solid #2196f3;
            margin-bottom: 15px;
            border-radius: 4px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="page-container">
            <!-- 페이지 헤더 -->
            <div class="page-header">
                <h1>📋 SCM Block Contract Manager</h1>
                <div class="user-info">
                    <span class="username">
                        👤 <asp:Label ID="lblUserName" runat="server"></asp:Label>
                    </span>
                    <a href="Logout.aspx" class="btn-logout">🚪 로그아웃</a>
                </div>
            </div>

            <!-- 조회 조건 패널 -->
            <div class="search-panel">
                <h2>🔍 조회 조건</h2>
                
                <div class="info-message">
                    <strong>Info:</strong> 현재 조회는 CASE_NO='MASTER', COMPANY_NO='1002' 기준으로 고정되어 있습니다.
                </div>

                <!-- 첫 번째 행: Company 구분, Company, 케이스 -->
                <div class="search-row">
                    <div class="search-item">
                        <label>Company 구분:</label>
                        <dx:ASPxComboBox ID="cmbCompanyType" runat="server" Width="150px" 
                                         ValueType="System.String" SelectedIndex="0">
                            <Items>
                                <dx:ListEditItem Text="전체" Value="" />
                                <dx:ListEditItem Text="본사" Value="H" />
                                <dx:ListEditItem Text="지사" Value="B" />
                            </Items>
                        </dx:ASPxComboBox>
                    </div>

                    <div class="search-item">
                        <label>Company:</label>
                        <dx:ASPxComboBox ID="cmbCompany" runat="server" Width="200px" 
                                         ValueType="System.String" TextField="Text" ValueField="Value">
                            <Items>
                                <dx:ListEditItem Text="1002 - SPELIX" Value="1002" Selected="true" />
                                <dx:ListEditItem Text="1001 - 본사" Value="1001" />
                            </Items>
                        </dx:ASPxComboBox>
                    </div>

                    <div class="search-item">
                        <label>케이스:</label>
                        <dx:ASPxComboBox ID="cmbCase" runat="server" Width="200px" 
                                         ValueType="System.String" TextField="Text" ValueField="Value">
                            <Items>
                                <dx:ListEditItem Text="MASTER" Value="MASTER" Selected="true" />
                                <dx:ListEditItem Text="CASE001" Value="CASE001" />
                                <dx:ListEditItem Text="CASE002" Value="CASE002" />
                            </Items>
                        </dx:ASPxComboBox>
                    </div>
                </div>

                <!-- 두 번째 행: 조회 기간 -->
                <div class="search-row">
                    <div class="search-item">
                        <dx:ASPxCheckBox ID="chkSelectAll" runat="server" Text="전체 조회" 
                                         Checked="true" CheckState="Checked">
                        </dx:ASPxCheckBox>
                    </div>

                    <div class="search-item">
                        <label>계약일 (시작):</label>
                        <dx:ASPxDateEdit ID="dtStartDate" runat="server" Width="150px" 
                                         DisplayFormatString="yyyy-MM-dd" EditFormat="Custom" 
                                         EditFormatString="yyyy-MM-dd" UseMaskBehavior="true">
                            <ValidationSettings Display="Dynamic" ErrorDisplayMode="ImageWithTooltip">
                            </ValidationSettings>
                        </dx:ASPxDateEdit>
                    </div>

                    <div class="search-item">
                        <label>계약일 (종료):</label>
                        <dx:ASPxDateEdit ID="dtEndDate" runat="server" Width="150px" 
                                         DisplayFormatString="yyyy-MM-dd" EditFormat="Custom" 
                                         EditFormatString="yyyy-MM-dd" UseMaskBehavior="true">
                            <ValidationSettings Display="Dynamic" ErrorDisplayMode="ImageWithTooltip">
                            </ValidationSettings>
                        </dx:ASPxDateEdit>
                    </div>
                </div>

                <!-- 버튼 그룹 -->
                <div class="button-group">
                    <dx:ASPxButton ID="btnSearch" runat="server" Text="조회" Width="120px" 
                                   OnClick="btnSearch_Click" Theme="Office365">
                        <Image IconID="actions_search_16x16">
                        </Image>
                    </dx:ASPxButton>
                    
                    <dx:ASPxButton ID="btnReset" runat="server" Text="초기화" Width="120px" 
                                   OnClick="btnReset_Click" Theme="Office365" 
                                   style="margin-left: 10px;">
                        <Image IconID="actions_refresh_16x16">
                        </Image>
                    </dx:ASPxButton>

                    <dx:ASPxButton ID="btnExport" runat="server" Text="엑셀 내보내기" Width="120px" 
                                   OnClick="btnExport_Click" Theme="Office365" 
                                   style="margin-left: 10px;">
                        <Image IconID="export_exporttoxlsx_16x16">
                        </Image>
                    </dx:ASPxButton>
                </div>
            </div>

            <!-- 그리드 컨테이너 -->
            <div class="grid-container">
                <dx:ASPxGridView ID="gridContracts" runat="server" Width="100%" 
                                 AutoGenerateColumns="False" KeyFieldName="CONTRACT_ID"
                                 Theme="Office365" EnableTheming="True">
                    <SettingsPager PageSize="20" >
                        <PageSizeItemSettings Visible="True" Items="10, 20, 50, 100" ShowAllItem="True" />
                    </SettingsPager>
                    
                    <Settings ShowFilterRow="True" ShowFilterRowMenu="True" 
                              ShowGroupPanel="False" ShowFooter="True" 
                              VerticalScrollBarMode="Auto" VerticalScrollableHeight="500" />
                    
                    <SettingsBehavior AllowSelectByRowClick="True" AllowFocusedRow="True" />
                    
                    <SettingsDataSecurity AllowDelete="False" AllowEdit="False" AllowInsert="False" />                    
                    <Columns>
                        <dx:GridViewDataTextColumn FieldName="COMPANY_NO" Caption="기업" Width="80px" VisibleIndex="0">
                            <HeaderStyle BackColor="#E6F2FF" ForeColor="#0066CC" Font-Bold="true" />
                        </dx:GridViewDataTextColumn>
    
                        <dx:GridViewDataTextColumn FieldName="CASE_NO" Caption="케이스" Width="100px" VisibleIndex="1">
                            <HeaderStyle BackColor="#E6F2FF" ForeColor="#0066CC" Font-Bold="true" />
                        </dx:GridViewDataTextColumn>
    
                        <dx:GridViewDataTextColumn FieldName="CONTRACT_ID" Caption="계약ID" Width="120px" VisibleIndex="2">
                            <HeaderStyle BackColor="#E6F2FF" ForeColor="#0066CC" Font-Bold="true" />
                        </dx:GridViewDataTextColumn>
    
                        <dx:GridViewDataTextColumn FieldName="ACT_NO" Caption="ACT명" Width="150px" VisibleIndex="3">
                            <HeaderStyle BackColor="#FFF4E6" ForeColor="#FF8C00" Font-Bold="true" />
                        </dx:GridViewDataTextColumn>
    
                        <dx:GridViewDataTextColumn FieldName="MAIN_CONTRACTOR" Caption="원청" Width="150px" VisibleIndex="4" />
    
                        <dx:GridViewDataTextColumn FieldName="SUB_CONTRACTOR" Caption="하청" Width="150px" VisibleIndex="5" />
    
                        <dx:GridViewDataTextColumn FieldName="MS_NO" Caption="도급번호" Width="120px" VisibleIndex="6" />
    
                        <dx:GridViewDataDateColumn FieldName="MS_DT" Caption="도급공급일" Width="120px" VisibleIndex="7" 
                                                   PropertiesDateEdit-DisplayFormatString="yyyy-MM-dd">
                            <HeaderStyle BackColor="#E6F9E6" ForeColor="#008000" Font-Bold="true" />
                        </dx:GridViewDataDateColumn>
    
                        <dx:GridViewDataTextColumn FieldName="MS_CONTRACTOR" Caption="도급업체" Width="150px" VisibleIndex="8" />
    
                        <dx:GridViewDataDateColumn FieldName="MAKING_DT" Caption="제작예정일" Width="120px" VisibleIndex="9" 
                                                   PropertiesDateEdit-DisplayFormatString="yyyy-MM-dd" />
    
                        <dx:GridViewDataDateColumn FieldName="MAKING_RES_DT" Caption="제작실적일" Width="120px" VisibleIndex="10" 
                                                   PropertiesDateEdit-DisplayFormatString="yyyy-MM-dd" />
    
                        <dx:GridViewDataTextColumn FieldName="TAG1" Caption="TAG1" Width="100px" VisibleIndex="11" />
                    </Columns>

                    <TotalSummary>
                        <dx:ASPxSummaryItem FieldName="CONTRACT_ID" SummaryType="Count" DisplayFormat="총 {0}건" />
                    </TotalSummary>
                    <Styles>
                        <Header Font-Bold="true" HorizontalAlign="Center" Wrap="True" />
                        <Cell HorizontalAlign="Left" VerticalAlign="Middle" Wrap="False" />
                        <FocusedRow BackColor="#D5E9FC" />
                        <AlternatingRow BackColor="#F8F8F8" />
                    </Styles>
                </dx:ASPxGridView>

                <!-- 그리드 하단 정보 -->
                <div style="margin-top: 15px; color: #666; font-size: 13px;">
                    <dx:ASPxLabel ID="lblRecordCount" runat="server" Text="조회된 데이터가 없습니다.">
                    </dx:ASPxLabel>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
