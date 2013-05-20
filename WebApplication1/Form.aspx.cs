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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                using (var serv = new ServiceReference1.ServiceClient())
                {
                    ServiceReference1.Package[] l = Controller.kewinhalp();
                    foreach (ServiceReference1.Package p in l)
                    {
                        DropDownList1.Items.Add(p.Id.ToString());
                    }
                    fixSource();
                }
                    InteractivePanelFiles.CssClass = "rightCol";
                    InteractivePanelOther.CssClass ="rightCol";         
            }
        }

        protected void fixSource()
        {
            ServiceReference1.Package p = Controller.GetPackageById(int.Parse(""+DropDownList1.SelectedValue));
            List<ServiceReference1.FileInfo> myList = new List<ServiceReference1.FileInfo>();

            foreach (int s in p.FileIds)
            {
                myList.Add(Controller.GetFileInfoById(s));
            }


            /*

            //ServiceReference1.FileInfo f = new ServiceReference1.FileInfo();
            //f.Description = "test fileinfo";
            //f.Name = "Name of f";
            //f.Id = 666;
            //f.OwnerEmail = "crelde@crelde.crelde";
            //f.Type = ServiceReference1.FileType.text;
            //f.Date = DateTime.Now;
            //myList.Add(f);

            //ServiceReference1.FileInfo f1 = new ServiceReference1.FileInfo();
            //f1.Description = "test fileinfo222";
            //f1.Name = "Name of f222";
            //f1.Id = 667;
            //f1.OwnerEmail = "crelde@crelde.crelde222";
            //f1.Type = ServiceReference1.FileType.text;
            //f1.Date = DateTime.Now;
            //myList.Add(f1);


            //ServiceReference1.FileInfo f2 = new ServiceReference1.FileInfo();
            //f2.Description = "Text about tequila";
            //f2.Name = "TEQUILA";
            //f2.Id = 668;
            //f2.OwnerEmail = "crelde@drunk.com";
            //f2.Type = ServiceReference1.FileType.text;
            //f2.Date = DateTime.Now;
            //myList.Add(f2);
            */
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
        }

        protected void ChangeUserButton_Click(object sender, EventArgs e)
        {
            try { Controller.LogOut(); }
            catch (NotLoggedInException)
            {
                messageBox("An error has occured, please log in again.");
                Response.Redirect("LogInForm.aspx");
            }
            Response.Redirect("LogInForm.aspx");
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

        protected void SubmitASP_Click(object sender, EventArgs e)
        {

        }
        
        protected void test_click(object sender, EventArgs e)
        {
            if (InteractivePanelFiles.Visible == true)
            {
                InteractivePanelFiles.Visible = false;
                InteractivePanelOther.Visible = true;
            }
            else
            {
                InteractivePanelFiles.Visible = true;
                InteractivePanelOther.Visible = false;
                
            }
            //Response.Write(@"<script language='javascript'>alert('error');</script>");
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "ErrorAlert", "alert('Some text here - maybe ex.Message');", true);
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
            InteractivePanelFiles.Visible = false;
            InteractivePanelOther.Visible = false;
            InteractivePanelAdmin.Visible = true;
        }

        protected void logOut_Click(object sender, EventArgs e)
        {
            try { Controller.LogOut(); }
            catch (NotLoggedInException) { } 
            finally { Response.Redirect("LogInForm.aspx"); } 
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
    }
}