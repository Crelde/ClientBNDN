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
         * Checking if methods allows access to standard users with required rights
         */
        [TestMethod]
        public void GetUserByEmailAllowedTest()
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

                Assert.AreEqual(
                    user,
                    Controller.GetUserByEmail(user.Email));
            }
        }

        [TestMethod]
        public void UpdateUserAllowedTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            User updatedUser = new User();
            updatedUser.Email = "test@123.com";
            updatedUser.Type = UserType.standard;
            updatedUser.Password = "new password";

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.UpdateUserUser =
                    (a, b) => { Assert.AreEqual(updatedUser, b); };

                Controller.LogIn("test@123.com", "drowssap");

                Controller.UpdateUser(updatedUser);
            }
        }

        [TestMethod]
        public void DownloadFileByIdAllowedTest()
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

            byte[] file = new byte[] { 1, 2, 3, 5, 10 };

            Right right = new Right();
            right.Type = RightType.view;
            right.ItemId = fileInfo.Id;
            right.UserEmail = user.Email;
            right.Until = DateTime.Now.AddDays(1);

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : anotherUser);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => right;

                ShimServiceClient.AllInstances.DownloadFileByIdInt32 =
                    (a, b) => file;

                Controller.LogIn("test@123.com", "drowssap");

                Assert.AreEqual(file, Controller.DownloadFileById(100));
            }
        }

        [TestMethod]
        public void UpdateFileInfoAllowedTest()
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

            FileInfo updatedFileInfo = new FileInfo();
            updatedFileInfo.Id = 100;
            updatedFileInfo.OwnerEmail = anotherUser.Email;
            updatedFileInfo.Name = "filename2";

            Right right = new Right();
            right.Type = RightType.edit;
            right.ItemId = fileInfo.Id;
            right.UserEmail = user.Email;
            right.Until = DateTime.Now.AddDays(1);

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : anotherUser);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => right;

                ShimServiceClient.AllInstances.UpdateFileInfoFileInfo =
                    (a, b) => { Assert.AreEqual(updatedFileInfo, b); };

                Controller.LogIn("test@123.com", "drowssap");

                Controller.UpdateFileInfo(updatedFileInfo);
            }
        }

        [TestMethod]
        public void UpdateFileDataAllowedTest()
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

            Right right = new Right();
            right.Type = RightType.edit;
            right.ItemId = fileInfo.Id;
            right.UserEmail = user.Email;
            right.Until = DateTime.Now.AddDays(1);

            byte[] file = new byte[] { 1, 2, 3, 5, 10 };

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => right;

                ShimServiceClient.AllInstances.UpdateFileDataByteArrayInt32 =
                    (a, b, c) => { Assert.AreEqual(file, b); Assert.AreEqual(fileInfo.Id, c); };

                Controller.LogIn("test@123.com", "drowssap");

                Controller.UpdateFileData(file, fileInfo.Id);
            }
        }

        [TestMethod]
        public void DeleteFileByIdAllowedTest()
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

            Right right = new Right();
            right.Type = RightType.edit;
            right.ItemId = fileInfo.Id;
            right.UserEmail = user.Email;
            right.Until = DateTime.Now.AddDays(1);

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => right;

                ShimServiceClient.AllInstances.DeleteFileByIdInt32 =
                    (a, b) => { Assert.AreEqual(fileInfo.Id, b); };

                Controller.LogIn("test@123.com", "drowssap");

                Controller.DeleteFileById(fileInfo.Id);
            }
        }

        [TestMethod]
        public void GetOwnedFileInfosAllowedTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = user.Email;
            fileInfo.Name = "filename";

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetOwnedFileInfosByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? new FileInfo[] { fileInfo } : null);

                ShimServiceClient.AllInstances.GetOwnedFileInfosByEmailString =
                    (a, b) => new FileInfo[] { fileInfo };

                Controller.LogIn("test@123.com", "drowssap");

                CollectionAssert.AreEqual(
                    new FileInfo[] { fileInfo },
                    Controller.GetOwnedFileInfos());
            }
        }

        [TestMethod]
        public void AddTagAllowedTest()
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

            Right right = new Right();
            right.Type = RightType.edit;
            right.ItemId = fileInfo.Id;
            right.UserEmail = user.Email;
            right.Until = DateTime.Now.AddDays(1);

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => null; // not actually used here

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => right;

                ShimServiceClient.AllInstances.AddTagStringInt32 =
                    (a, b, c) => { Assert.AreEqual("testTag", b); Assert.AreEqual(fileInfo.Id, c); };

                Controller.LogIn("test@123.com", "drowssap");

                Controller.AddTag("testTag", 100);
            }
        }

        [TestMethod]
        public void DropTagAllowedTest()
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

            Right right = new Right();
            right.Type = RightType.edit;
            right.ItemId = fileInfo.Id;
            right.UserEmail = user.Email;
            right.Until = DateTime.Now.AddDays(1);

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => null; // not actually used here

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => right;

                ShimServiceClient.AllInstances.DropTagStringInt32 =
                    (a, b, c) => { Assert.AreEqual("testTag", b); Assert.AreEqual(fileInfo.Id, c); };

                Controller.LogIn("test@123.com", "drowssap");

                Controller.DropTag("testTag", fileInfo.Id);
            }
        }

        [TestMethod]
        public void GetTagsByItemIdAllowedTest()
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

            Right right = new Right();
            right.Type = RightType.edit;
            right.ItemId = fileInfo.Id;
            right.UserEmail = user.Email;
            right.Until = DateTime.Now.AddDays(1);

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => null; // not actually used here

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => right;

                ShimServiceClient.AllInstances.GetTagsByItemIdInt32 =
                    (a, b) => new String[] { "testTag1", "secondTag" };

                Controller.LogIn("test@123.com", "drowssap");

                CollectionAssert.AreEqual(
                    new String[] { "testTag1", "secondTag" },
                    Controller.GetTagsByItemId(fileInfo.Id));
            }
        }

        [TestMethod]
        public void GetFileInfosByTagAllowedTest()
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

            Right right = new Right();
            right.Type = RightType.edit;
            right.ItemId = fileInfo.Id;
            right.UserEmail = user.Email;
            right.Until = DateTime.Now.AddDays(1);

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => right;

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
        public void CreatePackageAllowedTest()
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
            package.OwnerEmail = user.Email;
            package.Name = "packagename";
            package.FileIds = new int[] { 100 };

            Right right = new Right();
            right.Type = RightType.edit;
            right.ItemId = fileInfo.Id;
            right.UserEmail = user.Email;
            right.Until = DateTime.Now.AddDays(1);

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => ((b == package.Id) ? package : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => right;

                ShimServiceClient.AllInstances.CreatePackagePackage =
                    (a, b) =>  99;

                Controller.LogIn("test@123.com", "drowssap");

                Controller.CreatePackage(package);
            }
        }

        [TestMethod]
        public void GetPackageByIdAllowedTest()
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
            right.Type = RightType.view;
            right.ItemId = fileInfo.Id;
            right.UserEmail = user.Email;
            right.Until = DateTime.Now.AddDays(1);

            Right packageRight = new Right();
            packageRight.Type = RightType.view;
            packageRight.ItemId = package.Id;
            packageRight.UserEmail = user.Email;
            packageRight.Until = DateTime.Now.AddDays(1);

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => ((b == package.Id) ? package : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => ((c == package.Id) ? packageRight : right);

                Controller.LogIn("test@123.com", "drowssap");

                Assert.AreEqual(package, Controller.GetPackageById(package.Id));
            }
        }

        [TestMethod]
        public void AddToPackageAllowedTest()
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

            Right right = new Right();
            right.Type = RightType.edit;
            right.ItemId = fileInfo.Id;
            right.UserEmail = user.Email;
            right.Until = DateTime.Now.AddDays(1);

            Right packageRight = new Right();
            packageRight.Type = RightType.edit;
            packageRight.ItemId = package.Id;
            packageRight.UserEmail = user.Email;
            packageRight.Until = DateTime.Now.AddDays(1);

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => ((b == package.Id) ? package : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => ((c == package.Id) ? packageRight : right);

                ShimServiceClient.AllInstances.GetOwnedFileInfosByEmailString =
                    (a, b) => new FileInfo[] { fileInfo };

                ShimServiceClient.AllInstances.GetOwnedPackagesByEmailString =
                    (a, b) => new Package[] { package };

                ShimServiceClient.AllInstances.AddToPackageInt32ArrayInt32 =
                    (a, b, c) =>
                    {
                        CollectionAssert.AreEqual(new int[] { fileInfo.Id }, b);
                        Assert.AreEqual(package.Id, c);
                    };

                Controller.LogIn("test@123.com", "drowssap");

                Controller.AddToPackage(new int[] { fileInfo.Id }, package.Id);
            }
        }

        [TestMethod]
        public void RemoveFromPackageAllowedTest()
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
            right.Type = RightType.edit;
            right.ItemId = fileInfo.Id;
            right.UserEmail = user.Email;
            right.Until = DateTime.Now.AddDays(1);

            Right packageRight = new Right();
            packageRight.Type = RightType.edit;
            packageRight.ItemId = package.Id;
            packageRight.UserEmail = user.Email;
            packageRight.Until = DateTime.Now.AddDays(1);

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => ((b == package.Id) ? package : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => ((c == package.Id) ? packageRight : right);

                ShimServiceClient.AllInstances.RemoveFromPackageInt32ArrayInt32 =
                    (a, b, c) =>
                    {
                        CollectionAssert.AreEqual(
                            new int[] { fileInfo.Id },
                            b);
                        Assert.AreEqual(package.Id, c);
                    };

                Controller.LogIn("test@123.com", "drowssap");

                Controller.RemoveFromPackage(new int[] { fileInfo.Id }, package.Id);
            }
        }

        [TestMethod]
        public void DeletePackageByIdAllowedTest()
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

            Right packageRight = new Right();
            packageRight.Type = RightType.edit;
            packageRight.ItemId = package.Id;
            packageRight.UserEmail = user.Email;
            packageRight.Until = DateTime.Now.AddDays(1);

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => ((b == package.Id) ? package : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => packageRight;

                ShimServiceClient.AllInstances.DeletePackageByIdInt32 =
                    (a, b) => { Assert.AreEqual(package.Id, b); };

                Controller.LogIn("test@123.com", "drowssap");

                Controller.DeletePackageById(package.Id);
            }
        }

        [TestMethod]
        public void GetOwnedPackagesByEmailAllowedTest()
        {
            User user = new User();
            user.Email = "test@123.com";
            user.Password = "drowssap";
            user.Type = UserType.standard;

            FileInfo fileInfo = new FileInfo();
            fileInfo.Id = 100;
            fileInfo.OwnerEmail = user.Email;
            fileInfo.Name = "filename";

            Package package = new Package();
            package.Id = 99;
            package.OwnerEmail = user.Email;
            package.Name = "packagename";
            package.FileIds = new int[] { 100 };

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetOwnedPackagesByEmailString =
                    (a, b) => new Package[] { package };

                Controller.LogIn("test@123.com", "drowssap");

                CollectionAssert.AreEqual(
                    new Package[] { package },
                    Controller.GetOwnedPackages()

                );
            }
        }

        [TestMethod]
        public void GetPackagesByTagAllowedTest()
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

            Right packageRight = new Right();
            packageRight.Type = RightType.edit;
            packageRight.ItemId = package.Id;
            packageRight.UserEmail = user.Email;
            packageRight.Until = DateTime.Now.AddDays(1);

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => packageRight;

                ShimServiceClient.AllInstances.GetPackagesByTagString =
                    (a, b) => new Package[] { package };

                Controller.LogIn("test@123.com", "drowssap");

                CollectionAssert.AreEqual(
                    new Package[] { package },
                    Controller.GetPackagesByTag("testTag")
                    );
            }
        }

        [TestMethod]
        public void GrantRightAllowedTest()
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
            fileInfo.OwnerEmail = user.Email;
            fileInfo.Name = "filename";

            Package package = new Package();
            package.Id = 99;
            package.OwnerEmail = user.Email;
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
                    (a, b) => ((b.Equals(user.Email)) ? user : anotherUser);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => ((b == 99) ? package : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == 100) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => right;

                ShimServiceClient.AllInstances.GrantRightRight =
                    (a, b) => { Assert.AreEqual(right, b); };

                Controller.LogIn("test@123.com", "drowssap");

                Controller.GrantRight(right);
            }
        }

        [TestMethod]
        public void UpdateRightAllowedTest()
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
            fileInfo.OwnerEmail = user.Email;
            fileInfo.Name = "filename";

            Package package = new Package();
            package.Id = 99;
            package.OwnerEmail = user.Email;
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
                    (a, b) => ((b.Equals(user.Email)) ? user : anotherUser);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => ((b == 99) ? package : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == 100) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => ((b.Equals(right.UserEmail) && c == 100) ? right : null);

                ShimServiceClient.AllInstances.UpdateRightRight =
                    (a, b) => { Assert.AreEqual(right, b); };

                Controller.LogIn("test@123.com", "drowssap");

                Controller.UpdateRight(right);
            }
        }

        [TestMethod]
        public void DropRightAllowedTest()
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
            fileInfo.OwnerEmail = user.Email;
            fileInfo.Name = "filename";

            Package package = new Package();
            package.Id = 99;
            package.OwnerEmail = user.Email;
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
                    (a, b) => ((b.Equals(user.Email)) ? user : anotherUser);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => ((b == 99) ? package : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == 100) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => ((b.Equals(right.UserEmail) && c == 100) ? right : null);

                ShimServiceClient.AllInstances.DropRightStringInt32 =
                    (a, b, c) =>
                    {
                        Assert.AreEqual(right.UserEmail, b);
                        Assert.AreEqual(right.ItemId, c);
                    };


                Controller.LogIn("test@123.com", "drowssap");

                Controller.DropRight(right.UserEmail, right.ItemId);
            }
        }

        /**
         * Checking if methods denies access to standard users without required rights
         */
        [TestMethod]
        public void GetUserByEmailDeniedTest()
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
                    (a, b) => ((b.Equals(user.Email)) ? user : anotherUser);

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.GetUserByEmail(anotherUser.Email);

                    // If we're down here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All is good
                }
            }
        }

        [TestMethod]
        public void UpdateUserDeniedTest()
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
                    (a, b) => ((b.Equals(user.Email)) ? user : anotherUser);

                ShimServiceClient.AllInstances.UpdateUserUser =
                    (a, b) => { Assert.AreEqual(anotherUser, b); };

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.UpdateUser(anotherUser);

                    // If we're down here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {

                }
            }
        }

        [TestMethod]
        public void DownloadFileByIdDeniedTest()
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

            byte[] file = new byte[] { 1, 2, 3, 5, 10 };

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : anotherUser);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == fileInfo.Id) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                ShimServiceClient.AllInstances.DownloadFileByIdInt32 =
                    (a, b) => file;

                Controller.LogIn("test@123.com", "drowssap");


                try
                {
                    Controller.DownloadFileById(100);

                    // If we're down here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {

                }
            }
        }

        [TestMethod]
        public void UpdateFileInfoDeniedTest()
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
                
                try
                {
                    Controller.UpdateFileInfo(updatedFileInfo);

                    // If we're down here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {

                }
            }
        }

        [TestMethod]
        public void UpdateFileDataDeniedTest()
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

                try
                {
                    Controller.UpdateFileData(file, fileInfo.Id);

                    // If we're down here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {

                }
            }
        }

        [TestMethod]
        public void DeleteFileByIdDeniedTest()
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

                ShimServiceClient.AllInstances.DeleteFileByIdInt32 =
                    (a, b) => { Assert.AreEqual(fileInfo.Id, b); };

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.DeleteFileById(fileInfo.Id);

                    // If we're down here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {

                }
            }
        }

        [TestMethod]
        public void AddTagDeniedTest()
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

                ShimServiceClient.AllInstances.AddTagStringInt32 =
                    (a, b, c) => { Assert.AreEqual("testTag", b); Assert.AreEqual(fileInfo.Id, c); };

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.AddTag("testTag", 100);

                    // If we're down here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All is good
                }
            }
        }

        [TestMethod]
        public void DropTagDeniedTest()
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

                ShimServiceClient.AllInstances.DropTagStringInt32 =
                    (a, b, c) => { Assert.AreEqual("testTag", b); Assert.AreEqual(fileInfo.Id, c); };

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.DropTag("testTag", fileInfo.Id);

                    // If we're down here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All is good
                }
            }
        }

        [TestMethod]
        public void GetTagsByItemIdDeniedTest()
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

                ShimServiceClient.AllInstances.GetTagsByItemIdInt32 =
                    (a, b) => new String[] { "testTag1", "secondTag" };

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.GetTagsByItemId(fileInfo.Id);

                    // If we're down here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All is good
               } 
            }
        }

        [TestMethod]
        public void GetFileInfosByTagDeniedTest()
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
               
                Assert.AreEqual(Controller.GetFileInfosByTag("testTag").Length, 0);
            }
        }

        [TestMethod]
        public void CreatePackageDeniedTest()
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
            package.OwnerEmail = user.Email;
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

                ShimServiceClient.AllInstances.GetOwnedFileInfosByEmailString =
                    (a, b) => new FileInfo[] { };

                ShimServiceClient.AllInstances.GetOwnedPackagesByEmailString =
                    (a, b) => new Package[] { };

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                ShimServiceClient.AllInstances.CreatePackagePackage =
                    (a, b) => 99;

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.CreatePackage(package);

                    // If we're down here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All is good
                }
            }
        }

        [TestMethod]
        public void GetPackageByIdDeniedTest()
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
                    Controller.GetPackageById(package.Id);

                    // If we're down here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All is good
                }
            }
        }

        [TestMethod]
        public void AddToPackageDeniedTest()
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
                    (a, b, c) => null ;

                ShimServiceClient.AllInstances.GetOwnedFileInfosByEmailString =
                    (a, b) => new FileInfo[] { fileInfo };

                ShimServiceClient.AllInstances.GetOwnedPackagesByEmailString =
                    (a, b) => new Package[] { package };

                ShimServiceClient.AllInstances.AddToPackageInt32ArrayInt32 =
                    (a, b, c) =>
                    {
                        CollectionAssert.AreEqual(new int[] { fileInfo.Id }, b);
                        Assert.AreEqual(package.Id, c);
                    };

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.AddToPackage(new int[] { fileInfo.Id }, package.Id);

                    // If we're down here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All is good
                }
            }
        }

        [TestMethod]
        public void RemoveFromPackageDeniedTest()
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
                    (a, b, c) => null ;

                ShimServiceClient.AllInstances.RemoveFromPackageInt32ArrayInt32 =
                    (a, b, c) =>
                    {
                        CollectionAssert.AreEqual(
                            new int[] { fileInfo.Id },
                            b);
                        Assert.AreEqual(package.Id, c);
                    };

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.RemoveFromPackage(new int[] { fileInfo.Id }, package.Id);

                    // If we're down here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All is good
                }
            }
        }

        [TestMethod]
        public void DeletePackageByIdDeniedTest()
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

                ShimServiceClient.AllInstances.DeletePackageByIdInt32 =
                    (a, b) => { Assert.AreEqual(package.Id, b); };

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.DeletePackageById(package.Id);

                    // If we're down here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All is good
                }
            }
        }

        [TestMethod]
        public void GetOwnedPackagesByEmailDeniedTest()
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

                ShimServiceClient.AllInstances.GetOwnedPackagesByEmailString =
                    (a, b) => (anotherUser.Email.Equals(b) ? 
                        new Package[]{package} : 
                        new Package[]{}
                        );

                Controller.LogIn("test@123.com", "drowssap");

                CollectionAssert.AreEqual(
                    new Package[] { },
                    Controller.GetOwnedPackages()

                );
            }
        }

        [TestMethod]
        public void GetPackagesByTagDeniedTest()
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

                CollectionAssert.AreEqual(
                    new Package[] { },
                    Controller.GetPackagesByTag("testTag")
                    );
            }
        }

        [TestMethod]
        public void GrantRightDeniedTest()
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
                    (a, b) => ((b.Equals(user.Email)) ? user : anotherUser);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => ((b == 99) ? package : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == 100) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                ShimServiceClient.AllInstances.GrantRightRight =
                    (a, b) => { Assert.AreEqual(right, b); };

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.GrantRight(right);

                    // If we're down here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All is good
                }
            }
        }

        [TestMethod]
        public void UpdateRightDeniedTest()
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
            right.Type = RightType.view;
            right.Until = DateTime.Now.AddDays(1);
            right.ItemId = 100;

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : anotherUser);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => ((b == 99) ? package : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == 100) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => ((b.Equals(right.UserEmail) && c == 100) ? right : null);

                ShimServiceClient.AllInstances.UpdateRightRight =
                    (a, b) => { Assert.AreEqual(right, b); };

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.UpdateRight(right);

                    // If we're down here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All is good
                }
            }
        }

        [TestMethod]
        public void DropRightDeniedTest()
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
            right.Type = RightType.view;
            right.Until = DateTime.Now.AddDays(1);
            right.ItemId = 100;

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : anotherUser);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => ((b == 99) ? package : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == 100) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => ((b.Equals(right.UserEmail) && c == 100) ? right : null);

                ShimServiceClient.AllInstances.DropRightStringInt32 =
                    (a, b, c) =>
                    {
                        Assert.AreEqual(right.UserEmail, b);
                        Assert.AreEqual(right.ItemId, c);
                    };

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.DropRight(right.UserEmail, right.ItemId);

                    // If we're down here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All is good
                }
            }
        }

        /**
         * Checking if access is denied for admin users without required rights
         */
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

                try
                {
                    Controller.DownloadFileById(100);

                    // If we're here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good!
                }
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

                try
                {
                    Controller.UpdateFileInfo(updatedFileInfo);

                    // If we're here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good!
                }
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

                try
                {
                    Controller.UpdateFileData(file, fileInfo.Id);

                    // If we're here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good!
                }
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

                try
                {
                    Controller.DeleteFileById(fileInfo.Id);

                    // If we're here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good!
                }
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
                    (a, b) => new FileInfo[] { };

                Controller.LogIn("test@123.com", "drowssap");

                CollectionAssert.AreEqual(
                    new FileInfo[] { },
                    Controller.GetOwnedFileInfos());
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

                try
                {
                    Controller.AddTag("testTag", 100);

                    // If we're here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good!
                }
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

                try
                {
                    Controller.DropTag("testTag", fileInfo.Id);

                    // If we're here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good!
                }
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

                try
                {
                    Controller.GetTagsByItemId(fileInfo.Id);

                    // If we're here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good!
                }
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
                    (a, b) => new FileInfo[] { };

                Controller.LogIn("test@123.com", "drowssap");

                // Empty Array should be returned
                CollectionAssert.AreEqual(
                    new FileInfo[] { },
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

                try
                {
                    Controller.GetPackageById(package.Id);

                    // If we're here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good!
                }
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
                    (a, b, c) =>
                    {
                        CollectionAssert.AreEqual(new int[] { fileInfo.Id }, b);
                        Assert.AreEqual(package.Id, c);
                    };

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.AddToPackage(new int[] { fileInfo.Id }, package.Id);

                    // If we're here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good!
                }
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

                ShimServiceClient.AllInstances.RemoveFromPackageInt32ArrayInt32 =
                    (a, b, c) =>
                    {
                        CollectionAssert.AreEqual(
                            new int[] { fileInfo.Id },
                            b);
                        Assert.AreEqual(package.Id, c);
                    };

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.RemoveFromPackage(new int[] { fileInfo.Id }, package.Id);

                    // If we're here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good!
                }
            }
        }

        [TestMethod]
        public void DeletePackageByIdAdminTest()
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

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => ((b == package.Id) ? package : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                ShimServiceClient.AllInstances.DeletePackageByIdInt32 =
                    (a, b) => { Assert.AreEqual(package.Id, b); };

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                     Controller.DeletePackageById(package.Id);

                    // If we're here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good!
                }

               
            }
        }

        [TestMethod]
        public void GetPackagesByTagAdminTest()
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

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                ShimServiceClient.AllInstances.GetPackagesByTagString =
                    (a, b) => new Package[] { };

                Controller.LogIn("test@123.com", "drowssap");

                CollectionAssert.AreEqual(
                    new Package[] { },
                    Controller.GetPackagesByTag("testTag")
                    );
            }
        }

        [TestMethod]
        public void GrantRightAdminTest()
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

            Right right = new Right();
            right.UserEmail = anotherUser.Email;
            right.Type = RightType.edit;
            right.Until = DateTime.Now.AddDays(1);
            right.ItemId = 100;

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : anotherUser);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => ((b == 99) ? package : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == 100) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => null;

                ShimServiceClient.AllInstances.GrantRightRight =
                    (a, b) => { Assert.AreEqual(right, b); };

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.GrantRight(right);

                    // If we're here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good!
                }

                
            }
        }

        [TestMethod]
        public void UpdateRightAdminTest()
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

                ShimServiceClient.AllInstances.UpdateRightRight =
                    (a, b) => { Assert.AreEqual(right, b); };

                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.UpdateRight(right);

                    // If we're here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good!
                }

                
            }
        }

        [TestMethod]
        public void DropRightAdminTest()
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

            Right right = new Right();
            right.UserEmail = anotherUser.Email;
            right.Type = RightType.edit;
            right.Until = DateTime.Now.AddDays(1);
            right.ItemId = 100;

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString =
                    (a, b) => ((b.Equals(user.Email)) ? user : anotherUser);

                ShimServiceClient.AllInstances.GetPackageByIdInt32 =
                    (a, b) => ((b == 99) ? package : null);

                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 =
                    (a, b) => ((b == 100) ? fileInfo : null);

                ShimServiceClient.AllInstances.GetRightStringInt32 =
                    (a, b, c) => ((b.Equals(right.UserEmail) && c == 100) ? right : null);

                ShimServiceClient.AllInstances.DropRightStringInt32 =
                    (a, b, c) =>
                    {
                        Assert.AreEqual(right.UserEmail, b);
                        Assert.AreEqual(right.ItemId, c);
                    };


                Controller.LogIn("test@123.com", "drowssap");

                try
                {
                    Controller.DropRight(right.UserEmail, right.ItemId);

                    // If we're here, something is wrong
                    Assert.Fail();
                }
                catch (InsufficientRightsException)
                {
                    // All good!
                }

                
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
                    
                    // If we get this far, something is wrong
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

                    // If we get this far, something is wrong
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

                    // If we get this far, something is wrong
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

                    // If we get this far, something is wrong
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

                    // If we get this far, something is wrong
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

                    // If we get this far, something is wrong
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

                    // If we get this far, something is wrong
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

                    // If we get this far, something is wrong
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

                    // If we get this far, something is wrong
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

                    // If we get this far, something is wrong
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

                    // If we get this far, something is wrong
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
                    Controller.AddToPackage(new[] { 100 }, 99);

                    // If we get this far, something is wrong
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

                    // If we get this far, something is wrong
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

                    // If we get this far, something is wrong
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
                    Controller.AddToPackage(new[] { 1 }, 1);

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
                    Controller.GetOwnedFileInfos();

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
                    Controller.GetOwnedPackages();

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
                    Controller.RemoveFromPackage(new[] { 1 }, 1);

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
