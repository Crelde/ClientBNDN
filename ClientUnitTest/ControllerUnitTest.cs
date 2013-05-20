using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.QualityTools.Testing.Fakes;
using WebApplication1.ServiceReference1.Fakes;
using WebApplication1.ServiceReference1;
using WebApplication1;
using System.Collections.Generic;


namespace ClientUnitTest
{
    [TestClass]
    public class ControllerUnitTest
    {
        [TestInitialize]
        public void BeforeEach()
        {
            // Inserting blank methods instead of connecting to service
            ShimsContext.Create();
            ShimServiceClient.Constructor = (a) => { };
            ShimServiceClient.ConstructorBindingEndpointAddress = (a, b, c) => { };
            ShimServiceClient.ConstructorString = (a, b) => { };
            ShimServiceClient.ConstructorStringEndpointAddress = (a, b, c) => { };
            ShimServiceClient.ConstructorStringString = (a, b, c) => { };
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

        [TestMethod]
        public void LogInTest()
        {
            var testuser = new User { Email = "a@b.com", Password = "123", Type = UserType.standard};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => testuser;
                Controller.LogIn("a@b.com", "123");
            }
        }

        [TestMethod]
        public void LogInIncorrectPassTest()
        {
            var testuser = new User{ Email = "a@b.com", Password = "123", Type = UserType.standard };

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => testuser;
                try
                {
                    Controller.LogIn("a@b.com", "");
                    Assert.Fail();
                }
                catch (IncorrectPasswordException){ }
            }
        }

