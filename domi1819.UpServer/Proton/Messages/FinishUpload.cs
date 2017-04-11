//using System.IO;
//using domi1819.Proton.Message;

//namespace domi1819.UpServer.Proton.Messages
//{
//    internal class FinishUpload : IServerMessage<ProtonConnectionUser>
//    {
//        private readonly FileManager files;
//        private readonly UserManager users;
//        private readonly ServerConfig config;

//        public FinishUpload(FileManager files, UserManager users, ServerConfig config)
//        {
//            this.files = files;
//            this.users = users;
//            this.config = config;
//        }

//        public void OnMessage(MessageContext context, ProtonConnectionUser protonConnectionUser)
//        {
//            UploadUnit unit = protonConnectionUser.UploadUnit;

//            if (protonConnectionUser.UserId != null && unit != null)
//            {
//                unit.FileStream.Close();
//                unit.FileStream.Dispose();

//                string fileId = this.files.GetNewFileId();

//                this.files.AddFile(fileId, unit.FileName, protonConnectionUser.UserId, unit.Size);
//                File.Move(unit.TempFile, Path.Combine(this.config.FileStorageFolder, fileId));
//                this.users.RemoveTransferStorage(protonConnectionUser.UserId, unit.Size);
//                this.files.SetDownloadable(fileId, true);

//                context.WriteNextString(string.Format(this.files.GetLinkFormat(), fileId));

//                protonConnectionUser.UploadUnit = null;
//            }
//            else
//            {
//                context.Disconnect = true;
//            }
//        }
//    }
//}
