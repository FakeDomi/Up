using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using domi1819.NanoDB;
using domi1819.UpCore.Network;
using domi1819.UpCore.Utilities;

namespace domi1819.UpServer
{
    internal class FileManager
    {
        private readonly NanoDBFile dbFile;
        private readonly List<NanoDBLine> emptyFilterList = new List<NanoDBLine>(0);
        
        internal FileManager(UpServer upServer)
        {
            upServer.Console.WriteLine("Initializing file manager...");

            this.dbFile = new NanoDBFile(Path.Combine(upServer.Config.DataFolder, Constants.Database.FileDbName));

            InitializeResult initResult = this.dbFile.Initialize();

            if (initResult == InitializeResult.VersionMismatch)
            {
                //logger.Log("File database could not be read because it was saved in an unsupported format. Please fix or delete the file database.");
                throw new Exception("Database version not supported.");
            }
            
            if (initResult != InitializeResult.Success)
            {
                upServer.Console.WriteLine("File database does not exist or could not be read. Creating a new one...");

                // FileId - Filename - Downloads - Owner - Filesize - UploadDate - Downloadable
                this.dbFile.CreateNew(new NanoDBLayout(NanoDBElement.String8, NanoDBElement.String128, NanoDBElement.Int, NanoDBElement.String32, NanoDBElement.Long, NanoDBElement.DateTime, NanoDBElement.Bool), Index.FileId, Index.Owner);
            }

            LoadResult loadResult = this.dbFile.Load(Index.FileId, Index.Owner);

            if (loadResult != LoadResult.Okay && loadResult != LoadResult.HasDuplicates)
            {
                //logger.Log("File database could not be read because it seems to be corrupt. Please fix or delete the file database.");
                throw new Exception("Database file corrupt.");
            }

            this.dbFile.Bind();
        }

        internal bool FileExists(string key)
        {
            return this.dbFile.ContainsKey(key);
        }

        internal string GetNewFileId()
        {
            string suggestion = Util.GetRandomString(Constants.Server.FileIdLength);

            while (this.FileExists(suggestion))
            {
                suggestion = Util.GetRandomString(Constants.Server.FileIdLength);
            }

            return suggestion;
        }

        internal bool AddFile(string key, string fileName, string owner, long fileSize)
        {
            bool success = this.dbFile.AddLine(key, fileName, 0, owner, fileSize, DateTime.Now, false) != null;

            if (success)
            {
                UpServer.Instance.Users.AddUsedCapacity(owner, fileSize);
            }

            return success;
        }

        internal bool IsOwner(string key, string user)
        {
            return this.dbFile.ContainsKey(key) && this.dbFile.GetLine(key)[Index.Owner].Equals(user);
        }

        internal string GetFileName(string key)
        {
            return (string)this.dbFile.GetLine(key)[Index.FileName];
        }

        internal long GetFileSize(string key)
        {
            return (long)this.dbFile.GetLine(key)[Index.FileSize];
        }

        internal DateTime GetUploadDate(string key)
        {
            return (DateTime)this.dbFile.GetLine(key)[Index.UploadDate];
        }

        internal int GetFileDownloads(string key)
        {
            return (int?)this.dbFile.GetLine(key)[Index.Downloads] ?? 0;
        }

        internal bool GetDownloadableFlag(string key)
        {
            return (bool?)this.dbFile.GetLine(key)[Index.DirectDownloadFlag] ?? false;
        }

        internal List<NanoDBLine> GetFiles(string user)
        {
            return this.dbFile.GetSortedList(user) ?? this.emptyFilterList;
        }
        
        internal void SetDownloadable(string key, bool downloadable)
        {
            this.dbFile.GetLine(key)[Index.DirectDownloadFlag] = downloadable;
        }

        internal void IncrementDownloadCount(string key)
        {
            this.dbFile.GetLine(key)[Index.Downloads] = this.GetFileDownloads(key) + 1;
        }

        internal bool SerializeFileInfo(NanoDBLine line, MessageSerializer serializer, DateTime fromDate, DateTime toDate, long fromSize, long toSize, string filter, int filterMatchMode)
        {
            long fileSize = (long)line[Index.FileSize];

            if (fileSize < fromSize || fileSize > toSize)
            {
                return false;
            }

            string fileName = (string)line[Index.FileName];

            if (filterMatchMode > 0)
            {
                string cmpFileName = fileName.ToLower();
                string cmpFilter = filter.ToLower();

                if (filterMatchMode == 1 && !cmpFileName.Equals(cmpFilter))
                {
                    return false;
                }

                if (filterMatchMode == 2 && !cmpFileName.Contains(cmpFilter))
                {
                    return false;
                }

                if (filterMatchMode == 3 && !cmpFileName.StartsWith(cmpFilter))
                {
                    return false;
                }

                if (filterMatchMode == 4 && !cmpFileName.EndsWith(cmpFilter))
                {
                    return false;
                }

                if (filterMatchMode == 5 && !Regex.Match(fileName, filter).Success)
                {
                    return false;
                }
            }

            DateTime uploadDate = (DateTime)line[Index.UploadDate];

            if (uploadDate < fromDate || uploadDate > toDate)
            {
                return false;
            }

            serializer.WriteNextString((string)line[Index.FileId]);
            serializer.WriteNextString(fileName);
            serializer.WriteNextLong(fileSize);
            serializer.WriteNextDateTime(uploadDate);
            serializer.WriteNextInt((int)line[Index.Downloads]);

            return true;
        }

        internal bool IsValidFileName(string fileName)
        {
            // TODO: Pass this to the NanoDB

            return Encoding.UTF8.GetByteCount(fileName) <= 128;
        }

        internal void Shutdown()
        {
            this.dbFile.Unbind();
        }

        internal string GetLinkFormat()
        {
            ServerConfig config = UpServer.Instance.Config;

            if (!string.IsNullOrEmpty(config.UrlOverride))
            {
                return $"{config.UrlOverride}/{{0}}";
            }

            string port = config.HttpServerPort == 80 ? "" : $":{config.HttpServerPort}";

            return $"http://{config.HostName}{port}/d/{{0}}";
        }
        
        private static class Index
        {
            internal const int FileId = 0;
            internal const int FileName = 1;
            internal const int Downloads = 2;
            internal const int Owner = 3;
            internal const int FileSize = 4;
            internal const int UploadDate = 5;
            internal const int DirectDownloadFlag = 6;
        }
    }
}
