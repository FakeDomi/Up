using System.IO;

namespace domi1819.UpServer
{
    internal class UploadUnit
    {
        internal string Key { get; set; }
        internal string User { get; set; }
        internal string FileName { get; set; }
        internal long Size { get; set; }
        internal long Progress { get; set; }
        internal FileStream FileStream { get; set; }
    }
}
