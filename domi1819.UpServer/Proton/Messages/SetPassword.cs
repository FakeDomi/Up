//using domi1819.Proton.Message;

//namespace domi1819.UpServer.Proton.Messages
//{
//    internal class SetPassword : IServerMessage<ProtonConnectionUser>
//    {
//        private readonly UserManager users;

//        public SetPassword(UserManager users)
//        {
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

//            context.WriteNextBool(this.users.SetPassword(userId, context.ReadNextString()));
//        }
//    }
//}
