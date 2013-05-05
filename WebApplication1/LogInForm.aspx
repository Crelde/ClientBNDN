<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogInForm.aspx.cs" Inherits="WebApplication1.WebForm1" %>

<!DOCTYPE XHTML5>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

</head>
<body>

    <form id="form1" runat="server">
        <asp:Label ID="PleaseLoginLabel" runat="server" Font-Size="Large" Text="Please enter your email and password."></asp:Label>
        <br />
        <br />
        <asp:Label ID="EmailLabel" runat="server" Text="Email:"></asp:Label>
        <asp:TextBox ID="EmailTextBox" runat="server" style="margin-left: 71px"></asp:TextBox>
        <br />
        <br />
        <asp:Label ID="PasswordLabel" runat="server" Text="Password:"></asp:Label>
        <asp:TextBox ID="PasswordTextBox" runat="server" type="password" style="margin-left: 49px"> </asp:TextBox>
        <br />
        <br />
        <asp:Button ID="LogInButton" runat="server" style="margin-left: 81px" Text="Log in" OnClick="LogInButton_Click" />
    </form>

</body>
</html>
