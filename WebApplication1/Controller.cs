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

        public static void LogOut()
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            _sessionUser = null;
        }

        public static void CreateUser(User newUser)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (_sessionUser.Type != UserType.admin)
                throw new InsufficientRightsException();

            using (var client = new ServiceClient())
            {
                client.CreateUser(newUser);
            }
            
        }

        public static User GetUserByEmail(string email)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (_sessionUser.Type != UserType.admin && !_sessionUser.Email.Equals(email))
                throw new InsufficientRightsException();

            using (var client = new ServiceClient())
            {
                return client.GetUserByEmail(email);
            }
        }

        public static void UpdateUser(User updatedUser)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (_sessionUser.Type != UserType.admin 
                && !(_sessionUser.Email.Equals(updatedUser.Email) && _sessionUser.Type == updatedUser.Type))
                throw new InsufficientRightsException();

            using (var client = new ServiceClient())
            {
                client.UpdateUser(updatedUser);
            }
        }

        public static void DeleteUserByEmail(string email)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (_sessionUser.Type != UserType.admin && !_sessionUser.Email.Equals(email))
                throw new InsufficientRightsException();

            using (var client = new ServiceClient())
            {
                client.DeleteUserByEmail(email);

                if (_sessionUser.Email.Equals(email))
                    LogOut();
            }
        }

        public static void UploadFile(FileTransfer transfer)
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
                transfer.Info.OwnerEmail = _sessionUser.Email;
                client.UploadFile(transfer);
            }
        }

        public static byte[] DownloadFileById(int id)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            using (var client = new ServiceClient())
            {
                if (_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(client.GetFileInfoById(id).OwnerEmail)
                    && !HasViewRights(id))
                    throw new InsufficientRightsException();

                var file = client.DownloadFileById(id);
                // NOTE - What if file is null?
                // NOTE - What if the item with matching id is a package?
                return file;
            }
        }

        public static FileInfo GetFileInfoById(int id)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            using (var client = new ServiceClient())
            {
                if (_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(client.GetFileInfoById(id).OwnerEmail)
                    && !HasViewRights(id))
                    throw new InsufficientRightsException();

                var info = client.GetFileInfoById(id);
                // NOTE - What if info is null?
                // NOTE - What if the item with matching id is a package?
                return info;
            }            
        }

        public static void UpdateFileInfo(FileInfo updatedInfo)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (updatedInfo.Name == null
                || updatedInfo.Name.Length < 3
                || !updatedInfo.OwnerEmail.Equals(_sessionUser.Email))
                throw new InadequateObjectException();

            using (var client = new ServiceClient())
            {
                if (_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(client.GetFileInfoById(updatedInfo.Id).OwnerEmail)
                    && !HasEditRights(updatedInfo.Id))
                    throw new InsufficientRightsException();

                client.UpdateFileInfo(updatedInfo);
            }
        }

        public static void UpdateFileData(byte[] updatedData, int fileId)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (updatedData == null)
                throw new InadequateObjectException();

            using (var client = new ServiceClient())
            {
                if (_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(client.GetFileInfoById(fileId).OwnerEmail)
                    && !HasEditRights(fileId))
                    throw new InsufficientRightsException();

                client.UpdateFileData(updatedData, fileId);
            }
        }

        public static void DeleteFileById(int fileId)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            using (var client = new ServiceClient())
            {
                if (_sessionUser.Type != UserType.admin
                    && !_sessionUser.Email.Equals(client.GetFileInfoById(fileId).OwnerEmail)
                    && !HasEditRights(fileId))
                    throw new InsufficientRightsException();

                client.DeleteFileById(fileId);
            }
        }

        public FileInfo[] GetOwnedFileInfosByEmail(string email)
        {
            if (_sessionUser == null)
                throw new NotLoggedInException();

            if (_sessionUser.Type != UserType.admin
                && !_sessionUser.Email.Equals(email))
                throw new InsufficientRightsException();

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