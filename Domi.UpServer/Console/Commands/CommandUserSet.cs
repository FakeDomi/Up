namespace Domi.UpServer.Console.Commands
{
    internal class CommandUserSet : BaseCommand
    {
        public CommandUserSet(BaseCommand parent, UserManager users) : base(parent)
        {
            this.SubCommands.Add("password", new CommandUserSetPassword(this, users));
            this.SubCommands.Add("capacity", new CommandUserSetCapacity(this, users));
        }
    }
}
