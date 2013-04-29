<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Form.aspx.cs" Inherits="WebApplication1.Form" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="Style.css" />
    <link rel="stylesheet" type="text/css" href="Scripts/jquery-ui-1.10.2.custom.min.css" />

    <title>Common Knowledge</title>

    <script src="Scripts/jquery-2.0.0.min.js"></script>
    <script src="Scripts/jquery-ui-1.10.2.custom.min.js"></script>
    <style type="text/css">
        #UploadFileForm {
            width: 748px;
            height: 25px;
            margin-top: 9px;
        }
        #form1 {
            height: 899px;
            width: 1849px;
        }
    </style>
</head>
<body>
    
    <form id="form1" runat="server" target="UploadPage">
            <div class="topCol">
            <asp:Button ID="ChangeUserButton" runat="server" Text="Log Out" OnClick="ChangeUserButton_Click" Height="32px" style="margin-left: 240px" Width="116px" />
        </div>
        <div class="leftCol">
            <asp:Button ID="CreatePackageModal1" runat="server" Text="testmodalserver"  Height="32px" style="margin-left: 240px" Width="116px" />
            <input id="CreatePackageModal" type="button" value="Create Package" />
            <br />
            <input id="UploadFileModal" type="button" value="Upload File" />
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
     </div>

     <script>
         $(document).ready(function() {
             $('#CreatePackageModal').click(function() {
                 $('#dialog-form').dialog({
                     modal: true,
                     buttons: {
                         "Submit": function() { $(this).dialog("close"); },
                         "Cancel": function() { $(this).dialog("close"); }
                     },
                      
                 }).parent().appendTo("form1");;
             })});

    </script>

<div id="dialog-form" style="display:none"; title="Upload a new File">
    Please fill in all the fields!
    <table>
        <tr>
            <td>
                Name
            <td>
                <asp:TextBox ID="txtName" runat="server" />
            </td>
            </td>
        </tr>

                <tr>
            <td>
                Email
                <td>
                <asp:TextBox ID="TextBox1" runat="server" />
                    </td>
            </td>
        </tr>           
    </table>
            </form>
    <form>

    </form>
    
     <br /><br />
    </div> 
    </body>
</html>
