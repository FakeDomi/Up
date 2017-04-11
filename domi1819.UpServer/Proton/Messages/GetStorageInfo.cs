//using domi1819.Proton.Message;

//namespace domi1819.UpServer.Proton.Messages
//{
//    internal class GetStorageInfo : IServerMessage<ProtonConnectionUser>
//    {
//        private readonly FileManager files;
//        private readonly UserManager users;

//        public GetStorageInfo(FileManager files, UserManager users)
//        {
//            this.files = files;
//            this.users = users;
//        }

//        public void OnMessage(MessageContext context, ProtonConnectionUser protonConnectionUser)
//        {
//            string userId = protonConnectionUser.UserId;

//            if (!this.users.HasUser(userId))
//            {
//                context.Disconnect = true;
//                return;
//            }

//            context.WriteNextLong(this.users.GetMaxCapacity(userId));
//            context.WriteNextLong(this.users.GetUsedCapacity(userId));
//            context.WriteNextInt(this.files.GetFiles(userId).Count);
//        }
//    }
//}
