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
                //activeuserLabel.Text = Controller._sessionUser.Email;
                    DropDownList1.Items.Add("Crelde");
                    DropDownList1.Items.Add("Er");
                    DropDownList1.Items.Add("Sej");

                    fixSource();
                    BulletedList1.Items.Add(new ListItem("hello"));
                    InteractivePanelFiles.CssClass = "rightCol";
                    InteractivePanelOther.CssClass ="rightCol";
                    TagPanel.CssClass = "rightCol";
                    addFiletoPackagePanel.CssClass = "rightCol";
                    passwordPanel.CssClass = "rightCol";
                    deletePackagePanel.CssClass = "rightCol";
                    sharePackagePanel.CssClass = "rightCol";
                    TagPanel.Style["width"] = "350px";
                    //TagPanel.Style["margin-left"] ="1001px";
                    
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
            string[] commandarg = e.CommandArgument.ToString().Split(";".ToCharArray());
            string ids = commandarg[0];

            string filename = commandarg[1];
            int id = int.Parse(ids);

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
            else if (e.CommandName == "tag")
            {
                // foreach tag on file{
                //BulletedList1.Items.Add(new ListItem(""));
                // }
                hideRightPanels();
                TagPanel.Visible = true;
                fileI.Value = id.ToString();
                fileN.Value = filename;
            }
            else if (e.CommandName == "addToPackage")
            {
                hideRightPanels();
                addFiletoPackagePanel.Visible = true;
                fileI2.Value = id.ToString();
                
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

        protected void finalUpload_Click(object sender, EventArgs e)
        {
            using (var serv = new ServiceReference1.ServiceClient())
            {

                //make new file from the file coming from this bytearray!
                if (FileUpload1.FileName != "")
                {
                    byte[] filebytes = FileUpload1.FileBytes;
                    string desc = Description.Value;
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
            hideRightPanels();
            hideMidPanels();
            InteractivePanelAdmin.Visible = true;
        }

        // kewin do
        protected void CreateTag_Click(object sender, EventArgs e)
        {
            // id and name of files stored in hiddenfields, changes everytime tag button is pressed
            int idofchosenfile = int.Parse(fileI.Value);
            string nameofchosenfile = fileN.Value;
            //create tag by that string ---V REMEMBER TO CHECK IF EMPTY STRING 
            string tagtext = CreateBox.Text; 

        }
        // kewin do
        protected void DeleteTag_Click(object sender, EventArgs e)
        {
            // pretend ---v is tagname to be deleted REMEMBER TO CHECK IF EMPTY STRING 
            string tagtextToBeDeleted = DeleteBox.Text; 



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

            DropDownList1.SelectedIndex = DropDownList1.Items.Count - 4;
            hideRightPanels();
            hideMidPanels();
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

        protected void cancelpw_Click(object sender, EventArgs e)
        {
            hideMidPanels();
            hideRightPanels();
            InteractivePanelFiles.Visible = true;

        }

        protected void cancelTag_Click(object sender, EventArgs e)
        {
            hideMidPanels();
            hideRightPanels();
            InteractivePanelFiles.Visible = true;
        }

        protected void canceladdpack_Click(object sender, EventArgs e)
        {
            hideMidPanels();
            hideRightPanels();
            InteractivePanelFiles.Visible = true;
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

        protected void CancelShare_Click(object sender, EventArgs e)
        {
            hideMidPanels();
            hideRightPanels();
            InteractivePanelFiles.Visible = true;
        }

        protected void SharePackageBut_Click(object sender, EventArgs e)
        {
            string s = emailToShareWith.Text;
            // call the share with shit from controller here.
            hideMidPanels();
            hideRightPanels();
            InteractivePanelFiles.Visible = true;
        }

        protected void canceldelete_Click(object sender, EventArgs e)
        {
            hideMidPanels();
            hideRightPanels();
            InteractivePanelFiles.Visible = true;
        }

        protected void confirmdelete_Click(object sender, EventArgs e)
        {
            string packageToBeDeleted = DropDownList1.SelectedValue;

            hideMidPanels();
            hideRightPanels();
            InteractivePanelFiles.Visible = true;
            // MAYBE CALL FIXSOURCE TO FIX DROPDOWN NOT SHOWING THE ONE WE JUST KILLED
        }

        protected void confirmchange_Click(object sender, EventArgs e)
        {
            string oldpws = oldpw.Text;
            string newpws = newpw.Text;
            string confirmpws = confirmpw.Text;


            hideMidPanels();
            hideRightPanels();
            InteractivePanelFiles.Visible = true;
        }

        protected void canceladmin_Click(object sender, EventArgs e)
        {

            hideMidPanels();
            hideRightPanels();
            InteractivePanelFiles.Visible = true;
        }



    }

}