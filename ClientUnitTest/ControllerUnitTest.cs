using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.QualityTools.Testing.Fakes;
using WebApplication1.ServiceReference1.Fakes;
using WebApplication1.ServiceReference1;
using WebApplication1;
using System.Diagnostics;


namespace ClientUnitTest
{
    public class DummyDisposable : System.IDisposable
    {
        public void Dispose() { }
    }


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
        public void LoginTest()
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
        public void LogOutTest()
        {
            try
            {
                Controller.LogOut(); // this should fail
            }
            catch (NotLoggedInException)
            {
                // All is good
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
                catch (Exception e)
                {
                    //User not created. All is good.
                }
            }

            //  Controller.LogOut(); <- No need to log out after each test, it's done automatically :)
        }

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

                try
                {
                    Controller.CreateUser(testuser);
                }
                catch (Exception e)
                {
                    //User not created. Something is wrong
                }

                Controller.DeleteUserByEmail("p@s.com");

            }

        }


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
                catch (NotLoggedInException e)
                {
                    // everything is ok!
                }

            }
        }

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
                catch (InsufficientRightsException e)
                {
                    //Everything is good.
                }
            }
        }

        [TestMethod]
        public void ()
        {
        }
    }
}
