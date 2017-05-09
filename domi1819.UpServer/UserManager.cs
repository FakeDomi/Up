using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using domi1819.NanoDB;
using domi1819.UpCore.Utilities;
using domi1819.UpServer.Console;

namespace domi1819.UpServer
{
    internal class UserManager
    {
        private const int PasswordMinLength = 6;
        private const int PasswordMaxLength = 256;
        private const int SaltLength = 8;

        private readonly NanoDBFile dbFile;

        private readonly Dictionary<string, long> transferStorageDictionary = new Dictionary<string, long>();

        internal UserManager(UpServer upServer)
        {
            UpConsole.WriteLineRestoreCommand("Initializing user register...");

            this.dbFile = new NanoDBFile(Path.Combine(upServer.Config.DataFolder, Constants.Database.UserDbName));

            InitializeResult initResult = this.dbFile.Initialize();

            if (initResult == InitializeResult.VersionMismatch)
            {
                //logger.Log("User database could not be read because it was saved in an unsupported format. Please fix or delete the file database.", LogLevel.Error);
                throw new Exception("Database version not supported.");
            }

            bool newDb = false;

            /*else*/
            if (initResult != InitializeResult.Success)
            {
                UpConsole.WriteLineRestoreCommand("User database does not exist or could not be read. Creating a new one...");

                // UserName - PasswordHash - Salt - MaxCapacity - CurCapacity - Admin
                this.dbFile.CreateNew(new NanoDBLayout(NanoDBElement.String32, NanoDBElement.DataBlob32, NanoDBElement.String8, NanoDBElement.Long, NanoDBElement.Long, NanoDBElement.Bool), Index.UserName);

                newDb = true;
            }

            LoadResult loadResult = this.dbFile.Load(Index.UserName);

            if (loadResult != LoadResult.Okay && loadResult != LoadResult.HasDuplicates)
            {
                //logger.Log("User database could not be read because it seems to be corrupt. Please fix or delete the file database.", LogLevel.Error);
                throw new Exception("Database file corrupt.");
            }

            this.dbFile.Bind();

            if (newDb)
            {
                UpConsole.WriteLineRestoreCommand("Creating admin account...\n Username:  admin\n Password:  password\nDon't forget to change the password!!");
                this.CreateUser("admin", "password", 10737418240 /*314572800*/, true);
            }
        }

        internal bool HasUser(string name)
        {
            return this.dbFile.ContainsKey(name);
        }

        internal bool CreateUser(string name, string password, long capacity, bool admin)
        {
            string salt = Util.GetRandomString(SaltLength);
            byte[] hash = Util.Hash(password, salt);

            this.dbFile.AddLine(name, hash, salt, capacity, 0L, admin);

            return true;
        }

        internal bool Verify(string user, string password)
        {
            if (this.HasUser(user))
            {
                NanoDBLine line = this.dbFile.GetLine(user);

                string salt = (string)line[Index.Salt];
                byte[] savedHash = (byte[])line[Index.PasswdHash];
                byte[] verificationHash = Util.Hash(password, salt);

                return !savedHash.Where((t, i) => t != verificationHash[i]).Any();
            }

            return false;
        }

        internal bool SetPassword(string user, string password)
        {
            if (this.HasUser(user) && password.Length <= PasswordMaxLength && password.Length >= PasswordMinLength)
            {
                string salt = Util.GetRandomString(SaltLength);
                byte[] hash = Util.Hash(password, salt);

                NanoDBLine line = this.dbFile.GetLine(user);

                line[Index.PasswdHash] = hash;
                line[Index.Salt] = salt;

                return true;
            }

            return false;
        }

        internal long GetMaxCapacity(string user)
        {
            return this.HasUser(user) ? (long)this.dbFile.GetLine(user)[Index.MaxCapacity] : -1;
        }

        internal void SetMaxCapacity(string user, long capacity)
        {
            if (this.HasUser(user))
            {
                this.dbFile.GetLine(user)[Index.MaxCapacity] = capacity;
            }
        }

        internal long GetUsedCapacity(string user)
        {
            return this.HasUser(user) ? (long)this.dbFile.GetLine(user)[Index.CurCapacity] : -1;
        }

        public void AddUsedCapacity(string user, long fileSize)
        {
            this.dbFile.GetLine(user)[Index.CurCapacity] = this.GetUsedCapacity(user) + fileSize;
        }

        public long GetTransferStorage(string user)
        {
            return this.transferStorageDictionary.TryGetValue(user, out long capacity) ? capacity : 0;
        }

        public void AddTransferStorage(string user, long value)
        {
            long newValue = 0;

            if (this.transferStorageDictionary.ContainsKey(user))
            {
                newValue = this.transferStorageDictionary[user];
            }

            newValue += value;

            if (newValue < 0)
            {
                newValue = 0;
            }

            this.transferStorageDictionary[user] = newValue;
        }

        public void RemoveTransferStorage(string user, long value)
        {
            this.AddTransferStorage(user, 0 - value);
        }

        public long GetFreeCapacity(string user)
        {
            if (!this.HasUser(user))
            {
                return -1;
            }

            return this.GetMaxCapacity(user) - this.GetUsedCapacity(user) - this.GetTransferStorage(user);
        }

        internal bool IsAdmin(string user)
        {
            return this.HasUser(user) && (bool)this.dbFile.GetLine(user)[Index.Admin];
        }

        internal bool IsValidName(string name)
        {
            return Encoding.UTF8.GetByteCount(name) <= Constants.Database.MaxUsernameLength;
        }

        internal bool IsValidPassword(string password)
        {
            return password.Length >= PasswordMinLength && password.Length <= Constants.Database.PasswordMaxLength;
        }

        internal void Shutdown()
        {
            this.dbFile.Unbind();
        }

        private static class Index
        {
            internal const int UserName = 0;
            internal const int PasswdHash = 1;
            internal const int Salt = 2;
            internal const int MaxCapacity = 3;
            internal const int CurCapacity = 4;
            internal const int Admin = 5;
        }
    }
}
