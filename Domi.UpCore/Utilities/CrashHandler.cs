using System;
using System.IO;
using System.Text;

namespace Domi.UpCore.Utilities
{
    public static class CrashHandler
    {
        public static void Run(Action runAction, params Action[] cleanupActions)
        {
            try
            {
                runAction.Invoke();
            }
            catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached)
            {
                foreach (Action cleanupAction in cleanupActions)
                {
                    try
                    {
                        cleanupAction.Invoke();
                    }
                    catch
                    { }
                }

                string logPath = $"crash_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log";

                using (StreamWriter writer = new StreamWriter(logPath, false, Encoding.UTF8))
                {
                    writer.WriteLine(ex.ToString());
                }

                Console.Write("The program crashed. Stack trace saved to ");
                Console.WriteLine(logPath);

                Environment.Exit(1);
            }
        }
    }
}
