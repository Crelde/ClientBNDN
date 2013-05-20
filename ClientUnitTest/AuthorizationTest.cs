using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication1.ServiceReference1.Fakes;
using WebApplication1.ServiceReference1;
using WebApplication1;
using System.Diagnostics;
using Microsoft.QualityTools.Testing.Fakes;

namespace ClientUnitTest
{
    [TestClass]
    public class AuthorizationTest
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

        /**
         * Checking if admin-specific methods allows access for admins
         */
        [TestMethod]
        public void CreateUserAdminTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.admin;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.CreateUserUser =
                    (a, b) => { Assert.AreEqual(anotherUser, b); };

                Controller.LogIn("test@123.com", "drowssap");

                Controller.CreateUser(anotherUser);
            }
        }

        [TestMethod]
        public void GetUserByEmailAdminTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.admin;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : anotherUser);

                Controller.LogIn("test@123.com", "drowssap");

                Assert.AreEqual(
                    anotherUser, 
                    Controller.GetUserByEmail(anotherUser.Email));
            }
        }

        [TestMethod]
        public void UpdateUserAdminTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.admin;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            User updatedUser = new User();
            updatedUser.Email = "another@email.com";
            updatedUser.Type = UserType.standard;
            updatedUser.Password = "new password";

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : anotherUser);

                ShimServiceClient.AllInstances.UpdateUserUser =
                    (a, b) => { Assert.AreEqual(updatedUser, b); };

                Controller.LogIn("test@123.com", "drowssap");

                Controller.UpdateUser(updatedUser);
            }
        }

        [TestMethod]
        public void DeleteUserByEmailAdminTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.admin;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : anotherUser);

                ShimServiceClient.AllInstances.DeleteUserByEmailString =
                    (a, b) => { Assert.AreEqual(anotherUser.Email, b); };

                Controller.LogIn("test@123.com", "drowssap");

                Controller.DeleteUserByEmail(anotherUser.Email);
            }
        }

        [TestMethod]
        public void DownloadFileByIdAdminTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.admin;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;

            byte[] file = new byte[] { 1, 2, 3, 5, 10 };

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                ShimServiceClient.AllInstances.DownloadFileByIdInt32 =
                    (a, b) => file;

                Controller.LogIn("test@123.com", "drowssap");

                Assert.AreEqual(file, Controller.DownloadFileById(100));
            }
        }

        [TestMethod]
        public void UpdateFileInfoAdminTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.admin;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

            FileInfo updatedFileInfo = new FileInfo();
            updatedFileInfo.Id = 100;
            updatedFileInfo.OwnerEmail = anotherUser.Email;
            updatedFileInfo.Name = "filename2";

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : anotherUser);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                ShimServiceClient.AllInstances.UpdateFileInfoFileInfo =
                    (a, b) => { Assert.AreEqual(updatedFileInfo, b); };

                Controller.LogIn("test@123.com", "drowssap");

                Controller.UpdateFileInfo(updatedFileInfo);
            }
        }

        [TestMethod]
        public void UpdateFileDataAdminTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.admin;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

            byte[] file = new byte[] { 1, 2, 3, 5, 10 };

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                ShimServiceClient.AllInstances.UpdateFileDataByteArrayInt32 =
                    (a, b, c) => { Assert.AreEqual(file, b); Assert.AreEqual(fileInfo.Id, c); };

                Controller.LogIn("test@123.com", "drowssap");

                Controller.UpdateFileData(file, fileInfo.Id);
            }
        }

        [TestMethod]
        public void DeleteFileByIdAdminTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.admin;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                ShimServiceClient.AllInstances.DeleteFileByIdInt32 =
                    (a, b) => { Assert.AreEqual(fileInfo.Id, b); };

                Controller.LogIn("test@123.com", "drowssap");

                Controller.DeleteFileById(fileInfo.Id);
            }
        }

        [TestMethod]
        public void GetOwnedFileInfosAdminTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.admin;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetOwnedFileInfosByEmailString =
                    (a, b) => ((b.Equals(anotherUser.Email)) ? new FileInfo[] { fileInfo } : null);

                ShimServiceClient.AllInstances.GetOwnedFileInfosByEmailString =
                    (a, b) => new FileInfo[]{fileInfo};

                Controller.LogIn("test@123.com", "drowssap");

                CollectionAssert.AreEqual(
                    new FileInfo[]{fileInfo}, 
                    Controller.GetOwnedFileInfosByEmail(anotherUser.Email));
            }
        }

        [TestMethod]
        public void AddTagAdminTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.admin;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

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

                ShimServiceClient.AllInstances.AddTagStringInt32 =
                    (a, b, c) => { Assert.AreEqual("testTag", b); Assert.AreEqual(fileInfo.Id, c); };

                Controller.LogIn("test@123.com", "drowssap");

                Controller.AddTag("testTag", 100);
            }
        }

        [TestMethod]
        public void DropTagAdminTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.admin;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

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

                ShimServiceClient.AllInstances.DropTagStringInt32 =
                    (a, b, c) => { Assert.AreEqual("testTag", b); Assert.AreEqual(fileInfo.Id, c); };

                Controller.LogIn("test@123.com", "drowssap");

                Controller.DropTag("testTag", fileInfo.Id);
            }
        }

        [TestMethod]
        public void GetTagsByItemIdAdminTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.admin;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

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

                ShimServiceClient.AllInstances.GetTagsByItemIdInt32 =
                    (a, b) => new String[] { "testTag1", "secondTag" };

                Controller.LogIn("test@123.com", "drowssap");

                CollectionAssert.AreEqual(
                    new String[] { "testTag1", "secondTag" },
                    Controller.GetTagsByItemId(fileInfo.Id));
            }
        }

        [TestMethod]
        public void GetFileInfosByTagAdminTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.admin;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                ShimServiceClient.AllInstances.GetFileInfosByTagString =
                    (a, b) => new FileInfo[] { fileInfo };

                Controller.LogIn("test@123.com", "drowssap");

                // Empty Array should be returned
                CollectionAssert.AreEqual(
                    new FileInfo[] { fileInfo },
                    Controller.GetFileInfosByTag("testTag"));
            }
        }

        [TestMethod]
        public void GetPackageByIdAdminTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.admin;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

            Package package = new Package();
            package.Id = 99;
            package.OwnerEmail = anotherUser.Email;
            package.Name = "packagename";
            package.FileIds = new int[] { 100 };

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

                Assert.AreEqual(package, Controller.GetPackageById(package.Id));
            }
        }

        [TestMethod]
        public void AddToPackageAdminTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.admin;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

            Package package = new Package();
            package.Id = 99;
            package.OwnerEmail = anotherUser.Email;
            package.Name = "packagename";
            package.FileIds = new int[] { };

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

                ShimServiceClient.AllInstances.GetOwnedFileInfosByEmailString =
                    (a, b) => new FileInfo[] { fileInfo };

                ShimServiceClient.AllInstances.GetOwnedPackagesByEmailString =
                    (a, b) => new Package[] { package };

                ShimServiceClient.AllInstances.AddToPackageInt32ArrayInt32 =
                    (a, b, c) => {
                        CollectionAssert.AreEqual(new int[]{fileInfo.Id}, b);
                        Assert.AreEqual(package.Id, c);
                    };

                Controller.LogIn("test@123.com", "drowssap");

                Controller.AddToPackage(new int[] { fileInfo.Id }, package.Id); 
            }
        }

        [TestMethod]
        public void RemoveFromPackageAdminTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.admin;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

            Package package = new Package();
            package.Id = 99;
            package.OwnerEmail = anotherUser.Email;
            package.Name = "packagename";
            package.FileIds = new int[] { 100 };

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
                    Controller.RemoveFromPackage(new int[] { 100 }, 99);

                    // If we get this far, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good
                }
            }
        }

        /**
         * Checking if admin-specific methods denies access for non-admins
         */
        [TestMethod]
        public void CreateUserNonAdminDeniedTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.CreateUser(new User());
                    
                    // If we get this far, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void GetUserByEmailNonAdminDeniedTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.GetUserByEmail("another@email.com");

                    // If we get this far, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void UpdateUserNonAdminDeniedTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.UpdateUser(anotherUser);

                    // If we get this far, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void DeleteUserByEmailNonAdminDeniedTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.DeleteUserByEmail(anotherUser.Email);

                    // If we get this far, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void DownloadFileByIdNonAdminDeniedTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;

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

                    // If we get this far, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void UpdateFileInfoNonAdminDeniedTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

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

                    // If we get this far, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void UpdateFileDataNonAdminDeniedTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

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

                    // If we get this far, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void DeleteFileByIdNonAdminDeniedTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

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

                    // If we get this far, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void GetOwnedFileInfosNonAdminDeniedTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetOwnedFileInfosByEmailString =
                    (a, b) => ((b.Equals(anotherUser.Email)) ? new FileInfo[] { fileInfo } : null);

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.GetOwnedFileInfosByEmail(anotherUser.Email);

                    // If we get this far, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void AddTagNonAdminDeniedTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

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

                    // If we get this far, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void DropTagNonAdminDeniedTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

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

                    // If we get this far, something is wrong
                    Assert.Fail();
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
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

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

                    // If we get this far, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void GetFileInfosByTagNonAdminDeniedTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                ShimServiceClient.AllInstances.GetFileInfosByTagString =
                    (a, b) => new FileInfo[] { fileInfo };

                Controller.LogIn("test@123.com", "drowssap");

                // Empty Array should be returned
                if (Controller.GetFileInfosByTag("testTag").Length != 0)
                    Assert.Fail();
            }
        }

        [TestMethod]
        public void GetPackageByIdNonAdminDeniedTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

            Package package = new Package();
            package.Id = 99;
            package.OwnerEmail = anotherUser.Email;
            package.Name = "packagename";
            package.FileIds = new int[] { 100 };

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

                    // If we get this far, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void AddToPackageNonAdminDeniedTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

            Package package = new Package();
            package.Id = 99;
            package.OwnerEmail = anotherUser.Email;
            package.Name = "packagename";
            package.FileIds = new int[] { };

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
                    Controller.AddToPackage(new int[] { 100 }, 99);

                    // If we get this far, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void RemoveFromPackageNonAdminDeniedTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

            Package package = new Package();
            package.Id = 99;
            package.OwnerEmail = anotherUser.Email;
            package.Name = "packagename";
            package.FileIds = new int[] { 100 };

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
                    Controller.RemoveFromPackage(new int[] { 100 }, 99);

                    // If we get this far, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void DeletePackageByIdNonAdminDeniedTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

            Package package = new Package();
            package.Id = 99;
            package.OwnerEmail = anotherUser.Email;
            package.Name = "packagename";
            package.FileIds = new int[] { 100 };

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

                    // If we get this far, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void GetOwnedPackagesByIdNonAdminDeniedTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

            Package package = new Package();
            package.Id = 99;
            package.OwnerEmail = anotherUser.Email;
            package.Name = "packagename";
            package.FileIds = new int[] { 100 };

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.GetOwnedPackagesByEmail(anotherUser.Email);

                    // If we get this far, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good
                }
            }
        }

        [TestMethod]
        public void GetPackagesByTagNonAdminDeniedTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

            Package package = new Package();
            package.Id = 99;
            package.OwnerEmail = anotherUser.Email;
            package.Name = "packagename";
            package.FileIds = new int[] { 100 };

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                ShimServiceClient.AllInstances.GetPackagesByTagString =
                    (a, b) => new Package[] { package };

                Controller.LogIn("test@123.com", "drowssap");

                // Empty Array should be returned
                if (Controller.GetPackagesByTag("testTag").Length != 0)
                    Assert.Fail();
            }
        }

        [TestMethod]
        public void GrantRightNonAdminDeniedTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

            Package package = new Package();
            package.Id = 99;
            package.OwnerEmail = anotherUser.Email;
            package.Name = "packagename";
            package.FileIds = new int[] { 100 };

            Right right = new Right();
            right.UserEmail = user.Email;
            right.Type = RightType.edit;
            right.Until = DateTime.Now.AddDays(1);
            right.ItemId = 100;

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

                    // If we get this far, something is wrong
                    Assert.Fail();
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
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

            Package package = new Package();
            package.Id = 99;
            package.OwnerEmail = anotherUser.Email;
            package.Name = "packagename";
            package.FileIds = new int[] { 100 };

            Right right = new Right();
            right.UserEmail = anotherUser.Email;
            right.Type = RightType.edit;
            right.Until = DateTime.Now.AddDays(1);
            right.ItemId = 100;

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

                    // If we get this far, something is wrong
                    Assert.Fail();
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
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            User anotherUser = new User();
            anotherUser.Email = "another@email.com";
            anotherUser.Type = UserType.standard;
            anotherUser.Password = "second";

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = anotherUser.Email;
            fileInfo.Name = "filename";

            Package package = new Package();
            package.Id = 99;
            package.OwnerEmail = anotherUser.Email;
            package.Name = "packagename";
            package.FileIds = new int[] { 100 };

            Right right = new Right();
            right.UserEmail = anotherUser.Email;
            right.Type = RightType.edit;
            right.Until = DateTime.Now.AddDays(1);
            right.ItemId = 100;

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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                // If we get this far, something is wrong
                Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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
                    Controller.AddToPackage(new int[] { 1 }, 1);

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
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

                    // If we get this far, something is wrong
                    Assert.Fail();
                }
                catch (NotLoggedInException)
                {
                    // All good
                }
            }
        }


    }
}