        [TestMethod]
        public void LogInIncorrectPassTest2()
        {
            var testuser = new User { Email = "a@b.com", Password = "123", Type = UserType.standard };
            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => testuser;
                try
                {
                    Controller.LogIn("a@b.com", "1234");
                    Assert.Fail();
                }
                catch (IncorrectPasswordException) { }
            }
        }

        [TestMethod]
        public void LogInIncorrectFormatTest()
        {
            var testuser = new User { Email = "a@b.com", Password = "123", Type = UserType.standard };
            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => (testuser.Email.Equals(b)) ? testuser : null;

                try
                {
                    Controller.LogIn("abcom", "123");
                    Assert.Fail();
                }
                catch (NoSuchUserException) { }
            }
        }

        [TestMethod]
        public void LogInAndOutTest()
        {
            var testuser = new User { Email = "a@b.com", Password = "123", Type = UserType.standard };

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
            var testuserA = new User { Email = "c@b.com", Password = "101", Type = UserType.admin };
            var testuserS = new User { Email = "x@s.com", Password = "ps1", Type = UserType.standard };

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => testuserA;
                ShimServiceClient.AllInstances.CreateUserUser = (a, b) => { };

                Controller.LogIn("c@b.com", "101");
                Controller.CreateUser(testuserS);
            }
        }

        //check if regular users are denied making new users
        [TestMethod]
        public void StandardCreateUser()
        {
            var testuserS1 = new User { Email = "k@b.com", Password = "201", Type = UserType.standard };
            var testuser = new User { Email = "p@s.com", Password = "199", Type = UserType.standard };

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => testuserS1;

                Controller.LogIn("k@b.com", "201");

                try
                {
                    Controller.CreateUser(testuser);
                    Assert.Fail();
                }
                catch (InsufficientRightsException) { }
            }
        }

        // check if admin can delete users
        [TestMethod]
        public void DeletingUser()
        {
            var testAdmin = new User { Email = "ta@ta.com", Password = "ta123", Type = UserType.admin };
            var testuser = new User { Email = "p@s.com", Password = "199", Type = UserType.standard };
        
            var userDictionary = new Dictionary<string, User> {{"ta@ta.com", testAdmin}};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => userDictionary[b];
                ShimServiceClient.AllInstances.CreateUserUser = (a, b) => userDictionary.Add(b.Email, b);
                ShimServiceClient.AllInstances.DeleteUserByEmailString = (a, b) => userDictionary.Remove(b);
                Controller.LogIn("ta@ta.com", "ta123");
                Controller.CreateUser(testuser);
                Controller.DeleteUserByEmail("p@s.com");
            }
        }

        //check to see if authorization fails after you log out
        [TestMethod]
        public void AuthorizationFail()
        {
            var testAdmin = new User { Email = "ta@ta.com", Password = "ta123", Type = UserType.admin };
            
            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => testAdmin;
                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 = (a, b) => new FileInfo();
                Controller.LogIn("ta@ta.com", "ta123");
                Controller.LogOut();
                try
                {
                    Controller.DownloadFileById(011);
                    Assert.Fail();
                }
                catch (NotLoggedInException) { }
            }
        }

        //tests trying to download a none existing file. 
        [TestMethod]
        public void FileNotExistsTest()
        {
            var testAdmin = new User { Email = "ta@ta.com", Password = "ta123", Type = UserType.admin };
           
            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => testAdmin;
                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 = (a, b) => new FileInfo();
                ShimServiceClient.AllInstances.DownloadFileByIdInt32 = (a, b) => new byte[] { };
                
                Controller.LogIn("ta@ta.com", "ta123");
                try
                {
                    Controller.DownloadFileById(018);
                    Assert.Fail();
                }
                catch (ObjectNotFoundException) { }
            }
        }

        
        //check if adding and removing tags does what they're supposed to
        [TestMethod]
        public void TagTest1()
        {

            var user1 = new User { Email = "u1@mail.com", Password = "user1", Type = UserType.admin };
            var user7 = new User { Email = "u7@mail.com", Password = "user7", Type = UserType.standard};

            var p = new Package { Id = 1001, Name = "p1001", OwnerEmail = "u1@mail.com", FileIds = new[] { 001 } };

            var fi = new FileInfo { Id = 001, Name = "testItem", OwnerEmail ="u1@mail.com", Type = FileType.text};

            var r = new Right { ItemId = 001, Type = RightType.edit, Until = DateTime.Now.AddDays(1), UserEmail = "u1@mail.com"};
            var r2 = new Right { ItemId = 001, Type = RightType.edit, Until = DateTime.Now.AddDays(1), UserEmail = "u7@mail.com" };

            var tagsList = new List<string>();
            var userDictionary = new Dictionary<string,User> {{"u1@mail.com", user1}};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => userDictionary[b];
                ShimServiceClient.AllInstances.CreatePackagePackage = (a, b) => 1;
                ShimServiceClient.AllInstances.GetPackageByIdInt32 = (a, b) => p;
                ShimServiceClient.AllInstances.AddToPackageInt32ArrayInt32 = (a, b, c) => { };
                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 = (a, b) => fi;
                ShimServiceClient.AllInstances.GetRightStringInt32 = (a, b, c) => r;
                ShimServiceClient.AllInstances.UpdateFileInfoFileInfo = (a, b) => { };
                ShimServiceClient.AllInstances.GrantRightRight = (a, b) => { };
                ShimServiceClient.AllInstances.DropTagStringInt32 = (a, b, c) => tagsList.Remove(b);
                ShimServiceClient.AllInstances.GetTagsByItemIdInt32 = (a, b) => tagsList.ToArray();
                ShimServiceClient.AllInstances.AddTagStringInt32 = (a, b, c) => tagsList.Add(b);
                ShimServiceClient.AllInstances.CreateUserUser = (a, b) => userDictionary.Add(b.Email, b);
                ShimServiceClient.AllInstances.GetRightStringInt32 = (a, b, c) => b.Equals(r) ? r : r2;

                Controller.LogIn("u1@mail.com", "user1");
                Controller.CreateUser(user7);
                Controller.CreatePackage(p);
                Controller.AddToPackage(new[] { 001 }, 1001);
                Controller.AddTag("testTag", 001);
                Controller.GrantRight(r2);
                Controller.LogOut();

                Controller.LogIn("u7@mail.com", "user7");
                Controller.AddTag("tag2", 001);
                CollectionAssert.AreEqual(new[] { "testTag", "tag2" }, Controller.GetTagsByItemId(001));
                Controller.DropTag("testTag", 001);
                CollectionAssert.AreEqual(new[] { "tag2" }, Controller.GetTagsByItemId(001));
                Controller.LogOut();

                Controller.LogIn("u1@mail.com", "user1");
                CollectionAssert.AreEqual(new[] { "tag2" }, Controller.GetTagsByItemId(001));
                Controller.LogOut();
            }
        }

        //check to see if logging in keeps you logged in
        [TestMethod]
        public void Sessioning1()
        {
            var test = new User { Email = "au@au.com", Password = "test", Type = UserType.admin };
            var p = new Package { Id = 1001, Name = "p1001", OwnerEmail = "au@au.com", FileIds = new[] { 092 } };
            var fi = new FileInfo { Id = 092, Name = "testItem", OwnerEmail = "au@au.com", Type = FileType.text };
            var fiu = new FileInfo { Id = 092, Name = "UpdatedtestItem", OwnerEmail = "au@au.com", Type = FileType.text };
            var r = new Right { ItemId = 092, Type = RightType.edit, Until = DateTime.Now.AddDays(1), UserEmail = "au@au.com" };
            var db = new Dictionary<int, FileInfo> {{fi.Id,fi}};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => test;
                ShimServiceClient.AllInstances.CreatePackagePackage = (a, b) => 1;
                ShimServiceClient.AllInstances.GetPackageByIdInt32 = (a, b) => p; 
                ShimServiceClient.AllInstances.AddToPackageInt32ArrayInt32 = (a, b, c) => {}; 
                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 = (a, b) => db[b];
                ShimServiceClient.AllInstances.AddTagStringInt32 = (a, b, c) => { };
                ShimServiceClient.AllInstances.DeleteFileByIdInt32 = (a, b) => { };
                ShimServiceClient.AllInstances.GetRightStringInt32 = (a, b, c) => r;
                ShimServiceClient.AllInstances.UpdateFileInfoFileInfo = (a, b) => { };
                ShimServiceClient.AllInstances.DeleteFileByIdInt32 = (a, b) => db.Remove(b);
                ShimServiceClient.AllInstances.UpdateFileInfoFileInfo = (a, b) => { db.Remove(b.Id); db.Add(b.Id, b); };

                Controller.LogIn("au@au.com", "test");
                Controller.CreatePackage(p);
                Controller.AddToPackage(new[]{092},1001);              
                Controller.AddTag("tag1", 092);
                Controller.DeleteFileById(092);
                try
                {
                    Controller.UpdateFileInfo(fiu);
                    Assert.Fail();
                }
                catch (OriginalNotFoundException) { }
            }
        }
        

        // check if allowed to edit correct files
        [TestMethod]
        public void RwAuthorization1()
        {
            var u1 = new User { Email = "u1@u1.com", Password = "u1", Type = UserType.admin};
            var u2 = new User { Email = "u2@u2.com", Password = "u2", Type = UserType.standard };
            var u3 = new User { Email = "u3@u3.com", Password = "u3", Type = UserType.standard };

            var db = new Dictionary<string, User> {{"u1@u1.com", u1}};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => db[b];
                ShimServiceClient.AllInstances.CreateUserUser = (a, b) => db.Add(b.Email, b);

                Controller.LogIn("u1@u1.com", "u1");
                Controller.CreateUser(u2);
                Controller.CreateUser(u3);

            }
        }

        // check if readonly is actually read-only, or edit is possible
        [TestMethod]
        public void RwAuthorization2()
        {
        }

        // check if write rights also allows reading
        [TestMethod]
        public void RwAuthorization3()
        {
        }

        // check if non-/owners can delete files
        [TestMethod]
        public void RwAuthorization4()
        {
        }












        //check if logging into one account, logging out, then logging into another account gives correct authorization 
        [TestMethod]
        public void Authorization1()
        {
            var user1 = new User {Email = "u1@mail.com", Password = "user1", Type = UserType.admin};
            var user5 = new User {Email = "usertest@mail.com", Password = "mm", Type = UserType.standard};
            var p = new Package {Id = 1002, Name = "p1002", OwnerEmail = "u1@mail.com", FileIds = new[] {002}};
            var fi = new FileInfo {Id = 002, Name = "testItem2", OwnerEmail = "u1@mail.com", Type = FileType.text};
            var update = new FileInfo{Id = 002, Name = "updatedItem2", OwnerEmail = "u1@mail.com", Type = FileType.text };
            var r = new Right { ItemId = 002, Type = RightType.edit, Until = DateTime.Now.AddDays(1),UserEmail = "u1@mail.com" };
            var db = new Dictionary<string, User> {{"u1@mail.com", user1}};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => db[b];
                ShimServiceClient.AllInstances.CreatePackagePackage = (a, b) => 1;
                ShimServiceClient.AllInstances.GetPackageByIdInt32 = (a, b) => p;
                ShimServiceClient.AllInstances.AddToPackageInt32ArrayInt32 = (a, b, c) => { };
                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 = (a, b) => fi;
                ShimServiceClient.AllInstances.AddTagStringInt32 = (a, b, c) => { };
                ShimServiceClient.AllInstances.DeleteFileByIdInt32 = (a, b) => { };
                ShimServiceClient.AllInstances.GetRightStringInt32 = (a, b, c) => r;
                ShimServiceClient.AllInstances.UpdateFileInfoFileInfo = (a, b) => { };
                ShimServiceClient.AllInstances.CreateUserUser = (a, b) => db.Add(b.Email, b);

                Controller.LogIn("u1@mail.com", "user1");
                Controller.CreateUser(user5);
                Controller.CreatePackage(p);
                Controller.AddToPackage(new[] { 002 }, 1002);
                Controller.LogOut();
                Controller.LogIn("usertest@mail.com", "mm");
                try
                {
                    Controller.UpdateFileInfo(update);
                } catch(InadequateObjectException) 
                { 
                    //should fail 
                }
                Controller.LogOut();

                Controller.LogIn("u1@mail.com","user1");
                try
                {
                    Controller.UpdateFileInfo(update);
                } catch(InsufficientRightsException) 
                { 
                    //should not fail 
                }
            }
        }

        //check if other accounts are denied access to edit stranger accounts
        [TestMethod]
        public void Authorization2()
        {
            var test1 = new User { Email = "st@st.com", Password = "test1", Type = UserType.standard };
            var test2 = new User { Email = "t@t.com", Password = "test2", Type = UserType.standard };

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => test1;
                ShimServiceClient.AllInstances.UpdateUserUser = (a, b) => { };
                Controller.LogIn("st@st.com", "test1");
                try
                {
                    Controller.UpdateUser(test2);
                    Assert.Fail();
                }
                catch (InsufficientRightsException) { }
            }
        }


        //Tests that a user without rights are denied downloading a file. 
        [TestMethod]
        public void DownloadingWithoutRightsTest()
        {
            var user1 = new User {Email = "u1@mail.com", Password = "user1", Type = UserType.admin};
            var user5 = new User {Email = "usertest@mail.com", Password = "mm", Type = UserType.standard};
            var p = new Package {Id = 1002, Name = "p1002", OwnerEmail = "u1@mail.com", FileIds = new[] {002}};
            var fi = new FileInfo {Id = 002, Name = "testItem2", OwnerEmail = "u1@mail.com", Type = FileType.text};
            var r = new Right{ItemId = 002,Type = RightType.edit,Until = DateTime.Now.AddDays(1),UserEmail = "u1@mail.com" };
            var db = new Dictionary<string, User> {{"u1@mail.com", user1}};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => db[b];
                ShimServiceClient.AllInstances.CreatePackagePackage = (a, b) => 1;
                ShimServiceClient.AllInstances.GetPackageByIdInt32 = (a, b) => p;
                ShimServiceClient.AllInstances.AddToPackageInt32ArrayInt32 = (a, b, c) => { };
                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 = (a, b) => fi;
                ShimServiceClient.AllInstances.AddTagStringInt32 = (a, b, c) => { };
                ShimServiceClient.AllInstances.DeleteFileByIdInt32 = (a, b) => { };
                ShimServiceClient.AllInstances.GetRightStringInt32 = (a, b, c) => r;
                ShimServiceClient.AllInstances.UpdateFileInfoFileInfo = (a, b) => { };
                ShimServiceClient.AllInstances.CreateUserUser = (a, b) => db.Add(b.Email, b);
                ShimServiceClient.AllInstances.DownloadFileByIdInt32 = (a, b) => new byte[] { };

                Controller.LogIn("u1@mail.com", "user1");
                Controller.CreateUser(user5);
                Controller.CreatePackage(p);
                Controller.AddToPackage(new[] { 002 }, 1002);
                Controller.LogOut();
                Controller.LogIn("usertest@mail.com", "mm");
                try
                {
                    Controller.DownloadFileById(002);
                }
                catch (InsufficientRightsException)
                {
                    //should fail 
                }
            }
        }

        //check if editing file metadata is only allowed to correct people
        [TestMethod]
        public void MetaDataTest1()
        {
            var user1 = new User { Email = "u1@mail.com", Password = "user1", Type = UserType.admin};
            var user8 = new User { Email = "u8@mail.com", Password = "user8", Type = UserType.standard };
            var testAdmin = new User { Email = "ua@mail.com", Password = "usera", Type = UserType.admin };
            var p = new Package{Id = 1003, Name = "p1003", OwnerEmail = "u1@mail.com", FileIds = new[] {003}} ;
            var fi = new FileInfo{Id = 003, Name = "testItem", OwnerEmail = "u1@mail.com", Type = FileType.text};
            var update = new FileInfo{Id = 003, Name = "updatedItem", OwnerEmail = "u1@mail.com", Type = FileType.text};
            var r = new Right{ItemId=003, Type = RightType.edit, Until = DateTime.Now.AddDays(1), UserEmail = "u1@mail.com"};

            var db = new Dictionary<string, User> {{"u1@mail.com", user1}, {"ua@mail.com", testAdmin}};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => db[b];
                ShimServiceClient.AllInstances.CreatePackagePackage = (a, b) => 1;
                ShimServiceClient.AllInstances.GetPackageByIdInt32 = (a, b) => p;
                ShimServiceClient.AllInstances.AddToPackageInt32ArrayInt32 = (a, b, c) => { };
                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 = (a, b) => fi;
                ShimServiceClient.AllInstances.AddTagStringInt32 = (a, b, c) => { };
                ShimServiceClient.AllInstances.DeleteFileByIdInt32 = (a, b) => { };
                ShimServiceClient.AllInstances.UpdateFileInfoFileInfo = (a, b) => { };
                ShimServiceClient.AllInstances.GetRightStringInt32 = (a, b, c) => r.UserEmail.Equals(b) ? r : null;
                ShimServiceClient.AllInstances.CreateUserUser = (a, b) => db.Add(b.Email, b);
                
                //User1 uploads a file to a package:
                Controller.LogIn("u1@mail.com", "user1");
                Controller.CreateUser(user8);
                Controller.CreatePackage(p);
                Controller.AddToPackage(new[] { 003 }, 1003);
                Controller.LogOut();

                //User2 tries to update the file user1 just uploaded, but has no editing rights:
                Controller.LogIn("u8@mail.com", "user8");
                try
                {
                    Controller.UpdateFileInfo(update);
                    Assert.Fail();
                }
                catch (InsufficientRightsException){}
                Controller.LogOut();

                //User3 who is an admin tries the same as user2:
                Controller.LogIn("ua@mail.com", "usera");
                Controller.UpdateFileInfo(update);
                
            }
        }

        //check if editing file metadata does what it's supposed to
        [TestMethod]
        public void MetaDataTest2()
        {
            var user1 = new User {Email = "u1@mail.com", Password = "user1", Type = UserType.admin};
            var user8 = new User {Email = "u8@mail.com", Password = "user8", Type = UserType.standard};
            var testAdmin = new User {Email = "ua@mail.com", Password = "usera", Type = UserType.admin};

            var p = new Package {Id = 1003, Name = "p1003", OwnerEmail = "u1@mail.com", FileIds = new[] {003}};
            
            var fi = new FileInfo {Id = 003, Name = "testItem", OwnerEmail = "u1@mail.com", Type = FileType.text};
            var fio = new FileInfo {Id = 004, Name = "testItem4", OwnerEmail = "u1@mail.com", Type = FileType.text};
            var update2 = new FileInfo { Id = 003, Name = "updatedItem2", OwnerEmail = "u1@mail.com", Type = FileType.text};
            var update1 = new FileInfo { Id = 003, Name = "updatedItem1", OwnerEmail = "u1@mail.com", Type = FileType.text};
           
            var r = new Right { ItemId = 003, Type = RightType.edit, Until = DateTime.Now.AddDays(1), UserEmail = "u1@mail.com"};
            var ru = new Right { ItemId = 003, Type = RightType.edit, Until = DateTime.Now.AddDays(1), UserEmail = "u8@mail.com" };

            var db = new Dictionary<string, User> {{"u1@mail.com", user1}, {"ua@mail.com", testAdmin}};
            var dbFiles = new Dictionary<int, FileInfo> {{fi.Id, fi}, {fio.Id, fio}};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => db[b];
                ShimServiceClient.AllInstances.CreatePackagePackage = (a, b) => 1;
                ShimServiceClient.AllInstances.GetPackageByIdInt32 = (a, b) => p;
                ShimServiceClient.AllInstances.AddToPackageInt32ArrayInt32 = (a, b, c) => { };
                ShimServiceClient.AllInstances.GetFileInfoByIdInt32 = (a, b) => dbFiles[b];
                ShimServiceClient.AllInstances.AddTagStringInt32 = (a, b, c) => { };
                ShimServiceClient.AllInstances.DeleteFileByIdInt32 = (a, b) => dbFiles.Remove(b);
                ShimServiceClient.AllInstances.UpdateFileInfoFileInfo = (a, b) => { dbFiles.Remove(b.Id); dbFiles.Add(b.Id, b); };
                ShimServiceClient.AllInstances.GetRightStringInt32 = (a, b, c) => r;
                ShimServiceClient.AllInstances.CreateUserUser = (a, b) => db.Add(b.Email, b);
                ShimServiceClient.AllInstances.GrantRightRight = (a, b) => { };
                ShimServiceClient.AllInstances.DropRightStringInt32 = (a, b, c) => { };

                Controller.LogIn("u1@mail.com", "user1");
                Controller.CreateUser(user8);
                Controller.CreatePackage(p);
                Controller.AddToPackage(new[] { 004 }, 1003);
                Controller.GrantRight(ru);
                Controller.LogOut();

                Controller.LogIn("u8@mail.com", "user8");
                Controller.UpdateFileInfo(update1);
                Assert.AreEqual("updatedItem1", Controller.GetFileInfoById(003).Name);
                Controller.LogOut();

                Controller.LogIn("u1@mail.com", "user1");
                Controller.UpdateFileInfo(update2);
                Assert.AreEqual("updatedItem2",Controller.GetFileInfoById(003).Name);
                Controller.DropRight("u8@mail.com", 003);
                Controller.LogOut();

                Controller.LogIn("u8@mail.com", "user8");
                Controller.UpdateFileInfo(update1);
                   
                
            }
        }

        //check if an admin can edit edit its own account
        [TestMethod]
        public void EditProfile()
        {

            var editUserA = new User {Email = "eusera@mail.com", Password = "eu00", Type = UserType.admin};
            var updatedUserA = new User {Email = "eua@mail.com", Password = "eu00", Type = UserType.admin};
            var updatedUser = new User {Email = "eusera@mail.com", Password = "password", Type = UserType.admin};

            var db = new Dictionary<string, User> {{"eusera@mail.com", editUserA}};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => db[b];
                ShimServiceClient.AllInstances.UpdateUserUser = (a, b) => { db[b.Email] = b; };

                Controller.LogIn("eusera@mail.com","eu00");
                //A user should not be able to change their mail
                try
                {
                    Controller.UpdateUser(updatedUserA);
                    Assert.Fail();
                }
                catch (OriginalNotFoundException) { }
                Controller.UpdateUser(updatedUser);
                Controller.LogOut();
                Controller.LogIn("eusera@mail.com", "password");
            }
        }

        //Tests that a standard user are able to update its own userinfo
        [TestMethod]
        public void EditTest()
        {
            var editUserA = new User {Email = "eusera@mail.com", Password = "eu00", Type = UserType.admin};
            var updatedUserS = new User {Email = "eusers@mail.com", Password = "std00", Type = UserType.standard};
            var editUserS = new User {Email = "eusers@mail.com", Password = "pw", Type = UserType.standard};

            var db = new Dictionary<string, User> {{"eusera@mail.com", editUserA}};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => db[b];
                ShimServiceClient.AllInstances.CreateUserUser = (a, b) => db.Add(b.Email, b);
                ShimServiceClient.AllInstances.UpdateUserUser = (a, b) => { db[b.Email] = b; };

                Controller.LogIn("eusera@mail.com", "eu00");
                Controller.CreateUser(editUserS);
                Controller.LogOut();

                Controller.LogIn("eusers@mail.com", "pw");
                Controller.UpdateUser(updatedUserS);
                Controller.LogOut();

                Controller.LogIn("eusers@mail.com", "std00");
            }
        }

        //Tests if admins are allowed editing standard users info
        [TestMethod]
        public void EditTest1()
        {
            var editUserA = new User {Email = "eusera@mail.com", Password = "eu00", Type = UserType.admin};
            var updatedUserS = new User {Email = "eusers@mail.com", Password = "std00", Type = UserType.standard};
            var editUserS = new User {Email = "eusers@mail.com", Password = "pw", Type = UserType.standard};

            var db = new Dictionary<string, User> {{"eusera@mail.com", editUserA}};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => db[b];
                ShimServiceClient.AllInstances.CreateUserUser = (a, b) => db.Add(b.Email, b);
                ShimServiceClient.AllInstances.UpdateUserUser = (a, b) => { db[b.Email] = b; };

                Controller.LogIn("eusera@mail.com", "eu00");
                Controller.CreateUser(editUserS);
                Controller.UpdateUser(updatedUserS);
                Controller.LogOut();
                Controller.LogIn("eusers@mail.com", "std00");
            }
        }

        [TestMethod]
        public void EditTest3()
        {

            var editUserA = new User {Email = "eusera@mail.com", Password = "eu00", Type = UserType.admin};
            var editingUserS = new User {Email = "editing@mail.com", Password = "ed", Type = UserType.standard};
            var updatedUserS = new User {Email = "eusers@mail.com", Password = "std00", Type = UserType.standard};
            var editUserS = new User {Email = "eusers@mail.com", Password = "pw", Type = UserType.standard};

            var db = new Dictionary<string, User> {{"eusera@mail.com", editUserA}};

            using (ShimsContext.Create())
            {
                ShimServiceClient.AllInstances.GetUserByEmailString = (a, b) => db[b];
                ShimServiceClient.AllInstances.CreateUserUser = (a, b) => db.Add(b.Email, b);
                ShimServiceClient.AllInstances.UpdateUserUser = (a, b) => { db[b.Email] = b; };

                Controller.LogIn("eusera@mail.com", "eu00");
                Controller.CreateUser(editUserS);
                Controller.CreateUser(editingUserS);
                Controller.LogOut();

                Controller.LogIn("editing@mail.com", "ed");
                try
                {
                    Controller.UpdateUser(updatedUserS);
                    Assert.Fail();
                }
                catch (InsufficientRightsException) { }
                Controller.LogOut();

                Controller.LogIn("eusers@mail.com","pw");
            }

        }
    }
}
