using System;
using System.IO;
using System.Reflection;
using domi1819.UpCore.Crypto;
using domi1819.UpCore.Network;
using domi1819.UpCore.Utilities;
using domi1819.UpServer.Server;

namespace domi1819.UpServer
{
    internal class UpServer
    {
        internal static UpServer Instance { get; private set; }

        internal ServerConfig Config { get; private set; }

        internal UserManager Users { get; private set; }

        internal FileManager Files { get; private set; }

        //private Logger logger;

        private NetServer messageServer;

        internal UpServer()
        {
            Instance = this;
        }

        internal void RunServer(string[] args)
        {
            Console.WriteLine("================================");
            Console.WriteLine($"UpServer {Assembly.GetExecutingAssembly().GetName().Version}");
            Console.WriteLine("https://up.domi1819.xyz");
            Console.WriteLine("================================\n");

            this.Config = ServerConfig.Load();
            this.Config.Save();
            
            TryCreateDirectory(this.Config.DataFolder);
            TryCreateDirectory(this.Config.FileStorageFolder);
            TryCreateDirectory(this.Config.FileTransferFolder);

            string publicKeyPath = Path.Combine(this.Config.DataFolder, Constants.Encryption.PublicKeyFile);
            string privateKeyPath = Path.Combine(this.Config.DataFolder, Constants.Encryption.PrivateKeyFile);

            if (!File.Exists(privateKeyPath))
            {
                Console.Write($"Generating a RSA-{Constants.Encryption.RsaKeySize} key. This might take a few seconds... ");

                Rsa.GenerateKeyPair(privateKeyPath, publicKeyPath, Constants.Encryption.RsaKeySize);

                Console.WriteLine("Done.");
            }
            
            Console.WriteLine("Starting UpServer...");
            
            RsaKey rsaKey = RsaKey.FromFile(privateKeyPath);
            
            if (rsaKey.Csp.KeySize != Constants.Encryption.RsaKeySize)
            {
                Console.WriteLine($"Unsupported key size {rsaKey.Csp.KeySize}. Expected: {Constants.Encryption.RsaKeySize}");
                return;
            }

            this.Users = new UserManager(this);
            this.Files = new FileManager(this);

            this.messageServer = new NetServer(this);
            this.messageServer.Start(this.Config.UpServerPort, rsaKey);

            Console.WriteLine($"Message server listening on port {this.Config.UpServerPort}.");

            Console.WriteLine("UpServer started.");
            
            UpWebService webService = new UpWebService(this);
            webService.Start();

            while (true)
            {
                Console.ReadKey(true);
            }
        }

        private static void TryCreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
