namespace domi1819.UpServer.Server
{
    internal interface IMessage
    {
        void OnMessage(MessageContext context, Connection connection);
    }
}
