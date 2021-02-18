namespace Domi.UpServer.Server
{
    internal interface IMessage
    {
        void OnMessage(MessageContext context, Connection connection);
    }
}
