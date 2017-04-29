using System;
using System.Collections.Generic;
using domi1819.UpServer.Console.Commands;

namespace domi1819.UpServer.Console
{
    internal class UpConsole
    {
        private CommandRegistry commandRegistry;
        private List<char> command;
        
        internal void WriteLine(object obj)
        {
            if (this.command != null)
            {
                for (int i = 0; i < this.command.Count + 2; i++)
                {
                    System.Console.Write('\b');
                    System.Console.Write(' ');
                    System.Console.Write('\b');
                }
            }

            System.Console.WriteLine(obj);

            if (this.command != null)
            {
                System.Console.Write("> ");

                foreach (char c in this.command)
                {
                    System.Console.Write(c);
                }
            }
        }

        internal void ProcessConsoleInput()
        {
            this.commandRegistry = new CommandRegistry();

            this.commandRegistry.Register(new CommandStop(), "stop", "shutdown", "exit");
            this.commandRegistry.Register(new CommandUser(), "user");

            this.command = new List<char>();
            System.Console.Write("> ");

            while (true)
            {
                ConsoleKeyInfo key = System.Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        System.Console.WriteLine();
                        if (!this.commandRegistry.TryProcess(new string(this.command.ToArray())))
                        {
                            System.Console.WriteLine("Unrecognized command.");
                        }
                        else //TODO: implement proper processing
                        {
                            this.command = null;
                            return;
                        }
                        this.command.Clear();
                        System.Console.Write("> ");
                        break;
                    case ConsoleKey.Tab:
                        List<string> suggestions = this.commandRegistry.Find(new string(this.command.ToArray()));

                        if (suggestions.Count == 1)
                        {
                            int originalCount = this.command.Count;

                            for (int i = originalCount; i < suggestions[0].Length; i++)
                            {
                                this.command.Add(suggestions[0][i]);
                                System.Console.Write(suggestions[0][i]);
                            }
                        }
                        else if (suggestions.Count > 1)
                        {
                            this.WriteLine(string.Join(", ", suggestions));
                        }

                        // TODO: autocomplete
                        break;
                    case ConsoleKey.Backspace:
                        if (this.command.Count > 0)
                        {
                            this.command.RemoveAt(this.command.Count - 1);
                            System.Console.Write("\b \b");
                        }
                        break;
                    case ConsoleKey.Escape:
                        this.command.Clear();
                        System.Console.WriteLine();
                        System.Console.Write("> ");
                        break;
                    default:
                        if (!char.IsControl(key.KeyChar))
                        {
                            this.command.Add(key.KeyChar);
                            System.Console.Write(key.KeyChar);
                        }
                        break;
                }
            }
        }
    }
}
