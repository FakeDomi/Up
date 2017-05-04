using System;
using System.Collections.Generic;
using domi1819.UpServer.Console.Commands;

namespace domi1819.UpServer.Console
{
    internal class UpConsole
    {
        private List<char> inputChars;
        private RootCommand rootCommand;
        
        internal void WriteLine(object obj)
        {
            if (this.inputChars != null)
            {
                for (int i = 0; i < this.inputChars.Count + 2; i++)
                {
                    System.Console.Write('\b');
                    System.Console.Write(' ');
                    System.Console.Write('\b');
                }
            }

            System.Console.WriteLine(obj);

            if (this.inputChars != null)
            {
                System.Console.Write("> ");

                foreach (char c in this.inputChars)
                {
                    System.Console.Write(c);
                }
            }
        }

        internal void ProcessConsoleInput()
        {
            this.rootCommand = new RootCommand();
            
            this.inputChars = new List<char>();
            System.Console.Write("> ");

            while (true)
            {
                ConsoleKeyInfo key = System.Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        if (this.HandleEnterKey())
                        {
                            return;
                        }

                        break;

                    case ConsoleKey.Tab:
                        this.HandleTabKey();
                        break;

                    case ConsoleKey.Backspace:
                        if (this.inputChars.Count > 0)
                        {
                            this.inputChars.RemoveAt(this.inputChars.Count - 1);
                            System.Console.Write("\b \b");
                        }
                        break;

                    case ConsoleKey.Escape:
                        this.inputChars.Clear();
                        System.Console.WriteLine();
                        System.Console.Write("> ");
                        break;

                    default:
                        if (!char.IsControl(key.KeyChar))
                        {
                            this.inputChars.Add(key.KeyChar);
                            System.Console.Write(key.KeyChar);
                        }
                        break;
                }
            }
        }

        private bool HandleEnterKey()
        {
            if (this.inputChars.Count > 0)
            {
                System.Console.WriteLine();

                List<string> inputs = this.GetInputStrings();

                Result result = this.rootCommand.Process(inputs);

                switch (result)
                {
                    case Result.Shutdown:
                        this.inputChars = null;
                        return true;

                    case Result.ReuseCommand:
                        System.Console.Write("> ");

                        foreach (char c in this.inputChars)
                        {
                            System.Console.Write(c);
                        }

                        break;

                    default:
                        this.inputChars.Clear();
                        System.Console.Write("> ");
                        break;
                }
            }

            return false;
        }

        private void HandleTabKey()
        {
            List<string> inputs = this.GetInputStrings();
            List<string> suggestions = this.rootCommand.AutoComplete(inputs);

            if (suggestions.Count == 1)
            {
                string suggestion = suggestions[0];
                int originalCount = inputs[inputs.Count - 1].Length;

                for (int i = originalCount; i < suggestion.Length; i++)
                {
                    this.inputChars.Add(suggestion[i]);
                    System.Console.Write(suggestion[i]);
                }

                this.inputChars.Add(' ');
                System.Console.Write(' ');
            }
            else if (suggestions.Count > 1)
            {
                this.WriteLine(string.Join(", ", suggestions));
            }
        }

        private List<string> GetInputStrings()
        {
            List<string> results = new List<string>();

            if (this.inputChars.Count == 0)
            {
                results.Add("");
            }
            else
            {
                int wordStart = 0;

                for (int i = 0; i < this.inputChars.Count; i++)
                {
                    if (this.inputChars[i] == ' ')
                    {
                        results.Add(new string(this.inputChars.GetRange(wordStart, i - wordStart).ToArray()));
                        wordStart = i + 1;
                    }
                }

                results.Add(this.inputChars.Count == wordStart ? "" : new string(this.inputChars.GetRange(wordStart, this.inputChars.Count - wordStart).ToArray()));
            }

            return results;
        }
    }
}
