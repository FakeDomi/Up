//using domi1819.Proton.Message;

//namespace domi1819.UpServer.Proton.Messages
//{
//    internal class Login : IServerMessage<ProtonConnectionUser>
//    {
//        private readonly UserManager users;

//        public Login(UserManager users)
//        {
//            this.users = users;
//        }

//        public void OnMessage(MessageContext context, ProtonConnectionUser protonConnectionUser)
//        {
//            string userId = context.ReadNextString();
//            string password = context.ReadNextString();

//            if (this.users.Verify(userId, password))
//            {
//                context.WriteNextBool(true);
//                protonConnectionUser.UserId = userId;
//            }
//            else
//            {
//                context.WriteNextBool(false);
//                protonConnectionUser.UserId = null;
//            }
//        }
//    }
//}
