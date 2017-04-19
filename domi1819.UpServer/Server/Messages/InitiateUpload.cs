using System.IO;
using domi1819.UpCore.Utilities;

namespace domi1819.UpServer.Server.Messages
{
    internal class InitiateUpload : IMessage
    {
        private readonly FileManager files;
        private readonly UserManager users;

        public InitiateUpload(FileManager files, UserManager users)
        {
            this.files = files;
            this.users = users;
        }

        public void OnMessage(MessageContext context, Connection connection)
        {
            string userId = connection.UserId;

            if (!this.users.HasUser(userId) || connection.UploadUnit != null)
            {
                context.Disconnect = true;
                return;
            }

            string fileName = context.ReadNextString();
            long fileSize = context.ReadNextLong();

            if (!this.files.IsValidFileName(fileName))
            {
                context.Disconnect = true;
                return;
            }

            if (fileSize >= 0 && this.users.GetFreeCapacity(userId) >= fileSize)
            {
                this.users.AddTransferStorage(userId, fileSize);

                string tempFile;

                do
                {
                    tempFile = Path.Combine(UpServer.Instance.Config.FileTransferFolder, $"{Util.GetRandomString(8)}.tmp");
                } while (File.Exists(tempFile));

                connection.UploadUnit = new UploadUnit { FileName = fileName, TempFile = tempFile, Size = fileSize, FileStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write) };

                context.WriteNextBool(true);
            }
            else
            {
                context.WriteNextBool(false);
            }
        }
    }
}
