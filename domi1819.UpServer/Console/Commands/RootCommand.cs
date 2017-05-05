using System.Collections.Generic;

namespace domi1819.UpServer.Console.Commands
{
    internal class RootCommand : BaseCommand
    {
        public RootCommand(UpServer server) : base(null)
        {
            this.SubCommands.Add("stop", new CommandStop(this));
            this.SubCommands.Add("user", new CommandUser(this, server.Users));
            this.SubCommands.Add("file", new CommandFile(this, server.Files));
        }

        protected override Result Run(List<string> input)
        {
            System.Console.WriteLine("Unknown command \"\"");

            return Result.Default;
        }
    }
}
