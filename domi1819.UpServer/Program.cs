using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;

namespace domi1819.UpServer
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        internal static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "lib", new AssemblyName(eventArgs.Name).Name + ".dll");

                return File.Exists(path) ? Assembly.LoadFrom(path) : null;
            };

            new UpServer().RunServer(args);
        }
    }
}
