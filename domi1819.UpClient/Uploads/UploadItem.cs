using System.IO;

namespace domi1819.UpClient.Uploads
{
    internal struct UploadItem
    {
        internal string FolderPath { get; set; }

        internal string FileName { get; set; }

        internal string FileExtension { get; set; }

        internal bool TemporaryFile { get; set; }

        internal UploadItem(string folderPath, string fileName, string fileExtension, bool temporaryFile = false)
        {
            this.FolderPath = folderPath;
            this.FileName = fileName;
            this.FileExtension = fileExtension;
            this.TemporaryFile = temporaryFile;
        }

        internal UploadItem(string fullPath)
        {
            this.FolderPath = Path.GetDirectoryName(fullPath);
            this.FileName = Path.GetFileNameWithoutExtension(fullPath);
            this.FileExtension = Path.GetExtension(fullPath);

            this.TemporaryFile = false;
        }
    }
}
