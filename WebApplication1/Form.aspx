<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Form.aspx.cs" Inherits="WebApplication1.Form" %>

<!DOCTYPE XHTML5>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="Style.css" />
    <link rel="stylesheet" type="text/css" href="Scripts/jquery-ui-1.10.2.custom.min.css" />

    <title>Common Knowledge</title>

    <script src="Scripts/jquery-2.0.0.min.js"></script>
    <script src="Scripts/jquery-ui-1.10.2.custom.min.js"></script>
</head>
<body>
    
    <form id="form1" runat="server" target="UploadPage">
            <div class="topCol">
            <asp:Button ID="ChangeUserButton" runat="server" Text="Log Out" OnClick="ChangeUserButton_Click" Height="32px" style="margin-left: 240px" Width="116px" />
        </div>
        <div class="leftCol">
            <asp:Button ID="CreatePackageModal1" runat="server" Text="Fancy PopUp"  Height="32px" style="margin-left: 72px" Width="112px" />
                        <asp:Button ID="Button1" runat="server" Text="update!" onclick="SubmitASP_Click" Height="32px" style="margin-left: 72px" Width="112px"/>
            <br />
            <br />
            <asp:TextBox ID="FancyNameShow" runat="server" style="margin-left: 52px"></asp:TextBox>
            <asp:TextBox ID="FancyEmailShow" runat="server" style="margin-left: 52px"></asp:TextBox>
            <br />
             <br />
            <input id="CreatePackageModal" type="button" value="Create Package" />
            <br />
            &nbsp;<br />
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

            <asp:DataList ID="DataList1" runat="server" BackColor="White" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" CellPadding="3" ForeColor="Black" GridLines="Vertical" Width="953px">
                <AlternatingItemStyle BackColor="#CCCCCC" />
                <FooterStyle BackColor="#CCCCCC" />
                <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
            <ItemTemplate>
                <h2>
                    <%# DataBinder.Eval(Container.DataItem, "Name") %> <br />
                    <%# DataBinder.Eval(Container.DataItem, "Description") %> <br />
                    <%# DataBinder.Eval(Container.DataItem, "OwnerEmail") %><br />
                    <%# DataBinder.Eval(Container.DataItem, "Date") %><br />
                    <asp:Button ID="downloadItem" runat="server" Text="Download" Height="32px" Width="112px" CommandName="Download" OnCommand="download" /><br />
                    <asp:Button ID="deleteItem" runat="server" Text="Delete" Height="32px" Width="112px" CommandName="Delete" OnCommand="delete" />
                </h2>
                
            </ItemTemplate>
                <SelectedItemStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
            </asp:DataList>

            

     </div>
        <asp:HiddenField ID="FancyName" runat="server" />
        <asp:HiddenField ID="FancyEmail" runat="server" />
     <script>

         jQuery(document).ready(function () {
             $('#<%=CreatePackageModal1.ClientID%>').click(function () {
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
             var name =  $("#txtName").val();
             document.getElementById('FancyName').value = name;
             var email = $("#txtEmail").val();
             document.getElementById('FancyEmail').value = email;
             dialogclose();
             };
    </script>

<div id="dialog-form" style="display:none"; title="Upload a new File">
    Please fill in all the fields!
     
    <table>
        <tr>
            <td> Name
                <input id="txtName" type="text" />
                </td>
         
        </tr>

                <tr>
            <td> Email
             <input id="txtEmail" type="text" />
             <input id="Submit" type="submit" value="Submit" onclick="saveVariables()"/>
            </td>
        </tr>           
    </table>
    </div>
            </form>
    <form>

    </form>
    
     <br /><br />
    
    </body>
</html>
