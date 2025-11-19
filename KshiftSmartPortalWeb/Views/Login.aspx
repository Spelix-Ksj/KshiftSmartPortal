<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ScmBlockContractWeb.Login" %>
<%@ Register Assembly="DevExpress.Web.v25.1, Version=25.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>K-SHIFT Portal Management - Login</title>
    
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
            padding: 20px;
        }

        .login-container {
            background: white;
            border-radius: 20px;
            box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
            overflow: hidden;
            width: 100%;
            max-width: 450px;
            position: relative;
        }

        .login-header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 40px 30px;
            text-align: center;
        }

        .login-header h1 {
            font-size: 28px;
            font-weight: 600;
            margin-bottom: 10px;
        }

        .login-header p {
            font-size: 14px;
            opacity: 0.9;
        }

        .login-body {
            padding: 40px 30px;
        }

        .error-panel {
            background: #fee;
            border: 1px solid #fcc;
            border-radius: 8px;
            padding: 12px 15px;
            margin-bottom: 20px;
            color: #c33;
            display: none;
        }

        .error-panel.visible {
            display: block;
        }

        .form-group {
            margin-bottom: 25px;
        }

        .form-label {
            display: block;
            font-size: 14px;
            font-weight: 600;
            color: #333;
            margin-bottom: 8px;
        }

        .btn-container {
            margin-top: 30px;
        }

        .theme-selector-container {
            margin-top: 30px;
            padding-top: 25px;
            border-top: 1px solid #e5e7eb;
        }

        .theme-label {
            font-size: 12px;
            color: #666;
            margin-bottom: 8px;
            display: block;
            font-weight: 500;
        }

        .copyright {
            text-align: center;
            padding: 20px;
            font-size: 12px;
            color: #666;
            background: #f8f9fa;
        }

        /* DevExpress 컨트롤 커스터마이징 */
        .dxeBase {
            border-radius: 8px !important;
        }

        .dxeEditArea {
            border-radius: 8px !important;
        }
        
        /* ASPxTextBox 및 ASPxComboBox 내부의 입력 필드 폭을 100%로 맞추기 위한 CSS 수정 */
        /* 1) Validation 래퍼(table …_ET)부터 안쪽 에디터 테이블까지 전부 100% */
        /*.form-group table.dxeValidDynEditorTable,
        .form-group table.dxeValidDynEditorTable > tbody > tr > td,
        .form-group table.dxeValidDynEditorTable > tbody > tr > td > table.dxeTextBoxSys,
        .form-group table.dxeTextBoxSys > tbody > tr > td.dxic {
            width: 100% !important;
        }*/

        /* 2) 실제 입력 <input> 의 폭을 100% + border-box 로 고정 */
        .form-group table.dxeTextBoxSys input.dxeEditAreaSys {
            width: 100% !important;
            max-width: 100% !important;
            box-sizing: border-box !important;
            display: block;
        }
        
        .btn-container .dx-button,
        .btn-container table.dxeBase {
            width: 100% !important;
        }

        .theme-selector-container .dx-combobox,
        .theme-selector-container table.dxeBase {
            width: 100% !important;
        }        
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container">
            <div class="login-header">
                <h1>K-SHIFT Portal - Login</h1>
                <p>Supply Chain Management System</p>
            </div>

            <div class="login-body">
                <!-- 에러 메시지 패널 -->
                <asp:Panel ID="pnlError" runat="server" CssClass="error-panel">
                    <asp:Label ID="lblError" runat="server" />
                </asp:Panel>

                <!-- 회사 선택 -->
                <div class="form-group">
                    <label class="form-label">회사</label>
                    <dx:ASPxComboBox ID="cmbCompany" runat="server" 
                        Width="100%" 
                        Height="42px"
                        TextField="COMPANY_NAME" 
                        ValueField="COMPANY_NO"
                        ValueType="System.String"
                        DropDownStyle="DropDownList"
                        IncrementalFilteringMode="StartsWith">
                        <Border BorderWidth="1px" BorderColor="#ddd" />
                    </dx:ASPxComboBox>
                </div>

                <!-- 아이디 입력 -->
                <div class="form-group">
                    <label class="form-label">아이디</label>
                    <dx:ASPxTextBox ID="txtUserId" runat="server" 
                        Width="100%" 
                        AutoResizeWithContainer="true"
                        Height="42px"
                        MaxLength="150"
                        NullText="아이디를 입력하세요">
                        <Border BorderWidth="1px" BorderColor="#ddd" />
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
                <div class="form-group">
                    <label class="form-label">비밀번호</label>
                    <dx:ASPxTextBox ID="txtPassword" runat="server" 
                        Width="100%" 
                        Height="42px"
                        Password="true"
                        MaxLength="100"
                        NullText="비밀번호를 입력하세요">
                        <Border BorderWidth="1px" BorderColor="#ddd" />
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

                <!-- 로그인 버튼 -->
                <div class="btn-container">
                    <dx:ASPxButton ID="btnLogin" runat="server" 
                        Text="로그인" 
                        Width="100%" 
                        Height="45px"
                        OnClick="btnLogin_Click"
                        ValidationGroup="LoginGroup"
                        Theme="Office365">
                        <ClientSideEvents Click="function(s, e) { if (!ASPxClientEdit.ValidateGroup('LoginGroup')) e.processOnServer = false; }" />
                    </dx:ASPxButton>
                </div>

                <!-- 테마 선택 -->
                <div class="theme-selector-container">
                    <label class="theme-label">테마 선택</label>
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
                        <Border BorderWidth="1px" BorderColor="#ddd" />
                    </dx:ASPxComboBox>
                </div>
            </div>

            <div class="copyright">
                &copy; 2025 SPELIX. All rights reserved.
            </div>
        </div>
    </form>
</body>
</html>
