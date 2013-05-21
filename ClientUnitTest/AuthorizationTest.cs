using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication1.ServiceReference1.Fakes;
using WebApplication1.ServiceReference1;
using WebApplication1;
using Microsoft.QualityTools.Testing.Fakes;

namespace ClientUnitTest
{
    [TestClass]
    public class AuthorizationTest
    {
        [TestInitialize]
        public void BeforeEach()
        {
            // Inserting blank methods instead of connecting to service
            ShimsContext.Create();
            ShimServiceClient.Constructor = (a) => { };
            System.ServiceModel.Fakes.
                ShimClientBase<IService>.
                AllInstances.Close = (a) => { };
        }

        [TestCleanup]
        public void AfterEach()
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

        /**
         * Checking if admin-specific methods denies access for non-admins
         */
        [TestMethod]
        public void CreateUserNonAdminDeniedTest()
        {
            var user = new User {Email = "test@123.com", Password = "drowssap", Type = UserType.standard};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = 
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.CreateUser(new User());
                    Assert.Fail();
                }
                catch (InsufficientRightsException){}
            }
        }

        [TestMethod]
        public void GetUserByEmailNonAdminDeniedTest()
        {
            var user = new User {Email = "test@123.com", Password = "drowssap", Type = UserType.standard};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.GetUserByEmail("another@email.com");
                    Assert.Fail();
                }
                catch (InsufficientRightsException){}
            }
        }

        [TestMethod]
        public void UpdateUserNonAdminDeniedTest()
        {
            var user = new User {Email = "test@123.com", Password = "drowssap", Type = UserType.standard};
            var anotherUser = new User {Email = "another@email.com", Type = UserType.standard, Password = "second"};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.UpdateUser(anotherUser);
                    Assert.Fail();
                }
                catch (InsufficientRightsException){}
            }
        }

        [TestMethod]
        public void DeleteUserByEmailNonAdminDeniedTest()
        {
            var user = new User {Email = "test@123.com", Password = "drowssap", Type = UserType.standard};
            var anotherUser = new User {Email = "another@email.com", Type = UserType.standard, Password = "second"};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.DeleteUserByEmail(anotherUser.Email);
                    Assert.Fail();
                }
                catch (InsufficientRightsException){}
            }
        }

        [TestMethod]
        public void DownloadFileByIdNonAdminDeniedTest()
        {
            var user = new User {Email = "test@123.com", Password = "drowssap", Type = UserType.standard};
            var anotherUser = new User {Email = "another@email.com", Type = UserType.standard, Password = "second"};
            var fileInfo = new FileInfo {Id = 100, OwnerEmail = anotherUser.Email};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);
               
                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;
               
                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.DownloadFileById(100);
                    Assert.Fail();
                }
                catch (InsufficientRightsException){}
            }
        }

        [TestMethod]
        public void UpdateFileInfoNonAdminDeniedTest()
        {
            var user = new User {Email = "test@123.com", Password = "drowssap", Type = UserType.standard};
            var anotherUser = new User {Email = "another@email.com", Type = UserType.standard, Password = "second"};
            var fileInfo = new FileInfo {Id = 100, OwnerEmail = anotherUser.Email, Name = "filename"};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.UpdateFileInfo(fileInfo);
                    Assert.Fail();
                }
                catch (InsufficientRightsException){}
            }
        }

        [TestMethod]
        public void UpdateFileDataNonAdminDeniedTest()
        {
            var user = new User {Email = "test@123.com", Password = "drowssap", Type = UserType.standard};
            var anotherUser = new User {Email = "another@email.com", Type = UserType.standard, Password = "second"};
            var fileInfo = new FileInfo {Id = 100, OwnerEmail = anotherUser.Email, Name = "filename"};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.UpdateFileData(new byte[] { 1 }, fileInfo.Id);
                    Assert.Fail();
                }
                catch (InsufficientRightsException){}
            }
        }

        [TestMethod]
        public void DeleteFileByIdNonAdminDeniedTest()
        {
            var user = new User {Email = "test@123.com", Password = "drowssap", Type = UserType.standard};
            var anotherUser = new User {Email = "another@email.com", Type = UserType.standard, Password = "second"};
            var fileInfo = new FileInfo {Id = 100, OwnerEmail = anotherUser.Email, Name = "filename"};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.DeleteFileById(100);
                    Assert.Fail();
                }
                catch (InsufficientRightsException){}
            }
        }

        [TestMethod]
        public void GetOwnedFileInfosNonAdminDeniedTest()
        {
            var user = new User {Email = "test@123.com", Password = "drowssap", Type = UserType.standard};
            var anotherUser = new User {Email = "another@email.com", Type = UserType.standard, Password = "second"};
            var fileInfo = new FileInfo {Id = 100, OwnerEmail = anotherUser.Email, Name = "filename"};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetOwnedFileInfosByEmailString =
                    (a, b) => ((b.Equals(anotherUser.Email)) ? new[] {fileInfo} : null);

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.GetOwnedFileInfosByEmail(anotherUser.Email);
                    Assert.Fail();
                }
                catch (InsufficientRightsException){}
            }
        }

        [TestMethod]
        public void AddTagNonAdminDeniedTest()
        {
            var user = new User {Email = "test@123.com", Password = "drowssap", Type = UserType.standard};
            var anotherUser = new User {Email = "another@email.com", Type = UserType.standard, Password = "second"};
            var fileInfo = new FileInfo {Id = 100, OwnerEmail = anotherUser.Email, Name = "filename"};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => null; // not actually used here

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.AddTag("testTag", 100);
                    Assert.Fail();
                }
                catch (InsufficientRightsException) {}
            }
        }

        [TestMethod]
        public void DropTagNonAdminDeniedTest()
        {
            var user = new User {Email = "test@123.com", Password = "drowssap", Type = UserType.standard};
            var anotherUser = new User {Email = "another@email.com", Type = UserType.standard, Password = "second"};
            var fileInfo = new FileInfo {Id = 100, OwnerEmail = anotherUser.Email, Name = "filename"};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => null; // not actually used here

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.DropTag("testTag", 100);
                }
                catch (InsufficientRightsException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void GetTagsByItemIdNonAdminDeniedTest()
        {
            var user = new User {Email = "test@123.com", Password = "drowssap", Type = UserType.standard};
            var anotherUser = new User {Email = "another@email.com", Type = UserType.standard, Password = "second"};
            var fileInfo = new FileInfo {Id = 100, OwnerEmail = anotherUser.Email, Name = "filename"};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => null; // not actually used here

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.GetTagsByItemId(100);
                    Assert.Fail();
                }
                catch (InsufficientRightsException){}
            }
        }

        [TestMethod]
        public void GetFileInfosByTagNonAdminDeniedTest()
        {
            var user = new User {Email = "test@123.com", Password = "drowssap", Type = UserType.standard};
            var anotherUser = new User {Email = "another@email.com", Type = UserType.standard, Password = "second"};
            var fileInfo = new FileInfo {Id = 100, OwnerEmail = anotherUser.Email, Name = "filename"};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                ShimServiceClient.AllInstances.GetFileInfosByTagString =
                    (a, b) => new[] { fileInfo };

                Controller.LogIn("test@123.com", "drowssap");

                // Empty Array should be returned
                if (Controller.GetFileInfosByTag("testTag").Length != 0)
                    Assert.Fail();
            }
        }

        [TestMethod]
        public void GetPackageByIdNonAdminDeniedTest()
        {
            var user = new User {Email = "test@123.com", Password = "drowssap", Type = UserType.standard};
            var anotherUser = new User {Email = "another@email.com", Type = UserType.standard, Password = "second"};
            var fileInfo = new FileInfo {Id = 100, OwnerEmail = anotherUser.Email, Name = "filename"};
            var package = new Package {Id = 99, OwnerEmail = anotherUser.Email, Name = "packagename", FileIds = new[] {100} };

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => ((b == package.Id) ? package : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.GetPackageById(99);
                    Assert.Fail();
                }
                catch (InsufficientRightsException){}
            }
        }

        [TestMethod]
        public void AddToPackageNonAdminDeniedTest()
        {
            var user = new User {Email = "test@123.com", Password = "drowssap", Type = UserType.standard};
            var anotherUser = new User {Email = "another@email.com", Type = UserType.standard, Password = "second"};

            var fileInfo = new FileInfo {Id = 100, OwnerEmail = anotherUser.Email, Name = "filename"};

            var package = new Package { Id = 99, OwnerEmail = anotherUser.Email, Name = "packagename", FileIds = new int[] {} };

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => ((b == package.Id) ? package : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.AddToPackage(new[] { 100 }, 99);
                    Assert.Fail();
                }
                catch (InsufficientRightsException) {}
            }
        }

        [TestMethod]
        public void RemoveFromPackageNonAdminDeniedTest()
        {
            var user = new User {Email = "test@123.com", Password = "drowssap", Type = UserType.standard};
            var anotherUser = new User {Email = "another@email.com", Type = UserType.standard, Password = "second"};
            var fileInfo = new FileInfo {Id = 100, OwnerEmail = anotherUser.Email, Name = "filename"};
            var package = new Package { Id = 99, OwnerEmail = anotherUser.Email, Name = "packagename", FileIds = new[] {100} };

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => ((b == package.Id) ? package : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.RemoveFromPackage(new[] { 100 }, 99);
                    Assert.Fail();
                }
                catch (InsufficientRightsException){}
            }
        }

        [TestMethod]
        public void DeletePackageByIdNonAdminDeniedTest()
        {
            var user = new User {Email = "test@123.com", Password = "drowssap", Type = UserType.standard};
            var anotherUser = new User {Email = "another@email.com", Type = UserType.standard, Password = "second"};
            var package = new Package { Id = 99, OwnerEmail = anotherUser.Email, Name = "packagename", FileIds = new[] {100}};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => ((b == package.Id) ? package : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.DeletePackageById(99);
                    Assert.Fail();
                }
                catch (InsufficientRightsException){}
            }
        }

        [TestMethod]
        public void GetOwnedPackagesByIdNonAdminDeniedTest()
        {
            var user = new User {Email = "test@123.com", Password = "drowssap", Type = UserType.standard};
            var anotherUser = new User {Email = "another@email.com", Type = UserType.standard, Password = "second"};
            
            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => ((b.Equals(user.Email)) ? user : null);

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.GetOwnedPackagesByEmail(anotherUser.Email);
                    Assert.Fail();
                }
                catch (InsufficientRightsException){}
            }
        }

        [TestMethod]
        public void GetPackagesByTagNonAdminDeniedTest()
        {
            var user = new User {Email = "test@123.com", Password = "drowssap", Type = UserType.standard};
            var anotherUser = new User {Email = "another@email.com", Type = UserType.standard, Password = "second"};
            var package = new Package {Id = 99, OwnerEmail = anotherUser.Email, Name = "packagename", FileIds = new[] {100}};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                ShimServiceClient.AllInstances.GetPackagesByTagString =
                    (a, b) => new[] { package };

                Controller.LogIn("test@123.com", "drowssap");

                // Empty Array should be returned
                if (Controller.GetPackagesByTag("testTag").Length != 0)
                    Assert.Fail();
            }
        }

        [TestMethod]
        public void GrantRightNonAdminDeniedTest()
        {
            var user = new User {Email = "test@123.com", Password = "drowssap", Type = UserType.standard};
            var anotherUser = new User {Email = "another@email.com", Type = UserType.standard, Password = "second"};
            var fileInfo = new FileInfo {Id = 100, OwnerEmail = anotherUser.Email, Name = "filename"};
            var package = new Package { Id = 99, OwnerEmail = anotherUser.Email,Name = "packagename", FileIds = new[] {100} };
            var right = new Right{UserEmail = user.Email, Type = RightType.edit, Until = DateTime.Now.AddDays(1), ItemId = 100};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => ((b == 99) ? package : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == 100) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.GrantRight(right);
                }
                catch (InsufficientRightsException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void UpdateRightNonAdminDeniedTest()
        {
            var user = new User {Email = "test@123.com", Password = "drowssap", Type = UserType.standard};

            var anotherUser = new User {Email = "another@email.com", Type = UserType.standard, Password = "second"};

            var fileInfo = new FileInfo {Id = 100, OwnerEmail = anotherUser.Email, Name = "filename"};

            var package = new Package {Id = 99, OwnerEmail = anotherUser.Email, Name = "packagename", FileIds = new[] {100}};

            var right = new Right{UserEmail = anotherUser.Email, Type = RightType.edit,Until = DateTime.Now.AddDays(1),ItemId = 100 };

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => ((b == 99) ? package : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == 100) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => ((b.Equals(right.UserEmail) && c == 100) ? right : null);

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.UpdateRight(right);
                }
                catch (InsufficientRightsException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void DropRightNonAdminDeniedTest()
        {
            var user = new User {Email = "test@123.com", Password = "drowssap", Type = UserType.standard};
            var anotherUser = new User {Email = "another@email.com", Type = UserType.standard, Password = "second"};
            var fileInfo = new FileInfo {Id = 100, OwnerEmail = anotherUser.Email, Name = "filename"};
            var package = new Package {Id = 99, OwnerEmail = anotherUser.Email, Name = "packagename", FileIds = new[] {100} };
            var right = new Right {UserEmail = anotherUser.Email,Type = RightType.edit, Until = DateTime.Now.AddDays(1), ItemId = 100 };

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => ((b == 99) ? package : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == 100) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => ((b.Equals(right.UserEmail) && c == 100) ? right : null);

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.DropRight(right.UserEmail, right.ItemId);
                }
                catch (InsufficientRightsException)
                {
                    // All good
                }
            }
        }

        /**
         * Checking if methods deny access if you aren't logged in
         */
        [TestMethod]
        public void LogOutNotLoggedInTest()
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
        public void AddTagNotLoggedInTest()
        {
            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 = (a, b) => new FileInfo();
                ShimServiceClient.AllInstances.GetPackageByIdInt32 = (a, b) => new Package();
                ShimServiceClient.AllInstances.GetRightStringInt32 = (a, b, c) => new Right();

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
                    Controller.AddToPackage(new[] { 1 }, 1);
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
                    Controller.RemoveFromPackage(new[] { 1 }, 1);
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
                    Controller.UpdateFileData(new byte[] { 1 }, 1);
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
