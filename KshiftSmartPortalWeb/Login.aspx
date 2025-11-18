<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ScmBlockContractWeb.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>사용자 로그인 - SCM Block Contract</title>
    <style type="text/css">
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }
        
        body {
            font-family: 'Segoe UI', 'Malgun Gothic', Arial, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 20px;
        }
        
        .login-container {
            background: white;
            border-radius: 16px;
            box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
            overflow: hidden;
            max-width: 450px;
            width: 100%;
            animation: slideUp 0.5s ease-out;
        }
        
        @keyframes slideUp {
            from {
                opacity: 0;
                transform: translateY(30px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }
        
        .login-header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 40px 30px 30px;
            text-align: center;
            position: relative;
        }
        
        .login-header h1 {
            font-size: 28px;
            font-weight: 600;
            margin-bottom: 8px;
            letter-spacing: -0.5px;
        }
        
        .login-header .subtitle {
            font-size: 14px;
            opacity: 0.9;
            font-weight: 300;
        }
        
        .login-body {
            padding: 40px 30px;
        }
        
        .form-group {
            margin-bottom: 25px;
        }
        
        .form-group label {
            display: block;
            margin-bottom: 8px;
            font-weight: 500;
            color: #333;
            font-size: 14px;
        }
        
        .form-group label .required {
            color: #e74c3c;
            margin-left: 2px;
        }
        
        .btn-login {
            width: 100%;
            padding: 14px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border: none;
            border-radius: 8px;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
            margin-top: 10px;
        }
        
        .btn-login:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 20px rgba(102, 126, 234, 0.4);
        }
        
        .btn-login:active {
            transform: translateY(0);
        }
        
        .error-message {
            background: #fee;
            color: #c33;
            padding: 12px 15px;
            border-radius: 8px;
            margin-bottom: 20px;
            font-size: 14px;
            border-left: 4px solid #c33;
            animation: shake 0.5s;
        }
        
        @keyframes shake {
            0%, 100% { transform: translateX(0); }
            25% { transform: translateX(-10px); }
            75% { transform: translateX(10px); }
        }
        
        .login-footer {
            padding: 20px 30px;
            background: #f8f9fa;
            text-align: center;
            font-size: 13px;
            color: #666;
            border-top: 1px solid #e0e0e0;
        }
        
        .company-logo {
            position: absolute;
            top: 20px;
            right: 30px;
            color: white;
            font-weight: 700;
            font-size: 18px;
            letter-spacing: 2px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container">
            <!-- 헤더 -->
            <div class="login-header">
                <div class="company-logo">SPELIX</div>
                <h1>ID Framework</h1>
                <div class="subtitle">SCM Block Contract Management</div>
            </div>
            
            <!-- 본문 -->
            <div class="login-body">
                <!-- 오류 메시지 -->
                <asp:Panel ID="pnlError" runat="server" CssClass="error-message" Visible="false">
                    <asp:Label ID="lblError" runat="server"></asp:Label>
                </asp:Panel>
                
                <!-- 회사 선택 -->
                <div class="form-group">
                    <label for="cmbCompany">
                        회사 <span class="required">*</span>
                    </label>
                    <dx:ASPxComboBox ID="cmbCompany" runat="server" 
                                     Width="100%" 
                                     ValueType="System.String"
                                     TextField="COMPANY_NAME" 
                                     ValueField="COMPANY_NO"
                                     Theme="Office365"
                                     DropDownStyle="DropDown"
                                     IncrementalFilteringMode="Contains">
                        <ValidationSettings Display="Dynamic" 
                                          ErrorDisplayMode="ImageWithTooltip"
                                          ValidationGroup="LoginGroup">
                            <RequiredField IsRequired="true" ErrorText="회사를 선택하세요" />
                        </ValidationSettings>
                    </dx:ASPxComboBox>
                </div>
                
                <!-- 아이디 -->
                <div class="form-group">
                    <label for="txtUserId">
                        아이디 <span class="required">*</span>
                    </label>
                    <dx:ASPxTextBox ID="txtUserId" runat="server" 
                                   Width="100%" 
                                   Theme="Office365"
                                   MaxLength="50"
                                   NullText="아이디를 입력하세요">
                        <ValidationSettings Display="Dynamic" 
                                          ErrorDisplayMode="ImageWithTooltip"
                                          ValidationGroup="LoginGroup">
                            <RequiredField IsRequired="true" ErrorText="아이디를 입력하세요" />
                        </ValidationSettings>
                        <ClientSideEvents KeyPress="function(s, e) { 
                            if(e.htmlEvent.keyCode == 13) { 
                                var pwd = ASPxClientControl.GetControlCollection().GetByName('txtPassword');
                                if(pwd) pwd.Focus(); 
                            } 
                        }" />
                    </dx:ASPxTextBox>
                </div>
                
                <!-- 암호 -->
                <div class="form-group">
                    <label for="txtPassword">
                        암호 <span class="required">*</span>
                    </label>
                    <dx:ASPxTextBox ID="txtPassword" runat="server" 
                                   Width="100%" 
                                   Password="true"
                                   Theme="Office365"
                                   MaxLength="100"
                                   NullText="암호를 입력하세요">
                        <ValidationSettings Display="Dynamic" 
                                          ErrorDisplayMode="ImageWithTooltip"
                                          ValidationGroup="LoginGroup">
                            <RequiredField IsRequired="true" ErrorText="암호를 입력하세요" />
                        </ValidationSettings>
                        <ClientSideEvents KeyPress="function(s, e) { 
                            if(e.htmlEvent.keyCode == 13) { 
                                var btn = ASPxClientControl.GetControlCollection().GetByName('btnLogin');
                                if(btn) btn.DoClick(); 
                            } 
                        }" />
                    </dx:ASPxTextBox>
                </div>
                
                <!-- 로그인 버튼 -->
                <dx:ASPxButton ID="btnLogin" runat="server" 
                              Text="로그인" 
                              Width="100%"
                              Height="48px"
                              Theme="Office365"
                              OnClick="btnLogin_Click"
                              ValidationGroup="LoginGroup">
                    <Image IconID="actions_apply_16x16">
                    </Image>
                </dx:ASPxButton>
            </div>
            
            <!-- 푸터 -->
            <div class="login-footer">
                © 2025 SPELIX. All rights reserved.
            </div>
        </div>
    </form>
</body>
</html>
