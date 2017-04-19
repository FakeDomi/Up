namespace domi1819.UpServer.Server.Messages
{
    internal class Login : IMessage
    {
        private readonly UserManager users;

        public Login(UserManager users)
        {
            this.users = users;
        }

        public void OnMessage(MessageContext context, Connection connection)
        {
            string userId = context.ReadNextString();
            string password = context.ReadNextString();

            if (this.users.Verify(userId, password))
            {
                context.WriteNextBool(true);
                connection.UserId = userId;
            }
            else
            {
                context.WriteNextBool(false);
                connection.UserId = null;
            }
        }
    }
}
