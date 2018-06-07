<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignUpPage.aspx.cs" Inherits="FridayCore.Pages.SignUpPage" %>

<%@ Import Namespace="Sitecore.Configuration" %>
<%@ Import Namespace="Sitecore.SecurityModel.License" %>
<%@ Import Namespace="Sitecore.Resources" %>
<%@ Import Namespace="Sitecore.Web.UI" %>
<%@ Import Namespace="Sitecore" %>

<!DOCTYPE html>

<html>
<head runat="server">

    <link rel="shortcut icon" href="/sitecore/images/favicon.ico" />
    <meta name="robots" content="noindex, nofollow" />
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <sc:platformfontstyleslink runat="server" />

    <!-- Bootstrap for testing -->
    <link href="../login/Login.css" rel="stylesheet" />

    <style>
        .login-outer {
            background: url('/sitecore/login/drop_wallpaper.jpg') no-repeat center center fixed;
            background-size: cover;
        }
    </style>
</head>
<body class="sc">
    <div class="login-outer">
        <div class="login-main-wrap">
            <div class="login-box">
                <div class="logo-wrap">
                    <img src="login/logo_new.png" alt="Sitecore logo" />
                </div>

                <form id="LoginForm" runat="server" class="form-signin" role="form">

                    <div id="login">

                        <h2>Create user account
                        </h2>

                        <div class="scLoginFailedMessagesContainer">
                            <div id="credentialsError" class="scMessageBar scWarning" style="display: none">
                                <i class="scMessageBarIcon"></i>
                                <div class="scMessageBarTextContainer">
                                    <div class="scMessageBarText">
                                        <asp:Literal ID="credentialsAreNotInput" runat="server" Text="<%#Translate.Text(Texts.Please_Enter_Your_Login_Credentials)%>" />
                                    </div>
                                </div>
                            </div>
                            <asp:PlaceHolder runat="server" ID="FailureHolder" Visible="False">
                                <div id="loginFailedMessage" class="scMessageBar scWarning">
                                    <i class="scMessageBarIcon"></i>
                                    <div class="scMessageBarTextContainer">
                                        <div class="scMessageBarText">
                                            <asp:Literal ID="FailureText" runat="server" />
                                            <a href="/sitecore/login"><%#Translate.Text(TEXTS_BACK_TO_LOGIN) %></a>
                                        </div>
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                        </div>

                        <asp:PlaceHolder runat="server" ID="SuccessHolder" Visible="False">
                            <div class="sc-messageBar">
                                <div class="sc-messageBar-head alert alert-info">
                                    <i class="alert-ico"></i>
                                    <span class="sc-messageBar-messageText">
                                        <asp:Literal runat="server" ID="SuccessText" /><br />
                                        <a href="/sitecore/login"><%#Translate.Text(TEXTS_BACK_TO_LOGIN) %></a>
                                    </span>
                                </div>
                            </div>
                        </asp:PlaceHolder>

                        <div id="FormHolder" runat="server">
                            <div class="form-wrap">
                                <asp:Label runat="server" ID="loginLbl" CssClass="login-label" Text="<%#Translate.Text(Texts.UserName)%>" />
                                <asp:TextBox ID="UserName" CssClass="form-control" placeholder="<%#Translate.Text(Texts.ENTER_USER_NAME)%>" autofocus runat="server" ValidationGroup="SignUp" />
                                <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" ValidationGroup="SignUp" />

                                <asp:Label runat="server" ID="Label1" CssClass="login-label" Text="<%#Translate.Text(Texts.Email)%>" />
                                <asp:TextBox ID="Email" CssClass="form-control" placeholder="<%#Translate.Text(TEXTS_ENTER_EMAIL)%>" runat="server" ValidationGroup="SignUp" />
                                <asp:RequiredFieldValidator ID="EmailValidator" runat="server" ControlToValidate="Email" ValidationGroup="SignUp" />
                            </div>

                            <asp:Button runat="server" ValidationGroup="SignUp" OnClick="SignUpClick" UseSubmitBehavior="True" CssClass="btn btn-primary btn-block" Text="<%#Translate.Text(TEXTS_SIGN_UP)%>" />
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>
    <script type="text/javascript" src="/sitecore/login/login.js"></script>
</body>
</html>
