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
        

        
    }
}
