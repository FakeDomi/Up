using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using domi1819.UpCore.Network;
using domi1819.UpCore.Utilities;
using domi1819.UpServer.Console;
using domi1819.UpServer.Server.Messages;

namespace domi1819.UpServer.Server
{
    internal class NetServer
    {
        private readonly ArrayPool<byte> messageBufferPool = new ArrayPool<byte>(Constants.Network.MessageBufferSize);
        private readonly List<TcpListener> listeners = new List<TcpListener>();

        private RSACryptoServiceProvider rsaCsp;
        private byte[] rsaModulus;
        private byte[] rsaExponent;
        private byte[] rsaFingerprint;

        private readonly Dictionary<int, IMessage> messages = new Dictionary<int, IMessage>();

        internal NetServer(UpServer upServer)
        {
            this.messages.Add(NetworkMethods.LinkFormat, new LinkFormat(upServer.Files));
            this.messages.Add(NetworkMethods.GetStorageInfo, new GetStorageInfo(upServer.Files, upServer.Users));
            this.messages.Add(NetworkMethods.Login, new Login(upServer.Users));
            this.messages.Add(NetworkMethods.SetPassword, new SetPassword(upServer.Users));
            this.messages.Add(NetworkMethods.UploadPacket, new FileUploadData());
            this.messages.Add(NetworkMethods.FinishUpload, new FinishUpload(upServer.Files, upServer.Users, upServer.Config));
            this.messages.Add(NetworkMethods.InitiateUpload, new InitiateUpload(upServer.Files, upServer.Users));
            this.messages.Add(NetworkMethods.ListFiles, new ListFiles(upServer.Files, upServer.Users));
        }

        internal void Start(int[] ports, RsaKey rsaKey)
        {
            this.rsaCsp = rsaKey.Csp;
            this.rsaModulus = rsaKey.Modulus;
            this.rsaExponent = rsaKey.Exponent;
            this.rsaFingerprint = rsaKey.Fingerprint;

            foreach (int port in ports)
            {
                Thread dispatcher = new Thread(this.Run) { Name = "NetServer Dispatcher" };
                TcpListener listener = new TcpListener(IPAddress.Any, port);

                this.listeners.Add(listener);
                listener.Start();
                dispatcher.Start(listener);
                
                UpConsole.WriteLineRestoreCommand($"Message server listening on port {port}.");
            }
        }

        internal void Stop()
        {
            foreach (TcpListener listener in this.listeners)
            {
                listener.Stop();
            }
        }

        private void Run(object listenerObj)
        {
            TcpListener listener = (TcpListener)listenerObj;

            listener.Start();

            try
            {
                while (true)
                {
                    ThreadPool.QueueUserWorkItem(this.ProcessClient, listener.AcceptTcpClient());
                }
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.Interrupted)
            {
                // Tcp listener has been stopped
            }
        }

        private void ProcessClient(object clientObject)
        {
            TcpClient client = (TcpClient)clientObject;
            Connection connection = new Connection();

            try
            {
                UpConsole.WriteLineRestoreCommand($"Client {client.Client.RemoteEndPoint} connected."); //TODO

                NetworkStream baseStream = client.GetStream();
                connection.BaseStream = baseStream;

                baseStream.Write(this.rsaFingerprint, 0, this.rsaFingerprint.Length);

                int mode = baseStream.ReadByte();

                if (mode == 0x01) // Send full key
                {
                    baseStream.Write(this.rsaModulus, 0, this.rsaModulus.Length);
                    baseStream.Write(this.rsaExponent, 0, this.rsaExponent.Length);
                    
                    UpConsole.WriteLineRestoreCommand($"Client {client.Client.RemoteEndPoint} disconnected.");
                }
                else if (mode == 0x00) // Normal connection
                {
                    int position = 0;
                    byte[] buffer = new byte[Constants.Encryption.RsaModulusBytes];

                    while (position < buffer.Length)
                    {
                        int bytesRead = baseStream.Read(buffer, position, buffer.Length - position);
                        position += bytesRead;

                        if (bytesRead == 0)
                        {
                            throw new Exception($"Connection to client {client.Client.RemoteEndPoint} lost.");
                        }
                    }

                    connection.InitializeSymmetricEncryption(this.rsaCsp.Decrypt(buffer, true), baseStream, this.messageBufferPool);

                    MessageSerializer serializer = new MessageSerializer { Bytes = connection.WriterBuffer, Stream = connection.OutStream };
                    MessageDeserializer deserializer = new MessageDeserializer { Bytes = connection.ReaderBuffer, Stream = connection.InStream };

                    deserializer.ReadMessage(NetworkMethods.GetServerVersion);

                    serializer.Start(NetworkMethods.GetServerVersion);
                    serializer.WriteNextInt(deserializer.ReadNextInt() + 1);
                    serializer.WriteNextInt(Constants.Server.MinClientBuild);
                    serializer.Flush();

                    baseStream.ReadTimeout = Timeout.Infinite;

                    this.RunMessageLoop(new MessageContext(deserializer, serializer), connection);

                    UpConsole.WriteLineRestoreCommand($"Client {client.Client.RemoteEndPoint} disconnected.");
                }
                else
                {
                    UpConsole.WriteLineRestoreCommand($"Client {client.Client.RemoteEndPoint} tried to connect with unknown request mode {mode}. Disconnected.");
                }
            }
            catch (Exception ex)
            {
                try
                {
                    UpConsole.WriteLineRestoreCommand($"Client {client.Client.RemoteEndPoint} did something stupid, I guess...");
                    UpConsole.WriteLineRestoreCommand(ex.Message);
                }
                catch
                {
                    UpConsole.WriteLineRestoreCommand("A client did something stupid and their socket doesn't exist...");
                }
            }
            finally
            {
                connection.Cleanup(this.messageBufferPool);
                client.Close();
            }
        }

        private void RunMessageLoop(MessageContext context, Connection connection)
        {
            while (true)
            {
                int messageId = context.FetchMessage();

                //TODO: Proper error handling
                if (!this.messages.TryGetValue(messageId, out IMessage message))
                {
                    return;
                }

                context.ShouldPush = true;
                context.MessageWriter.Start(messageId);

                message.OnMessage(context, connection);

                if (context.Disconnect)
                {
                    return;
                }

                if (context.ShouldPush)
                {
                    context.PushMessage();
                }
            }
        }
    }
}
