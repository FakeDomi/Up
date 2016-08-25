using System;
using System.IO;
using System.Reflection;
using domi1819.Crypto;
using domi1819.UpCore.Utilities;

namespace domi1819.UpServer
{
    internal class UpServer
    {
        internal const int RsaKeySize = 4096;
        internal const int MessageServerPort = 1819;
        internal const int TransferServerPort = 1820;

        internal const string KeyFileName = "private.key";

        internal static UpServer Instance { get; private set; }

        internal ServerConfigSettings Settings { get; private set; }

        internal UserRegister Users { get; private set; }
        internal FileRegister Files { get; private set; }

        private Logger logger;

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

            this.Settings = ServerConfigSettings.Load();
            this.Settings.Save();

            if (args.Length > 0 && args[0].Equals("generate-rsa") || !File.Exists(KeyFileName))
            {
                Console.Write("Generating a RSA-{0} key. This might take a few seconds... ", RsaKeySize);

                Rsa.GenerateKeyPair("private.key", "public.key", RsaKeySize);

                Console.WriteLine("Done.");
            }

            if (!Directory.Exists(Constants.Server.FileStorageFolder))
            {
                Directory.CreateDirectory(Constants.Server.FileStorageFolder);
            }

            if (!Directory.Exists(Constants.Server.FileTransferFolder))
            {
                Directory.CreateDirectory(Constants.Server.FileTransferFolder);
            }

            Console.WriteLine("Starting UpServer...");

            this.logger = new Logger("upserver.log");

            this.messageServer = new NetServer();
            this.messageServer.Start(MessageServerPort, "private.key");

            Console.WriteLine("Message server listening on port {0}.", MessageServerPort);

            this.Users = new UserRegister(this.logger);
            this.Files = new FileRegister(this.logger);

            Console.WriteLine("UpServer started.");
            
            //server.Start(555, "");

            UpWebService webService = new UpWebService();

            while (true)
            {
                Console.ReadKey(true);
            }
        }
    }
}
