using System;
using System.IO;
using System.Reflection;

namespace domi1819.UpClient
{
    internal static class Program
    {
        private static UpClient instance;

        [STAThread]
        internal static void Main(string[] cmdArgs)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "lib", new AssemblyName(args.Name).Name + ".dll");

                return File.Exists(path) ? Assembly.LoadFrom(path) : null;
            };

            instance = new UpClient();
            instance.LaunchApplication(cmdArgs);
        }
    }
}
