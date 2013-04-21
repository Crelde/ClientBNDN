using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void LogInButton_Click(object sender, EventArgs e)
        {
            using (var serv = new ServiceReference1.ServiceClient())
            {
                
                string email = EmailTextBox.Text;
                string password = PasswordTextBox.Text;
                
               
                // Make this ---v
                // Controller.login(email, password);
                
                Response.Redirect("Form.aspx"); // If accepted user
            }
        }

    }
}