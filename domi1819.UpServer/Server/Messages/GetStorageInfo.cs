namespace domi1819.UpServer.Server.Messages
{
    internal class GetStorageInfo : IMessage
    {
        private readonly FileManager files;
        private readonly UserManager users;

        public GetStorageInfo(FileManager files, UserManager users)
        {
            this.files = files;
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

            context.WriteNextLong(this.users.GetMaxCapacity(userId));
            context.WriteNextLong(this.users.GetUsedCapacity(userId));
            context.WriteNextInt(this.files.GetFiles(userId).Count);
        }
    }
}
