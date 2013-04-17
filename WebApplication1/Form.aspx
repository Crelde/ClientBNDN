<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Form.aspx.cs" Inherits="WebApplication1.Form" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        #form1 {
            width: 591px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div style="width: 613px; margin-left: 480px">
            <asp:Label ID="Label1" runat="server" EnableTheming="True" Font-Bold="True" Font-Overline="True" Font-Size="X-Large" Font-Underline="True" Text="Hello and welcome to our asp.net client thingy HIHI"></asp:Label>
        </div>
        <p>
            <asp:Label ID="Label2" runat="server" Text="Enter an Email and see info about the user."></asp:Label>
        </p>
        <p>
            <asp:TextBox ID="TextBox1" runat="server" Width="207px"></asp:TextBox>
            <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" style="margin-left: 32px" Text="Execute" Width="87px" />
        </p>
        <p>
            <asp:TextBox ID="GetUserTextBox" runat="server" Height="82px" Width="405px" TextMode="MultiLine"></asp:TextBox>
        &nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="ChangeUserButton" runat="server" Text="Change User" OnClick="ChangeUserButton_Click" />
        </p>
    </form>
</body>
</html>
