using System;
using WebApplication1.ServiceReference1;

namespace WebApplication1
{
    public class Controller
    {
        private static User _sessionUser;

        public static void LogIn(string email, string password)
        {
            if (_sessionUser != null)
            {
                // NOTE - Should we ask for an explicit logout, before logging in as another user?
                return;
            }

            using (var client = new ServiceClient())
            {
                var temp = client.GetUserByEmail(email);
                if (temp == null)
                {
                    // NOTE - Should we throw an exception if the user does not exist?
                    return;
                }

                if (temp.Password.Equals(password))
                {
                    _sessionUser = temp;
                }
                // NOTE - Should we throw an exception if the password is a mismatch?
            }
        }

        public static void LogOut()
        {
            // NOTE - Should we throw an exception if the user is already logged out.
            _sessionUser = null;
        }

        public static void CreateUser(User newUser)
        {
            if (_sessionUser == null)
            {
                // NOTE - What to do if you aren't log in?
                return;
            }

            if (_sessionUser.Type != UserType.admin)
            {
                // NOTE - What to do if you aren't admin?
                return;
            }

            using (var client = new ServiceClient())
            {
                client.CreateUser(newUser);
            }
            
        }

        public static User GetUserByEmail(string email)
        {
            if (_sessionUser == null)
            {
                // NOTE - What if you aren't logged in?
                return null;
            }

            if (_sessionUser.Type != UserType.admin && !_sessionUser.Email.Equals(email))
            {
                // NOTE - What if you aren't admin, and don't have the matching email?
                return null;
            }

            using (var client = new ServiceClient())
            {
                return client.GetUserByEmail(email);
            }
        }

        public static void UpdateUser(User updatedUser)
        {
            if (_sessionUser == null)
            {
                // NOTE - What if you aren't logged in?
                return;
            }

            if (_sessionUser.Type != UserType.admin && !(_sessionUser.Email.Equals(updatedUser.Email) && _sessionUser.Type != updatedUser.Type))
            {
                // NOTE - What if you aren't admin, and don't have the matching email and type? (non-admin's shouldn't be able to promote themselves.)
                return;
            }

            using (var client = new ServiceClient())
            {
                client.UpdateUser(updatedUser);
            }
        }

        public static void DeleteUserByEmail(string email)
        {
            if (_sessionUser == null)
            {
                // NOTE - What if you aren't logged in?
                return;
            }

            if (_sessionUser.Type != UserType.admin && !_sessionUser.Email.Equals(email))
            {
                // NOTE - What if you aren't admin, and don't have the matching email?
                return;
            }

            using (var client = new ServiceClient())
            {
                client.DeleteUserByEmail(email);
                if (_sessionUser.Email.Equals(email))
                {
                    // If you're deleting the account you're currently logged in as, you will be logged out.
                    LogOut();
                }
            }
        }

        public static void UploadFile(FileTransfer transfer)
        {
            if (_sessionUser == null)
            {
                // NOTE - What if you aren't logged in?
                return; 
            }

            if (transfer == null 
                || transfer.Data == null 
                || transfer.Info == null 
                || transfer.Info.Name == null
                || transfer.Info.Name.Length < 3)
            {
                // NOTE - What if the transfer is not formatted correctly?
                return;
            }

            using (var client = new ServiceClient())
            {
                transfer.Info.OwnerEmail = _sessionUser.Email;
                client.UploadFile(transfer);
            }
        }

        public static byte[] DownloadFileById(int id)
        {
            if (_sessionUser == null)
            {
                // NOTE - What if you aren't logged in?
                return null;
            }

            using (var client = new ServiceClient())
            {
                if (_sessionUser.Type != UserType.admin 
                    && !_sessionUser.Email.Equals(client.GetFileInfoById(id).OwnerEmail) 
                    && !HasViewRights(id))
                {
                    // NOTE - Throw some exceptions?
                    return null;
                }

                var file = client.DownloadFileById(id);
                // NOTE - What if file is null?
                // NOTE - What if the item with matching id is a package?
                return file;
            }
        }

        public static FileInfo GetFileInfoById(int id)
        {
            if (_sessionUser == null)
            {
                // NOTE - What if you aren't logged in?
                return null;
            }

            using (var client = new ServiceClient())
            {
                if (_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(client.GetFileInfoById(id).OwnerEmail)
                    && !HasViewRights(id))
                {
                    // NOTE - Throw some exceptions?
                    return null;
                }

                var info = client.GetFileInfoById(id);
                // NOTE - What if info is null?
                // NOTE - What if the item with matching id is a package?
                return info;
            }            
        }

        public static void UpdateFileInfo(FileInfo updatedInfo)
        {
            if (_sessionUser == null)
            {
                // NOTE - What if you aren't logged in?
                return;   
            }

            if (updatedInfo.Name == null
                || updatedInfo.Name.Length < 3
                || !updatedInfo.OwnerEmail.Equals(_sessionUser.Email))
            {
                // NOTE - Info not properly formatted. Throw an exception?
                return;
            }

            using (var client = new ServiceClient())
            {
                if (_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(client.GetFileInfoById(updatedInfo.Id).OwnerEmail)
                    && !HasEditRights(updatedInfo.Id))
                {
                    // NOTE - Throw some exceptions?
                    return;
                }

                client.UpdateFileInfo(updatedInfo);
            }
        }

        public static void UpdateFileData(byte[] updatedData, int fileId)
        {
            if (_sessionUser == null)
            {
                // NOTE - What if you aren't logged in?
                return;
            }

            if (updatedData == null)
            {
                // NOTE - No data, throw exception?
                return;
            }

            using (var client = new ServiceClient())
            {
                if (_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(client.GetFileInfoById(fileId).OwnerEmail)
                    && !HasEditRights(fileId))
                {
                    // NOTE - Throw some exceptions?
                    return;
                }

                client.UpdateFileData(updatedData, fileId);
            }
        }

        public static void DeleteFileById(int fileId)
        {
            if (_sessionUser == null)
            {
                // NOTE - What if you aren't logged in?
                return;
            }

            using (var client = new ServiceClient())
            {
                if (_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(client.GetFileInfoById(fileId).OwnerEmail)
                    && !HasEditRights(fileId))
                {
                    // NOTE - Throw some exceptions?
                    return;
                }

                client.DeleteFileById(fileId);
            }
        }

        public FileInfo[] GetOwnedFileInfosByEmail(string email)
        {
            if (_sessionUser == null)
            {
                // NOTE - What if you aren't logged in?
                return null;
            }

            if (_sessionUser.Type != UserType.admin
                && !_sessionUser.Email.Equals(email))
            {
                // NOTE - Throw some exceptions?
                return null;
            }

            using (var client = new ServiceClient())
            {
                return client.GetOwnedFileInfosByEmail(email);
            }                
        }









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
    }
}