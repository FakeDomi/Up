using System.Collections.Generic;
using domi1819.UpCore.Utilities;

namespace domi1819.UpServer.Console.Commands
{
    internal class CommandUser : BaseCommand
    {
        public CommandUser(BaseCommand parent, UserManager users) : base(parent)
        {
            this.SubCommands.Add("add", new CommandUserAdd(this, users));
            this.SubCommands.Add("set", new CommandUserSet(this, users));
        }

        private class CommandUserAdd : BaseCommand
        {
            private readonly UserManager users;

            public CommandUserAdd(BaseCommand parent, UserManager users) : base(parent)
            {
                this.users = users;
            }

            protected override Result Run(IEnumerable<string> input)
            {
                if (Feedback.ReadString("User name?", x => this.users.IsValidName(x), "Invalid name (too short or too long).", out string userName))
                {
                    if (Feedback.ReadString("Password?", x => this.users.IsValidPassword(x), "Invalid password (too short or too long).", out string password, true))
                    {
                        long capacity = 0;

                        if (Feedback.Read("Storage capacity? (\"1000000000\" or \"1 GB\")", x => Util.TryParseByteSize(x, out capacity), "Unable to understand your input!"))
                        {
                            bool admin = false;

                            if (Feedback.Read("Admin user? (Y/N)", x => Util.TryParseYesNo(x, out admin), "Please answer \"Y\" (Yes) or \"N\" (No)!"))
                            {
                                Feedback.WriteLine(this.users.CreateUser(userName, password, capacity, admin) ? "User successfully created." : "User could not be added. Name might be taken already.");
                                return Result.Default;
                            }
                        }
                    }
                }

                Feedback.WriteLine("User creation cancelled.");
                return Result.Default;
            }
        }
    }
}
