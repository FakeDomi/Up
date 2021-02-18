using System.Collections.Generic;
using Domi.UpCore.Utilities;

namespace Domi.UpServer.Console.Commands
{
    internal class CommandUserSetCapacity : BaseCommand
    {
        private readonly UserManager users;

        public CommandUserSetCapacity(BaseCommand parent, UserManager users) : base(parent)
        {
            this.users = users;
        }

        protected override Result Run(IEnumerable<string> input)
        {
            if (Feedback.ReadString("User name?", x => this.users.HasUser(x), "User not found.", out string userName))
            {
                long capacity = 0;

                if (Feedback.Read("Capacity? (like 1000000000, 1 GB, 1g)", x => Util.TryParseByteSize(x, out capacity), "Unable to understand your input!"))
                {
                    this.users.SetMaxCapacity(userName, capacity);

                    Feedback.WriteLine($"Set new capacity to {Util.GetByteSizeText(capacity)}");
                    return Result.Default;
                }
            }

            Feedback.WriteLine("Capacity change cancelled.");
            return Result.Default;
        }
    }
}
