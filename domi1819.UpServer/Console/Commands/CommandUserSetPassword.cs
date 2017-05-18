using System.Collections.Generic;

namespace domi1819.UpServer.Console.Commands
{
    internal class CommandUserSetPassword : BaseCommand
    {
        private readonly UserManager users;

        public CommandUserSetPassword(BaseCommand parent, UserManager users) : base(parent)
        {
            this.users = users;
        }

        protected override Result Run(List<string> input)
        {
            if (Feedback.ReadString("User name?", x => this.users.HasUser(x), "User not found.", out string userName))
            {
                if (Feedback.ReadString("Password?", x => this.users.IsValidPassword(x), "Invalid password (too short or too long).", out string password, true))
                {
                    Feedback.WriteLine(this.users.SetPassword(userName, password) ? "Password set." : "Error while setting password.");
                    return Result.Default;
                }
            }

            Feedback.WriteLine("Password change cancelled.");
            return Result.Default;
        }
    }
}
