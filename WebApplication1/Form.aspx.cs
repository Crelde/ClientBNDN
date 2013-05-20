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
        //bool updateTime = true;
        protected void Page_Load(object sender, EventArgs e)
        {
                

            // Here is a good place to add which packages should be able to be seen to the dropdown.
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
                        //DropDownList1.Items.Add("");
                        //DropDownList1.Items.Add("");

                     
                }

                /*
                
                {
                    Somebody get a list of all packages the user is supposed to see, and make it into a list, so my foreach works :)
                     
                    List<ServiceReference1.Package> l = new List<ServiceReference1.Package>(YOUR LIST);
                    
                    */


                    InteractivePanelFiles.CssClass = "rightCol";
                    InteractivePanelOther.CssClass ="rightCol";
                    
            }
                

            //}

        }
        protected void fixSource()
        {
           // var fi = Controller.GetOwnedFileInfosByEmail(Controller._sessionUser.Email);
            ServiceReference1.Package p = Controller.GetPackageById(int.Parse(""+DropDownList1.SelectedValue));
            List<ServiceReference1.FileInfo> myList = new List<ServiceReference1.FileInfo>();

            foreach (int s in p.FileIds)
            {
                myList.Add(Controller.GetFileInfoById(s));
            }




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

            DataList1.DataSource = myList;
            DataList1.DataBind();
        }

        protected void btn_command(object sender, CommandEventArgs e)
        {
            string idOfFile = e.CommandArgument.ToString();
            if (e.CommandName == "download")
            {
                int id = int.Parse(idOfFile);
                byte[] file;
                try 
                { 
                    file = Controller.DownloadFileById(id);
                    /*
                    File.
                    //test
                    Response.BinaryWrite(file);
                    Response.Flush();
                    

                    File.wr
                    File.WriteAllBytes("mor.jpg", file);
                    */
                }
                catch (NotLoggedInException)
                {
                    // Shouldn't ever happen, but if it does, do a popup and send back to login screen. #Crelde
                }
                catch (ObjectNotFoundException)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "ErrorAlert", "alert('An error has occured, try reloading the page.');", true);
                }

                // Call downloadfilebyID
                string s = "download was pressed";
            }
            else if (e.CommandName == "delete")
            {
                // DeletefilebyID
                string s1 = "delete was pressed";

            }
        }

        protected void ChangeUserButton_Click(object sender, EventArgs e)
        {
            try { Controller.LogOut(); }
            catch (NotLoggedInException)
            {
                // Shouldn't ever happen, but if it does, do a popup and send back to login screen. #Crelde
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
                // Shouldn't ever happen, but if it does, do a popup and send back to login screen. #Crelde
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
                        // #Kewin
                    }
                    catch (InadequateObjectException)
                    {
                        // Tell the user he has to selece a file from file system # Kewin
                    }
                    //Check if everything is fine, and upload the file.
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
            catch (NotLoggedInException) { } // Shouldn't happen, but doesn't really matter, 
            // as the result will still be that the user should be redirected to the logIn screen.
            finally { Response.Redirect("LogInForm.aspx"); } //Redirect to login screen, is it done correct? #Crelde
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
                // Shouldn't ever happen, but if it does, do a popup and send back to login screen. #Crelde
            }
            catch (InsufficientRightsException)
            {
                // Shouldn't ever happen as the button shouldn't be visible to a user without rights.  
            }
            catch (InadequateObjectException)
            {
                // User has not given enough information, explain to the user that both email and password are required. #Crelde
            }
            catch (KeyOccupiedException)
            {
                // The email that was given is already in use on the server, explain this to the user #Crelde
            }
        }

        protected void GetUserByEmail_Click(object sender, EventArgs e)
        {
            string email = null; // Set this via field #Crelde
            ServiceReference1.User user = null;
            try { user = Controller.GetUserByEmail(email); }
            catch (NotLoggedInException)
            {
                // Shouldn't ever happen, but if it does, do a popup and send back to login screen. #Crelde
            }
            catch (InsufficientRightsException)
            {
                // Shouldn't ever happen as the button shouldn't be visible to a user without rights.  
            }
            catch (ObjectNotFoundException)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ErrorAlert", "alert('A user does not exist with that e-mail.');", true);
                // A user does not exist with that email, communicate to user.
            }

        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            fixSource();
        }



    }
}