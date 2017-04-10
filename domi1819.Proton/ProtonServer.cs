using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using domi1819.UpCore.Network;
using domi1819.UpCore.Utilities;

namespace domi1819.Proton
{
    public class ProtonServer
    {

        private TcpListener listener;
        private ArrayPool<byte> messageBufferPool;

        private RSACryptoServiceProvider rsaCsp;
        private byte[] rsaModulus;
        private byte[] rsaExponent;
        private byte[] rsaFingerprint;
        
        public void Start(int port, RsaKey rsaKey)
        {
            this.listener = new TcpListener(IPAddress.Any, port);

            this.rsaCsp = rsaKey.Csp;
            this.rsaModulus = rsaKey.Modulus;
            this.rsaExponent = rsaKey.Exponent;
            this.rsaFingerprint = rsaKey.Fingerprint;

            this.messageBufferPool = new ArrayPool<byte>(Constants.Network.MessageBufferSize);

            new Thread(this.Run) { Name = "ProtonServerDispatcher" }.Start();
        }

        private void Run()
        {
            this.listener.Start();

            while (true)
            {
                if (this.listener.Pending())
                {
                    TcpClient client = this.listener.AcceptTcpClient();

                    ThreadPool.QueueUserWorkItem(this.ProcessClient, client);
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        private void ProcessClient(object clientObj)
        {
            TcpClient client = (TcpClient)clientObj;
            ProtonServerConnection connection = new ProtonServerConnection();

            try
            {
                Console.WriteLine($"Client {client.Client.RemoteEndPoint} connected."); // TODO

                NetworkStream stream = client.GetStream();

                stream.Write(this.rsaFingerprint, 0, this.rsaFingerprint.Length);

                int mode = stream.ReadByte();

                if (mode == 0x01) // Send public key
                {
                    stream.Write(this.rsaModulus, 0, this.rsaModulus.Length);
                    stream.Write(this.rsaExponent, 0, this.rsaExponent.Length);
                }
                else if (mode == 0x00) // Normal connection
                {
                    int position = 0;
                    byte[] buffer = new byte[Constants.Encryption.RsaModulusBytes];

                    while (position < buffer.Length)
                    {
                        int bytesRead = stream.Read(buffer, position, buffer.Length - position);
                        position += bytesRead;

                        if (bytesRead == 0)
                        {
                            throw new Exception($"Connection to client {client.Client.RemoteEndPoint} lost.");
                        }
                    }

                    byte[] secret = this.rsaCsp.Decrypt(buffer, true);

                    byte[] key = new byte[16];
                    byte[] ivEncrypt = new byte[16];
                    byte[] ivDecrypt = new byte[16];

                    Array.Copy(secret, 0, key, 0, 16);
                    Array.Copy(secret, 16, ivDecrypt, 0, 16);
                    Array.Copy(secret, 32, ivEncrypt, 0, 16);

                    ICryptoTransform encryptor, decryptor;

                    using (RijndaelManaged rijndaelEncrypt = new RijndaelManaged { Key = key, IV = ivEncrypt, Mode = CipherMode.CBC, Padding = PaddingMode.None })
                    {
                        encryptor = rijndaelEncrypt.CreateEncryptor();
                        connection.Encryptor = encryptor;
                    }

                    using (RijndaelManaged rijndaelDecrypt = new RijndaelManaged { Key = key, IV = ivDecrypt, Mode = CipherMode.CBC, Padding = PaddingMode.None })
                    {
                        decryptor = rijndaelDecrypt.CreateDecryptor();
                        connection.Decryptor = decryptor;
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
