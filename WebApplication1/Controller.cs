using System;
using System.Linq;
using WebApplication1.ServiceReference1;

namespace WebApplication1
{
    public class Controller
    {
        /// <summary>
        /// The controller acts as a session in which actions
        /// provided by the controller can be performed.
        /// Most of these actions require some form of identification.
        /// This identification is done by tying a User object to the
        /// controller through the LogIn method.
        /// This object is stored in the following private field until
        /// LogOut is called.
        /// </summary>
        private static User _sessionUser;

        /// <summary>Attempts to log in as the User that identifies itself with the given email and password.</summary>
        /// <param name="email">The email that identifies the user.</param>
        /// <param name="password">The password that authorizes the user.</param>
        public static void LogIn(string email, string password)
        {
            if (_sessionUser != null)
                throw new NotLoggedOutException();

            using (var client = new ServiceClient())
            {
                var temp = client.GetUserByEmail(email);

                if (temp == null)
                    throw new NoSuchUserException();

                if (!temp.Password.Equals(password))
                    throw new IncorrectPasswordException();

                _sessionUser = temp;
            }
        }

        /// <summary>Logs out the active user.</summary>
        public static void LogOut()
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            _sessionUser = null;
        }

        /// <summary>Create a new User on the service.</summary>
        /// <param name="newUser">The User object that should be created on the service.</param>
        public static void CreateUser(User newUser)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (_sessionUser.Type != UserType.admin)
                throw new InsufficientRightsException();

            if (newUser == null
                || newUser.Email == null
                || newUser.Password == null)
                throw new InadequateObjectException();

            if (UserExists(newUser.Email))
                throw new KeyOccupiedException();

