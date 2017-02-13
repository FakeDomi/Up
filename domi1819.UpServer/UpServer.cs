using System;
using System.IO;
using System.Reflection;
using domi1819.UpCore.Crypto;
using domi1819.UpCore.Network;
using domi1819.UpCore.Utilities;

namespace domi1819.UpServer
{
    internal class UpServer
    {
        internal static UpServer Instance { get; private set; }

        internal ServerConfig Settings { get; private set; }

        internal UserRegister Users { get; private set; }
        internal FileRegister Files { get; private set; }

        //private Logger logger;

        private NetServer messageServer;

        internal UpServer()
        {
            Instance = this;
        }

        internal void RunServer(string[] args)
        {
            Constants.IsServer = true;

            Console.WriteLine("================================");
            Console.WriteLine("UpServer " + Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine("https://up.domi1819.xyz");
            Console.WriteLine("2016 domi1819");
            Console.WriteLine("All rights reserved");
            Console.WriteLine("================================\n");

            this.Settings = ServerConfig.Load();
            this.Settings.Save();

            this.TryCreateDirectory(this.Settings.DataFolder);
            this.TryCreateDirectory(this.Settings.FileStorageFolder);
            this.TryCreateDirectory(this.Settings.FileTransferFolder);
            
            string privateKeyPath = Path.Combine(this.Settings.DataFolder, Constants.Encryption.PrivateKeyFile);
            string publicKeyPath = Path.Combine(this.Settings.DataFolder, Constants.Encryption.PublicKeyFile);

            if (!File.Exists(privateKeyPath))
            {
                Console.Write($"Generating a RSA-{Constants.Encryption.RsaKeySize} key. This might take a few seconds... ");

                Rsa.GenerateKeyPair(privateKeyPath, publicKeyPath, Constants.Encryption.RsaKeySize);

                Console.WriteLine("Done.");
            }
            
            Console.WriteLine("Starting UpServer...");

            //this.logger = new Logger("upserver.log");

            RsaKey rsaKey = RsaKey.FromFile(privateKeyPath);
            
            if (rsaKey.Csp.KeySize != Constants.Encryption.RsaKeySize)
            {
                Console.WriteLine($"Unsupported key size {rsaKey.Csp.KeySize}. Expected: {Constants.Encryption.RsaKeySize}");
                return;
            }

            this.messageServer = new NetServer();
            this.messageServer.Start(this.Settings.ServerPort, rsaKey);

            Console.WriteLine($"Message server listening on port {this.Settings.ServerPort}.");
            
            this.Users = new UserRegister(this);
            this.Files = new FileRegister(this);

            Console.WriteLine("UpServer started.");
            
            UpWebService webService = new UpWebService();

            while (true)
            {
                Console.ReadKey(true);
            }
        }

        private void TryCreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
