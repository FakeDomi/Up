using System;
using System.Collections.Generic;
using domi1819.UpServer.Console.Commands;

namespace domi1819.UpServer.Console
{
    internal static class UpConsole
    {
        private static List<char> inputChars;
        private static RootCommand rootCommand;

        private static int indentCharCount;
        private static string indentString = "";

        internal static int IndentCharCount
        {
            get => indentCharCount;
            set
            {
                indentCharCount = value;
                indentString = new string(' ', value);
            }
        }
        
        internal static void Write(object obj)
        {
            System.Console.Write(indentString);
            System.Console.Write(obj);
        }

        internal static void WriteLine()
        {
            System.Console.WriteLine();
        }

        internal static void WriteLine(object obj)
        {
            System.Console.Write(indentString);
            System.Console.WriteLine(obj);
        }
        
        internal static void ProcessConsoleInput(UpServer server)
        {
            rootCommand = new RootCommand(server);

            inputChars = new List<char>();
            Write("> ");

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
                        WriteLine();
                        Write("> ");
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
                WriteLine(text);
            }

            Write("  > ");

            while (true)
            {
                ConsoleKeyInfo key = System.Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        WriteLine();
                        return new string(chars.ToArray());

                    case ConsoleKey.Escape:
                        WriteLine();
                        return null;

                    case ConsoleKey.Backspace:
                        if (chars.Count > 0)
                        {
                            chars.RemoveAt(chars.Count - 1);
                            System.Console.Write("\b \b");
                        }
                        break;

                    default:
                        if (!char.IsControl(key.KeyChar))
                        {
                            chars.Add(key.KeyChar);
                            System.Console.Write(key.KeyChar);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Asks the user to enter an input and runs a validation Func, until a valid input is entered or the user decided to cancel.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="validator"></param>
        /// <param name="errorText"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        internal static string GetInput(string text, Func<string, bool> validator, string errorText, bool mask = false)
        {
            string result;

            WriteLine(text);

            do
            {
                result = ReadInput(null, mask);

                if (result == null)
                {
                    return null;
                }

                if (!validator.Invoke(result))
                {
                    WriteLine(errorText);
                    result = null;
                }

            } while (result == null);

            return result;
        }

        private static bool HandleEnterKey()
        {
            if (inputChars.Count >= 0)
            {
                WriteLine();

                List<string> inputs = GetInputStrings();

                Result result = rootCommand.Process(inputs);

                switch (result)
                {
                    case Result.Shutdown:
                        inputChars = null;
                        return true;

                    case Result.ReuseCommand:
                        Write("> ");

                        foreach (char c in inputChars)
                        {
                            System.Console.Write(c);
                        }

                        break;

                    default:
                        inputChars.Clear();
                        Write("> ");
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
                    Write(suggestion[i]);
                }

                inputChars.Add(' ');
                Write(' ');
            }
            else if (suggestions.Count > 1)
            {
                WriteLine(string.Join(", ", suggestions));
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
