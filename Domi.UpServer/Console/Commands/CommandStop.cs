using System.Collections.Generic;

namespace Domi.UpServer.Console.Commands
{
    internal class CommandStop : BaseCommand
    {
        public CommandStop(BaseCommand parent) : base(parent)
        {
        }

        protected override Result Run(IEnumerable<string> input)
        {
            return Result.Shutdown;
        }
    }
}
