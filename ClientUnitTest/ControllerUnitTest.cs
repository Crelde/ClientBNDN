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
        
        [TestMethod]
        public void LoginTest()
        {
            User testuser = new User();

            testuser.Email = "a@b.com";
            testuser.Password = "123";
            testuser.Type = UserType.standard;

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a,b) => testuser;

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
                ShimServiceClient.AllInstances.GetUserByEmailString = (a,b) => testuser;

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
                
                Controller.LogIn("c@b.com","101");
                Controller.CreateUser(testuserS);
            }

            Controller.LogOut();
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
                
                } catch(Exception e)
                {
                    //User not created. All is good.
                }
            }

            Controller.LogOut();
        }




     }
}