using System;
using System.Collections.Generic;
using domi1819.UpServer.Console.Commands;

namespace domi1819.UpServer.Console
{
    internal static class UpConsole
    {
        private static List<char> inputChars;
        private static RootCommand rootCommand;
        
        internal static void WriteLineRestoreCommand(object obj)
        {
            if (inputChars != null)
            {
                for (int i = 0; i < inputChars.Count + 2; i++)
                {
                    System.Console.Write("\b \b");
                }
            }
            
            System.Console.WriteLine(obj);
            
            if (inputChars != null)
            {
                System.Console.Write("> ");

                foreach (char c in inputChars)
                {
                    System.Console.Write(c);
                }
            }
        }

        internal static void ProcessConsoleInput(UpServer server)
        {
            rootCommand = new RootCommand(server);

            inputChars = new List<char>();
            System.Console.Write("> ");

            while (true)
            {
                ConsoleKeyInfo key = System.Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        if (HandleEnterKey())
                        {
                            return;
                        }
                        break;

                    case ConsoleKey.Tab:
                        HandleTabKey();
                        break;

                    case ConsoleKey.Backspace:
                        if (inputChars.Count > 0)
                        {
                            inputChars.RemoveAt(inputChars.Count - 1);
                            System.Console.Write("\b \b");
                        }
                        break;

                    case ConsoleKey.Escape:
                        inputChars.Clear();
                        System.Console.WriteLine();
                        System.Console.Write("> ");
                        break;

                    default:
                        if (!char.IsControl(key.KeyChar))
                        {
                            inputChars.Add(key.KeyChar);
                            System.Console.Write(key.KeyChar);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Asks the user to enter an input.
        /// </summary>
        /// <param name="text">The text to print prior to the input line.</param>
        /// <param name="mask">Whether to mask the input.</param>
        /// <returns>The user-entered string or null if input was cancelled.</returns>
        internal static string ReadInput(string text, bool mask = false)
        {
            List<char> chars = new List<char>();

            if (text != null)
            {
                System.Console.Write("    ");
                System.Console.WriteLine(text);
            }

            System.Console.Write(mask ? "    * " : "    > ");

            while (true)
            {
                ConsoleKeyInfo key = System.Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        System.Console.WriteLine();
                        return new string(chars.ToArray());

                    case ConsoleKey.Escape:
                        System.Console.WriteLine();
                        return null;

                    case ConsoleKey.Backspace:
                        if (chars.Count > 0)
                        {
                            chars.RemoveAt(chars.Count - 1);

                            if (!mask)
                            {
                                System.Console.Write("\b \b");
                            }
                        }
                        break;

                    default:
                        if (!char.IsControl(key.KeyChar))
                        {
                            chars.Add(key.KeyChar);

                            if (!mask)
                            {
                                System.Console.Write(key.KeyChar);
                            }
                        }
                        break;
                }
            }
        }
        
        private static bool HandleEnterKey()
        {
            if (inputChars.Count >= 0)
            {
                System.Console.WriteLine();

                List<string> inputs = GetInputStrings();

                Result result = rootCommand.Process(inputs);

                switch (result)
                {
                    case Result.Shutdown:
                        inputChars = null;
                        return true;

                    case Result.ReuseCommand:
                        System.Console.Write("> ");

                        foreach (char c in inputChars)
                        {
                            System.Console.Write(c);
                        }

                        break;

                    default:
                        inputChars.Clear();
                        System.Console.Write("> ");
                        break;
                }
            }

            return false;
        }

        private static void HandleTabKey()
        {
            List<string> inputs = GetInputStrings();
            List<string> suggestions = rootCommand.AutoComplete(inputs);

            if (suggestions.Count == 1)
            {
                string suggestion = suggestions[0];
                int originalCount = inputs[inputs.Count - 1].Length;

                for (int i = originalCount; i < suggestion.Length; i++)
                {
                    inputChars.Add(suggestion[i]);
                    System.Console.Write(suggestion[i]);
                }

                inputChars.Add(' ');
                System.Console.Write(' ');
            }
            else if (suggestions.Count > 1)
            {
                WriteLineRestoreCommand(string.Join(", ", suggestions));
            }
        }

        private static List<string> GetInputStrings()
        {
            List<string> results = new List<string>();

            if (inputChars.Count == 0)
            {
                results.Add("");
            }
            else
            {
                int wordStart = 0;

                for (int i = 0; i < inputChars.Count; i++)
                {
                    if (inputChars[i] == ' ')
                    {
                        results.Add(new string(inputChars.GetRange(wordStart, i - wordStart).ToArray()));
                        wordStart = i + 1;
                    }
                }

                results.Add(inputChars.Count == wordStart ? "" : new string(inputChars.GetRange(wordStart, inputChars.Count - wordStart).ToArray()));
            }

            return results;
        }
    }
}
