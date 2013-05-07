<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Form.aspx.cs" Inherits="WebApplication1.Form" %>

<!DOCTYPE XHTML5>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="Style.css" />
    <link rel="stylesheet" type="text/css" href="Scripts/jquery-ui-1.10.2.custom.min.css" />

    <title>Common Knowledge</title>
    <script src="Scripts/jquery.fileupload.js"></script>
    <script src="Scripts/jquery-2.0.0.min.js"></script>
    <script src="Scripts/jquery-ui-1.10.2.custom.min.js"></script>
    <style type="text/css">
        #CreatePackageModal {
            margin-left: 61px;
        }
        #CreatePackage {
            margin-left: 146px;
        }
    </style>
</head>
<body>
    
    <form id="form1" runat="server" target="UploadPage">
            <div class="topCol">
            <asp:Button ID="ChangeUserButton" runat="server" Text="Log Out" OnClick="ChangeUserButton_Click" Height="32px" style="margin-left: 240px" Width="116px" />
        </div>
        <div class="leftCol">
             <br />
            <input id="CreatePackage" type="button" value="Create Package" />
            <br />
            &nbsp;<br />
            <asp:Label ID="ChooseAPackageLabel" runat="server" Font-Size="Large" Text="These are the packages you have the rights to"></asp:Label>
            <br />
            <asp:DropDownList ID="DropDownList1" runat="server" Height="27px" style="margin-left: 101px; margin-top: 22px" Width="141px">
            </asp:DropDownList>
        </div>
    <div class="rightCol">
            <h3>
            <asp:Label ID="uploadfilelabel" Text="Upload a new file here" runat="server"/>
            </h3>
            <asp:FileUpload ID="FileUpload1" runat="server" />
            <asp:Button ID="UploadModal" runat="server" Text="Create a new File"  style="margin-left: 37px" Width="150px" />
            <asp:Button ID="finalUpload" runat="server" Text="Upload!" style="margin-left: 47px" Width="116px" OnClick="finalUpload_Click" />
            <br />
            <br />


            <asp:DataList ID="DataList1" runat="server" BackColor="White" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" CellPadding="3" ForeColor="Black" GridLines="Vertical" Width="953px" >
                <AlternatingItemStyle BackColor="#CCCCCC" />
                <FooterStyle BackColor="#CCCCCC" />
                <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
            <ItemTemplate>
                <h2>
                   <asp:Label ID="dName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>'></asp:Label> <br />
                   <asp:Label ID="dId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Id") %>'></asp:Label> <br />
                   <asp:Label ID="dDesc" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Description") %>'></asp:Label> <br />
                   <asp:Label ID="dEmail" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "OwnerEmail") %>'></asp:Label> <br />
                   <asp:Label ID="dDate" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Date") %>'></asp:Label> <br />
                    <asp:Button ID="downloadItem" runat="server" Text="Download" Height="32px" Width="112px" CommandName="download" OnCommand="btn_command" CommandArgument='<%# Eval("Id") %>' /> <br />
                    <asp:Button ID="deleteItem" runat="server" Text="Delete" Height="32px" Width="112px" CommandName="delete" OnCommand="btn_command" />
                </h2>
                
            </ItemTemplate>
                <SelectedItemStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
            </asp:DataList>

            

     </div>
        <asp:HiddenField ID="Email" runat="server" />
        <asp:HiddenField ID="FileName" runat="server" />
        <asp:HiddenField ID="Description" runat="server" />
        <asp:HiddenField ID="FilePath" runat="server" />
     <script>

         jQuery(document).ready(function () {
             $('#<%=UploadModal.ClientID%>').click(function () {
                 var dlg = $('#dialog-form').dialog({
                     modal: true,
                     height: 500,
                     width: 600,
                     resizable: false,
                     show: 'fold',
                     hide: 'fold',
                     open: function (type, data) {
                         $(this).parent().appendTo("form1");
                     }
                 });
                 dlg.parent().appendTo("form1");
                 return false;
             });
         });
         function dialogclose() {
                 $('#dialog-form').dialog('close');
             }

         function saveVariables(){
             var name =  $("#fName").val();
             document.getElementById('FileName').value = name;
             var email = $("#fEmail").val();
             document.getElementById('Email').value = email;
             var desc = $("#fDesc").val();
             document.getElementById('Description').value = desc;
             dialogclose();
         };
         </script>

<div id="dialog-form" style="display:none"; title="Upload a new File">
    Please fill in all the fields!
     
    <table>
        <tr> <td> File Name: <input id="fName" type="text" /> </td> </tr>
        <tr> <td> File Description: <input id="fDesc" type="text" /> </td> </tr>    
        <tr> <td> Email: <input id="fEmail" type="text" /> </td> </tr>        
        </table>
        <input id="Submit" type="submit" value="Submit" onclick="saveVariables()"/>       

    </div>
            </form>
    <form>

    </form>
    
     <br /><br />
    
    </body>
</html>
