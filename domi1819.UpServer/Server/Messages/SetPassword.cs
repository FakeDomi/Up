namespace domi1819.UpServer.Server.Messages
{
    internal class SetPassword : IMessage
    {
        private readonly UserManager users;

        public SetPassword(UserManager users)
        {
            this.users = users;
        }

        public void OnMessage(MessageContext context, Connection connection)
        {
            string userId = connection.UserId;

            if (!this.users.HasUser(userId))
            {
                context.Disconnect = true;
                return;
            }

            context.WriteNextBool(this.users.SetPassword(userId, context.ReadNextString()));
        }
    }
}
