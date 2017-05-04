using System.Collections.Generic;

namespace domi1819.UpServer.Console.Commands
{
    internal class CommandStop : BaseCommand
    {
        public CommandStop(BaseCommand parent) : base(parent)
        {
        }

        protected override Result Run(List<string> input)
        {
            return Result.Shutdown;
        }
    }
}
