﻿using System.Collections.Generic;
using System.Linq;

namespace Domi.UpServer.Console.Commands
{
    internal abstract class BaseCommand
    {
        protected readonly Dictionary<string, BaseCommand> SubCommands = new Dictionary<string, BaseCommand>();
        
        private readonly int level;

        internal BaseCommand(BaseCommand parent)
        {
            this.level = parent?.level + 1 ?? 0;
        }

        internal Result Process(List<string> input)
        {
            if (this.level == 0 && input[input.Count - 1] == "")
            {
                input.RemoveAt(input.Count - 1);
            }

            if (input.Count > this.level && this.SubCommands.Count > 0)
            {
                BaseCommand subCommand = this.GetSubCommand(input[this.level]);

                if (subCommand != null)
                {
                    return subCommand.Process(input);
                }

                System.Console.WriteLine($"Unknown command \"{input[this.level]}\"");

                return Result.Default;
            }

            return this.Run(input);
        }

        internal List<string> AutoComplete(List<string> input)
        {
            if (input.Count == this.level + 1)
            {
                return (from command in this.SubCommands where command.Key.StartsWith(input[input.Count - 1]) select command.Key).ToList();
            }

            if (input.Count > this.level + 1 && this.SubCommands.Count > 0)
            {
                BaseCommand subCommand = this.GetSubCommand(input[this.level]);

                if (subCommand != null)
                {
                    return subCommand.AutoComplete(input);
                }
            }

            return new List<string>();
        }

        protected virtual Result Run(IEnumerable<string> input)
        {
            System.Console.WriteLine($"Usage: {string.Join(" ", input)} <{string.Join("/", this.SubCommands.Keys)}>");
            return Result.ReuseCommand;
        }

        private BaseCommand GetSubCommand(string name)
        {
            return this.SubCommands.TryGetValue(name, out BaseCommand command) ? command : null;
        }
    }
}
