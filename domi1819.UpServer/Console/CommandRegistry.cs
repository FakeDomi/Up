using System.Collections.Generic;
using System.Linq;

namespace domi1819.UpServer.Console
{
    internal class CommandRegistry
    {
        private readonly Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();
        
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

        internal List<string> Find(string input)
        {
            return (from command in this.commands where command.Key.StartsWith(input) select command.Key).ToList();
        }
    }
}
