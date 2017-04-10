using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using domi1819.UpCore.Network;
using domi1819.UpCore.Utilities;

namespace domi1819.Proton
{
    public class ProtonServer<T> where T : new()
    {
        private readonly ArrayPool<byte> messageBufferPool = new ArrayPool<byte>(Constants.Network.MessageBufferSize);
        private TcpListener listener;

        private RSACryptoServiceProvider rsaCsp;
        private byte[] rsaModulus;
        private byte[] rsaExponent;
        private byte[] rsaFingerprint;

        public MessageRegistry<T> MessageRegistry { get; } = new MessageRegistry<T>();
        
        public void Start(int port, RsaKey rsaKey)
        {
            this.listener = new TcpListener(IPAddress.Any, port);

            this.rsaCsp = rsaKey.Csp;
            this.rsaModulus = rsaKey.Modulus;
            this.rsaExponent = rsaKey.Exponent;
            this.rsaFingerprint = rsaKey.Fingerprint;
            
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
                    }

                    using (RijndaelManaged rijndaelDecrypt = new RijndaelManaged { Key = key, IV = ivDecrypt, Mode = CipherMode.CBC, Padding = PaddingMode.None })
                    {
                        decryptor = rijndaelDecrypt.CreateDecryptor();
                    }

                    CryptoStream outStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write);
                    CryptoStream inStream = new CryptoStream(stream, decryptor, CryptoStreamMode.Read);

                    connection.Encryptor = encryptor;
                    connection.Decryptor = decryptor;
                    connection.BaseStream = stream;
                    connection.OutStream = outStream;
                    connection.InStream = inStream;
                    connection.WriterBuffer = this.messageBufferPool.Get();
                    connection.ReaderBuffer = this.messageBufferPool.Get();

                    MessageContext context = new MessageContext(inStream, connection.ReaderBuffer, outStream, connection.WriterBuffer);

                    context.FetchRequestMessage();

                    context.WriteNextInt(context.ReadNextInt() + 1);
                    context.WriteNextInt(Constants.Server.MinClientBuild);

                    context.PushResponseMessage();

                    stream.ReadTimeout = Timeout.Infinite;

                    this.RunMessageLoop(context);

                    Console.WriteLine($"Client {client.Client.RemoteEndPoint} disconnected.");
                }
                else
                {
                    Console.WriteLine($"Client {client.Client.RemoteEndPoint} tried to connect with unknown request mode {mode}. Disconnected.");
                }
            }
            catch (Exception e)
            {
                // TODO change, currently crashes when socket is not connected
                Console.WriteLine($"Client {client.Client.RemoteEndPoint} did something stupid, I guess...");
                Console.WriteLine(e.Message);
            }
            finally
            {
                Util.SafeDispose(connection.Encryptor, connection.Decryptor, connection.OutStream, connection.InStream, connection.BaseStream);
                
                this.messageBufferPool.Return(connection.WriterBuffer);
                this.messageBufferPool.Return(connection.ReaderBuffer);

                client.Close();
            }
        }

        private void RunMessageLoop(MessageContext context)
        {
            T connectionObject = new T();

            while (true)
            {
                //TODO: Proper error handling
                
                IMessageDefinition<T> message = this.MessageRegistry[context.FetchRequestMessage()];

                if (message != null)
                {
                    message.OnMessage(context, connectionObject);

                    if (context.Disconnect)
                    {
                        return;
                    }
                    
                    context.PushResponseMessage();
                }
            }
        }
    }
}
