using System;
using System.Reflection;

namespace domi1819.UpCore.Utilities
{
    public static class Constants
    {
        public static readonly int BuildYear = 2016;
        public static readonly int DefaultPort = 1819;

        public static readonly int MaxFilesPerPacket = 250;


        public static int Build { get; set; }
        public static string Version { get; set; }
        public static bool IsServer { get; set; }

        static Constants()
        {
            Version version = Assembly.GetEntryAssembly().GetName().Version;

            Build = version.Revision;
            Version = version.ToString();
        }

        public static class Database
        {
            public static readonly int MaxUsernameLength = 32;
            public static readonly int PasswordMaxLength = 256;
        }

        public static class Encryption
        {
            public static readonly int AesBlockSize = 16;
        }

        public static class Network
        {
            public static readonly int Timeout = 10000;
            public static readonly int MessageHeaderSize = 8;
            public static readonly int MessageBufferSize = 65536;
            public static readonly int FileBufferSize = 32768;
        }

        public static class Server
        {
            public static readonly int MinClientBuild = 1;

            public static readonly string FileStorageFolder = "filestor";
            public static readonly string FileTransferFolder = "filetrans";
        }

        public static class Client
        {
            public static readonly string LocalItemsFolder = "local";
            public static readonly string ConfigFileName = "up.xml";
        }
    }
}
