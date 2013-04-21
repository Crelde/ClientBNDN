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

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            using (var derp = new ServiceReference1.ServiceClient())
            {
                ServiceReference1.User myUser = derp.GetUserByEmail(TextBox1.Text);
                string type = myUser.Type.ToString();
                string pw = myUser.Password;
                string email = myUser.Email;

                GetUserTextBox.Text = "Type: " + type + "\nPassword: " + pw + "\nEmail: " + email;
            
            }
            


        }

        protected void ChangeUserButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("LogInForm.aspx"); 
        }

    }
}