using domi1819.Proton;

namespace domi1819.UpServer.Proton.Messages
{
    internal class Login : IMessageDefinition<User>
    {
        public void OnMessage(MessageContext context, User user)
        {
            string userId = context.ReadNextString();
            string password = context.ReadNextString();

            if (UpServer.Instance.Users.Verify(userId, password))
            {
                context.WriteNextBool(true);
                user.UserId = userId;
            }
            else
            {
                context.WriteNextBool(false);
                user.UserId = null;
            }
        }
    }
}
