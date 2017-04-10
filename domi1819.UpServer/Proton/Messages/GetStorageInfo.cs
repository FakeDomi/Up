using domi1819.Proton;

namespace domi1819.UpServer.Proton.Messages
{
    internal class GetStorageInfo : IMessageDefinition<User>
    {
        public void OnMessage(MessageContext context, User user)
        {
            string userId = user.UserId;

            if (!UpServer.Instance.Users.HasUser(userId))
            {
                context.Disconnect = true;
                return;
            }

            context.WriteNextLong(UpServer.Instance.Users.GetMaxCapacity(userId));
            context.WriteNextLong(UpServer.Instance.Users.GetUsedCapacity(userId));
            context.WriteNextInt(UpServer.Instance.Files.GetFiles(userId).Count);
        }
    }
}
