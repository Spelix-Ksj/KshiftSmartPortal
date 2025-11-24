<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="KShiftSmartPortalWeb.Login" %>
<%@ Register Assembly="DevExpress.Web.v25.1, Version=25.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>K-SHIFT Portal Management - Login</title>
    
    <!-- Bootstrap 5.3 CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
    <!-- Bootstrap Icons -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.0/font/bootstrap-icons.css" rel="stylesheet">
    
    <style>
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
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            display: flex;
            justify-content: center;
            align-items: center;
            padding: 20px;
        }

        /* 로그인 컨테이너 - 반응형 크기 */
        .login-wrapper {
            width: 100%;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        /* Bootstrap 카드 - 반응형 최대 너비 */
        .login-card {
            border: none;
            border-radius: 20px;
            box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
            overflow: hidden;
            width: 100%;
            max-width: 450px;  /* 기본 최대 너비 */
        }

        .login-card .card-header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 20px 30px;
            text-align: center;
            border: none;
        }

        .login-card .card-header h1 {
            font-size: 28px;
            font-weight: 600;
            margin-bottom: 10px;
        }

        .login-card .card-header p {
            font-size: 14px;
            opacity: 0.9;
            margin-bottom: 0;
        }

        .login-card .card-body {
            padding: 40px 30px;
        }

        .login-card .card-footer {
            background: #f8f9fa;
            border-top: 1px solid #e9ecef;
            padding: 20px;
            text-align: center;
            font-size: 12px;
            color: #666;
        }

        /* Bootstrap Alert 스타일 */
        .error-alert {
            border-radius: 8px;
            display: none;
        }

        .error-alert.visible {
            display: block;
        }

        /* DevExpress 컨트롤과 Bootstrap 조화 */
        .form-label {
            font-weight: 600;
            color: #333;
            margin-bottom: 8px;
        }

        /* DevExpress 컨트롤 Bootstrap 스타일링 */
        .dxeBase {
            border-radius: 8px !important;
        }

        .dxeEditArea {
            border-radius: 8px !important;
        }

        /* DevExpress TextBox 100% 너비 적용 */
        .mb-3 table.dxeTextBoxSys input.dxeEditAreaSys {
            width: 100% !important;
            max-width: 100% !important;
            box-sizing: border-box !important;
            display: block;
        }

        /* DevExpress ComboBox 스타일 */
        .mb-3 .dxeBase,
        .mb-3 table.dxeBase {
            width: 100% !important;
        }

        /* 로그인 버튼 스타일 */
        .btn-login {
            height: 45px;
            font-size: 16px;
            font-weight: 600;
        }

        /* 테마 선택 영역 */
        .theme-selector {
            padding-top: 25px;
            border-top: 1px solid #dee2e6;
        }

        .theme-selector label {
            font-size: 12px;
            color: #666;
            font-weight: 500;
        }

        /* 반응형 디자인 - 화면 크기에 따라 카드 크기 조정 */
        
        /* 모바일 세로 (매우 작은 화면) */
        @media (max-width: 575px) {
            .login-card {
                max-width: 100%;  /* 전체 너비 사용 */
            }
            
            .login-card .card-header h1 {
                font-size: 24px;
            }
            
            .login-card .card-body {
                padding: 30px 20px;
            }
        }

        /* 모바일 가로 & 작은 태블릿 */
        @media (min-width: 576px) and (max-width: 767px) {
            .login-card {
                max-width: 500px;
            }
        }

        /* 태블릿 */
        @media (min-width: 768px) and (max-width: 991px) {
            .login-card {
                max-width: 550px;
            }
        }

        /* 데스크톱 (일반 모니터) */
        @media (min-width: 992px) and (max-width: 1399px) {
            .login-card {
                max-width: 600px;
            }
            
            .login-card .card-header h1 {
                font-size: 32px;
            }
            
            .login-card .card-body {
                padding: 45px 35px;
            }
        }

        /* 큰 데스크톱 (큰 모니터) */
        @media (min-width: 1400px) {
            .login-card {
                max-width: 650px;
            }
            
            .login-card .card-header {
                padding: 50px 40px;
            }
            
            .login-card .card-header h1 {
                font-size: 34px;
            }
            
            .login-card .card-body {
                padding: 50px 40px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <!-- 중앙 정렬 래퍼 -->
        <div class="login-wrapper">
            <!-- Bootstrap Card -->
            <div class="card login-card">
                <!-- 카드 헤더 -->
                <div class="card-header">
                    <h1>
                        <i class="bi bi-box-seam"></i> K-SHIFT Portal
                    </h1>
                    <p>Master Plan Management System</p>
                </div>

                <!-- 카드 본문 -->
                <div class="card-body">
                    <!-- 에러 메시지 (Bootstrap Alert) -->
                    <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger alert-dismissible fade show error-alert" role="alert">
                        <i class="bi bi-exclamation-triangle-fill me-2"></i>
                        <asp:Label ID="lblError" runat="server" />
                        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                    </asp:Panel>

                    <!-- 회사 선택 (Bootstrap Form Group) -->
                    <div class="mb-3">
                        <label class="form-label">
                            <i class="bi bi-building me-1"></i> 회사
                        </label>
                        <dx:ASPxComboBox ID="cmbCompany" runat="server" 
                            Width="100%" 
                            Height="42px"
                            TextField="COMPANY_NAME" 
                            ValueField="COMPANY_NO"
                            ValueType="System.String"
                            DropDownStyle="DropDownList"
                            IncrementalFilteringMode="StartsWith">
                            <Border BorderWidth="1px" BorderColor="#ced4da" />
                        </dx:ASPxComboBox>
                    </div>

                    <!-- 아이디 입력 -->
                    <div class="mb-3">
                        <label class="form-label">
                            <i class="bi bi-person-fill me-1"></i> 아이디
                        </label>
                        <dx:ASPxTextBox ID="txtUserId" runat="server" 
                            Width="100%" 
                            AutoResizeWithContainer="true"
                            Height="42px"
                            MaxLength="150"
                            NullText="아이디를 입력하세요">
                            <Border BorderWidth="1px" BorderColor="#ced4da" />
                            <ValidationSettings ValidationGroup="LoginGroup"
                                Display="Dynamic"
                                ErrorDisplayMode="Text"                            
                                ErrorFrameStyle-VerticalAlign="NotSet" 
                                ErrorTextPosition="Bottom" 
                                SetFocusOnError="True">
                                <RequiredField IsRequired="true" ErrorText="아이디를 입력하세요" />
                            </ValidationSettings>
                        </dx:ASPxTextBox>
                    </div>

                    <!-- 비밀번호 입력 -->
                    <div class="mb-3">
                        <label class="form-label">
                            <i class="bi bi-lock-fill me-1"></i> 비밀번호
                        </label>
                        <dx:ASPxTextBox ID="txtPassword" runat="server" 
                            Width="100%" 
                            Height="42px"
                            Password="true"
                            MaxLength="100"
                            NullText="비밀번호를 입력하세요">
                            <Border BorderWidth="1px" BorderColor="#ced4da" />
                            <ClientSideEvents KeyPress="function(s, e) { if (e.htmlEvent.keyCode == 13) btnLogin.Click(); }" />
                            <ValidationSettings ValidationGroup="LoginGroup"
                                Display="Dynamic"
                                ErrorDisplayMode="Text"                            
                                ErrorFrameStyle-VerticalAlign="NotSet" 
                                ErrorTextPosition="Bottom" 
                                SetFocusOnError="True">
                                <RequiredField IsRequired="true" ErrorText="비밀번호를 입력하세요" />
                            </ValidationSettings>
                        </dx:ASPxTextBox>
                    </div>

                    <!-- 로그인 버튼 (Bootstrap Button + DevExpress) -->
                    <div class="d-grid gap-2 mt-4">
                        <dx:ASPxButton ID="btnLogin" runat="server" 
                            Text="로그인" 
                            Width="100%" 
                            Height="45px"
                            OnClick="btnLogin_Click"
                            ValidationGroup="LoginGroup"
                            Theme="Office365"
                            CssClass="btn-login">
                            <ClientSideEvents Click="function(s, e) { if (!ASPxClientEdit.ValidateGroup('LoginGroup')) e.processOnServer = false; }" />
                            <Image IconID="actions_apply_16x16" />
                        </dx:ASPxButton>
                    </div>

                    <!-- 테마 선택 -->
                    <div class="theme-selector mt-4">
                        <label class="form-label">
                            <i class="bi bi-palette-fill me-1"></i> 테마 선택
                        </label>
                        <dx:ASPxComboBox ID="cmbTheme" runat="server" 
                            Width="100%"
                            Height="36px"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="cmbTheme_SelectedIndexChanged"
                            DropDownStyle="DropDownList">
                            <Items>
                                <dx:ListEditItem Text="Office 365" Value="Office365" Selected="true" />
                                <dx:ListEditItem Text="Material" Value="Material" />
                                <dx:ListEditItem Text="Material Compact" Value="MaterialCompact" />
                                <dx:ListEditItem Text="Metropolis" Value="Metropolis" />
                                <dx:ListEditItem Text="iOS" Value="iOS" />
                            </Items>
                            <Border BorderWidth="1px" BorderColor="#ced4da" />
                        </dx:ASPxComboBox>
                    </div>
                </div>

                <!-- 카드 푸터 -->
                <div class="card-footer">
                    <i class="bi bi-c-circle me-1"></i> 2025 SPELIX. All rights reserved.
                </div>
            </div>
        </div>
    </form>

    <!-- Bootstrap 5.3 JS Bundle -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
