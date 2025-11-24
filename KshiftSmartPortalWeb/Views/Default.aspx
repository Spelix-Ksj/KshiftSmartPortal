<%@ Page Language="C#" MasterPageFile="~/Views/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="KShiftSmartPortalWeb.Default" %>
<%@ Register Assembly="DevExpress.Web.Bootstrap.v25.1, Version=25.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.Bootstrap" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    K-SHIFT Smart Portal
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageTitleContent" runat="server">
    계약 관리 화면
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .search-panel {
            background-color: white;
            padding: 25px;
            margin-bottom: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }

        .search-panel h2 {
            margin: 0 0 20px 0;
            font-size: 20px;
            color: #333;
            border-bottom: 3px solid #0078d4;
            padding-bottom: 12px;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .search-row {
            margin-bottom: 20px;
        }

        .button-group {
            margin-top: 20px;
            padding-top: 20px;
            border-top: 2px solid #e0e0e0;
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
        }

        /* 버튼 아이콘 스타일 */
        .button-group .btn i {
            margin-right: 5px;
        }

        .grid-container {
            background-color: white;
            padding: 25px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }

        .info-message {
            padding: 15px;
            background-color: #e3f2fd;
            border-left: 4px solid #2196f3;
            margin-bottom: 20px;
            border-radius: 4px;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .info-message i {
            color: #2196f3;
            font-size: 20px;
        }

        .grid-header {
            margin-bottom: 15px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .grid-header h3 {
            margin: 0;
            font-size: 18px;
            color: #333;
        }

        .record-count {
            margin-top: 15px;
            padding: 10px;
            background-color: #f8f9fa;
            border-radius: 4px;
            color: #666;
            font-size: 14px;
        }

        /* Bootstrap GridView 커스터마이징 - 헤더 색상 강력하게 수정 */
        table.table thead tr th,
        table.table > thead > tr > th,
        .dxgvHeader,
        .dxgvHeader td,
        .dxgvHeader_Office365,
        .dxgvHeader_Office365 td {
            background-color: #0078d4 !important;
            color: #ffffff !important;
            font-weight: 600 !important;
            text-align: center !important;
            vertical-align: middle !important;
            border: 1px solid #005a9e !important;
        }

        /* 헤더 내부 모든 요소 강제 흰색 */
        table.table thead tr th *,
        table.table > thead > tr > th *,
        .dxgvHeader *,
        .dxgvHeader a,
        .dxgvHeader span,
        .dxgvHeader td a,
        .dxgvHeader td span,
        .dxgvHeader_Office365 *,
        .dxgvHeader_Office365 a,
        .dxgvHeader_Office365 span {
            color: #ffffff !important;
        }

        /* 필터 아이콘도 흰색으로 */
        .dxgvHeader .dxIcon,
        .dxgvHeader_Office365 .dxIcon {
            color: #ffffff !important;
        }

        .table > tbody > tr > td {
            vertical-align: middle;
            padding: 8px;
        }

        .table-striped > tbody > tr:nth-of-type(odd) {
            background-color: #f8f9fa;
        }

        .table-hover > tbody > tr:hover {
            background-color: #e9ecef;
            cursor: pointer;
        }

        /* Grid 반응형 처리 */
        .table-responsive {
            width: 100%;
            overflow-x: auto;
            -webkit-overflow-scrolling: touch;
        }

        /* 불필요한 스크롤바 숨기기 */
        .dxgvHSDC {
            display: none !important;
        }

        /* 검색 조건 레이블 스타일 */
        .search-label {
            display: flex;
            align-items: center;
            height: 38px;
            font-weight: 600;
            color: #333;
        }

        /* 반응형 테이블 */
        @media (max-width: 1200px) {
            .grid-container {
                padding: 15px;
            }

            .table {
                font-size: 14px;
            }
        }

        @media (max-width: 768px) {
            .grid-container {
                padding: 10px;
            }

            .table {
                font-size: 12px;
            }

            .search-panel {
                padding: 15px;
            }

            .button-group {
                flex-direction: column;
            }

            .button-group .btn {
                width: 100%;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <!-- 조회 조건 패널 -->
    <div class="search-panel">
        <h2>
            <i class="fas fa-search"></i>
            조회 조건
        </h2>

        <!-- 전체 조회 체크박스 -->
        <div class="search-row">
            <dx:BootstrapCheckBox ID="chkSelectAll" runat="server"
                                  Text="전체 조회"
                                  Checked="true">
            </dx:BootstrapCheckBox>
        </div>

        <!-- 첫 번째 행: Company 구분 -->
        <div class="search-row">
            <div class="row g-2 align-items-center">
                <div class="col-auto">
                    <label class="search-label">Company 구분:</label>
                </div>
                <div class="col">
                    <dx:BootstrapComboBox ID="cmbCompanyType" runat="server"
                                          ValueType="System.String">
                        <Items>
                            <dx:BootstrapListEditItem Text="전체" Value="" Selected="true" />
                            <dx:BootstrapListEditItem Text="본사" Value="H" />
                            <dx:BootstrapListEditItem Text="지사" Value="B" />
                        </Items>
                    </dx:BootstrapComboBox>
                </div>
            </div>
        </div>

        <!-- 두 번째 행: Company, 케이스, 계약일 -->
        <div class="search-row">
            <div class="row g-3">
                <div class="col-md-4">
                    <div class="row g-2 align-items-center">
                        <div class="col-auto">
                            <label class="search-label">Company:</label>
                        </div>
                        <div class="col">
                            <dx:BootstrapComboBox ID="cmbCompany" runat="server"
                                                  ValueType="System.String"
                                                  TextField="Text"
                                                  ValueField="Value">
                                <Items>
                                    <dx:BootstrapListEditItem Text="1002 - SPELIX" Value="1002" Selected="true" />
                                    <dx:BootstrapListEditItem Text="1001 - 본사" Value="1001" />
                                </Items>
                            </dx:BootstrapComboBox>
                        </div>
                    </div>
                </div>

                <div class="col-md-4">
                    <div class="row g-2 align-items-center">
                        <div class="col-auto">
                            <label class="search-label">케이스:</label>
                        </div>
                        <div class="col">
                            <dx:BootstrapComboBox ID="cmbCase" runat="server"
                                                  ValueType="System.String"
                                                  TextField="Text"
                                                  ValueField="Value">
                                <Items>
                                    <dx:BootstrapListEditItem Text="MASTER" Value="MASTER" Selected="true" />
                                    <dx:BootstrapListEditItem Text="CASE001" Value="CASE001" />
                                    <dx:BootstrapListEditItem Text="CASE002" Value="CASE002" />
                                </Items>
                            </dx:BootstrapComboBox>
                        </div>
                    </div>
                </div>

                <div class="col-md-4">
                    <!-- 빈 공간 -->
                </div>
            </div>
        </div>

        <!-- 세 번째 행: 계약일 -->
        <div class="search-row">
            <div class="row g-3">
                <div class="col-md-6">
                    <div class="row g-2 align-items-center">
                        <div class="col-auto">
                            <label class="search-label">계약일 (시작):</label>
                        </div>
                        <div class="col">
                            <dx:BootstrapDateEdit ID="dtStartDate" runat="server"
                                                  DisplayFormatString="yyyy-MM-dd">
                            </dx:BootstrapDateEdit>
                        </div>
                    </div>
                </div>

                <div class="col-md-6">
                    <div class="row g-2 align-items-center">
                        <div class="col-auto">
                            <label class="search-label">계약일 (종료):</label>
                        </div>
                        <div class="col">
                            <dx:BootstrapDateEdit ID="dtEndDate" runat="server"
                                                  DisplayFormatString="yyyy-MM-dd">
                            </dx:BootstrapDateEdit>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- 버튼 그룹 -->
        <div class="button-group">
            <dx:BootstrapButton ID="btnSearch" runat="server"
                                Text="<i class='fas fa-search'></i> 조회"
                                OnClick="btnSearch_Click"
                                EncodeHtml="false">
                <SettingsBootstrap RenderOption="Primary" />
            </dx:BootstrapButton>

            <dx:BootstrapButton ID="btnReset" runat="server"
                                Text="<i class='fas fa-redo'></i> 초기화"
                                OnClick="btnReset_Click"
                                EncodeHtml="false">
                <SettingsBootstrap RenderOption="Secondary" />
            </dx:BootstrapButton>

            <dx:BootstrapButton ID="btnExport" runat="server"
                                Text="<i class='fas fa-file-excel'></i> 엑셀 내보내기"
                                OnClick="btnExport_Click"
                                EncodeHtml="false">
                <SettingsBootstrap RenderOption="Success" />
            </dx:BootstrapButton>
        </div>
    </div>

    <!-- 그리드 컨테이너 -->
    <div class="grid-container">
        <div class="grid-header">
            <h3>
                <i class="fas fa-table"></i>
                계약 목록
            </h3>
        </div>

        <!-- 반응형 래퍼 -->
        <div class="table-responsive">
            <!-- BootstrapGridView -->
            <dx:BootstrapGridView ID="gridContracts" runat="server"
                                  AutoGenerateColumns="False"
                                  KeyFieldName="CONTRACT_ID"
                                  Width="100%">
                <SettingsPager PageSize="20">
                    <PageSizeItemSettings Visible="true" Items="10, 20, 50, 100" ShowAllItem="true" />
                </SettingsPager>

                <Settings ShowFilterRow="true"
                          ShowFilterRowMenu="true"
                          ShowFooter="true" />

                <SettingsBehavior AllowSelectByRowClick="true"
                                  AllowFocusedRow="true" />

                <SettingsDataSecurity AllowDelete="false"
                                      AllowEdit="false"
                                      AllowInsert="false" />

                <SettingsBootstrap Striped="true" />

                <CssClasses Table="table table-striped table-hover table-bordered" />

                <Columns>
                    <dx:BootstrapGridViewTextColumn FieldName="COMPANY_NO"
                                                    Caption="기업"
                                                    Width="80px"
                                                    VisibleIndex="0">
                    </dx:BootstrapGridViewTextColumn>

                    <dx:BootstrapGridViewTextColumn FieldName="CASE_NO"
                                                    Caption="케이스"
                                                    Width="100px"
                                                    VisibleIndex="1">
                    </dx:BootstrapGridViewTextColumn>

                    <dx:BootstrapGridViewTextColumn FieldName="CONTRACT_ID"
                                                    Caption="계약ID"
                                                    Width="120px"
                                                    VisibleIndex="2">
                    </dx:BootstrapGridViewTextColumn>

                    <dx:BootstrapGridViewTextColumn FieldName="ACT_NO"
                                                    Caption="ACT명"
                                                    Width="150px"
                                                    VisibleIndex="3">
                    </dx:BootstrapGridViewTextColumn>

                    <dx:BootstrapGridViewTextColumn FieldName="MAIN_CONTRACTOR"
                                                    Caption="원청"
                                                    Width="150px"
                                                    VisibleIndex="4" />

                    <dx:BootstrapGridViewTextColumn FieldName="SUB_CONTRACTOR"
                                                    Caption="하청"
                                                    Width="150px"
                                                    VisibleIndex="5" />

                    <dx:BootstrapGridViewTextColumn FieldName="MS_NO"
                                                    Caption="도급번호"
                                                    Width="120px"
                                                    VisibleIndex="6" />

                    <dx:BootstrapGridViewDateColumn FieldName="MS_DT"
                                                    Caption="도급공급일"
                                                    Width="120px"
                                                    VisibleIndex="7">
                        <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                    </dx:BootstrapGridViewDateColumn>

                    <dx:BootstrapGridViewTextColumn FieldName="MS_CONTRACTOR"
                                                    Caption="도급업체"
                                                    Width="150px"
                                                    VisibleIndex="8" />

                    <dx:BootstrapGridViewDateColumn FieldName="MAKING_DT"
                                                    Caption="제작예정일"
                                                    Width="120px"
                                                    VisibleIndex="9">
                        <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                    </dx:BootstrapGridViewDateColumn>

                    <dx:BootstrapGridViewDateColumn FieldName="MAKING_RES_DT"
                                                    Caption="제작실적일"
                                                    Width="120px"
                                                    VisibleIndex="10">
                        <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                    </dx:BootstrapGridViewDateColumn>

                    <dx:BootstrapGridViewTextColumn FieldName="TAG1"
                                                    Caption="TAG1"
                                                    Width="100px"
                                                    VisibleIndex="11" />
                </Columns>

                <TotalSummary>
                    <dx:ASPxSummaryItem FieldName="CONTRACT_ID"
                                        SummaryType="Count"
                                        DisplayFormat="총 {0}건" />
                </TotalSummary>
            </dx:BootstrapGridView>
        </div>
        <!-- 반응형 래퍼 끝 -->

        <!-- 그리드 하단 정보 -->
        <div class="record-count">
            <i class="fas fa-info-circle"></i>
            <asp:Label ID="lblRecordCount" runat="server" Text="조회된 데이터가 없습니다."></asp:Label>
        </div>
    </div>
</asp:Content>
