using System;

namespace Domi.UpCore.Network
{
    public class FileDetails
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; }
        public int Downloads { get; set; }
    }
}
