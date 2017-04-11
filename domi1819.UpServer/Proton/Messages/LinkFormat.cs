//using domi1819.Proton.Message;

//namespace domi1819.UpServer.Proton.Messages
//{
//    internal class LinkFormat : IServerMessage<ProtonConnectionUser>
//    {
//        private readonly FileManager files;

//        public LinkFormat(FileManager files)
//        {
//            this.files = files;
//        }

//        public void OnMessage(MessageContext context, ProtonConnectionUser protonConnectionUser)
//        {
//            context.WriteNextString(this.files.GetLinkFormat());
//        }
//    }
//}
