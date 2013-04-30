using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

namespace WebApplication1
{
    public partial class Form : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {


            // Here is a good place to add which packages should be able to be seen to the dropdown.
            if (!IsPostBack)
            {
                DropDownList1.Items.Add("Crelde");
                DropDownList1.Items.Add("Er");
                DropDownList1.Items.Add("Sej");

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
            package.FileIds = new int[] {0, 0}; // Which files are to be put in new package? These files have to be present on the server, og man skal kun kunne vælge dem man har edit rights til, vel? #Crelde
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

        protected void UploadFileButton_Click(object sender, EventArgs e)
        {
            using (var serv = new ServiceReference1.ServiceClient())
            {
                //make new file from the file coming from this bytearray!
                byte[] filebytes = FileUpload1.FileBytes ;
                if (FileUpload1.FileName != "")
                {
                    
                }
            }

        }

        protected void SubmitASP_Click(object sender, EventArgs e)
        {
            FancyNameShow.Text = FancyName.Value;
            FancyEmailShow.Text = FancyEmail.Value;
        }
       

    }

}