using System.IO;

namespace domi1819.UpServer.Server
{
    internal class UploadUnit
    {
        internal string FileName { get; set; }

        internal string TempFile { get; set; }

        internal long Size { get; set; }

        internal long Position { get; set; }

        internal FileStream FileStream { get; set; }
    }
}
