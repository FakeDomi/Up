using System.Collections.Generic;

namespace domi1819.UpServer.Console
{
    internal class CommandRegistry
    {
        internal Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();
        
        internal void Register(ICommand command, string name)
        {
            this.commands.Add(name, command);
        }

        internal void Register(ICommand command, params string[] names)
        {
            foreach (string name in names)
            {
                this.commands.Add(name, command);
            }
        }

        internal bool TryProcess(string str)
        {
            return this.commands.TryGetValue(str.Split(new[] { ' ' }, 2)[0], out ICommand command) && command.Process(str);
        }
    }
}
