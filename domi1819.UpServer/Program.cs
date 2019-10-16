using domi1819.UpCore.Utilities;
using System;
using System.Globalization;
using System.Threading;

namespace domi1819.UpServer
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            
            UpServer server = new UpServer();

            CrashHandler.Run(server.RunServer, () => server.Files?.Shutdown(), () => server.Users?.Shutdown(), () => server.MessageServer?.Stop());
        }
    }
}
