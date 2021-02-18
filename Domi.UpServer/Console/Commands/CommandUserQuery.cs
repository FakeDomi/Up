using System.Collections.Generic;
using System.Linq;
using Domi.UpCore.Utilities;

namespace Domi.UpServer.Console.Commands
{
    internal class CommandUserQuery : BaseCommand
    {
        private readonly UserManager users;
        private readonly FileManager files;

        public CommandUserQuery(BaseCommand parent, UserManager users, FileManager files) : base(parent)
        {
            this.users = users;
            this.files = files;
        }

        protected override Result Run(IEnumerable<string> input)
        {
            string userName = input.Skip(2).FirstOrDefault();

            if (!this.users.HasUser(userName))
            {
                if (!Feedback.ReadString("User name?", x => this.users.HasUser(x), "User not found.", out userName))
                {
                    return Result.Default;
                }
            }

            Feedback.WriteLine($"Used capacity: {Util.GetByteSizeText(this.users.GetUsedCapacity(userName))} of {Util.GetByteSizeText(this.users.GetMaxCapacity(userName))}");
            Feedback.WriteLine($"Total files: {this.files.GetFiles(userName).Count}");
            Feedback.WriteLine($"Is admin: {(this.users.IsAdmin(userName) ? "yes" : "no")}");

            return Result.Default;
        }
    }
}
