<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="KShiftSmartPortalWeb.Login" %>
<%@ Register Assembly="DevExpress.Web.v25.1, Version=25.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no"/>
    <title>K-SHIFT Portal - Login</title>

    <!-- Google Analytics 4 -->
    <script async src="https://www.googletagmanager.com/gtag/js?id=G-4YBX6Q8Q2B"></script>
    <script>
        window.dataLayer = window.dataLayer || [];
        function gtag(){dataLayer.push(arguments);}
        gtag('js', new Date());
        gtag('config', 'G-4YBX6Q8Q2B');
    </script>

    <!-- Microsoft Clarity -->
    <script>
        (function(c,l,a,r,i,t,y){
            c[a]=c[a]||function(){(c[a].q=c[a].q||[]).push(arguments)};
            t=l.createElement(r);t.async=1;t.src="https://www.clarity.ms/tag/"+i;
            y=l.getElementsByTagName(r)[0];y.parentNode.insertBefore(t,y);
        })(window, document, "clarity", "script", "uivndhuj52");
    </script>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />

    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }

        html, body {
            height: 100%;
            font-family: 'Segoe UI', Tahoma, sans-serif;
        }

        body {
            background: linear-gradient(135deg, #1e3c72 0%, #2a5298 50%, #1e3c72 100%);
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
            padding: 20px;
        }

        .login-container {
            width: 100%;
            max-width: 420px;
        }

        .logo-section {
            text-align: center;
            margin-bottom: 25px;
            color: #fff;
        }
        .logo-section i {
            font-size: 48px;
            margin-bottom: 10px;
            text-shadow: 0 2px 10px rgba(0,0,0,0.3);
        }
        .logo-section h1 {
            font-size: 26px;
            font-weight: 700;
            margin: 0;
            letter-spacing: 1px;
        }
        .logo-section p {
            font-size: 12px;
            opacity: 0.8;
            margin-top: 5px;
        }

        .login-card {
            background: #fff;
            border-radius: 16px;
            box-shadow: 0 15px 50px rgba(0,0,0,0.3);
            overflow: hidden;
        }

        .login-card-body {
            padding: 35px 30px;
        }

        .error-box {
            background: #fee2e2;
            border-left: 3px solid #dc2626;
            padding: 12px 14px;
            margin-bottom: 20px;
            border-radius: 0 8px 8px 0;
            font-size: 14px;
            color: #b91c1c;
            display: none;
        }
        .error-box.show { display: block; }

        .input-row {
            display: flex;
            align-items: center;
            margin-bottom: 16px;
        }

        .input-label {
            width: 80px;
            font-size: 14px;
            font-weight: 600;
            color: #374151;
            flex-shrink: 0;
        }

        .input-field {
            flex: 1;
            min-width: 0;
            position: relative;
        }

        .input-wrapper {
            display: flex;
            align-items: center;
            border: 2px solid #e5e7eb;
            border-radius: 10px;
            background: #fff;
            transition: all 0.2s ease;
            height: 46px;
            padding: 0 14px;
        }
        .input-wrapper:focus-within {
            border-color: #2a5298;
            box-shadow: 0 0 0 3px rgba(42, 82, 152, 0.15);
        }

        .input-wrapper input,
        .input-wrapper select {
            flex: 1;
            border: none !important;
            background: transparent !important;
            font-size: 15px !important;
            padding: 0 !important;
            height: 100% !important;
            outline: none !important;
            box-shadow: none !important;
        }

        /* DevExpress ComboBox */
        .input-wrapper .dxeBase,
        .input-wrapper table.dxeBase {
            width: 100% !important;
            border: none !important;
            background: transparent !important;
        }
        .input-wrapper .dxeEditArea,
        .input-wrapper input.dxeEditAreaSys {
            border: none !important;
            background: transparent !important;
            font-size: 15px !important;
            padding: 0 !important;
            height: 100% !important;
            outline: none !important;
        }
        .input-wrapper .dxeButtonEditSys {
            border: none !important;
            background: transparent !important;
        }
        .input-wrapper .dxeButtonEdit .dxeButtonEditButton {
            background: transparent !important;
            border: none !important;
        }
        .input-wrapper .dxeDropDownButtonInner .dxEditors_edtDropDown_DevEx {
            background: transparent !important;
        }

        /* DevExpress Focus Border Override */
        .input-wrapper .dxeTextBoxSys,
        .input-wrapper .dxeEditAreaSys,
        .input-wrapper .dxeBase,
        .input-wrapper .dxeTextBox,
        .input-wrapper .dxeButtonEditSys,
        .input-wrapper table.dxeTextBoxSys {
            border: none !important;
            outline: none !important;
            box-shadow: none !important;
        }
        .input-wrapper .dxeTextBoxSys_DevEx,
        .input-wrapper .dxeTextBox_DevEx,
        .input-wrapper .dxeButtonEdit_DevEx,
        .input-wrapper .dxeButtonEditSys_DevEx,
        .input-wrapper .dxeFocused,
        .input-wrapper .dxeTextBoxSys.dxeFocused,
        .input-wrapper td.dxe {
            border: none !important;
            outline: none !important;
            box-shadow: none !important;
            border-color: transparent !important;
        }
        .input-wrapper *:focus {
            outline: none !important;
            box-shadow: none !important;
        }

        /* DevExpress Moderno Theme Override - 파란색 하단 보더 제거 */
        .input-wrapper .dxeTextBox_Moderno,
        .input-wrapper .dxeTextBoxSys_Moderno,
        .input-wrapper .dxeButtonEdit_Moderno,
        .input-wrapper .dxeButtonEditSys_Moderno,
        .input-wrapper .dxeComboBox_Moderno {
            border: none !important;
            border-bottom: none !important;
            background: transparent !important;
        }
        .input-wrapper .dxeTextBox_Moderno::after,
        .input-wrapper .dxeTextBox_Moderno::before,
        .input-wrapper .dxeButtonEdit_Moderno::after,
        .input-wrapper .dxeButtonEdit_Moderno::before {
            display: none !important;
            border: none !important;
        }
        .input-wrapper .dxeTextBox_Moderno .dxeEditArea_Moderno,
        .input-wrapper .dxeButtonEdit_Moderno .dxeEditArea_Moderno {
            border: none !important;
            background: transparent !important;
        }
        .input-wrapper .dxeTextBox_Moderno.dxeControlsCell_Moderno,
        .input-wrapper td.dxeControlsCell_Moderno {
            border: none !important;
            background: transparent !important;
        }
        /* Moderno 테마 포커스 라인 제거 */
        .input-wrapper .dxeFocused_Moderno,
        .input-wrapper .dxeTextBox_Moderno.dxeFocused_Moderno,
        .input-wrapper .dxeButtonEdit_Moderno.dxeFocused_Moderno {
            border: none !important;
            box-shadow: none !important;
        }
        .input-wrapper [class*="_Moderno"] {
            border: none !important;
            border-bottom: none !important;
        }

        /* Password toggle */
        .pwd-toggle {
            cursor: pointer;
            color: #9ca3af;
            padding: 5px;
            transition: color 0.2s;
        }
        .pwd-toggle:hover {
            color: #374151;
        }

        /* Login button */
        .btn-row {
            margin-top: 24px;
        }
        .btn-login-wrapper {
            display: flex;
            align-items: center;
            border: 2px solid #1e3c72;
            border-radius: 10px;
            background: linear-gradient(135deg, #1e3c72 0%, #2a5298 100%);
            height: 46px;
            cursor: pointer;
            transition: all 0.3s ease;
            justify-content: center;
        }
        .btn-login-wrapper:hover,
        .btn-login-wrapper:focus {
            background: linear-gradient(135deg, #2a5298 0%, #1e3c72 100%);
            transform: translateY(-1px);
            box-shadow: 0 4px 15px rgba(30, 60, 114, 0.4);
            outline: none;
        }
        .btn-login-wrapper span {
            color: #fff;
            font-size: 15px;
            font-weight: 600;
        }

        /* Hide DevExpress button styling */
        .btn-login-wrapper .dxeBase {
            background: transparent !important;
            border: none !important;
        }
        .btn-login-wrapper .dx-btn {
            background: transparent !important;
            border: none !important;
            color: #fff !important;
            font-size: 15px !important;
            font-weight: 600 !important;
        }

        .login-footer {
            text-align: center;
            padding: 14px;
            background: #f8fafc;
            font-size: 12px;
            color: #94a3b8;
        }

        @media (max-width: 480px) {
            .login-container { max-width: 100%; }
            .login-card-body { padding: 28px 20px; }
            .input-label { width: 70px; font-size: 13px; }
            .logo-section i { font-size: 40px; }
            .logo-section h1 { font-size: 22px; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container">
            <div class="logo-section">
                <i class="fas fa-cubes"></i>
                <h1>K-SHIFT Portal</h1>
                <p>Master Plan Management System</p>
            </div>

            <div class="login-card">
                <div class="login-card-body">
                    <asp:Panel ID="pnlError" runat="server" CssClass="error-box">
                        <i class="fas fa-exclamation-circle"></i>
                        <asp:Label ID="lblError" runat="server" />
                    </asp:Panel>

                    <div class="input-row">
                        <span class="input-label">Company</span>
                        <div class="input-field">
                            <div class="input-wrapper">
                                <dx:ASPxComboBox ID="cmbCompany" runat="server"
                                    Width="100%"
                                    TextField="COMPANY_NAME"
                                    ValueField="COMPANY_NO"
                                    ValueType="System.String"
                                    DropDownStyle="DropDownList"
                                    IncrementalFilteringMode="StartsWith"
                                    NullText="Select company">
                                </dx:ASPxComboBox>
                            </div>
                        </div>
                    </div>

                    <div class="input-row">
                        <span class="input-label">ID</span>
                        <div class="input-field">
                            <div class="input-wrapper">
                                <dx:ASPxTextBox ID="txtUserId" runat="server"
                                    Width="100%"
                                    NullText="Enter your ID">
                                    <ValidationSettings ValidationGroup="LoginGroup" Display="None">
                                        <RequiredField IsRequired="true" ErrorText="Please enter ID" />
                                    </ValidationSettings>
                                </dx:ASPxTextBox>
                            </div>
                        </div>
                    </div>

                    <div class="input-row">
                        <span class="input-label">Password</span>
                        <div class="input-field">
                            <div class="input-wrapper">
                                <dx:ASPxTextBox ID="txtPassword" runat="server"
                                    ClientInstanceName="txtPassword"
                                    Width="100%"
                                    Password="true"
                                    NullText="Enter password">
                                    <ClientSideEvents KeyPress="function(s, e) { if (e.htmlEvent.keyCode == 13) { ASPxClientEdit.ValidateGroup('LoginGroup'); btnLogin.DoClick(); } }" />
                                    <ValidationSettings ValidationGroup="LoginGroup" Display="None">
                                        <RequiredField IsRequired="true" ErrorText="Please enter password" />
                                    </ValidationSettings>
                                </dx:ASPxTextBox>
                                <i class="fas fa-eye pwd-toggle" id="pwdToggle" onclick="togglePassword()" tabindex="-1"></i>
                            </div>
                        </div>
                    </div>

                    <div class="btn-row">
                        <div class="btn-login-wrapper" tabindex="0" onclick="btnLogin.DoClick();" onkeypress="if(event.keyCode==13) btnLogin.DoClick();">
                            <span><i class="fas fa-sign-in-alt" style="margin-right:8px;"></i>Login</span>
                        </div>
                        <dx:ASPxButton ID="btnLogin" runat="server"
                            ClientInstanceName="btnLogin"
                            Text="Login"
                            OnClick="btnLogin_Click"
                            ValidationGroup="LoginGroup"
                            UseSubmitBehavior="false"
                            CssClass="d-none">
                            <ClientSideEvents Click="function(s, e) { if (!ASPxClientEdit.ValidateGroup('LoginGroup')) e.processOnServer = false; }" />
                        </dx:ASPxButton>
                    </div>
                </div>

                <div class="login-footer">
                    Copyright 2025.12ㄴ SPELIX. All rights reserved.
                </div>
            </div>
        </div>
    </form>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script>
        var pwdVisible = false;
        function togglePassword() {
            pwdVisible = !pwdVisible;
            var icon = document.getElementById('pwdToggle');
            var input = txtPassword.GetInputElement();
            if (pwdVisible) {
                input.type = 'text';
                icon.className = 'fas fa-eye-slash pwd-toggle';
            } else {
                input.type = 'password';
                icon.className = 'fas fa-eye pwd-toggle';
            }
        }
    </script>
</body>
</html>
