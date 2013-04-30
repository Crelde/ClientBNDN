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

                try { 
                    Controller.LogIn(email, password);
                    Response.Redirect("Form.aspx"); 
                }
                catch (NotLoggedOutException)
                {
                    Controller.LogOut();
                    //Shouldnt happen, but if it does, do something like reload page after this #Crelde
                }
                catch (NoSuchUserException)
                {
                    // Tell the user that the username doesnt exist. #Crelde
                }
                catch (IncorrectPasswordException)
                {
                    // Tell the user that the username and the password doesnt match. #Crelde
                }

                
                
            }
        }

    }
}