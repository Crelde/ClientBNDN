using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class Form : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {


            // Here is a good place to add which packages should be able to be seen to the dropdown.
            DropDownList1.Items.Add("Crelde");
            DropDownList1.Items.Add("Er");
            DropDownList1.Items.Add("Sej");

            ListViewDataItem item = new ListViewDataItem(1,1);

            BulletedList1.Items.Add("Crelde");
            BulletedList1.Items.Add("Er");
            BulletedList1.Items.Add("Stadig");
            BulletedList1.Items.Add("Sej");

        }

        

        protected void ChangeUserButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("LogInForm.aspx"); 
        }

        protected void CreatePackageButton_Click(object sender, EventArgs e)
        {
            using (var serv = new ServiceReference1.ServiceClient())
            {
                ServiceReference1.Package pack = new ServiceReference1.Package();
                // Add stuff needed to package and create it
                //serv.CreatePackage(pack);
            }
        }

        protected void UploadFileButton_Click(object sender, EventArgs e)
        {
            using (var serv = new ServiceReference1.ServiceClient())
            {
                //make new file from the file coming from this bytearray!
                byte[] filebytes = FileUpload1.FileBytes ;
            }

        }


    }
}