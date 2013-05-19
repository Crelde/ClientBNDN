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
                // oh well, no user wasn't logged in
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
                ShimServiceClient.AllInstances.GetUserByEmailString = (a,b) => testuser;

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
                ShimServiceClient.AllInstances.CreateUserUser = (a, b) => { };
                
                Controller.LogIn("c@b.com","101");
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
                
                } catch(Exception e)
                {
                    //User not created. All is good.
                }
            }

          //  Controller.LogOut(); <- No need to log out after each test, it's done automatically :)
        }


        [TestMethod]
        public void AddTagNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 = (a, b) => new FileInfo();
                ShimServiceClient.AllInstances.GetPackageByIdInt32 = (a, b) => new Package();
                ShimServiceClient.AllInstances.GetRightStringInt32= (a, b, c) => new Right();

                try
                {
                    Controller.AddTag("", 1);
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void AddToPackageNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 = (a, b) => new FileInfo();
                ShimServiceClient.AllInstances.GetPackageByIdInt32 = (a, b) => new Package();
                ShimServiceClient.AllInstances.GetRightStringInt32 = (a, b, c) => new Right();
                ShimServiceClient.AllInstances.GetOwnedPackagesByEmailString = (a, b) => new Package[1];

                try
                {
                    Controller.AddToPackage(new int[]{1}, 1);
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void CreatePackageNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {
               
                try
                {
                    Controller.CreatePackage(new Package());
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void CreateUserNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.CreateUser(new User());
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void DeleteFileByIdNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.DeleteFileById(1);
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void DeletePackageByIdNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.DeletePackageById(1);
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void DeleteUserByEmailNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.DeleteUserByEmail("");
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void DownloadFileByIdNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.DownloadFileById(1);
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void DropRightNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.DropRight("", 1);
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void DropTagNotLoggedInTest()
        {
            using (ShimsContext.Create())   
            {
                
                try
                {
                    Controller.DropTag("", 1);
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void GetFileInfoByIdNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.GetFileInfoById(1);
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void GetFileInfosByTagNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.GetFileInfosByTag("");
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void GetOwnedFileInfosByEmailNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.GetOwnedFileInfosByEmail("");
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void GetOwnedPackagesByEmailNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.GetOwnedPackagesByEmail("");
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void GetPackageByIdNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.GetPackageById(1);
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void GetPackagesByTagNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.GetPackagesByTag("");
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void GetTagsByItemIdNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.GetTagsByItemId(1);
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void GetUserByEmailNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.GetUserByEmail("");
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void GrantRightNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.GrantRight(new Right());
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void HasEditRightsNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.HasEditRights(1);
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void HasViewRightsNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.HasViewRights(1);
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void IsOwnerOfNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.IsOwnerOf(1);
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void RemoveFromPackageNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.RemoveFromPackage(new int[] { 1 }, 1);
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void SearchFileInfosNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.SearchFileInfos("");
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void SearchPackagesNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.SearchPackages("");
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void UpdateFileDataNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.UpdateFileData(new byte[]{1}, 1);
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void UpdateFileInfoNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.UpdateFileInfo(new FileInfo());
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void UpdateRightNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.UpdateRight(new Right());
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void UpdateUserNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.UpdateUser(new User());
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void UploadFileNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {

                try
                {
                    Controller.UploadFile(new FileTransfer());
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }
     }
}
