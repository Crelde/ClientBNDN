using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace WebApplication1
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try { Controller.LogOut(); }
            catch (NotLoggedInException) { } // Expected. // test

        
        }

        protected void LogInButton_Click(object sender, EventArgs e)
        {
            using (var serv = new ServiceReference1.ServiceClient())
            {
                
                string email = EmailTextBox.Text;
                string password = PasswordTextBox.Text;

                try { 
                    Controller.LogIn(email, password);
                    Response.Redirect("Form.aspx"); 
                }
                catch (NotLoggedOutException)
                {
                    Response.Redirect("LogInForm.aspx");
                     
                }
                catch (NoSuchUserException)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "ErrorAlert", "alert('No user exists with that email.');", true);
                }
                catch (IncorrectPasswordException)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "ErrorAlert", "alert('The password does not match the email.');", true);
                }

                
                
            }
        }

    }
}