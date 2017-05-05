using System.Collections.Generic;
using System.ComponentModel;
using domi1819.UpCore.Utilities;

namespace domi1819.UpServer.Console.Commands
{
    internal class CommandUser : BaseCommand
    {
        public CommandUser(BaseCommand parent, UserManager users) : base(parent)
        {
            this.SubCommands.Add("add", new CommandUserAdd(this, users));
            this.SubCommands.Add("drop", new CommandUserDrop(this));
        }

        private class CommandUserAdd : BaseCommand
        {
            private readonly UserManager users;

            public CommandUserAdd(BaseCommand parent, UserManager users) : base(parent)
            {
                this.users = users;
            }

            protected override Result Run(List<string> input)
            {
                UpConsole.IndentCharCount += 2;

                string userName = UpConsole.GetInput("User name?", x => x.Length > 0 && this.users.IsValidName(x), "Invalid name (too short or too long).");

                if (userName == null)
                {
                    goto Cancel;
                }

                string password = UpConsole.GetInput("Password?", x => this.users.IsValidPassword(x), "Invalid password (too short or too long).", true);

                if (password == null)
                {
                    goto Cancel;
                }

                long capacity = 0;
                string capacityString = UpConsole.GetInput("Storage capacity? (\"1000000000\" or \"1 GB\")", x => Util.TryParseByteSize(x, out capacity), "Unable to understand your input!");

                if (capacityString == null)
                {
                    goto Cancel;
                }

                bool admin = false;
                string adminString = UpConsole.GetInput("Admin user? (Y/N)", x => Util.TryParseYesNo(x, out admin), "Please answer \"Y\" (Yes) or \"N\" (No)!");

                if (adminString == null)
                {
                    goto Cancel;
                }

                bool accept = false;
                string acceptString = UpConsole.GetInput($"Create new user \"{userName}\" with {capacity} bytes of storage with admin status \"{admin}\"?", x => Util.TryParseYesNo(x, out accept), "Please answer \"Y\" (Yes) or \"N\" (No)!");

                if (acceptString == null)
                {
                    goto Cancel;
                }

                if (accept)
                {
                    System.Console.WriteLine(this.users.CreateUser(userName, password, capacity, admin) ? "User successfully created." : "User could not be added. Name might be taken already.");

                    UpConsole.IndentCharCount -= 2;

                    return Result.Default;
                }

                Cancel:
                System.Console.WriteLine("User creation cancelled.");

                UpConsole.IndentCharCount -= 2;

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
