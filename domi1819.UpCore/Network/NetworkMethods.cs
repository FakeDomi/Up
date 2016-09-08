namespace domi1819.UpCore.Network
{
    public static class NetworkMethods
    {
        /// <summary>
        /// Signal when a connection was closed.
        /// </summary>
        public const int ConnectionClosed = -1;

        /// <summary>
        /// Request:
        ///   Connection test (int)
        /// Response:
        ///   Connection test (int)
        ///   Minimum client build (int)
        /// </summary>
        public const int GetServerVersion = 0;

        /// <summary>
        /// Request:
        ///   UserId (string)
        ///   Password (string)
        /// Response:
        ///   Login successful? (bool)
        /// </summary>
        public const int Login = 1;

        /// <summary>
        /// Request:
        ///   (Empty)
        /// Response:
        ///   Max capacity (long)
        ///   Used capacity (long)
        ///   Stored files (int)
        /// </summary>
        public const int GetStorageInfo = 2;

        /// <summary>
        /// Request:
        ///   New password (string)
        /// Response:
        ///   Successful? (bool)
        /// </summary>
        public const int SetPassword = 3;

        /// <summary>
        /// Request:
        ///   File size (long)
        /// Response:
        ///   Enough space? (bool)
        ///   Transfer key (string)
        /// </summary>
        public const int InitiateUpload = 4;

        /// <summary>
        /// Request:
        ///   Transfer key (string)
        ///   Byte count (int)
        ///   Bytes (byte...)
        /// Response:
        ///   (Empty)
        /// </summary>
        public const int UploadPacket = 5;

        /// <summary>
        /// Request:
        ///   Transfer key (string)
        ///   Original file name (string)
        /// Response:
        ///   File link (string)
        /// </summary>
        public const int FinishUpload = 6;

        /// <summary>
        /// Request:
        ///   File offset (int)
        ///   From date (DateTime)
        ///   To date (DateTime)
        ///   From size (long)
        ///   To size (long)
        ///   Filename filter (string)
        ///   Filter match mode (int)
        /// Response:
        ///   File count (int)
        ///   { File id (string), File name (string), File size (long), Upload date (DateTime), Downloads (int) }...
        ///   Resume location (int) [-1 when the end is reached]
        /// </summary>
        public const int ListFiles = 7;

        /// <summary>
        /// Request:
        ///   File Id (string)
        /// Response:
        ///   Success? (bool)
        /// </summary>
        public const int DeleteFile = 8;

        /// <summary>
        /// Request:
        ///   (Empty)
        /// Response:
        ///   Link format (string)
        /// </summary>
        public const int LinkFormat = 9;
    }
}
