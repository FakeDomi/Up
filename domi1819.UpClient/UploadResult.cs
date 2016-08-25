using System.Collections.Generic;

namespace domi1819.UpClient
{
    internal class UploadResult
    {
        internal List<string> FileLinks { get; set; }

        internal int SucceededFiles { get; set; }

        internal int FailedFiles { get; set; }

        internal string Title { get; set; }

        internal string Message { get; set; }

        internal UploadResult()
        {
            this.FileLinks = new List<string>();
        }
    }
}
