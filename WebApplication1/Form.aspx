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
        #createPackage0 {
            margin-left: 0px;
        }
        #createPackage0 {
            margin-left: 0px;
        }
        </style>
   <style type="text/css">
        ul.BList li{color:Black ;}
        ul.BList a:hover{color:grey ;}
        ul.BList li a {color: #000000; } 
    </style>
</head>
<body>
    
    <form id="form1" runat="server" target="UploadPage">
        <div class="topCol">
            &nbsp;&nbsp;
                <asp:Label ID="Label4" runat="server" Text="Currently logged in as:"></asp:Label>
                <asp:Button ID="changepw" runat="server" style="margin-left: 73px" Text="Change password" Width="161px" OnClick="changepw_Click" />
            <asp:Button ID="ChangeUserButton" runat="server" Text="Log Out" OnClick="ChangeUserButton_Click"  Width="161px" style="margin-left: 53px; margin-top: 7px;"  />
                <br />
                <asp:Label ID="activeuserLabel" runat="server" style="margin-left: 8px; margin-top: 8px"></asp:Label>
        </div>
        <div class="leftCol">
             <asp:Button ID="createPackage" runat="server" Text="Create new package" Width="178px" OnClick="createPackage_Click" />
             <br />
            <br />
            <asp:Button ID="createPackage0" runat="server" Text="Create new File" Width="178px" OnClick="createPackage0_Click" />
             <br />
            <br />
            <asp:Label ID="ChooseAPackageLabel" runat="server" Font-Size="Small" Text="Here are the items you have the rights to"></asp:Label>
            <br />
            <asp:DropDownList ID="DropDownList1" runat="server" Height="27px" style="margin-left: 1px; margin-top: 22px" Width="141px" AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
            </asp:DropDownList>
            <br />
            <br />
             <asp:Button ID="DeletePackage" runat="server" Text="Delete this package" OnClick="DeletePackage_Click" style="margin-top: 19px" Width="178px"  />
             <asp:Button ID="SharePackage" runat="server" Text="Share this package" style="margin-top: 19px" OnClick="SharePackage_Click"  Width="178px" />
             <br />
             <asp:Button ID="adminButton" runat="server" Text="Admin Controls" OnClick="adminButton_Click" style="margin-top: 11px"  Width="178px" />
        </div>

        <asp:Panel ID="InteractivePanelFiles" runat="server" ScrollBars="Vertical" >
                <asp:Label ID="uploadfilelabel" Text="Upload a new file here" runat="server"/>
                <br />

                <asp:Button ID="UploadModal" runat="server" style="margin-left: 0px" Text="Create a new File" Width="124px" />
                <asp:Button ID="finalUpload" runat="server" OnClick="finalUpload_Click" style="margin-left: 0px" Text="Upload!" Width="116px" />
                <asp:DataList ID="DataList1" runat="server" BackColor="White" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" CellPadding="3" ForeColor="Black" GridLines="Vertical" Width="650px" style="margin-right: 0px">
                    <AlternatingItemStyle BackColor="#CCCCCC" />
                    <FooterStyle BackColor="#CCCCCC" />
                    <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
                    <ItemTemplate>
                        <h4>

                            <asp:Label ID="dName" runat="server" Text='<%#"Filename:" + DataBinder.Eval(Container.DataItem, "Name") %>'></asp:Label>
                            <br />
                      <!--   <asp:Label ID="dId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Id") %>'></asp:Label> -->

                            <asp:Label ID="dDesc" runat="server" Text='<%# "Description: " + DataBinder.Eval(Container.DataItem, "Description") %>'></asp:Label>

                            <br />
                            <asp:Label ID="dEmail" runat="server" Text='<%#"Owner email: " + DataBinder.Eval(Container.DataItem, "OwnerEmail") %>'></asp:Label>
                            <br />
                            <asp:Label ID="dOrigin" runat="server" Text='<%#"Origin: " + DataBinder.Eval(Container.DataItem, "Origin") %>'></asp:Label>
                            <br />
                            <asp:Label ID="dDate" runat="server" Text='<%#"Date: " + DataBinder.Eval(Container.DataItem, "Date") %>'></asp:Label>
                            <br />
                            <asp:Button ID="downloadItem" runat="server" CommandArgument='<%# Eval("Id") + ";" +Eval("Name")%>' CommandName="download" Height="20px" OnCommand="btn_command" Text="Download" Width="80px" />
                            <asp:Button ID="edit" runat="server" CommandArgument='<%# Eval("Id") + ";" +Eval("Name") + ";" + Eval("Description")%>' CommandName="edit" Height="20px" OnCommand="btn_command" Text="Edit" Width="80px" />
                            <asp:Button ID="deleteItem" runat="server" CommandArgument='<%# Eval("Id") + ";" +Eval("Name")%>' CommandName="delete" Height="20px" OnCommand="btn_command" Text="Delete" Width="80px" />
                            <asp:Button ID="addtopackage" runat="server" CommandArgument='<%# Eval("Id") + ";" +Eval("Name")%>' CommandName="addToPackage"  Height="20px" OnCommand="btn_command" Text="Add File to a package" Width="150px" />
                            <asp:Button ID="shareFile" runat="server" CommandArgument='<%# Eval("Id") + ";" +Eval("Name")%>' CommandName="shareFile"  Height="20px" OnCommand="btn_command" Text="Share this file" Width="100px" />
                            <asp:Button ID="tagsB" runat="server" CommandArgument='<%# Eval("Id") + ";" +Eval("Name")%>' CommandName="tag" Height="20px" style="margin-left: 50px" OnCommand="btn_command" Text="Tags" Width="80px" />
                        </h4>
                    </ItemTemplate>
                    <SelectedItemStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                </asp:DataList>
                          </asp:Panel>
        <asp:Panel ID="TagPanel" runat="server" Visible="False">
                    <asp:BulletedList ID="BulletedList1" runat="server" OnClick="BulletedList1_Click" DisplayMode="LinkButton" CssClass="Blist">
                    </asp:BulletedList>
                    <asp:Button ID="CreateTag" runat="server" Text="Create new tag" Width="158px" OnClick="CreateTag_Click" />
                    <asp:TextBox ID="CreateBox" runat="server" style="margin-left: 25px"></asp:TextBox>
                    <br />
                    <br />
                    <br />
                    <asp:Label ID="deleteLabel" runat="server" Text="Click on the tag you wish to delete"></asp:Label>
                    <br />
                    <br />
                    <asp:Button ID="DeleteTag" runat="server" Text="Delete tag" Width="158px" OnClick="DeleteTag_Click" />
                    <asp:TextBox ID="DeleteBox" runat="server" style="margin-left: 25px" Enabled="False"></asp:TextBox>
                    <br />
            <asp:Button ID="cancelTag" runat="server" Text="Cancel" OnClick="returnToFiles" />
                    <asp:HiddenField ID="fileI" runat="server" />
                    <asp:HiddenField ID="fileN" runat="server" />
                </asp:Panel>
        <asp:Panel ID="addFiletoPackagePanel" runat="server"  Visible="False" >
                    <asp:Label ID="Label3" runat="server" Text="Select the package you wish to add your file to:"></asp:Label>
                    <br />
                    <br />
                    <asp:DropDownList ID="PackageDropDownF" runat="server" > </asp:DropDownList >
                    <br />
                    <asp:Button ID="addfiletoP" runat="server" Text="Add File" OnClick="addfiletoP_Click" />
                    <br />
                    <asp:Button ID="canceladdpack" runat="server" Text="Cancel" OnClick="returnToFiles" />
                    <asp:HiddenField ID="fileI2" runat="server" />
       </asp:Panel>
        <asp:Panel ID="InteractivePanelOther" runat="server" Visible="false">
            <asp:Label ID="Label1" runat="server" Text="Enter a name for your new package:" Font-Size="Larger"></asp:Label>
            <br />
            <asp:TextBox ID="packageName" runat="server" Width="199px"></asp:TextBox>
            <asp:Button ID="submitpackage" runat="server" Text="Create Package" OnClick="submitpackage_Click" style="margin-left: 41px" />
            <asp:Button ID="cancelcreatepacakge" runat="server" Text="Cancel" style="margin-left: 46px" OnClick="returnToFiles" />

        </asp:Panel>
        <asp:Panel ID="passwordPanel" runat="server" Visible="false">
            <asp:Label ID="Label888" runat="server" Text="Please enter your current password:"  Font-Size="Larger"></asp:Label>
            <br />
            <asp:TextBox ID="oldpw" type="password" runat="server"></asp:TextBox>
            <br />
            <asp:Label ID="Label5" runat="server" Text="Please enter a new password:" Font-Size="Larger"></asp:Label>
            <br />
            <asp:TextBox ID="newpw" type="password" runat="server"></asp:TextBox>

            <br />
            <asp:Label ID="Label6" runat="server" Text="Please confirm your password:" Font-Size="Larger"></asp:Label>
            <br />
            <asp:TextBox ID="confirmpw" type="password" runat="server"></asp:TextBox>
            <br />
            <asp:Button ID="confirmchange" runat="server" Text="Change Password" OnClick="confirmchange_Click" />


            <asp:Button ID="cancelpw" runat="server" OnClick="returnToFiles" style="margin-left: 57px" Text="Cancel" />


            &nbsp;</asp:Panel>
        <asp:Panel ID="InteractivePanelAdmin" runat="server" Visible="false">
            <asp:Label ID="Label2" runat="server" Text="Here goes all that is admin related, only visible to admins" Font-Size="Larger"></asp:Label>
            <br />
            <asp:Button ID="Button3" runat="server" Text="Create a new User" />
            <asp:Button ID="Button4" runat="server" Text="Update existing user" />
            <asp:Button ID="Button5" runat="server" Text="Button" />
            <br />
            <asp:Button ID="Button2" runat="server" Text="Delete existing user" />
            <br />
            <br />
            <asp:Button ID="canceladmin" runat="server" Text="Cancel" OnClick="returnToFiles" />
            &nbsp;</asp:Panel>
        <asp:Panel ID="sharePackagePanel" runat="server" Visible="false">
            <asp:Label ID="Label7" runat="server" Text="Enter the email of a user you wish to share this package with:" Font-Size="Larger"></asp:Label>
            <br />
            <asp:TextBox ID="emailToShareWith" runat="server" Width="178px"></asp:TextBox>
            <br />
            <asp:Label ID="Label10" runat="server" Text="How many days should the user have access to the package?" Font-Size="Larger"></asp:Label>
            <br />
            <asp:TextBox ID="daysAccess" runat="server" Width="63px"></asp:TextBox>
            <br />
            <asp:Label ID="Label11" runat="server" Text="Which kind of access should the user have?" Font-Size="Larger"></asp:Label>
            <br />
            <asp:DropDownList ID="kindofrightDD" runat="server" style="margin-left: 10px" ></asp:DropDownList>
            <br />
            &nbsp;<asp:Button ID="SharePackageBut" runat="server" Text="Share Package" OnClick="SharePackageBut_Click" />
            <asp:Button ID="CancelShare" runat="server" Text="Cancel" style="margin-left: 50px" OnClick="returnToFiles" />
            <br />
            &nbsp;</asp:Panel>
        <asp:Panel ID="deletePackagePanel" runat="server" Visible="false">
            <asp:Label ID="Label8" runat="server" Text="Are you sure you wish to delete this package?" Font-Size="Larger"></asp:Label>
            <br />
            <br />
            <asp:Button ID="confirmdelete" runat="server" Text="Yes" OnClick="confirmdelete_Click" />
            <asp:Button ID="canceldelete" runat="server" Text="Cancel" style="margin-left: 70px" OnClick="returnToFiles" />
            <br />
            <br />
            &nbsp;</asp:Panel>
        <asp:Panel ID="editFilePanel" runat="server" Visible="false">
            <asp:Label ID="Label9" runat="server" Text="Fill in the fields that you want to change:" Font-Size="Larger"></asp:Label>
            <br />
            <asp:Label ID="Label889" runat="server" Text="Description:"></asp:Label>
            <br />
            <asp:TextBox ID="newDesc" runat="server" Width="295px" TextMode="MultiLine" Height="71px"></asp:TextBox>
            <br />
            <br />
            <br />
            <asp:Button ID="updatefilebut" runat="server" OnClick="updatefilebut_Click" Text="Update file" />
            <asp:Button ID="cancelEdit" runat="server" OnClick="returnToFiles" style="margin-left: 38px" Text="Cancel" />
            &nbsp;</asp:Panel>
        <asp:Panel ID="CreateNewFilePanel" runat="server" Visible="false">
            <asp:Label ID="Label12" runat="server" Text="Choose a file to upload" Font-Size="Larger"></asp:Label>
            <br />
            <asp:FileUpload ID="FileUpload1" runat="server" />
            <br />
            <asp:Label ID="Label14" runat="server" Text="File Name:"></asp:Label>
            <br />
            <asp:TextBox ID="FileNamebox" runat="server"></asp:TextBox>
            <br />
            
            
            <asp:Label ID="Label13" runat="server" Text="Origin:"></asp:Label>
            <br />
            <asp:TextBox ID="originText" runat="server"></asp:TextBox>
            <br />
            <asp:Label ID="Label15" runat="server" Text="Description:"></asp:Label>
            <br />
            <asp:TextBox ID="descBox" runat="server" Height="72px" TextMode="MultiLine" Width="299px"></asp:TextBox>
            <br />
            <br />
            <br />
            <asp:Button ID="submitFileBut" runat="server" Text="Upload" OnClick="submitFileBut_Click"/>
            <asp:Button ID="Button9" runat="server" Text="Cancel" OnClick="returnToFiles" style="margin-left: 17px" />
            &nbsp;</asp:Panel>
        <asp:Panel ID="shareFilePanel" runat="server" Visible="false">
            <asp:Label ID="Label16" runat="server" Text="Enter the email of a user you wish to share this file with:" Font-Size="Larger"></asp:Label>
            <br />
            <asp:TextBox ID="fileShareEmail" runat="server" Width="178px"></asp:TextBox>
            <br />
            <asp:Label ID="Label17" runat="server" Text="How many days should the user have access to the file?" Font-Size="Larger"></asp:Label>
            <br />
            <asp:TextBox ID="daysOfAccessFile" runat="server" Width="60px"></asp:TextBox>
            <br />
            <asp:Label ID="Label18" runat="server" Text="Which kind of access should the user have?" Font-Size="Larger"></asp:Label>
            <br />
            <asp:DropDownList ID="kindofrightddF" runat="server" style="margin-left: 10px" ></asp:DropDownList>
            <br />
            &nbsp;<asp:Button ID="submitShareFile" runat="server" Text="Share File" OnClick="submitShareFile_Click" />
            <asp:Button ID="Button6" runat="server" Text="Cancel" style="margin-left: 50px" OnClick="returnToFiles" />
            <br />
            &nbsp;</asp:Panel>
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
