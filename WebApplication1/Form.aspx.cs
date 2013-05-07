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
                /*
                using (var serv = new ServiceReference1.ServiceClient())
                {
                    Somebody get a list of all packages the user is supposed to see, and make it into a list, so my foreach works :)
                     
                    List<ServiceReference1.Package> l = new List<ServiceReference1.Package>(YOUR LIST);
                    foreach (ServiceReference1.Package p in l)
                    {
                        DropDownList1.Items.Add(p.Name);
                    }
                    */
                    DropDownList1.Items.Add("Crelde");
                    DropDownList1.Items.Add("Er");
                    DropDownList1.Items.Add("Sej");
                    fixSource();
                }

           //}

        }
        protected void fixSource()
        {
            //var fi = Controller.GetOwnedFileInfosByEmail("test@test.com");
            List<ServiceReference1.FileInfo> myList = new List<ServiceReference1.FileInfo>();

            ServiceReference1.FileInfo f = new ServiceReference1.FileInfo();
            f.Description = "test fileinfo";
            f.Name = "Name of f";
            f.Id = 666;
            f.OwnerEmail = "crelde@crelde.crelde";
            f.Type = ServiceReference1.FileType.text;
            f.Date = DateTime.Now;
            myList.Add(f);

            ServiceReference1.FileInfo f1 = new ServiceReference1.FileInfo();
            f1.Description = "test fileinfo222";
            f1.Name = "Name of f222";
            f1.Id = 667;
            f1.OwnerEmail = "crelde@crelde.crelde222";
            f1.Type = ServiceReference1.FileType.text;
            f1.Date = DateTime.Now;
            myList.Add(f1);


            ServiceReference1.FileInfo f2 = new ServiceReference1.FileInfo();
            f2.Description = "Text about tequila";
            f2.Name = "TEQUILA";
            f2.Id = 668;
            f2.OwnerEmail = "crelde@drunk.com";
            f2.Type = ServiceReference1.FileType.text;
            f2.Date = DateTime.Now;
            myList.Add(f2);

                DataList1.DataSource = myList;
                DataList1.DataBind();
        }

        protected void btn_command(object sender, CommandEventArgs e)
        {
            string idOfFile = e.CommandArgument.ToString();
            if (e.CommandName == "download")
            {
                // Call downloadfilebyID
                string s = "download was pressed";
            }
            else if (e.CommandName == "delete")
            {
                // DeletefilebyID
                string s1 = "delete was pressed";

            }
        }

        protected void delete(object sender, EventArgs e)
        {

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

        protected void SubmitASP_Click(object sender, EventArgs e)
        {
           
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
                    string filename = FileName.Value;
                    string email = Email.Value;

                    //Check if everything is fine, and upload the file.
                }
            }

        }
       

    }

}