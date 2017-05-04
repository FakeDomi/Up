using System;
using System.Collections.Generic;

namespace domi1819.UpServer.Console.Commands
{
    internal class RootCommand : BaseCommand
    {
        public RootCommand() : base(null)
        {
            this.SubCommands.Add("stop", new CommandStop(this));
            this.SubCommands.Add("user", new CommandUser(this));
        }

        protected override Result Run(List<string> input)
        {
            throw new Exception("\"Run\" should never happen on the RootCommand!");
        }
    }
}
