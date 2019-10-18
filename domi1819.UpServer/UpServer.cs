using System.IO;
using System.Reflection;
using System.Threading;
using domi1819.UpCore.Crypto;
using domi1819.UpCore.Network;
using domi1819.UpCore.Utilities;
using domi1819.UpServer.Console;
using domi1819.UpServer.Server;

namespace domi1819.UpServer
{
    internal class UpServer
    {
        internal static UpServer Instance { get; private set; }

        internal ServerConfig Config { get; private set; }

        internal UserManager Users { get; private set; }

        internal FileManager Files { get; private set; }

        internal UpWebService WebService { get; private set; }

        public NetServer MessageServer { get; private set; }

        internal UpServer()
        {
            Instance = this;
        }

        internal void RunServer()
        {
            UpConsole.WriteLineRestoreCommand("================================");
            UpConsole.WriteLineRestoreCommand($"UpServer {Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}");
            UpConsole.WriteLineRestoreCommand("https://up.domi.re");
            UpConsole.WriteLineRestoreCommand("================================\n");

            this.Config = ServerConfig.Load(Constants.Server.ConfigFileName);
            this.Config.Save(Constants.Server.ConfigFileName);

            Directory.CreateDirectory(this.Config.DataFolder);
            Directory.CreateDirectory(this.Config.FileStorageFolder);
            Directory.CreateDirectory(this.Config.FileTransferFolder);
            Directory.CreateDirectory(this.Config.WebFolder);

            string publicKeyPath = Path.Combine(this.Config.DataFolder, Constants.Encryption.PublicKeyFile);
            string privateKeyPath = Path.Combine(this.Config.DataFolder, Constants.Encryption.PrivateKeyFile);

            if (!File.Exists(privateKeyPath))
            {
                UpConsole.WriteLineRestoreCommand($"Generating an RSA-{Constants.Encryption.RsaKeySize} key. This might take a few seconds... ");

                Rsa.GenerateKeyPair(privateKeyPath, publicKeyPath, Constants.Encryption.RsaKeySize);

                UpConsole.WriteLineRestoreCommand("Done.");
            }

            UpConsole.WriteLineRestoreCommand("Starting UpServer...");

            RsaKey rsaKey = RsaKey.FromFile(privateKeyPath);

            if (rsaKey.Csp.KeySize != Constants.Encryption.RsaKeySize)
            {
                UpConsole.WriteLineRestoreCommand($"Unsupported key size {rsaKey.Csp.KeySize}. Expected: {Constants.Encryption.RsaKeySize}");
                return;
            }

            this.Users = new UserManager(this);
            this.Files = new FileManager(this);

            this.MessageServer = new NetServer(this);
            this.MessageServer.Start(this.Config.UpServerPorts, rsaKey);

            this.WebService = new UpWebService(this);
            this.WebService.Start();

            Thread.Sleep(500);

            UpConsole.WriteLineRestoreCommand("UpServer started.");
            UpConsole.ProcessConsoleInput(this);

            UpConsole.WriteLineRestoreCommand("Stopping UpServer...");

            this.WebService.Stop();
            this.MessageServer.Stop();
            this.Users.Shutdown();
            this.Files.Shutdown();
        }
    }
}
