using System.Collections.Generic;

namespace Domi.UpClient.Uploads
{
    internal class UploadResult
    {
        internal List<string> FileLinks { get; } = new List<string>();

        internal int SucceededFiles { get; set; }

        internal int FailedFiles { get; set; }

        internal string Title { get; set; }

        internal string Message { get; set; }
    }
}
