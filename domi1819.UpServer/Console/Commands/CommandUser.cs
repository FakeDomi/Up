using System.Collections.Generic;

namespace domi1819.UpServer.Console.Commands
{
    internal class CommandUser : BaseCommand
    {
        public CommandUser(BaseCommand parent) : base(parent)
        {
            this.SubCommands.Add("add", new CommandUserAdd(this));
            this.SubCommands.Add("drop", new CommandUserDrop(this));
        }

        protected override Result Run(List<string> input)
        {
            return this.ShowUsages(input);
        }
        
        private class CommandUserAdd : BaseCommand
        {
            public CommandUserAdd(BaseCommand parent) : base(parent)
            {
            }

            protected override Result Run(List<string> input)
            {
                return Result.Default;
            }
        }

        private class CommandUserDrop : BaseCommand
        {
            public CommandUserDrop(BaseCommand parent) : base(parent)
            {
            }

            protected override Result Run(List<string> input)
            {
                return Result.Default;
            }
        }
    }
}
