using domi1819.Proton;

namespace domi1819.UpServer.Proton.Messages
{
    internal class SetPassword : IMessageDefinition<User>
    {
        public void OnMessage(MessageContext context, User user)
        {
            string userId = user.UserId;

            if (!UpServer.Instance.Users.HasUser(userId))
            {
                context.Disconnect = true;
                return;
            }

            context.WriteNextBool(UpServer.Instance.Users.SetPassword(userId, context.ReadNextString()));
        }
    }
}
