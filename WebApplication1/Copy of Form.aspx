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
        <p>
            <asp:Button ID="ChangeUserButton" runat="server" Text="Change User" OnClick="ChangeUserButton_Click" />
        </p
        <p>
        <asp:Panel ID="Panel1" runat="server" BorderColor="#006600" BorderStyle="Solid" style="margin-left: 242px" Width="503px">
            &nbsp;
            <asp:Label ID="Label2" runat="server" Text="Enter an Email and see info about the user."></asp:Label>
            <br />
            <p>
                <asp:TextBox ID="TextBox1" runat="server" Width="207px"></asp:TextBox>
                <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" style="margin-left: 32px" Text="Execute" Width="87px" />
            </p>
            <p>
                <asp:TextBox ID="GetUserTextBox" runat="server" EnableTheming="True" Height="82px" ReadOnly="True" style="margin-right: 82px" TextMode="MultiLine" Width="488px"></asp:TextBox>
                &nbsp;&nbsp;&nbsp;&nbsp;
            </p>
        </asp:Panel>
    </form>
</body>
</html>
