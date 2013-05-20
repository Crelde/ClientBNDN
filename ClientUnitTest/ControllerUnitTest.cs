using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.QualityTools.Testing.Fakes;
using WebApplication1.ServiceReference1.Fakes;
using WebApplication1.ServiceReference1;
using WebApplication1;
using System.Diagnostics;


namespace ClientUnitTest
{
    [TestClass]
    public class ControllerUnitTest
    {
        [TestInitialize]
        public void beforeEach()
        {
            // Inserting blank methods instead of connecting to service
            ShimsContext.Create();
            ShimServiceClient.Constructor = (a) => { };
            System.ServiceModel.Fakes.
                ShimClientBase<WebApplication1.ServiceReference1.IService>.
                AllInstances.Close = (a) => { };
        }

        [TestCleanup]
        public void afterEach()
        {
            // Making sure that the controller is reset after each test
            try
            {
                Controller.LogOut();
            }
            catch (Exception)
            {
                //no user was logged in
            }
        }

        [TestMethod]
        public void LogInTest()
        {
            User testuser = new User();

            testuser.Email = "a@b.com";
            testuser.Password = "123";
            testuser.Type = UserType.standard;

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => testuser;

                Controller.LogIn("a@b.com", "123");

            }
        }

        [TestMethod]
        public void LogInIncorrectPassTest()
        {
            User testuser = new User();

            testuser.Email = "a@b.com";
            testuser.Password = "123";
            testuser.Type = UserType.standard;

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => testuser;

                try
                {
                    Controller.LogIn("a@b.com", "");
                }
                catch (IncorrectPasswordException)
                {
                    // All is good
                }

            }
        }

        [TestMethod]
        public void LogInIncorrectPassTest2()
        {
            User testuser = new User();

            testuser.Email = "a@b.com";
            testuser.Password = "123";
            testuser.Type = UserType.standard;

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => testuser;

                try
                {
                    Controller.LogIn("a@b.com", "1234");
                }
                catch (IncorrectPasswordException)
                {
                    // All is good
                }

            }
        }

        [TestMethod]
        public void LogInIncorrectFormatTest()
        {
            User testuser = new User();

            testuser.Email = "a@b.com";
            testuser.Password = "123";
            testuser.Type = UserType.standard;

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => testuser;

                try
                {
                    Controller.LogIn("abcom", "123");
                }
                catch (NoSuchUserException)
                {
                    // All is good
                }

            }
        }

        [TestMethod]
        public void LogInAndOutTest()
        {
            User testuser = new User();

            testuser.Email = "a@b.com";
            testuser.Password = "123";
            testuser.Type = UserType.standard;

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => testuser;

                Controller.LogIn("a@b.com", "123");

            }

            Controller.LogOut();
        }        


        //check if admin can make new users
        [TestMethod]
        public void CreateUserTest()
        {
            User testuserA = new User();
            testuserA.Email = "c@b.com";
            testuserA.Password = "101";
            testuserA.Type = UserType.admin;

            User testuserS = new User();
            testuserS.Email = "x@s.com";
            testuserS.Password = "ps1";
            testuserS.Type = UserType.standard;

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => testuserA;
                ShimServiceClient.AllInstances.CreateUserUser = (a, b) => { };

                Controller.LogIn("c@b.com", "101");
                Controller.CreateUser(testuserS);
            }

            //  Controller.LogOut(); <- No need to log out after each test, it's done automatically :)
        }

        //check if regular users are denied making new users
        [TestMethod]
        public void StandardCreateUser()
        {
            User testuserS1 = new User();
            User testuser = new User();

            testuserS1.Email = "k@b.com";
            testuserS1.Password = "201";
            testuserS1.Type = UserType.standard;

            testuser.Email = "p@s.com";
            testuser.Password = "199";
            testuser.Type = UserType.standard;

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => testuserS1;

                Controller.LogIn("k@b.com", "201");

                try
                {
                    Controller.CreateUser(testuser); //this should fail

                }
                catch (InsufficientRightsException)
                {
                    //User not created. All is good.
                }
            }

            //  Controller.LogOut(); <- No need to log out after each test, it's done automatically :)
        }

        // check if admin can delete users
        [TestMethod]
        public void deletingUser()
        {
            User testAdmin = new User();
            testAdmin.Email = "ta@ta.com";
            testAdmin.Password = "ta123";
            testAdmin.Type = UserType.admin;

            User testuser = new User();
            testuser.Email = "p@s.com";
            testuser.Password = "199";
            testuser.Type = UserType.standard;

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) =>
                {
                    if (b.Equals(testAdmin.Email)) { return testAdmin; } else { return testuser; }
                };
                ShimServiceClient.AllInstances.CreateUserUser = (a, b) => { };
                ShimServiceClient.AllInstances.DeleteUserByEmailString = (a, b) => { };
                Controller.LogIn("ta@ta.com", "ta123");

                Controller.CreateUser(testuser);
                
                Controller.DeleteUserByEmail("p@s.com");
            }
        }

        //check to see if authorization fails after you log out
        [TestMethod]
        public void authorizationFail()
        {
            User testAdmin = new User();
            testAdmin.Email = "ta@ta.com";
            testAdmin.Password = "ta123";
            testAdmin.Type = UserType.admin;
            Item testItem = new Item();
            testItem.Id = 011;
            testItem.Name = "test";
            testItem.OwnerEmail = "ta@ta.com";

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => testAdmin;
                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 = (a, b) => { return new FileInfo(); };
                Controller.LogIn("ta@ta.com", "ta123");
                Controller.LogOut();
                try
                {
                    Controller.DownloadFileById(011);
                }
                catch (NotLoggedInException)
                {
                    // everything is ok!
                }

            }
        }

        //check if other accounts are denied access to edit stranger accounts
        [TestMethod]
        public void authorizationFail2()
        {
            User test1 = new User();
            test1.Email = "st@st.com";
            test1.Password = "test1";
            test1.Type = UserType.standard;

            User test2 = new User();
            test2.Email = "t@t.com";
            test2.Password = "test2";
            test2.Type = UserType.standard;

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => test1;
                ShimServiceClient.AllInstances.UpdateUserUser = (a, b) => { };
                Controller.LogIn("st@st.com", "test1");
                try
                {
                    Controller.UpdateUser(test2);
                }
                catch (InsufficientRightsException)
                {
                    //Everything is good.
                }
            }
        }

        //check to see if logging in keeps you logged in
        [TestMethod]
        public void sessioning1()
        {
            User test = new User();
            test.Email = "au@au.com";
            test.Password = "test";
            test.Type = UserType.admin;
            Package p = new Package();
            p.Id = 1001;
            p.Name = "p1001";
            p.OwnerEmail = "au@au.com";
            p.FileIds = new int[] {092};
            FileInfo fi = new FileInfo();
            fi.Id = 092;
            fi.Name = "testItem";
            fi.OwnerEmail = "au@au.com";
            fi.Type = FileType.text;
            FileInfo fiu = new FileInfo();
            fiu.Id = 092;
            fiu.Name = "UpdatedtestItem";
            fiu.OwnerEmail = "au@au.com";
            fiu.Type = FileType.text;
            Right r = new Right();
            r.ItemId = 092;
            r.Type = RightType.edit;
            r.Until = DateTime.Now.AddDays(1);
            r.UserEmail = "au@au.com";
                 
          
            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => test;
                ShimServiceClient.AllInstances.CreatePackagePackage = (a, b) => {return 1;};
                ShimServiceClient.AllInstances.GetPackageByIdInt32 = (a, b) => {return p;}; 
                ShimServiceClient.AllInstances.AddToPackageInt32ArrayInt32 = (a, b, c) => {}; 
                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 = (a, b) => {return fi;};
                ShimServiceClient.AllInstances.AddTagStringInt32 = (a, b, c) => { };
                ShimServiceClient.AllInstances.DeleteFileByIdInt32 = (a, b) => { };
                ShimServiceClient.AllInstances.GetRightStringInt32 = (a, b, c) => {return r;};
                ShimServiceClient.AllInstances.UpdateFileInfoFileInfo = (a, b) => { };

                Controller.LogIn("au@au.com", "test");
                Controller.CreatePackage(p);
                Controller.AddToPackage(new int[]{092},1001);              
                Controller.AddTag("tag1", 092);
                Controller.DeleteFileById(092);
                try
                {
                    Controller.UpdateFileInfo(fiu);
                } catch(OriginalNotFoundException){
                    //This should be thrown
                }
            }
        }
    }
}
