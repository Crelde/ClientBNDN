using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.IO;

namespace WebApplication1
{
    public partial class Form : System.Web.UI.Page
    {
        static Dictionary<string, int> packageDictionary = new Dictionary<string, int>(); // Dictionary of all packages in the package drop down to the left, and their ids.
        static ServiceReference1.Package[] packageList; // List of packages in the package drop down.
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                {
                    fixSource(true);
                    activeuserLabel.Text = Controller._sessionUser.Email;

                    kindofrightDD.Items.Add(new ListItem("View"));
                    kindofrightDD.Items.Add(new ListItem("Edit"));
                    kindofrightddF.Items.Add(new ListItem("View"));
                    kindofrightddF.Items.Add(new ListItem("Edit"));
                    createUserTypeDD.Items.Add(new ListItem("admin"));
                    createUserTypeDD.Items.Add(new ListItem("student"));


                    if (Controller._sessionUser.Type == ServiceReference1.UserType.admin)
                    {
                        adminButton.Visible = true;
                    }
                }
            }
        }

        // This function reloads the data list of files.
        // If redo is set to true, the function also reloads the list of packages.
        // If index is set, it defines which package should be selected after reload.
        protected void fixSource(bool redo = false, int index =0)
        {
            if (redo)
            {
                packageList = Controller.CollectPackages();
                packageDictionary.Clear();
                DropDownList1.Items.Clear();
                foreach (ServiceReference1.Package p in packageList)
                {
                    packageDictionary.Add(p.Name, p.Id);
                    DropDownList1.Items.Add(p.Name);
                }
            }

            ServiceReference1.Package pack = null;
            if (index == 0)
            {
                int packageId;
                packageDictionary.TryGetValue(DropDownList1.SelectedValue, out packageId);

                foreach (ServiceReference1.Package package in packageList)
                {
                    if (package.Id == packageId)
                        pack = package;
                }
            }
            else
            {
                pack = packageList[index];
                DropDownList1.SelectedIndex = index;
            }

            List<ServiceReference1.FileInfo> myList = new List<ServiceReference1.FileInfo>();
            foreach (int i in pack.FileIds)
            {
                ServiceReference1.FileInfo fi;
                try
                {
                    fi = Controller.GetFileInfoById(i);
                    if (fi != null)
                        myList.Add(fi);
                }
                catch (InsufficientRightsException) { }                
            }
            DataList1.DataSource = myList;
            DataList1.DataBind();

                       
        }
        // This function holds all buttons that are present on the data list of files.
        protected void btn_command(object sender, CommandEventArgs e)
        {
            string[] commandarg = e.CommandArgument.ToString().Split(";".ToCharArray());
            string ids = commandarg[0];
            string filename = commandarg[1];
            int id = int.Parse(ids);
            // When the download button is pressed, download the file to the users browser.
            if (e.CommandName == "download")
            {
                byte[] file;
                try 
                { 
                    file = Controller.DownloadFileById(id);
                    Response.Clear();
                    Response.AppendHeader("Content-Disposition", "attachment; filename="+filename);
                    Response.BinaryWrite(file);
                    Response.End();
                }
                catch (NotLoggedInException)
                {
                    messageBox("An error has occured, please log in again.");
                    Response.Redirect("LogInForm.aspx");
                }
                catch (ObjectNotFoundException)
                {
                    messageBox("An error has occured, try reloading the page.");
                }
            }
            // When the edit button is pressed, open the edit panel.
            else if (e.CommandName == "edit")
            {
                fileI2.Value = id.ToString();
                string desc = commandarg[2];
                newDesc.Text = desc;
                editFilePanel.Visible = true;
            }
            // When the delete button is pressed, delete the file if the user is allowed to.
            else if (e.CommandName == "delete")
            {
                try 
                { 
                    int previndex = DropDownList1.SelectedIndex;
                    Controller.DeleteFileById(id);
                    fixSource(true, previndex);
                }
                catch (NotLoggedInException)
                {
                    messageBox("An error has occured, please log in again.");
                    Response.Redirect("LogInForm.aspx");
                }
                catch (ObjectNotFoundException)
                {
                    messageBox("An error has occured, try reloading the page.");
                }
                catch (InsufficientRightsException)
                {
                    messageBox("You do not have rights to delete this file, as the file is only shared for you to view it.");
                }
            }
            // When the tag button is pressed, open the tag panel.
            else if (e.CommandName == "tag")
            {
                updateBulletList(id);
                hideRightPanels();
                TagPanel.Visible = true;
            }
            // When the addToPackage button is pressed, open the add to package panel,
            // and fill the package dropdown in the panel with the avaible packages.
            else if (e.CommandName == "addToPackage")
            {
                fileI2.Value = id.ToString();
                hideRightPanels();
                addFiletoPackagePanel.Visible = true;

                foreach(ServiceReference1.Package package in packageList)
                {
                    if (package.Id > 0)
                        PackageDropDownF.Items.Add(package.Name);
                }
                fileI.Value = id.ToString();

            }
            // When the shareFile button is pressed, open the share file panel.
            else if (e.CommandName == "shareFile")
            {
                hideRightPanels();
                shareFilePanel.Visible = true;
                fileI.Value = id.ToString();               
            }
        }
        // The function that is called when the user clicks the logout button.
        protected void ChangeUserButton_Click(object sender, EventArgs e)
        {
            try { Controller.LogOut(); }
            catch (NotLoggedInException) { }
            finally { Response.Redirect("LogInForm.aspx"); }          
        }
        // This function is called when the admin button is clicked, it displays the admin controls to the user.
        protected void adminButton_Click(object sender, EventArgs e)
        {
            hideRightPanels();
            hideMidPanels();
            InteractivePanelAdmin.Visible = true;
        }
        // When the user is finished creating a tag and clicks the button,
        // add the tag to the item if allowed.
        protected void CreateTag_Click(object sender, EventArgs e)
        {
            int idofchosenfile = int.Parse(fileI.Value);
            string tagtext = CreateBox.Text;

            try { Controller.AddTag(tagtext, idofchosenfile); }
            catch (NotLoggedInException)
            {
                messageBox("An error has occured, please log in again.");
                Response.Redirect("LogInForm.aspx");
            }
            catch (InadequateObjectException)
            {
                messageBox("The tag has to be at least 4 characters long");
            }
            catch (ObjectNotFoundException)
            {
                messageBox("An error has occured, try reloading the page.");
            }
            catch (InsufficientRightsException)
            {
                messageBox("You are not allowed to add a tag to this file as you have only viewing rights to it.");
            }
            updateBulletList(idofchosenfile);
            CreateBox.Text = "";
        }
        // When the user has chosen a tag to delete and clicks the button,
        // delete the tag to the item if allowed.
        protected void DeleteTag_Click(object sender, EventArgs e)
        {
            int idofchosenfile = int.Parse(fileI.Value);
            string tagtext = DeleteBox.Text;

            try { Controller.DropTag(tagtext, idofchosenfile); }
            catch (NotLoggedInException)
            {
                messageBox("An error has occured, please log in again.");
                Response.Redirect("LogInForm.aspx");
            }
            catch (InadequateObjectException)
            {
                messageBox("An error has occured, try reloading the page.");
            }
            catch (ObjectNotFoundException)
            {
                messageBox("An error has occured, try reloading the page.");
            }
            catch (InsufficientRightsException)
            {
                messageBox("You do not have rights to delete tags, as you only have viewing rights to the file.");
                messageBox("An error has occured, try reloading the page.");
            }
            updateBulletList(idofchosenfile);
            DeleteBox.Text = "";
        }
        // When the user selects an item in the bulleted list, 
        // display that tag in the Delete Box.
        protected void BulletedList1_Click(object sender, BulletedListEventArgs e)
        {
            DeleteBox.Text = BulletedList1.Items[e.Index].ToString();
        }
        // When the user presses the createpackage button, open the createpackage panel.
        protected void createPackage_Click(object sender, EventArgs e)
        {
            hideMidPanels();
            hideRightPanels();
            InteractivePanelOther.Visible = true;
        }
        // When the user is done creating a package and submits,
        // create the package on the server if allowed.
        protected void submitpackage_Click(object sender, EventArgs e)
        {
            string pName = packageName.Text;

            ServiceReference1.Package package = new ServiceReference1.Package();

            package.Name = pName;
            hideRightPanels();
            hideMidPanels();

            try
            {
                Controller.CreatePackage(package);
                fixSource(true, DropDownList1.Items.Count - 3);
                InteractivePanelFiles.Visible = true;
            }
            catch (NotLoggedInException)
            {
                messageBox("An error has occured, please log in again.");
                Response.Redirect("LogInForm.aspx");

            }
            catch (InadequateObjectException)
            {
                messageBox("The name of the package has to be at least 4 characers long");
            }
            catch (InsufficientRightsException)
            {
                messageBox("An error has occured, please try reloading the page.");
            }            
        }
        // If the user regrets wanting to create a package and presses cancel,
        // return to the data list with files.
        protected void cancelcreatepacakge_Click(object sender, EventArgs e)
        {
            hideRightPanels();
            hideMidPanels();
            InteractivePanelFiles.Visible = true;
        }
        // This functions hides all panels that are positioned to the right,
        // this is usually called after a new panel is about to be shown,
        // or when a panel has fulfilled its purpose.
        protected void hideRightPanels()
        {
            editFilePanel.Visible = false;
            TagPanel.Visible = false;
            passwordPanel.Visible = false;
            addFiletoPackagePanel.Visible = false;
            shareFilePanel.Visible = false;
            CreateNewUserPanel.Visible = false;
            UpdateUserPanel.Visible = false;
            DeleteUserPanel.Visible = false;
        }
        // This function hides panels that are positioned in the middle.
        protected void hideMidPanels()
        {
            InteractivePanelOther.Visible = false;
            InteractivePanelFiles.Visible = false;
            InteractivePanelAdmin.Visible = false;
            deletePackagePanel.Visible = false;
            sharePackagePanel.Visible = false;
            CreateNewFilePanel.Visible = false;
        }
        // This is called when the user presses the change password button,
        // it displays the change password panel.
        protected void changepw_Click(object sender, EventArgs e)
        {
            hideRightPanels();
            hideMidPanels();
            passwordPanel.Visible = true;
        }
        // This is called when the user presses the delete package button,
        // it displays the confirm delete package panel.
        protected void DeletePackage_Click(object sender, EventArgs e)
        {
            hideMidPanels();
            hideRightPanels();
            deletePackagePanel.Visible = true;
        }
        // This is called when the user presses the share package button,
        // it displays the share package panel.
        protected void SharePackage_Click(object sender, EventArgs e)
        {
            hideMidPanels();
            hideRightPanels();
            sharePackagePanel.Visible = true;
        }
        // This is called when the user confirms that he wants to share a package,
        // it created the appropriate view/edit right on the server, if allowed.
        protected void SharePackageBut_Click(object sender, EventArgs e)
        {
            string rightType = kindofrightDD.SelectedValue;
            string days = daysAccess.Text;
            string s = emailToShareWith.Text;

            ServiceReference1.Right right = new ServiceReference1.Right();
            right.UserEmail = s;
            right.Until = DateTime.Now.AddDays(double.Parse(days));
            int packageid;
            packageDictionary.TryGetValue(DropDownList1.SelectedValue, out packageid);
            right.ItemId = packageid;

            if (string.Compare(rightType, "View") == 0)
                right.Type = ServiceReference1.RightType.view;
            else
                right.Type = ServiceReference1.RightType.edit;
            try
            {
                Controller.GrantRight(right);
                returnToFiles(sender, e);
            }
            catch (NotLoggedInException)
            {
                messageBox("An error has occured, please log in again.");
                Response.Redirect("LogInForm.aspx");
            }
            catch (InsufficientRightsException)
            {
                messageBox("An error has occured, try reloading the page.");
            }
            catch (ObjectNotFoundException)
            {
                messageBox("An error has occured, try reloading the page.");
            }

            catch (InadequateObjectException)
            {
                messageBox("A user with the given email does not exist");
            }
        }
        // This is called when the user confirms that he wants to delete a package,
        // it deletes the package from the server, if allowed.
        protected void confirmdelete_Click(object sender, EventArgs e)
        {
            int packageId; 
            packageDictionary.TryGetValue(DropDownList1.SelectedValue, out packageId);
            try
            {
                Controller.DeletePackageById(packageId);
                hideMidPanels();
                hideRightPanels();
                InteractivePanelFiles.Visible = true;
                fixSource(true);
            }
            catch (NotLoggedInException)
            {
                messageBox("An error has occured, please log in again.");
                Response.Redirect("LogInForm.aspx");
            }
            catch (InsufficientRightsException)
            {
                messageBox("An error has occured, please try reloading the page.");
            }
            catch (ObjectNotFoundException)
            {
                messageBox("An error has occured, please try reloading the page.");
            }
            
        }
        // This is called when the user confirms tat he wants to change his password,
        // ít checks if he remembers his old password, if the confirmation
        // of new password matches, try to update the users password on the server if able.
        protected void confirmchange_Click(object sender, EventArgs e)
        {
            string oldpws = oldpw.Text;
            string newpws = newpw.Text;
            string confirmpws = confirmpw.Text;

            ServiceReference1.User user = Controller._sessionUser;

            if (string.Compare(user.Password, oldpws) != 0)
                messageBox("Your current password is not correct");
            else if (string.Compare(newpws, confirmpws) != 0)
                messageBox("The new passwords do not match the confirmation password box");
            else
            {
                user.Password = newpws;
                try 
                { 
                    Controller.UpdateUser(user);
                    hideMidPanels();
                    hideRightPanels();
                    oldpw.Text = "";
                    newpw.Text = "";
                    confirmpw.Text = "";
                    InteractivePanelFiles.Visible = true;
                }
                catch (NotLoggedInException)
                {
                    messageBox("An error has occured, please log in again.");
                    Response.Redirect("LogInForm.aspx");
                }
                catch (InsufficientRightsException)
                {
                    messageBox("An error has occured, please try reloading the page.");
                }
                catch (InadequateObjectException)
                {
                    messageBox("An error has occured, please try reloading the page.");
                }
                catch (OriginalNotFoundException)
                {
                    messageBox("An error has occured, please try reloading the page.");
                }
            }
        }
        // This function is called when we want to return to the list of files,
        // often after performing and operation.
        protected void returnToFiles(object sender, EventArgs e)
        {
            hideMidPanels();
            hideRightPanels();
            InteractivePanelFiles.Visible = true;
        }
        // This function is called when the user is done editting a files description,
        // it tries to update tries to update it on the server, if able.
        protected void updatefilebut_Click(object sender, EventArgs e)
        {
            int id = int.Parse(fileI2.Value);
            string s = newDesc.Text;
            try 
            { 
                int prevIndex = DropDownList1.SelectedIndex;
                ServiceReference1.FileInfo fi = Controller.GetFileInfoById(id);
                fi.Description = s;
                Controller.UpdateFileInfo(fi);
                returnToFiles(sender, e);
                fixSource(true, prevIndex);
            }
            catch (NotLoggedInException)
            {
                messageBox("An error has occured, please log in again.");
                Response.Redirect("LogInForm.aspx");
            }
            catch (InsufficientRightsException)
            {
                messageBox("You do not have rights to do that, as the file is only shared for you to view it.");
            }
            catch (InadequateObjectException)
            {
                messageBox("An error has occured, try reloading the page.");
            }
            catch (OriginalNotFoundException)
            {
                messageBox("An error has occured, try reloading the page.");
            }
        }
        // This function is called when the user confirms he wants to upload a file,
        // it tries to upload it to the server, if able.
        protected void submitFileBut_Click(object sender, EventArgs e)
        {
            int previndex = DropDownList1.SelectedIndex;
            byte[] filebytes = FileUpload1.FileBytes;
            string origin = originText.Text;
            string desc = descBox.Text;

            ServiceReference1.FileTransfer filetrans = new ServiceReference1.FileTransfer();
            ServiceReference1.FileInfo fileinfo = new ServiceReference1.FileInfo();
            filetrans.Data = filebytes;
            filetrans.Info = fileinfo;

            fileinfo.Date = DateTime.Now;
            fileinfo.Description = desc;
            fileinfo.Name = FileUpload1.FileName;
            fileinfo.Origin = origin;

            try
            {
                Controller.UploadFile(filetrans);
                returnToFiles(sender, e);
                fixSource(true, DropDownList1.Items.Count -1);
                returnToFiles(sender, e);
                
            }
            catch (NotLoggedInException)
            {
                messageBox("An error has occured, please log in again.");
                Response.Redirect("LogInForm.aspx");
            }
            catch (InadequateObjectException)
            {
                messageBox("You have to choose a file.");
            }
        }
        // This function is called when the user presses the create package button,
        // it opens the create package panel.
        protected void createPackage0_Click(object sender, EventArgs e)
        {
            hideMidPanels();
            hideRightPanels();
            CreateNewFilePanel.Visible = true;
        }
        // This function is called when the user confirms that he wants to share a file,
        // it tries to create a view/edit right on the server, if able.
        protected void submitShareFile_Click(object sender, EventArgs e)
        {
            string email = fileShareEmail.Text;
            string days = daysOfAccessFile.Text;
            string rightType = kindofrightddF.SelectedValue;

            ServiceReference1.Right right = new ServiceReference1.Right();

            right.UserEmail = email;
            try 
            { 
                right.Until = DateTime.Now.AddDays(double.Parse(days));
                right.ItemId = int.Parse(fileI.Value);

                if (string.Compare(rightType, "View") == 0)
                    right.Type = ServiceReference1.RightType.view;
                else
                    right.Type = ServiceReference1.RightType.edit;
                Controller.GrantRight(right);
                returnToFiles(sender, e);
            }
            catch (FormatException)
            {
                messageBox("You must write a whole number of days (such as '10')");
            }
            catch (NotLoggedInException)
            {
                messageBox("An error has occured, please log in again.");
                Response.Redirect("LogInForm.aspx");
            }
            catch (InsufficientRightsException)
            {
                messageBox("You do not have rights to share the file, as it is only shared with you for you to view it.");
            }
            catch (ObjectNotFoundException)
            {
                messageBox("An error has occured, try reloading the page.");
            }

            catch (InadequateObjectException)
            {
                messageBox("A user with the given email does not exist");
            }
        }
        // This function is called when an admin confirms that he wants to create a new user,
        // it tries to add the user to ther server, if able.
        protected void CreateNewUserSubmit_Click(object sender, EventArgs e)
        {
            string email = createUserEmail.Text;
            string pw = createUserPw.Text;
            string type = createUserTypeDD.SelectedValue;

            ServiceReference1.User user = new ServiceReference1.User(); // Set this via fields #Crelde
            user.Email = email;
            user.Password = pw;

            if (string.Compare(type, "admin") == 0)
                user.Type = ServiceReference1.UserType.admin;
            else
                user.Type = ServiceReference1.UserType.standard;
            try
            {
                Controller.CreateUser(user);
                hideRightPanels();
            }
            catch (NotLoggedInException)
            {
                messageBox("An error has occured, please log in again.");
                Response.Redirect("LogInForm.aspx");
            }
            catch (InsufficientRightsException)
            {
                messageBox("An error has occured, try reloading the page.");
            }
            catch (InadequateObjectException)
            {
                messageBox("Both email and password is required to create a user.");
            }
            catch (KeyOccupiedException)
            {
                messageBox("A user already exists with the given email.");
            }
        }
        // This function is called when an admin presses the create user button,
        // it opens the create user panel.
        protected void createUserBut_Click(object sender, EventArgs e)
        {
            hideRightPanels();
            CreateNewUserPanel.Visible = true;
        }
        // This function is called when an admin presses cancel while in the 
        // process of creating a new user.
        protected void cancelNewUser_Click(object sender, EventArgs e)
        {
            hideRightPanels();
        }
        // This function is called when an admin presses the delete user button,
        // it opens the confirm delete user panel.
        protected void deleteUserBut_Click(object sender, EventArgs e)
        {
            try 
            { 
                Controller.GetUserByEmail(userToBeDeleted.Text);
                hideRightPanels();
                DeleteUserPanel.Visible = true;
            }
            catch (NotLoggedInException)
            {
                messageBox("An error has occured, please log in again.");
                Response.Redirect("LogInForm.aspx");
            }
            catch (InsufficientRightsException)
            {
                messageBox("An error has occured, try reloading the page.");
            }
            catch (ObjectNotFoundException)
            {
                messageBox("No user exists with that email.");
            }    
        }
        // This function is called when an admin clicks the update user button
        // it opens the update user panel with the given users information.
        protected void updateUserBut_Click(object sender, EventArgs e)
        {
            ServiceReference1.User user;
            try 
            { 
                user = Controller.GetUserByEmail(userToBeUpdated.Text);
                UpdatedUserPw.Text = user.Password;
                hideRightPanels();
                UpdateUserPanel.Visible = true;
            }
            catch (NotLoggedInException)
            {
                messageBox("An error has occured, please log in again.");
                Response.Redirect("LogInForm.aspx");
            }
            catch (InsufficientRightsException)
            {
                messageBox("An error has occured, try reloading the page.");
            }
            catch (ObjectNotFoundException)
            {
                messageBox("No user exists with that email.");
            }
        }
        // This function is called when an admin confirms that he wishes to delete a user
        // it tries to delete the user on the server if able.
        protected void confirmDeleteUser_Click(object sender, EventArgs e)
        {
            try
            {
                Controller.DeleteUserByEmail(userToBeDeleted.Text);
                hideRightPanels();
            }
            catch (NotLoggedInException)
            {
                messageBox("An error has occured, please log in again.");
                Response.Redirect("LogInForm.aspx");
            }
            catch (InsufficientRightsException)
            {
                messageBox("An error has occured, try reloading the page.");
            }
            catch (ObjectNotFoundException)
            {
                messageBox("An error has occured, try reloading the page.");
            }
        }
        // This function is called when an admin confirms that he wishes to update a users password on the server,
        // it tries to update the password on the server if able.
        protected void submitUpdatedUser_Click(object sender, EventArgs e)
        {
            string pw = UpdatedUserPw.Text;
            ServiceReference1.User user = Controller._sessionUser;
            user.Password = pw;

            try
            {
                Controller.UpdateUser(user);
                hideRightPanels();
            }
            catch (NotLoggedInException)
            {
                messageBox("An error has occured, please log in again.");
                Response.Redirect("LogInForm.aspx");
            }
            catch (OriginalNotFoundException)
            {
                messageBox("An error has occured, try reloading the page.");
            }
            catch (InadequateObjectException)
            {
                messageBox("An error has occured, try reloading the page.");
            }
            catch (InsufficientRightsException)
            {
                messageBox("An error has occured, try reloading the page.");
            }
        }
        // This function is called when the package drop down to the left is used and the index changed.
        // it changes the data list of files to represent the newly selected packages files.
        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            fixSource();
            returnToFiles(sender, e);
        }
        // This is a private function used to give messages to the user with an alert box,
        // it takes a string, and prints a message box containing the given string.
        private void messageBox(string message)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ErrorAlert", "alert('"+message+"');", true);
        }
        // This function is called when we want to update the bulletList of tags that is currently displayed.
        private void updateBulletList(int id)
        {
            BulletedList1.Items.Clear();
            string[] tags;
            try
            {
                tags = Controller.GetTagsByItemId(id);

                foreach (string tag in tags)
                {
                    BulletedList1.Items.Add(new ListItem(tag));
                }
                
                TagPanel.Visible = true;
                fileI.Value = id.ToString();
            }
            catch (NotLoggedInException)
            {
                messageBox("An error has occured, please log in again.");
                Response.Redirect("LogInForm.aspx");
            }
            catch (ObjectNotFoundException)
            {
                messageBox("An error has occured, try reloading the page.");
            }
            catch (InsufficientRightsException)
            {
                messageBox("An error has occured, try reloading the page.");
            }
        }
        // This function is called when the user confirms that he wants to add the selected file to the selected package
        // it tries to add it to the package on the server, if able.
        protected void addfiletoP_Click(object sender, EventArgs e)
        {
            int[] asArray = { int.Parse(fileI2.Value) };
            int packageId;
            packageDictionary.TryGetValue(PackageDropDownF.SelectedValue, out packageId);

            try 
            { 
                Controller.AddToPackage(asArray, packageId);
                hideRightPanels();
                fixSource(true);
            }
            catch (NotLoggedInException)
            {
                messageBox("An error has occured, please log in again.");
                Response.Redirect("LogInForm.aspx");
            }
            catch (InadequateObjectException)
            {
                messageBox("An error has occured, try reloading the page.");
            }
            catch (ObjectNotFoundException)
            {
                messageBox("An error has occured, try reloading the page.");
            }
            catch (InsufficientRightsException)
            {
                messageBox("You do not have the rights to add this file to a package, as it is only shared with you to view it.");
            }
            
        }
    }
}