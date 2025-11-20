<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ScmBlockContractWeb.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>SCM Block Contract Manager</title>
    
    <!-- Bootstrap 5.3 CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
    <!-- Bootstrap Icons -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.0/font/bootstrap-icons.css" rel="stylesheet">
    
    <style type="text/css">
        /* 전역 스타일 */
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        html, body {
            height: 100%;
            width: 100%;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f5f7fa;
        }

        /* 메인 컨테이너 - 반응형 최대 너비 */
        .main-container {
            width: 100%;
            max-width: 1200px;  /* 기본 최대 너비 */
            margin: 0 auto;
            padding: 20px;
        }

        /* 네비게이션 헤더 (Bootstrap Navbar 스타일) */
        .page-navbar {
            background: linear-gradient(135deg, #0078d4 0%, #005a9e 100%);
            color: white;
            padding: 15px 0;
            margin-bottom: 25px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            border-radius: 12px;
        }

        .page-navbar .navbar-content {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 0 20px;
        }

        .page-navbar .navbar-brand {
            color: white;
            font-size: 24px;
            font-weight: 600;
            text-decoration: none;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .page-navbar .navbar-brand:hover {
            color: #f0f0f0;
        }

        .user-info-section {
            display: flex;
            align-items: center;
            gap: 15px;
        }

        .user-info-badge {
            background-color: rgba(255,255,255,0.2);
            padding: 8px 16px;
            border-radius: 20px;
            font-size: 14px;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .user-info-badge .username {
            font-weight: 600;
        }

        /* Bootstrap 카드 스타일 */
        .search-card, .grid-card {
            border: none;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
            margin-bottom: 25px;
            background: white;
        }

        .search-card .card-header {
            background: linear-gradient(135deg, #0078d4 0%, #005a9e 100%);
            color: white;
            border: none;
            border-radius: 12px 12px 0 0 !important;
            padding: 15px 20px;
        }

        .search-card .card-header h5 {
            margin: 0;
            font-size: 18px;
            font-weight: 600;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .search-card .card-body {
            padding: 25px;
        }

        /* Bootstrap Alert 스타일 */
        .info-alert {
            border-left: 4px solid #0dcaf0;
            border-radius: 8px;
        }

        /* Bootstrap Form Label 스타일 */
        .form-label {
            font-weight: 600;
            color: #495057;
            margin-bottom: 8px;
            display: flex;
            align-items: center;
            gap: 5px;
        }

        .form-label i {
            color: #0078d4;
        }

        /* DevExpress 컨트롤과 Bootstrap 조화 */
        .row .dxeBase {
            border-radius: 6px !important;
        }

        /* Bootstrap 버튼 그룹 스타일 */
        .button-toolbar {
            padding-top: 20px;
            border-top: 1px solid #dee2e6;
            margin-top: 20px;
        }

        /* 그리드 컨테이너 */
        .grid-card .card-header {
            background: white;
            border-bottom: 2px solid #0078d4;
            padding: 15px 20px;
        }

        .grid-card .card-header h5 {
            margin: 0;
            font-size: 18px;
            font-weight: 600;
            color: #333;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .grid-card .card-body {
            padding: 25px;
        }

        /* DevExpress GridView Bootstrap 스타일링 */
        .dxgvTable {
            border-radius: 8px;
            overflow: hidden;
        }

        /* 로그아웃 버튼 스타일 */
        .btn-logout {
            background-color: rgba(255,255,255,0.2);
            color: white;
            border: 1px solid rgba(255,255,255,0.3);
            padding: 8px 16px;
            border-radius: 6px;
            text-decoration: none;
            transition: all 0.3s;
            display: inline-flex;
            align-items: center;
            gap: 5px;
        }

        .btn-logout:hover {
            background-color: rgba(255,255,255,0.3);
            color: white;
        }

        /* 반응형 디자인 - 화면 크기에 따라 컨테이너 크기 조정 */
        
        /* 모바일 세로 (매우 작은 화면) */
        @media (max-width: 575px) {
            .main-container {
                max-width: 100%;
                padding: 15px;
            }

            .page-navbar {
                margin-bottom: 20px;
            }

            .page-navbar .navbar-content {
                flex-direction: column;
                gap: 15px;
                text-align: center;
            }

            .page-navbar .navbar-brand {
                font-size: 18px;
            }

            .user-info-badge {
                font-size: 12px;
                padding: 6px 12px;
            }

            .search-card .card-body,
            .grid-card .card-body {
                padding: 15px;
            }

            /* 모바일에서 폼 필드를 세로로 배치 */
            .row.g-3 > div {
                flex: 0 0 100%;
                max-width: 100%;
            }
        }

        /* 모바일 가로 & 작은 태블릿 */
        @media (min-width: 576px) and (max-width: 767px) {
            .main-container {
                max-width: 100%;
                padding: 20px;
            }

            .page-navbar .navbar-brand {
                font-size: 20px;
            }
        }

        /* 태블릿 */
        @media (min-width: 768px) and (max-width: 991px) {
            .main-container {
                max-width: 95%;
            }

            .page-navbar .navbar-brand {
                font-size: 22px;
            }
        }

        /* 데스크톱 (일반 모니터) */
        @media (min-width: 992px) and (max-width: 1399px) {
            .main-container {
                max-width: 1200px;
            }
        }

        /* 큰 데스크톱 (큰 모니터) */
        @media (min-width: 1400px) and (max-width: 1919px) {
            .main-container {
                max-width: 1400px;
            }

            .page-navbar .navbar-brand {
                font-size: 26px;
            }

            .search-card .card-body,
            .grid-card .card-body {
                padding: 30px;
            }
        }

        /* 초대형 모니터 */
        @media (min-width: 1920px) {
            .main-container {
                max-width: 1600px;
            }

            .page-navbar {
                padding: 20px 0;
            }

            .page-navbar .navbar-brand {
                font-size: 28px;
            }

            .search-card .card-body,
            .grid-card .card-body {
                padding: 35px;
            }

            .form-label {
                font-size: 15px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <!-- 메인 컨테이너 (반응형 중앙 정렬) -->
        <div class="main-container">
            <!-- 네비게이션 바 (Bootstrap Navbar 스타일) -->
            <div class="page-navbar">
                <div class="navbar-content">
                    <a href="#" class="navbar-brand">
                        <i class="bi bi-box-seam"></i>
                        <span>SCM Block Contract Manager</span>
                    </a>
                    <div class="user-info-section">
                        <div class="user-info-badge">
                            <i class="bi bi-person-circle"></i>
                            <span class="username">
                                <asp:Label ID="lblUserName" runat="server"></asp:Label>
                            </span>
                        </div>
                        <a href="Logout.aspx" class="btn-logout">
                            <i class="bi bi-box-arrow-right"></i>
                            <span>로그아웃</span>
                        </a>
                    </div>
                </div>
            </div>

            <!-- 조회 조건 카드 (Bootstrap Card) -->
            <div class="card search-card">
                <div class="card-header">
                    <h5>
                        <i class="bi bi-search"></i>
                        <span>조회 조건</span>
                    </h5>
                </div>
                <div class="card-body">
                    <!-- Bootstrap Alert -->
                    <div class="alert alert-info info-alert mb-4" role="alert">
                        <i class="bi bi-info-circle-fill me-2"></i>
                        <strong>안내:</strong> 현재 조회는 CASE_NO='MASTER', COMPANY_NO='1002' 기준으로 고정되어 있습니다.
                    </div>

                    <!-- 첫 번째 행: Bootstrap Row & Col -->
                    <div class="row g-3 mb-3">
                        <div class="col-12 col-md-6 col-lg-4">
                            <label class="form-label">
                                <i class="bi bi-building"></i>
                                <span>Company 구분</span>
                            </label>
                            <dx:ASPxComboBox ID="cmbCompanyType" runat="server" Width="100%" 
                                             ValueType="System.String" SelectedIndex="0">
                                <Items>
                                    <dx:ListEditItem Text="전체" Value="" />
                                    <dx:ListEditItem Text="본사" Value="H" />
                                    <dx:ListEditItem Text="지사" Value="B" />
                                </Items>
                                <Border BorderWidth="1px" BorderColor="#ced4da" />
                            </dx:ASPxComboBox>
                        </div>

                        <div class="col-12 col-md-6 col-lg-4">
                            <label class="form-label">
                                <i class="bi bi-briefcase"></i>
                                <span>Company</span>
                            </label>
                            <dx:ASPxComboBox ID="cmbCompany" runat="server" Width="100%" 
                                             ValueType="System.String" TextField="Text" ValueField="Value">
                                <Items>
                                    <dx:ListEditItem Text="1002 - SPELIX" Value="1002" Selected="true" />
                                    <dx:ListEditItem Text="1001 - 본사" Value="1001" />
                                </Items>
                                <Border BorderWidth="1px" BorderColor="#ced4da" />
                            </dx:ASPxComboBox>
                        </div>

                        <div class="col-12 col-md-6 col-lg-4">
                            <label class="form-label">
                                <i class="bi bi-folder2"></i>
                                <span>케이스</span>
                            </label>
                            <dx:ASPxComboBox ID="cmbCase" runat="server" Width="100%" 
                                             ValueType="System.String" TextField="Text" ValueField="Value">
                                <Items>
                                    <dx:ListEditItem Text="MASTER" Value="MASTER" Selected="true" />
                                    <dx:ListEditItem Text="CASE001" Value="CASE001" />
                                    <dx:ListEditItem Text="CASE002" Value="CASE002" />
                                </Items>
                                <Border BorderWidth="1px" BorderColor="#ced4da" />
                            </dx:ASPxComboBox>
                        </div>
                    </div>

                    <!-- 두 번째 행: 날짜 조회 -->
                    <div class="row g-3 mb-3">
                        <div class="col-12 col-md-4">
                            <dx:ASPxCheckBox ID="chkSelectAll" runat="server" Text="전체 조회" 
                                             Checked="true" CheckState="Checked">
                            </dx:ASPxCheckBox>
                        </div>

                        <div class="col-12 col-md-4">
                            <label class="form-label">
                                <i class="bi bi-calendar-range"></i>
                                <span>계약일 (시작)</span>
                            </label>
                            <dx:ASPxDateEdit ID="dtStartDate" runat="server" Width="100%" 
                                             DisplayFormatString="yyyy-MM-dd" EditFormat="Custom" 
                                             EditFormatString="yyyy-MM-dd" UseMaskBehavior="true">
                                <ValidationSettings Display="Dynamic" ErrorDisplayMode="ImageWithTooltip">
                                </ValidationSettings>
                                <Border BorderWidth="1px" BorderColor="#ced4da" />
                            </dx:ASPxDateEdit>
                        </div>

                        <div class="col-12 col-md-4">
                            <label class="form-label">
                                <i class="bi bi-calendar-check"></i>
                                <span>계약일 (종료)</span>
                            </label>
                            <dx:ASPxDateEdit ID="dtEndDate" runat="server" Width="100%" 
                                             DisplayFormatString="yyyy-MM-dd" EditFormat="Custom" 
                                             EditFormatString="yyyy-MM-dd" UseMaskBehavior="true">
                                <ValidationSettings Display="Dynamic" ErrorDisplayMode="ImageWithTooltip">
                                </ValidationSettings>
                                <Border BorderWidth="1px" BorderColor="#ced4da" />
                            </dx:ASPxDateEdit>
                        </div>
                    </div>

                    <!-- 버튼 그룹 (Bootstrap Button Group) -->
                    <div class="button-toolbar">
                        <div class="d-flex gap-2 flex-wrap">
                            <dx:ASPxButton ID="btnSearch" runat="server" Text="조회" Width="120px" 
                                           OnClick="btnSearch_Click" Theme="Office365">
                                <Image IconID="actions_search_16x16" />
                            </dx:ASPxButton>
                            
                            <dx:ASPxButton ID="btnReset" runat="server" Text="초기화" Width="120px" 
                                           OnClick="btnReset_Click" Theme="Office365">
                                <Image IconID="actions_refresh_16x16" />
                            </dx:ASPxButton>

                            <dx:ASPxButton ID="btnExport" runat="server" Text="엑셀 내보내기" Width="140px" 
                                           OnClick="btnExport_Click" Theme="Office365">
                                <Image IconID="export_exporttoxlsx_16x16" />
                            </dx:ASPxButton>
                        </div>
                    </div>
                </div>
            </div>

            <!-- 그리드 카드 (Bootstrap Card) -->
            <div class="card grid-card">
                <div class="card-header">
                    <h5>
                        <i class="bi bi-table"></i>
                        <span>계약 목록</span>
                    </h5>
                </div>
                <div class="card-body">
                    <!-- DevExpress GridView -->
                    <dx:ASPxGridView ID="gridContracts" runat="server" Width="100%" 
                                     AutoGenerateColumns="False" KeyFieldName="CONTRACT_ID"
                                     Theme="Office365" EnableTheming="True">
                        <SettingsPager PageSize="20">
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

                    <!-- 그리드 하단 정보 (Bootstrap Badge) -->
                    <div class="mt-3">
                        <div class="d-flex align-items-center">
                            <i class="bi bi-info-circle me-2 text-primary"></i>
                            <dx:ASPxLabel ID="lblRecordCount" runat="server" 
                                          Text="조회된 데이터가 없습니다." 
                                          CssClass="text-muted">
                            </dx:ASPxLabel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>

    <!-- Bootstrap 5.3 JS Bundle -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
