using System;

namespace domi1819.UpServer.Console.Commands
{
    internal static class Feedback
    {
        internal static void WriteLine(object obj)
        {
            System.Console.Write("   ");
            System.Console.WriteLine(obj);
        }

        internal static bool ReadString(string displayText, Func<string, bool> validator, string invalidText, out string userInput, bool mask = false)
        {
            if (displayText != null)
            {
                WriteLine(displayText);
            }

            do
            {
                userInput = UpConsole.ReadInput(null, mask);

                if (userInput == null)
                {
                    return false;
                }

                if (validator != null && !validator.Invoke(userInput))
                {
                    WriteLine(invalidText);
                    userInput = null;
                }
            } while (userInput == null);

            return true;
        }

        internal static bool Read(string displayText, Func<string, bool> validator, string invalidText, bool mask = false)
        {
            string userInput;

            if (displayText != null)
            {
                WriteLine(displayText);
            }

            do
            {
                userInput = UpConsole.ReadInput(null, mask);

                if (userInput == null)
                {
                    return false;
                }

                if (validator != null && !validator.Invoke(userInput))
                {
                    WriteLine(invalidText);
                    userInput = null;
                }
            } while (userInput == null);

            return true;
        }
    }
}
