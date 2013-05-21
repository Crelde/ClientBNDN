<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Form.aspx.cs" Inherits="WebApplication1.Form" %>

<!DOCTYPE XHTML5>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="Style.css" />
    <title>Common Knowledge</title>

    <!-- this style defines how the bulletpoint list in  tags looks like -->
    <style type="text/css">
        ul.BList li{color:Black ;}
        ul.BList a:hover{color:grey ;}
        ul.BList li a {color: #000000; } 
    </style>

</head>
<body>
    
    <form id="form1" runat="server" >
        
        <!-- the Header div -->
        <div class="topCol">
            <asp:Panel runat="server" Width="180">
                <asp:Label ID="Label4" runat="server" Text="Currently logged in as:" style="margin-left: 7px"></asp:Label>
                <br />
                <asp:Label ID="activeuserLabel" runat="server" style="margin-left: 8px; margin-top: 8px"></asp:Label>
                <br />
                <asp:Button ID="changepw" runat="server" style="margin-left: 7px" Text="Change password" Width="161px" OnClick="changepw_Click" />
                <br />
                <asp:Button ID="ChangeUserButton" runat="server" Text="Log Out" OnClick="ChangeUserButton_Click"  Width="161px" style="margin-left: 7px; margin-top: 2px;"   />
            <asp:Panel ID="Panel1" runat="server" CssClass ="headerLabel" >
                <asp:Label ID="headerL" runat="server" Text="CommonKnowledge" Font-Size="XX-Large" ></asp:Label>
                <br />
                <asp:Label ID="Label14" runat="server" Text="RentIt" Font-Size="X-Large" style="float: right; margin-left: 176px; "></asp:Label>

            </asp:Panel>
            </asp:Panel>

        </div>
                    

        <!-- The left div for packages -->
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
            <asp:Button ID="adminButton" runat="server" Text="Admin Controls" OnClick="adminButton_Click" style="margin-top: 11px"  Width="178px" Visible="False" />
        </div>
        <!-- This panel includes the datalist and is the main mid panel -->
        <asp:Panel ID="InteractivePanelFiles" runat="server" ScrollBars="Vertical" CssClass="rightCol" >
             <asp:DataList ID="DataList1" runat="server" BackColor="White" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" CellPadding="3" ForeColor="Black" GridLines="Vertical" Width="650px" style="margin-right: 0px">
                 <AlternatingItemStyle BackColor="#CCCCCC" />
                 <FooterStyle BackColor="#CCCCCC" />
                 <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
                 <ItemTemplate>
                     <h4>
                        <asp:Label ID="dName" runat="server" Text='<%#"Filename:" + DataBinder.Eval(Container.DataItem, "Name") %>'></asp:Label>
                        <br />
                        <asp:Label ID="dDesc" runat="server" Text='<%# "Description: " + DataBinder.Eval(Container.DataItem, "Description") %>'></asp:Label>
                        <br />
                        <asp:Label ID="dEmail" runat="server" Text='<%#"Owner email: " + DataBinder.Eval(Container.DataItem, "OwnerEmail") %>'></asp:Label>
                        <br />
                        <asp:Label ID="dOrigin" runat="server" Text='<%#"Origin: " + DataBinder.Eval(Container.DataItem, "Origin") %>'></asp:Label>
                        <br />
                        <asp:Label ID="dDate" runat="server" Text='<%#"Date: " + DataBinder.Eval(Container.DataItem, "Date") %>'></asp:Label>
                        <br />
                        <asp:Button ID="downloadItem" runat="server" CommandArgument='<%# Eval("Id") + ";" +Eval("Name")%>' CommandName="download" Height="23px" OnCommand="btn_command" Text="Download" Width="80px" />
                        <asp:Button ID="edit" runat="server" CommandArgument='<%# Eval("Id") + ";" +Eval("Name") + ";" + Eval("Description")%>' CommandName="edit" Height="23px" OnCommand="btn_command" Text="Edit" Width="80px" />
                        <asp:Button ID="deleteItem" runat="server" CommandArgument='<%# Eval("Id") + ";" +Eval("Name")%>' CommandName="delete" Height="23px" OnCommand="btn_command" Text="Delete" Width="80px" />
                        <asp:Button ID="addtopackage" runat="server" CommandArgument='<%# Eval("Id") + ";" +Eval("Name")%>' CommandName="addToPackage"  Height="23px" OnCommand="btn_command" Text="Add File to a package" Width="150px" />
                        <asp:Button ID="shareFile" runat="server" CommandArgument='<%# Eval("Id") + ";" +Eval("Name")%>' CommandName="shareFile"  Height="23px" OnCommand="btn_command" Text="Share this file" Width="100px" />
                        <asp:Button ID="tagsB" runat="server" CommandArgument='<%# Eval("Id") + ";" +Eval("Name")%>' CommandName="tag" Height="23px" style="margin-left: 50px" OnCommand="btn_command" Text="Tags" Width="80px" />
                     </h4>
                 </ItemTemplate>
                 <SelectedItemStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
             </asp:DataList>
                       </asp:Panel>
        <!-- This panel includes everything regarding tags -->
        <asp:Panel ID="TagPanel" runat="server" Visible="False" CssClass="rightCol" Width="350px">
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
        <!-- This panel includes elements to add a file to an existing panel -->
        <asp:Panel ID="addFiletoPackagePanel" runat="server"  Visible="False" CssClass="rightCol" Width="350px" >
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
        <!-- This panel includes everything regarding creating a new package -->
        <asp:Panel ID="InteractivePanelOther" runat="server" Visible="false" CssClass="rightCol">
            <asp:Label ID="Label1" runat="server" Text="Enter a name for your new package:" Font-Size="Larger"></asp:Label>
            <br />
            <asp:TextBox ID="packageName" runat="server" Width="199px"></asp:TextBox>
            <asp:Button ID="submitpackage" runat="server" Text="Create Package" OnClick="submitpackage_Click" style="margin-left: 41px" />
            <asp:Button ID="cancelcreatepacakge" runat="server" Text="Cancel" style="margin-left: 46px" OnClick="returnToFiles" />
        </asp:Panel>
        <!-- this panel includes elements to change a users password -->
        <asp:Panel ID="passwordPanel" runat="server" Visible="false" CssClass="rightCol">
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
        </asp:Panel>
        <!-- This panel includes everything an admin can do that normal users cant -->
        <asp:Panel ID="InteractivePanelAdmin" runat="server" Visible="false" CssClass="rightCol">
            <asp:Label ID="Label2" runat="server" Text="Here you can change properties to any user" Font-Size="Larger"></asp:Label>
            <br />
            <br />
            <asp:Button ID="createUserBut" runat="server" Text="Create a new User" OnClick="createUserBut_Click" style="height: 26px" />
            <br />
            <br />
            <asp:Label ID="Label23" runat="server" Text="Enter the email of a user you wish to update:" Font-Size="Larger"></asp:Label>
            <br />
            <asp:TextBox ID="userToBeUpdated" runat="server"></asp:TextBox>
            <asp:Button ID="updateUserBut" runat="server" Text="Update existing user" style="margin-left: 30px" OnClick="updateUserBut_Click" />
            <br />
            <br />
            <asp:Label ID="Label24" runat="server" Text="Enter the email of a user you wish to delete:" Font-Size="Larger"></asp:Label>
            <br />
            <asp:TextBox ID="userToBeDeleted" runat="server"></asp:TextBox>
            <asp:Button ID="deleteUserBut" runat="server" Text="Delete existing user" style="margin-left: 35px" OnClick="deleteUserBut_Click" />
            <br />
            <br />
            <asp:Button ID="canceladmin" runat="server" Text="Back" OnClick="returnToFiles" style="margin-left: 142px" />
        </asp:Panel>
        <!-- This panel has everything that is regarding sharing a package -->
        <asp:Panel ID="sharePackagePanel" runat="server" Visible="false" CssClass="rightCol">
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
            <asp:Button ID="SharePackageBut" runat="server" Text="Share Package" OnClick="SharePackageBut_Click" />
            <asp:Button ID="CancelShare" runat="server" Text="Cancel" style="margin-left: 50px" OnClick="returnToFiles" />
            <br />
        </asp:Panel>
        <!-- This panel includes a confirmation for deleting packages -->
        <asp:Panel ID="deletePackagePanel" runat="server" Visible="false" CssClass="rightCol">
            <asp:Label ID="Label8" runat="server" Text="Are you sure you wish to delete this package?" Font-Size="Larger"></asp:Label>
            <br />
            <br />
            <asp:Button ID="confirmdelete" runat="server" Text="Yes" OnClick="confirmdelete_Click" />
            <asp:Button ID="canceldelete" runat="server" Text="Cancel" style="margin-left: 70px" OnClick="returnToFiles" />
            <br />
            <br />
        </asp:Panel>
        <!-- This panel includes elements to edit a files description -->
        <asp:Panel ID="editFilePanel" runat="server" Visible="false" CssClass="rightCol" Width="350px">
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
        </asp:Panel>
        <!-- This panel includes functionality to create a new file -->
        <asp:Panel ID="CreateNewFilePanel" runat="server" Visible="false" CssClass="rightCol">
            <asp:Label ID="Label12" runat="server" Text="Choose a file to upload" Font-Size="Larger"></asp:Label>
            <br />
            <asp:FileUpload ID="FileUpload1" runat="server" />
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
        </asp:Panel>
        <!-- This panel includes functionality to share a single file with another user -->
        <asp:Panel ID="shareFilePanel" runat="server" Visible="false" CssClass="rightCol" Width="350px">
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
            <asp:Button ID="submitShareFile" runat="server" Text="Share File" OnClick="submitShareFile_Click" />
            <asp:Button ID="Button6" runat="server" Text="Cancel" style="margin-left: 50px" OnClick="returnToFiles" />
            <br />
        </asp:Panel>
        <!-- This panel makes it possible to create a new user as an admin -->
        <asp:Panel ID="CreateNewUserPanel" runat="server" Visible="false" CssClass="rightCol" Width="350px">
            <asp:Label ID="Label19" runat="server" Text="Please fill out all the fields" Font-Size="Larger"></asp:Label>
            <br />
            <asp:Label ID="Label20" runat="server" Text="Email:" Font-Size="Larger"></asp:Label>
            <br />
            <asp:TextBox ID="createUserEmail" runat="server"></asp:TextBox>
            <br />
            <asp:Label ID="Label21" runat="server" Text="Password:" Font-Size="Larger"></asp:Label>
            <br />
            <asp:TextBox ID="createUserPw" runat="server"></asp:TextBox>
            <br />
            <asp:Label ID="Label22" runat="server" Text="Type:" Font-Size="Larger"></asp:Label>
            <br />
            <asp:DropDownList ID="createUserTypeDD" runat="server" />
            <br />
            <br />
            <asp:Button ID="CreateNewUserSubmit" runat="server" Text="Submit" OnClick="CreateNewUserSubmit_Click" />
            <asp:Button ID="cancelNewUser" runat="server" Text="Cancel" OnClick="cancelNewUser_Click" style="margin-left: 12px" />
        </asp:Panel>
        <!-- This panel includes functionality to update a users password as an admin -->
        <asp:Panel ID="UpdateUserPanel" runat="server" Visible="false" Height="121px" CssClass="rightCol" Width="350px">
            <asp:Label ID="Label25" runat="server" Text="Fill out what the users password should be:" Font-Size="Larger"></asp:Label>
            <br />
            <br />
            <asp:TextBox ID="UpdatedUserPw" runat="server"></asp:TextBox>
            <br />
            <br />
            <asp:Button ID="submitUpdatedUser" runat="server" Text="Submit" OnClick="submitUpdatedUser_Click"  />
            <asp:Button ID="Button2" runat="server" Text="Cancel" OnClick="cancelNewUser_Click" style="margin-left: 12px" />
        </asp:Panel>
        <!-- This panel includes a confirmation as for if the admin wants to delete the certain user -->
        <asp:Panel ID="DeleteUserPanel" runat="server" Visible="false" CssClass="rightCol" Width="350px">
            <asp:Label ID="Label29" runat="server" Text="Are you sure you wish to delete the user?" Font-Size="Larger"></asp:Label>
            <br />
            <br />
            <asp:Button ID="confirmDeleteUser" runat="server" Text="Yes" OnClick="confirmDeleteUser_Click" />
            <asp:Button ID="cancelDeleteUser" runat="server" Text="Cancel" OnClick="cancelNewUser_Click" style="margin-left: 12px" />
        </asp:Panel>
    </form>     
</body>
</html>
