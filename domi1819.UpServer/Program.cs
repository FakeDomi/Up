using domi1819.UpCore.Utilities;
using System;
using System.Globalization;
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
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            
            UpServer server = new UpServer();

            CrashHandler.Run(server.RunServer, () => server.Files?.Shutdown(), () => server.Users?.Shutdown(), () => server.MessageServer?.Stop());
        }
    }
}
