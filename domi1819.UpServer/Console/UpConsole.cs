using System;
using System.Collections.Generic;
using domi1819.UpServer.Console.Commands;

namespace domi1819.UpServer.Console
{
    internal class UpConsole
    {
        private CommandRegistry commandRegistry;
        private List<char> command;
        
        internal void Write(object obj)
        {
            if (this.command == null)
            {
                System.Console.Write(obj);
            }
        }

        internal void WriteLine(object obj)
        {
            this.Write(obj);
            this.Write('\n');
        }

        internal void ProcessConsoleInput()
        {
            this.commandRegistry = new CommandRegistry();

            this.commandRegistry.Register(new Stop(), "stop", "shutdown", "exit");

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
                            return;
                        }
                        this.command.Clear();
                        System.Console.Write("> ");
                        break;
                    case ConsoleKey.Tab:
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
