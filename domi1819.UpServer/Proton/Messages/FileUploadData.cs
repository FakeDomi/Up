//using domi1819.Proton.Message;

//namespace domi1819.UpServer.Proton.Messages
//{
//    internal class FileUploadData : IServerMessage<ProtonConnectionUser>
//    {
//        public void OnMessage(MessageContext context, ProtonConnectionUser protonConnectionUser)
//        {
//            if (protonConnectionUser.UploadUnit != null)
//            {
//                int byteCount = context.ReadNextInt();

//                protonConnectionUser.UploadUnit.FileStream.Write(context.MessageReader.Bytes, context.MessageReader.Offset, byteCount);
//                protonConnectionUser.UploadUnit.Position += byteCount;

//                context.ShouldPush = false;

//                // TODO: kill when too much data is sent
//            }
//            else
//            {
//                context.Disconnect = true;
//            }
//        }
//    }
//}
