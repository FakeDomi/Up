using System;
using System.Reflection;

namespace domi1819.UpCore.Utilities
{
    public static class Constants
    {
        public static int Build { get; }
        public static string Version { get; }

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

            public static readonly string FileDbName = "files.nano";
            public static readonly string UserDbName = "users.nano";
        }

        public static class Encryption
        {
            public static readonly int AesBlockSize = 16;

            public static readonly int RsaKeySize = 4096;
            public static readonly int RsaModulusBytes = 512;
            public static readonly int RsaExponentBytes = 3;
            public static readonly int FingerprintSize = 64;

            public static readonly string PrivateKeyFile = "private.key";
            public static readonly string PublicKeyFile = "public.key";
        }

        public static class Network
        {
            public static readonly int MaxFilesPerPacket = 250;

            public static readonly int Timeout = 10000;
            public static readonly int MessageHeaderSize = 8;
            public static readonly int MessageBufferSize = 65536;
        }

        public static class Server
        {
            public static readonly int MinClientBuild = 1;

            public static readonly int DefaultPort = 1819;

            public static readonly int FileIdLength = 8;

            public static readonly string ConfigFileName = "config.xml";

            public static readonly int SniffBytes = 512;
        }

        public static class Client
        {
            public static readonly int ProtocolVersion = 1;

            public static readonly string LocalItemsFolder = "local";

            public static readonly string ConfigFileName = "up.xml";

            public static readonly int UploadQueueMaxItemCount = 7;

            public static readonly int InfoSuccessTimeout = 3000;
            public static readonly int InfoErrorTimeout = 5000;
        }
    }
}
