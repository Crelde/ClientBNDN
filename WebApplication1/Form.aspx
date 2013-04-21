<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Form.aspx.cs" Inherits="WebApplication1.Form" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="Style.css" />
    <title></title>
    <style type="text/css">
        #form1 {
            width: 1207px;
            height: 731px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" aria-dropeffect="none">
            <div class="topCol">
            <asp:Button ID="ChangeUserButton" runat="server" Text="Log Out" OnClick="ChangeUserButton_Click" Height="32px" style="margin-left: 240px" Width="116px" />
        </div>
        <div class="leftCol">
            <asp:Button ID="CreatePackageButton" runat="server" Text="Create New Package" Height="36px"  Width="173px" OnClick="CreatePackageButton_Click" style="margin-left: 102px; margin-top: 36px" />
            <br />
            <br />
            <asp:Label ID="ChooseAPackageLabel" runat="server" Font-Size="Large" Text="These are the packages you have the rights to"></asp:Label>
            <br />
            <asp:DropDownList ID="DropDownList1" runat="server" Height="27px" style="margin-left: 101px; margin-top: 22px" Width="141px">
            </asp:DropDownList>
        </div>
    <div class="rightCol">
            <asp:Label ID="UploadFileLabel" runat="server" Text="Upload a new file into the current package"></asp:Label>
            <br />
            <asp:FileUpload ID="FileUpload1" runat="server" Height="27px" Width="397px" />
            <asp:Button ID="UploadFileButton" runat="server" Text="Upload File!"  Height="25px" Width="141px" style="margin-left: 75px" OnClick="UploadFileButton_Click" />
            <asp:BulletedList ID="BulletedList1" runat="server" Height="105px" >
            </asp:BulletedList>
    </div>

    </form>
    </body>
</html>
