using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.ExceptionServices;
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
                string userName = Feedback.ReadString("User name?", x => x.Length > 0 && this.users.IsValidName(x), "Invalid name (too short or too long).");

                if (userName == null)
                {
                    goto Cancel;
                }

                string password = Feedback.ReadString("Password?", x => this.users.IsValidPassword(x), "Invalid password (too short or too long).", true);

                if (password == null)
                {
                    goto Cancel;
                }

                long capacity = 0;
                string capacityString = Feedback.ReadString("Storage capacity? (\"1000000000\" or \"1 GB\")", x => Util.TryParseByteSize(x, out capacity), "Unable to understand your input!");

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
                
                Feedback.WriteLine(this.users.CreateUser(userName, password, capacity, admin) ? "User successfully created." : "User could not be added. Name might be taken already.");
                return Result.Default;

                Cancel:
                Feedback.WriteLine("User creation cancelled.");
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
