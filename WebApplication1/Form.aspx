<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Form.aspx.cs" Inherits="WebApplication1.Form" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="Style.css" />
    <title>Common Knowledge</title>
    <script src="Scripts/jquery-2.0.0.min.js"></script>
</head>
<body>
    <form id="form1" runat="server" aria-dropeffect="none">
            <div class="topCol">
            <asp:Button ID="ChangeUserButton" runat="server" Text="Log Out" OnClick="ChangeUserButton_Click" Height="32px" style="margin-left: 240px" Width="116px" />
        </div>
        <div class="leftCol">
            <asp:Button ID="CreatePackageButton" runat="server" Text="Create New Package" Height="36px"  Width="173px" OnClick="CreatePackageButton_Click" style="margin-left: 102px; margin-top: 36px" />
            <br />
            <asp:Label ID="ChooseAPackageLabel" runat="server" Font-Size="Large" Text="These are the packages you have the rights to"></asp:Label>
            <br />
            <asp:DropDownList ID="DropDownList1" runat="server" Height="27px" style="margin-left: 101px; margin-top: 22px" Width="141px">
            </asp:DropDownList>
        </div>
    <div class="rightCol">
            <asp:Label ID="UploadFileLabel" runat="server" Text="Upload a new file into the current package"></asp:Label>
            <br />
             &nbsp;<asp:FileUpload ID="FileUpload1" runat="server" Height="27px" Width="397px" />
            <asp:Button ID="UploadFileButton" runat="server" Text="Upload File!"  Height="25px" Width="141px" style="margin-left: 75px" OnClick="UploadFileButton_Click" />
        <asp:BulletedList ID="BulletedList1" runat="server" Height="105px"  > 
             </asp:BulletedList>           
        <asp:Panel ID="Panel1" runat="server" Height="209px">
                        Hi, im panel i have some very interesting text, i think you should try to press the button underneath me, it might enhance the reading experience, or maybe it won't :OooLOoOOOOoOO
                        <br />
                        <br />
                        <br />
            </asp:Panel>
             <input id="DareButton" type="button" value="Press it if u dare" /><script type="text/javascript">
            $(document).ready(function()
            {
                $("#UploadFileButton").click(function()
                {
                    alert("SHIT DOESN'T WORK YET, CHILL GOD DAMNIT");
                });
            });
        </script><script type="text/javascript">
            $(document).ready(function () {
                $("#DareButton").click(function () {
                    var p = $("#Panel1");
                    startAnimation();
                    function startAnimation() {
                        p.animate({ height: 500 }, "slow");
                        p.animate({ width: 500 }, "slow");
                        p.css("background-color", "green");
                        p.animate({ height: 200 }, "slow");
                        p.animate({ width: 200 }, "slow",startAnimation);
                    }
                });
            });
        </script></div>

    </form>
    </body>
</html>
