using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace domi1819.UpServer
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        internal static void Main()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "lib", new AssemblyName(eventArgs.Name).Name + ".dll");

                return File.Exists(path) ? Assembly.LoadFrom(path) : null;
            };

            UpServer server = new UpServer();

            try
            {
                server.RunServer();
            }
            catch (Exception ex)
            {
                try
                {
                    server.Files?.Shutdown();
                    server.Users?.Shutdown();
                }
                catch (Exception)
                {
                    // idc
                }

                string logPath = $"crash_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log";
                System.Console.WriteLine($"Server crashed! Saving crash log to {logPath}.");

                using (StreamWriter writer = new StreamWriter(logPath, false, Encoding.UTF8))
                {
                    writer.WriteLine(ex.ToString());
                }

                Environment.Exit(1);
            }
        }
    }
}
