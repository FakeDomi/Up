using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace domi1819.UpServer.Console.Commands
{
    class Feedback
    {
        internal static void WriteLine(object obj)
        {
            System.Console.Write("   ");
            System.Console.WriteLine(obj);
        }

        /// <summary>
        /// Asks the user to enter an input and runs a validation Func, until a valid input is entered or the user decided to cancel.
        /// </summary>
        /// <param name="text">The text to print prior to the input line.</param>
        /// <param name="validator"></param>
        /// <param name="invalidText"></param>
        /// <param name="mask">Whether to mask the input.</param>
        /// <returns>The user-entered string or null if input was cancelled.</returns>
        internal static string ReadString(string text, Func<string, bool> validator, string invalidText, bool mask = false)
        {
            string result;

            if (text != null)
            {
                WriteLine(text);
            }

            do
            {
                result = UpConsole.ReadInput(null, mask);

                if (result == null)
                {
                    return null;
                }

                if (!validator.Invoke(result))
                {
                    result = null;
                }
            } while (result == null);

            return result;
        }
    }
}
