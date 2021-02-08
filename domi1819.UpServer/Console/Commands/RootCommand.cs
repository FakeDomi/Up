using System.Collections.Generic;

namespace domi1819.UpServer.Console.Commands
{
    internal class RootCommand : BaseCommand
    {
        public RootCommand(UpServer server) : base(null)
        {
            this.SubCommands.Add("stop", new CommandStop(this));
            this.SubCommands.Add("user", new CommandUser(this, server.Users, server.Files));
            this.SubCommands.Add("reload-web-content", new CommandReloadWebContent(this, server.WebService));
        }

        protected override Result Run(IEnumerable<string> input)
        {
            System.Console.WriteLine("Unknown command \"\"");

            return Result.Default;
        }
    }
}
