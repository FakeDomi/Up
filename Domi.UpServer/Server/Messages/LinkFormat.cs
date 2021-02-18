namespace Domi.UpServer.Server.Messages
{
    internal class LinkFormat : IMessage
    {
        private readonly FileManager files;

        public LinkFormat(FileManager files)
        {
            this.files = files;
        }

        public void OnMessage(MessageContext context, Connection connection)
        {
            context.WriteNextString(this.files.GetLinkFormat());
        }
    }
}
