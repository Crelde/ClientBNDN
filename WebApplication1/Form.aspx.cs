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
        static Dictionary<string, int> packageDictionary = new Dictionary<string, int>();
        static ServiceReference1.Package[] packageList;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                using (var serv = new ServiceReference1.ServiceClient())
                {
                    fixSource(true);
                activeuserLabel.Text = Controller._sessionUser.Email;
                    InteractivePanelFiles.CssClass = "rightCol";
                    InteractivePanelOther.CssClass = "rightCol";
                    TagPanel.CssClass = "rightCol";
                    addFiletoPackagePanel.CssClass = "rightCol";
                    passwordPanel.CssClass = "rightCol";
                    deletePackagePanel.CssClass = "rightCol";
                    sharePackagePanel.CssClass = "rightCol";
                    editFilePanel.CssClass = "rightCol";
                    InteractivePanelAdmin.CssClass = "rightCol";
                    editFilePanel.Style["width"] = "350px";
                    TagPanel.Style["width"] = "350px";
                    addFiletoPackagePanel.Style["width"] = "350px";
                    newDesc.Text = Description.Value;
                }
            }
        }

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
                ServiceReference1.FileInfo fi = Controller.GetFileInfoById(i);
                if (fi!=null)
                    myList.Add(fi);
            }
            DataList1.DataSource = myList;
            DataList1.DataBind();

                       
        }

        protected void btn_command(object sender, CommandEventArgs e)
        {
            string[] commandarg = e.CommandArgument.ToString().Split(";".ToCharArray());
            string ids = commandarg[0];

            string filename = commandarg[1];
            int id = int.Parse(ids);

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
            else if (e.CommandName == "edit")
            {
                fileI2.Value = id.ToString();
                string desc = commandarg[2];
                newDesc.Text = desc;
                editFilePanel.Visible = true;
            }

            else if (e.CommandName == "delete")
            {
                try { Controller.DeleteFileById(id); }
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
            else if (e.CommandName == "tag")
            {
                updateBulletList(id);
                hideRightPanels();
                TagPanel.Visible = true;
            }
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

            }
            
        }

        protected void ChangeUserButton_Click(object sender, EventArgs e)
        {
            try { Controller.LogOut(); }
            catch (NotLoggedInException)
            {
                messageBox("An error has occured, please log in again.");
            }
            finally { Response.Redirect("LogInForm.aspx"); }
            
        }

        protected void CreatePackageButton_Click(object sender, EventArgs e)
        {
            ServiceReference1.Package package = new ServiceReference1.Package();
            package.Name = null; // Set name of new package 
            package.FileIds = new int[] { 0, 0 }; // Which files are to be put in new package? These files have to be present on the server, og man skal kun kunne vælge dem man har edit rights til, vel? #Crelde
            package.Description = null; // Set description of package.

            try { Controller.CreatePackage(package); } // This function also returns the ID of the new package created, do u need that info? #Crelde #Kewin
            catch (NotLoggedInException)
            {
                messageBox("An error has occured, please log in again.");
                Response.Redirect("LogInForm.aspx");
            }
            catch (InadequateObjectException)
            {
                /* The package does not meet at least one of the following criteria:
                 * The name is not set
                 * The name is not at least 3 characters
                 * The FileIds array is empty
                 * One of the specified FileIds doesnt exist on the server.
                 */
                // Communicate to the user what is required, and lead him back to the "create package screen" #Crelde
            }
        }

        protected void finalUpload_Click(object sender, EventArgs e)
        {
            using (var serv = new ServiceReference1.ServiceClient())
            {

                //make new file from the file coming from this bytearray!
                if (FileUpload1.FileName != "")
                {
                    byte[] filebytes = FileUpload1.FileBytes;
                    string desc = Description.Value;
                    string filename = FileUpload1.FileName;
                    string origin = Origin.Value;  
                
                    ServiceReference1.FileTransfer filetrans = new ServiceReference1.FileTransfer();

                    filetrans.Data = filebytes;
                    
                    ServiceReference1.FileInfo fileinf = new ServiceReference1.FileInfo();

                    fileinf.Date = DateTime.Now;
                    fileinf.Description = desc;
                    fileinf.Name = FileUpload1.FileName;
                    fileinf.Origin = origin;
                    filetrans.Info = fileinf;

                    try { Controller.UploadFile(filetrans); }
                    catch (NotLoggedInException)
                    {
                        messageBox("An error has occured, please log in again.");
                        Response.Redirect("LogInForm.aspx");
                    }
                    catch (InadequateObjectException)
                    {
                        messageBox("You have to select a file.");
                    }
                }
            }

        }

        protected void adminButton_Click(object sender, EventArgs e)
        {
            hideRightPanels();
            hideMidPanels();
            InteractivePanelAdmin.Visible = true;
        }

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
                messageBox("An error has occured, try reloading the page.");
            }
            updateBulletList(idofchosenfile);
            CreateBox.Text = "";
        }

        protected void DeleteTag_Click(object sender, EventArgs e)
        {
            int idofchosenfile = int.Parse(fileI.Value);
            string tagtext = CreateBox.Text;

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
                messageBox("An error has occured, try reloading the page.");
            }
            updateBulletList(idofchosenfile);
            DeleteBox.Text = "";
        }

        protected void BulletedList1_Click(object sender, BulletedListEventArgs e)
        {
            DeleteBox.Text = BulletedList1.Items[e.Index].ToString();
        }

        protected void createPackage_Click(object sender, EventArgs e)
        {
            hideMidPanels();
            hideRightPanels();
            InteractivePanelOther.Visible = true;
        }

        

        protected void submitpackage_Click(object sender, EventArgs e)
        {
            string pName = packageName.Text;

            ServiceReference1.Package package = new ServiceReference1.Package();

            package.Name = pName;
            hideRightPanels();
            hideMidPanels();

            try { Controller.CreatePackage(package); }
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

            fixSource(true, DropDownList1.Items.Count - 3);
            InteractivePanelFiles.Visible = true;
        }

        protected void cancelcreatepacakge_Click(object sender, EventArgs e)
        {
            hideRightPanels();
            hideMidPanels();
            InteractivePanelFiles.Visible = true;
        }
        protected void hideRightPanels()
        {
            editFilePanel.Visible = false;
            TagPanel.Visible = false;
            passwordPanel.Visible = false;
            addFiletoPackagePanel.Visible = false;
        }
        protected void hideMidPanels()
        {
            InteractivePanelOther.Visible = false;
            InteractivePanelFiles.Visible = false;
            InteractivePanelAdmin.Visible = false;
            deletePackagePanel.Visible = false;
            sharePackagePanel.Visible = false;

        }
        protected void changepw_Click(object sender, EventArgs e)
        {
            hideRightPanels();
            hideMidPanels();
            passwordPanel.Visible = true;
        }

        protected void DeletePackage_Click(object sender, EventArgs e)
        {
            string s = DropDownList1.SelectedValue;
            // Kewin do you stuff, s is the name of the package to be deleted.
            hideMidPanels();
            hideRightPanels();
            deletePackagePanel.Visible = true;
        }
        protected void SharePackage_Click(object sender, EventArgs e)
        {
            hideMidPanels();
            hideRightPanels();
            sharePackagePanel.Visible = true;
        }
        protected void SharePackageBut_Click(object sender, EventArgs e)
        {
            string s = emailToShareWith.Text;
            
            ServiceReference1.Right right = new ServiceReference1.Right();

            int packageId; 
            packageDictionary.TryGetValue(DropDownList1.SelectedValue, out packageId);
            right.ItemId = packageId;
            right.UserEmail = Controller._sessionUser.Email;

            try 
            { 
                Controller.GrantRight(right);
                hideMidPanels();
                hideRightPanels();
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
            catch (ObjectNotFoundException)
            {
                messageBox("An error has occured, please try reloading the page.");
            }
        }
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
        protected void cancelAndReturnToFiles(object sender, EventArgs e)
        {
            hideMidPanels();
            hideRightPanels();
            InteractivePanelFiles.Visible = true;
        }
        // This is the method that edits the file
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
                cancelAndReturnToFiles(sender, e);
                fixSource(true, prevIndex);
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
                messageBox("An error has occured, try reloading the page.");
            }
            catch (OriginalNotFoundException)
            {
                messageBox("An error has occured, try reloading the page.");
            }
        }

        // THIS IS NOT YET DONE FRONT END
        protected void createUserSubmit_Click(object sender, EventArgs e)
        {
            ServiceReference1.User user = new ServiceReference1.User(); // Set this via fields #Crelde
            try
            {
                Controller.CreateUser(user);
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

        protected void GetUserByEmail_Click(object sender, EventArgs e)
        {
            string email = null; // Set this via field #Crelde
            ServiceReference1.User user = null;
            try { user = Controller.GetUserByEmail(email); }
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
                messageBox("A user does not exist with that e-mail.");
            }

        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            fixSource();
        }

        private void messageBox(string message)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ErrorAlert", "alert('"+message+"');", true);
        }

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

        protected void addfiletoP_Click(object sender, EventArgs e)
        {
            int[] asArray = { int.Parse(fileI2.Value) };
            int packageId;
            packageDictionary.TryGetValue(PackageDropDownF.SelectedValue, out packageId);

            try { Controller.AddToPackage(asArray, packageId); }
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
                messageBox("An error has occured, try reloading the page.");
            }
            hideRightPanels();
            fixSource(true);
        }
    }
}