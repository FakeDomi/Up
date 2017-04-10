using domi1819.Proton;

namespace domi1819.UpServer.Proton.Messages
{
    internal class LinkFormat : IMessageDefinition<User>
    {
        public void OnMessage(MessageContext context, User user)
        {
            context.WriteNextString(UpServer.Instance.Files.GetLinkFormat());
        }
    }
}
