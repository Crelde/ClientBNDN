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
            margin-left: 0px;
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
            <input id="CreatePackage" type="button" value="Create Package"/>
            <br />
            <asp:Button ID="testVisibility" runat="server" Text="testvisibility" OnClick="test_click"/>
            &nbsp;<br />
            <asp:Label ID="ChooseAPackageLabel" runat="server" Font-Size="Large" Text="These are the packages you have the rights to"></asp:Label>
            <br />
            <asp:DropDownList ID="DropDownList1" runat="server" Height="27px" style="margin-left: 1px; margin-top: 22px" Width="141px" AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
            </asp:DropDownList>
             <br />
             <asp:Button ID="adminButton" runat="server" style="margin-left: 31px; margin-top: 8px;" Text="Admin Controls" OnClick="adminButton_Click" />
             <asp:Button ID="addtopackage" runat="server" Text="AddFileToPackage" />
        </div>
            <asp:Panel ID="InteractivePanelFiles" runat="server" ScrollBars="Vertical" >
                <asp:Label ID="uploadfilelabel" Text="Upload a new file here" runat="server"/>
                <br />
                <asp:FileUpload ID="FileUpload1" runat="server" />
                <asp:Button ID="UploadModal" runat="server" style="margin-left: 22px" Text="Create a new File" Width="124px" />
                <asp:Button ID="finalUpload" runat="server" OnClick="finalUpload_Click" style="margin-left: 19px" Text="Upload!" Width="116px" />
                <asp:DataList ID="DataList1" runat="server" BackColor="White" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" CellPadding="3" ForeColor="Black" GridLines="Vertical" Width="500px" style="margin-right: 0px">
                    <AlternatingItemStyle BackColor="#CCCCCC" />
                    <FooterStyle BackColor="#CCCCCC" />
                    <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
                    <ItemTemplate>
                        <h2>
                            <asp:Label ID="dName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>'></asp:Label>
                            <asp:Button ID="tagsB" runat="server" CommandArgument='<%# Eval("Id") + ";" +Eval("Name")%>' CommandName="tag" style="float: right" Height="32px" OnCommand="btn_command" Text="Tags" Width="112px" />
                            <br />
                            <asp:Label ID="dId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Id") %>'></asp:Label>
                            <br />
                            <asp:Label ID="dDesc" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Description") %>'></asp:Label>
                            <br />
                            <asp:Label ID="dEmail" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "OwnerEmail") %>'></asp:Label>
                            <br />
                            <asp:Label ID="dDate" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Date") %>'></asp:Label>
                            <br />
                            <asp:Button ID="downloadItem" runat="server" CommandArgument='<%# Eval("Id") + ";" +Eval("Name")%>' CommandName="download" Height="32px" OnCommand="btn_command" Text="Download" Width="112px" />
                            <br />
                            <asp:Button ID="deleteItem" runat="server" CommandArgument='<%# Eval("Id") + ";" +Eval("Name")%>' CommandName="delete" Height="32px" OnCommand="btn_command" Text="Delete" Width="112px" />
                        </h2>
                    </ItemTemplate>
                    <SelectedItemStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                </asp:DataList>
                          </asp:Panel>
        <style type="text/css">
ul.BList li{color:Black ;}
ul.BList a:hover{color:grey ;}
ul.BList li a {color: #000000; } 
            </style>

                <asp:Panel ID="TagPanel" runat="server" CssClass="rightrightCol" Visible="False">
                    <asp:BulletedList ID="BulletedList1" runat="server" OnClick="BulletedList1_Click" DisplayMode="LinkButton" CssClass="Blist">
                    </asp:BulletedList>
                    <asp:Button ID="CreateTag" runat="server" Text="Create a new Tag" Width="158px" OnClick="CreateTag_Click" />
                    <asp:TextBox ID="CreateBox" runat="server" style="margin-left: 25px"></asp:TextBox>
                    <br />
                    <br />
                    <br />
                    <asp:Label ID="deleteLabel" runat="server" Text="Click on the tag you wish to delete"></asp:Label>
                    <br />
                    <br />
                    <asp:Button ID="DeleteTag" runat="server" Text="Delete a tag" Width="158px" OnClick="DeleteTag_Click" />
                    <asp:TextBox ID="DeleteBox" runat="server" style="margin-left: 25px"> </asp:TextBox>
                    <br />
                </asp:Panel>
   

        <asp:Panel ID="InteractivePanelOther" runat="server" Visible="false">
            <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
            <asp:FileUpload ID="FileUpload2" runat="server" />
            <asp:Button ID="Button1" runat="server" Text="Button" />
        </asp:Panel>
        <asp:Panel ID="InteractivePanelAdmin" runat="server" Visible="false">
            <asp:Label ID="Label2" runat="server" Text="Here goes all that is admin related, only visible to admins" Font-Size="Larger"></asp:Label>
            <br />
            <asp:Button ID="Button3" runat="server" Text="Create a new User" />
            <asp:Button ID="Button4" runat="server" Text="Update existing user" />
            <asp:Button ID="Button5" runat="server" Text="Button" />
            <br />
            <asp:Button ID="Button2" runat="server" Text="Delete existing user" />
            <br />
            &nbsp;</asp:Panel>


            <h3>&nbsp;</h3>
            <br />
            <br />



        <asp:HiddenField ID="Origin" runat="server" />
        <asp:HiddenField ID="Description" runat="server" />
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
             var email = $("#fOrigin").val();
             document.getElementById('Origin').value = email;
             var desc = $("#fDesc").val();
             document.getElementById('Description').value = desc;
             dialogclose();
         };
         </script>

<div id="dialog-form" style="display:none"; title="Upload a new File">
    Please fill in all the fields!
      <table>
          <tr>
          Please write where the files originates:
         <td> Origin: <input id="fOrigin" type="text" /> </td> </tr>    
          <tr>   
          Please provide a description of the file:
         <td> File Description: <input id="fDesc" type="text" /> </td> </tr>       >    
        </table>
        <input id="Submit" type="submit" value="Submit" onclick="saveVariables()"/>       

    </div>
            </form>     
    </body>
</html>