            using (var client = new ServiceClient())
            {
                client.CreateUser(newUser);
            }
        }

        /// <summary>Look up a User by its email address.</summary>
        /// <param name="email">The email of the User to be returned.</param>
        /// <returns>The User whose Email property matches the given email.</returns>
        public static User GetUserByEmail(string email)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (_sessionUser.Type != UserType.admin && !_sessionUser.Email.Equals(email))
                throw new InsufficientRightsException();

            if(!UserExists(email))
                throw new ObjectNotFoundException();

            using (var client = new ServiceClient())
            {
                return client.GetUserByEmail(email);
            }
        }

        /// <summary>Updates the details of an existing User whose Email property matches the Email property of the one given.</summary>
        /// <param name="updatedUser">The User object which contains the updated details.</param>
        public static void UpdateUser(User updatedUser)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (_sessionUser.Type != UserType.admin
                && !(_sessionUser.Email.Equals(updatedUser.Email) && _sessionUser.Type == updatedUser.Type))
                throw new InsufficientRightsException();

            if (updatedUser == null
                || updatedUser.Email == null
                || updatedUser.Password == null)
                throw new InadequateObjectException();

            if (!UserExists(updatedUser.Email))
                throw new OriginalNotFoundException();

            using (var client = new ServiceClient())
            {
                client.UpdateUser(updatedUser);
            }
        }

        /// <summary>Deletes the User whose Email property matches the given email.</summary>
        /// <param name="email">The email address of the User which should be deleted.</param>
        public static void DeleteUserByEmail(string email)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (_sessionUser.Type != UserType.admin && !_sessionUser.Email.Equals(email))
                throw new InsufficientRightsException();

            if(!UserExists(email))
                throw new ObjectNotFoundException();

            using (var client = new ServiceClient())
            {
                client.DeleteUserByEmail(email);

                if (_sessionUser.Email.Equals(email))
                    LogOut();
            }
        }

        /// <summary>Uploads the binary data and FileInfo contained within the given transfer object.</summary>
        /// <param name="transfer">The object containing the info and data of the file which should be uploaded.</param>
        /// <returns>The Id that has been assigned to the uploaded file.</returns>
        public static int UploadFile(FileTransfer transfer)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (transfer == null
                || transfer.Data == null
                || transfer.Info == null
                || transfer.Info.Name == null
                || transfer.Info.Name.Length < 3)
                throw new InadequateObjectException();

            using (var client = new ServiceClient())
            {
                // NOTE - OwnerEmail field is force-set to the _sessionUser's Email.
                transfer.Info.OwnerEmail = _sessionUser.Email;
                return client.UploadFile(transfer);
            }
        }

        /// <summary>Downloads the binary data of the file whose Id property matches the given fileId.</summary>
        /// <param name="fileId">The Id of the file whose data should be downloaded.</param>
        /// <returns>The binary data of the matching file.</returns>
        public static byte[] DownloadFileById(int fileId)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if(!FileExists(fileId))
                throw new ObjectNotFoundException();

            using (var client = new ServiceClient())
            {
                if (_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(client.GetFileInfoById(fileId).OwnerEmail)
                    && !HasViewRights(fileId))
                    throw new InsufficientRightsException();

                return client.DownloadFileById(fileId);
            }
        }

        /// <summary>Looks up the info of a file by its Id.</summary>
        /// <param name="fileId">The Id of the file whose info should be returned.</param>
        /// <returns>The FileInfo of the file whose Id property matched the given fileId.</returns>
        /// <exception cref="NotLoggedInException">Thrown if you are not logged in.</exception>
        /// <exception cref="InsufficientRightsException">Thrown if you are logged in, but don't have the required rights.</exception>
        /// <exception cref="ObjectNotFoundException">Thrown if the requested object could not be retrieved.</exception>
        public static FileInfo GetFileInfoById(int fileId)
        {
            
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (!FileExists(fileId))
                throw new ObjectNotFoundException();
            
            using (var client = new ServiceClient())
            {
                var info = client.GetFileInfoById(fileId);

                if (_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(info.OwnerEmail)
                    && !HasViewRights(fileId))
                    throw new InsufficientRightsException();

                return info;
            }
        }

        /// <summary>Updates the file that matches updatedInfo's Id with the details contained within it.</summary>
        /// <param name="updatedInfo">The FileInfo object that contains the new info.</param>      
        public static void UpdateFileInfo(FileInfo updatedInfo)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (updatedInfo.Name == null
                || updatedInfo.Name.Length < 3
                || !updatedInfo.OwnerEmail.Equals(_sessionUser.Email))
                throw new InadequateObjectException();

            if(!FileExists(updatedInfo.Id))
                throw new OriginalNotFoundException();

            using (var client = new ServiceClient())
            {
                if (_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(client.GetFileInfoById(updatedInfo.Id).OwnerEmail)
                    && !HasEditRights(updatedInfo.Id))
                    throw new InsufficientRightsException();

                client.UpdateFileInfo(updatedInfo);
            }
        }

        /// <summary>Updates the File with the matching fileId with the givne updatedData.</summary>
        /// <param name="updatedData">The updated data.</param>
        /// <param name="fileId">The Id of the file which data should be updated.</param>
        public static void UpdateFileData(byte[] updatedData, int fileId)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (updatedData == null)
                throw new InadequateObjectException();

            if (!FileExists(fileId))
                throw new OriginalNotFoundException();

            using (var client = new ServiceClient())
            {
                if (_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(client.GetFileInfoById(fileId).OwnerEmail)
                    && !HasEditRights(fileId))
                    throw new InsufficientRightsException();

                client.UpdateFileData(updatedData, fileId);
            }
        }

        /// <summary>Deletes the File with the matching fileId.</summary>
        /// <param name="fileId">The Id of the file which should be deleted.</param>
        public static void DeleteFileById(int fileId)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (!FileExists(fileId))
                throw new ObjectNotFoundException();

            using (var client = new ServiceClient())
            {
                if (_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(client.GetFileInfoById(fileId).OwnerEmail)
                    && !HasEditRights(fileId))
                    throw new InsufficientRightsException();

                client.DeleteFileById(fileId);
            }
        }

        /// <summary>Returns the FileInfos of the Files owned by the User with the given email.</summary>
        /// <param name="email">The email of the User in question.</param>
        /// <returns>The FileInfos of the files owned by the User.</returns>
        public static FileInfo[] GetOwnedFileInfosByEmail(string email)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (_sessionUser.Type != UserType.admin
                && !_sessionUser.Email.Equals(email))
                throw new InsufficientRightsException();

            using (var client = new ServiceClient())
            {
                try
                {
                    var infos = client.GetOwnedFileInfosByEmail(email);

                    if (infos == null)
                        throw new ObjectNotFoundException();

                    return infos;
                }
                catch (Exception)
                {
                    throw new ObjectNotFoundException();
                }
            }
        }

        /// <summary>Adds the given tag to the Item with the given Id.</summary>
        /// <param name="tag">The tag text.</param>
        /// <param name="itemId">The Id of the Item which should be tagged.</param>
        public static void AddTag(string tag, int itemId)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (tag == null || tag.Length < 3)
                throw new InadequateObjectException();

            using (var client = new ServiceClient())
            {
                Item item = null;

                if (FileExists(itemId))
                    item = client.GetFileInfoById(itemId);

                if (PackageExists(itemId))
                    item = client.GetPackageById(itemId);

                if (item == null)
                    throw new ObjectNotFoundException();

                if (_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(item.OwnerEmail)
                    && !HasEditRights(itemId))
                    throw new InsufficientRightsException();

                client.AddTag(tag, itemId);
            }
        }

        /// <summary>Removes the given tag from the Item with the given Id.</summary>
        /// <param name="tag">The tag text.</param>
        /// <param name="itemId">The Id of the Item which should be untagged.</param>
        public static void DropTag(string tag, int itemId)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (tag == null || tag.Length < 3)
                throw new InadequateObjectException();

            using (var client = new ServiceClient())
            {
                Item item = null;

                if (FileExists(itemId))
                    item = client.GetFileInfoById(itemId);

                if (PackageExists(itemId))
                    item = client.GetPackageById(itemId);

                if (item == null)
                    throw new ObjectNotFoundException();

                if (_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(item.OwnerEmail)
                    && !HasEditRights(itemId))
                    throw new InsufficientRightsException();

                client.DropTag(tag, itemId);
            }
        }

        /// <summary>Returns the tags that the Item with the given Id has.</summary>
        /// <param name="itemId">The Id of the Item whose tags should be returned.</param>
        /// <returns>The tags of the matching Item.</returns>
        public static string[] GetTagsByItemId(int itemId)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            using (var client = new ServiceClient())
            {
                Item item = null;

                if (FileExists(itemId))
                    item = client.GetFileInfoById(itemId);

                if (PackageExists(itemId))
                    item = client.GetFileInfoById(itemId);

                if (item == null)
                    throw new ObjectNotFoundException();

                if (_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(item.OwnerEmail)
                    && !HasViewRights(itemId))
                    throw new InsufficientRightsException();

                return client.GetTagsByItemId(itemId);
            }
        }

        /// <summary>Looks up FileInfos with a matching tag.</summary>
        /// <param name="tag">The tag that should be used to look up FileInfos.</param>
        /// <returns>The FileInfos that contain the given tag.</returns>
        public static FileInfo[] GetFileInfosByTag(string tag)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            using (var client = new ServiceClient())
            {
                var infos = client.GetFileInfosByTag(tag);

                return _sessionUser.Type == UserType.admin ? infos : infos.Where(info => info.OwnerEmail.Equals(_sessionUser.Email) || HasViewRights(info.Id)).ToArray();
            }
        }

        /// <summary>Creates the given Package on the service.</summary>
        /// <param name="newPackage">The Package that should be created.</param>
        /// <returns>The Id that the created Package has been given.</returns>
        public static int CreatePackage(Package newPackage)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (newPackage == null
                || newPackage.Name == null
                || newPackage.Name.Length < 3
                || newPackage.FileIds == null
                || newPackage.FileIds.All(FileExists)
                )
                throw new InadequateObjectException();

            using (var client = new ServiceClient())
            {
                // NOTE - OwnerEmail field is force-set to the _sessionUser's Email.
                newPackage.OwnerEmail = _sessionUser.Email;
                return client.CreatePackage(newPackage);
            }
        }

        /// <summary>Look up a Package by its Id.</summary>
        /// <param name="packageId">The Id of the Package that should be returned.</param>
        /// <returns>The Package with the matching packageId.</returns>
        public static Package GetPackageById(int packageId)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (!PackageExists(packageId))
                throw new ObjectNotFoundException();

            using (var client = new ServiceClient())
            {
                var package = client.GetPackageById(packageId);

                if (_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(package.OwnerEmail)
                    && !HasViewRights(packageId))
                    throw new InsufficientRightsException();

                return package;
            }
        }

        /// <summary>Adds some Files to a Package.</summary>
        /// <param name="fileIds">The Ids of the Files that should be added to the Pacakge.</param>
        /// <param name="packageId">The Id of the Package to which the Files should be added.</param>
        public static void AddToPackage(int[] fileIds, int packageId)
        {
            // TODO - What about rights to the Files that are added?
            // Should I be allowed to add a File to which I don't have editing rights?

            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (fileIds == null || fileIds.Length == 0 || fileIds.Any(fileId => !FileExists(fileId)))
                throw new InadequateObjectException();

            if (!PackageExists(packageId))
                throw new ObjectNotFoundException();
            
            using (var client = new ServiceClient())
            {
                var package = client.GetPackageById(packageId);

                if (_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(package.OwnerEmail)
                    && !HasEditRights(packageId))
                    throw new InsufficientRightsException();

                client.AddToPackage(fileIds, packageId);
            }
        }

        /// <summary>Removes some Files from a Package.</summary>
        /// <param name="fileIds">The Ids of the Files that should be removed from the Package.</param>
        /// <param name="packageId">The Id of the Package from which the Files should be removed.</param>
        public static void RemoveFromPackage(int[] fileIds, int packageId)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (fileIds == null || fileIds.Length == 0 || fileIds.Any(fileId => !FileExists(fileId)))
                throw new InadequateObjectException();

            if (!PackageExists(packageId))
                throw new ObjectNotFoundException();

            using (var client = new ServiceClient())
            {
                var package = client.GetPackageById(packageId);

                if (_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(package.OwnerEmail)
                    && !HasEditRights(packageId))
                    throw new InsufficientRightsException();

                client.RemoveFromPackage(fileIds, packageId);
            }
        }

        /// <summary>Deletes a Package by its Id.</summary>
        /// <param name="packageId">The Id of the Package which should be deleted.</param>
        public static void DeletePackageById(int packageId)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (!PackageExists(packageId))
                throw new ObjectNotFoundException();

            using (var client = new ServiceClient())
            {
                if (_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(client.GetPackageById(packageId).OwnerEmail)
                    && !HasEditRights(packageId))
                    throw new InsufficientRightsException();

                client.DeletePackageById(packageId);
            }
        }

        /// <summary>Returns the Packages owned by the User with the given email.</summary>
        /// <param name="email">The email of the User in question.</param>
        /// <returns>The Packages owned by the User.</returns>
        public static Package[] GetOwnedPackagesByEmail(string email)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (_sessionUser.Type != UserType.admin
                && !_sessionUser.Email.Equals(email))
                throw new InsufficientRightsException();

            using (var client = new ServiceClient())
            {
                try
                {
                    var packages = client.GetOwnedPackagesByEmail(email);

                    if (packages == null)
                        throw new ObjectNotFoundException();

                    return packages;
                }
                catch (Exception)
                {
                    throw new ObjectNotFoundException();
                }
            }
        }

        /// <summary>Looks up Packages with a matching tag.</summary>
        /// <param name="tag">The tag that should be used to look up Packages.</param>
        /// <returns>The Packages that contain the given tag.</returns>
        public static Package[] GetPackagesByTag(string tag)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            using (var client = new ServiceClient())
            {
                var packages = client.GetPackagesByTag(tag);

                return _sessionUser.Type == UserType.admin ? packages : packages.Where(package => package.OwnerEmail.Equals(_sessionUser.Email) || HasViewRights(package.Id)).ToArray();
            }
        }

        /// <summary>Adds the given Right to the service.</summary>
        /// <param name="newRight">The Right that should be created on the service.</param>
        public static void GrantRight(Right newRight)
        {
            if(_sessionUser == null)
                throw new NotLoggedInException();

            if(newRight == null 
                || newRight.UserEmail == null 
                || !(FileExists(newRight.ItemId) 
                || PackageExists(newRight.ItemId)) 
                || !UserExists(newRight.UserEmail))
                throw new InadequateObjectException();

            using (var client = new ServiceClient())
            {
                Item item = null;

                if (FileExists(newRight.ItemId))
                    item = client.GetFileInfoById(newRight.ItemId);

                if (PackageExists(newRight.ItemId))
                    item = client.GetPackageById(newRight.ItemId);

                if(item == null)
                    throw new ObjectNotFoundException();

                if(_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(item.OwnerEmail)
                    && !HasEditRights(newRight.ItemId))
                    throw new InsufficientRightsException();

                client.GrantRight(newRight);
            }
        }

        /* TODO - It is noted that it should never be called by other than the controller.
         * Do we even need a public method then?
         * If it should only be used by the controller, it can just use client.GetRight(...) as it already does.
         * 
        /// <summary>Looks up a Right by the email of the User and the Id of the Item involved.</summary>
        /// <param name="email">The Email of the User whom the Right concerns.</param>
        /// <param name="itemId">The Id of the Item which the Right concerns.</param>
        /// <returns>The Right with the matching email and itemId.</returns>
        private static Right GetRight(string email, int itemId)
        {
            throw new NotImplementedException();
        }
         */

        /// <summary>Updates the exising right that has matching UserEmail and ItemId fields, with the rest of the fields from the given updatedRight.</summary>
        /// <param name="updatedRight">The updated Right object.</param>
        public static void UpdateRight(Right updatedRight)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (updatedRight == null || updatedRight.UserEmail == null)
                throw new InadequateObjectException();

            using (var client = new ServiceClient())
            {
                Right right;

                try
                {
                    right = client.GetRight(updatedRight.UserEmail, updatedRight.ItemId);
                }
                catch (Exception)
                {
                    throw new OriginalNotFoundException();
                }

                if(right == null)
                    throw new OriginalNotFoundException();


                Item item = null;

                if (FileExists(updatedRight.ItemId))
                    item = client.GetFileInfoById(updatedRight.ItemId);

                if (PackageExists(updatedRight.ItemId))
                    item = client.GetPackageById(updatedRight.ItemId);

                if(item == null)
                    throw new ObjectNotFoundException();

                if (_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(item.OwnerEmail)
                    && !HasEditRights(updatedRight.ItemId))
                    throw new InsufficientRightsException();

                client.UpdateRight(updatedRight);
            }
        }

        /// <summary>Removes the right associated with the User with the given email address, and the Item with the given Id.</summary>
        /// <param name="email">The Email of the User whom the Right concerns.</param>
        /// <param name="itemId">The Id of the Item which the Right concerns.</param>
        public static void DropRight(string email, int itemId)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (email == null)
                throw new InadequateObjectException();

            using (var client = new ServiceClient())
            {
                Right right;

                try
                {
                    right = client.GetRight(email, itemId);
                }
                catch (Exception)
                {
                    throw new OriginalNotFoundException();
                }

                if (right == null)
                    throw new OriginalNotFoundException();


                Item item = null;

                if (FileExists(itemId))
                    item = client.GetFileInfoById(itemId);

                if (PackageExists(itemId))
                    item = client.GetPackageById(itemId);

                if (item == null)
                    throw new ObjectNotFoundException();

                if (_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(item.OwnerEmail)
                    && !HasEditRights(itemId))
                    throw new InsufficientRightsException();

                client.DropRight(email,itemId);
            }
        }

        /// <summary>Looks up Files by matching their details with a string of text.</summary>
        /// <param name="text">The text which should be contained in the Files.</param>
        /// <returns>The Files that contain the given text somewhere in their details.</returns>
        public static FileInfo[] SearchFileInfos(string text)
        {
            if (text == null)
                throw new InadequateObjectException();

            using (var client = new ServiceClient())
            {
                try
                {
                    var infos = client.SearchFileInfos(text);

                    if (infos == null)
                        throw new ObjectNotFoundException();

                    return infos;
                }
                catch (Exception)
                {
                    throw new ObjectNotFoundException();
                }
            }
        }

        /// <summary>Looks up Packages by matching their details with a string of text.</summary>
        /// <param name="text">The text which should be contained in the Files.</param>
        /// <returns>The Packages that contain the given text somewhere in their details.</returns>
        public static Package[] SearchPackages(string text)
        {
            if (text == null)
                throw new InadequateObjectException();

            using (var client = new ServiceClient())
            {
                try
                {
                    var packages = client.SearchPackages(text);

                    if (packages == null)
                        throw new ObjectNotFoundException();

                    return packages;
                }
                catch (Exception)
                {
                    throw new ObjectNotFoundException();
                }
            }
        }



        /// <summary>
        /// Utility method that checks whether the current user has 
        /// viewing rights to the Item which is identified by the given itemId.
        /// NOTE - This only checks if there exists a Right object stating that
        /// the current user has view/edit rights. It does NOT check if the user
        /// is the owner of the Item, which would also permit viewing.
        /// </summary>
        /// <param name="itemId">The id of the Item in question.</param>
        /// <returns>True of the current user can view the item, false if not.</returns>
        private static bool HasViewRights(int itemId)
        {
            using (var client = new ServiceClient())
            {
                var right = client.GetRight(_sessionUser.Email, itemId);
                // NOTE - We're using the current systems datetime.
                // Users can thus regain rights that have run out, by changing their clocks.
                return right != null && (right.Until == null || DateTime.Compare(right.Until.Value, DateTime.Now) > 0);
            }
        }

        /// <summary>
        /// Utility method that checks whether the current user has 
        /// editing rights to the Item which is identified by the given itemId.
        /// NOTE - This only checks if there exists a Right object stating that
        /// the current user has edit rights. It does NOT check if the user
        /// is the owner of the Item, which would also permit editing.
        /// </summary>
        /// <param name="itemId">The id of the Item in question.</param>
        /// <returns>True of the current user can edit the item, false if not.</returns>
        private static bool HasEditRights(int itemId)
        {
            using (var client = new ServiceClient())
            {
                var right = client.GetRight(_sessionUser.Email, itemId);
                // NOTE - We're using the current systems datetime.
                // Users can thus regain rights that have run out, by changing their clocks.
                return right != null && right.Type == RightType.edit && (right.Until == null || DateTime.Compare(right.Until.Value, DateTime.Now) > 0);
            }
        }

        /// <summary>Used to check if there exists a User with a certain email.</summary>
        /// <param name="email">The email of the User in question.</param>
        /// <returns>True if there exists such a User, false if not.</returns>
        private static bool UserExists(string email)
        {
            using (var client = new ServiceClient())
            {
                try
                {
                    return client.GetUserByEmail(email).Email.Equals(email);
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>Used to check if there exists a File with a certain id.</summary>
        /// <param name="fileId">The id of the File in question.</param>
        /// <returns>True if there exists such a File, false if not.</returns>
        private static bool FileExists(int fileId)
        {
            using (var client = new ServiceClient())
            {
                try
                {
                    return client.GetFileInfoById(fileId).Id == fileId;
                }
                catch (Exception)
                {
                    return false;
                }
                
            }
        }

        /// <summary>Used to check if there exists a Package with a certain id.</summary>
        /// <param name="packageId">The id of the Package in question.</param>
        /// <returns>True if there exists such a Package, false if not.</returns>
        private static bool PackageExists(int packageId)
        {
            using (var client = new ServiceClient())
            {
                try
                {
                    return client.GetPackageById(packageId).Id == packageId;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}