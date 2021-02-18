namespace Domi.UpServer.Server.Messages
{
    internal class FileUploadData : IMessage
    {
        public void OnMessage(MessageContext context, Connection connection)
        {
            if (connection.UploadUnit != null)
            {
                int byteCount = context.ReadNextInt();

                connection.UploadUnit.FileStream.Write(context.MessageReader.Bytes, context.MessageReader.Index, byteCount);
                connection.UploadUnit.Position += byteCount;

                context.ShouldPush = false;

                // TODO: kill when too much data is sent
            }
            else
            {
                context.Disconnect = true;
            }
        }
    }
}
