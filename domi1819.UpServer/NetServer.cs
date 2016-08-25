using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using domi1819.Crypto;
using domi1819.NanoDB;
using domi1819.UpCore.Network;
using domi1819.UpCore.Utilities;

namespace domi1819.UpServer
{
    internal class NetServer
    {
        internal int Port { get; private set; }
        internal bool Running { get; private set; }

        private TcpListener listener;
        private AutoResetEvent resetEvent;
        private RSACryptoServiceProvider rsaProvider;
        private ArrayPool<byte> messageBufferPool;
        private ArrayPool<byte> fileBufferPool; 
        private Dictionary<string, UploadUnit> fileTransfersByKey;
        private byte encryptionMode;
        private bool shutdown;
        
        internal void Start(int port, string privateKeyFile)
        {
            this.Port = port;

            this.listener = new TcpListener(IPAddress.Any, this.Port);
            this.resetEvent = new AutoResetEvent(false);
            this.rsaProvider = Rsa.GetProvider(privateKeyFile);
            this.messageBufferPool = new ArrayPool<byte>(Constants.Network.MessageBufferSize);
            this.fileBufferPool = new ArrayPool<byte>(Constants.Network.FileBufferSize);
            this.fileTransfersByKey = new Dictionary<string, UploadUnit>();

            this.encryptionMode = (byte)EncryptionMode.Aes128;

            switch (this.rsaProvider.KeySize)
            {
                case 1024:
                    this.encryptionMode |= (byte)EncryptionMode.Rsa1024;
                    break;
                case 2048:
                    this.encryptionMode |= (byte)EncryptionMode.Rsa2048;
                    break;
                case 4096:
                    this.encryptionMode |= (byte)EncryptionMode.Rsa4096;
                    break;
                default:
                    throw new Exception($"Invalid RSA keysize {this.rsaProvider.KeySize}. FileServer only supports 1024, 2048 and 4096 bit keys.");
            }

            new Thread(this.Run) { Name = "NetServerMain" }.Start();
        }

        private void Run()
        {
            this.listener.Start();
            this.Running = true;

            while (!this.shutdown)
            {
                if (this.listener.Pending())
                {
                    this.listener.BeginAcceptTcpClient(this.AcceptTcpClientCallback, this.listener);

                    this.resetEvent.WaitOne();
                }
                else
                {
                    Thread.Sleep(250);
                }
            }

            this.Running = false;
        }

        private void AcceptTcpClientCallback(IAsyncResult result)
        {
            TcpClient client = this.listener.EndAcceptTcpClient(result);

            this.resetEvent.Set();

            ThreadPool.QueueUserWorkItem(this.ProcessClient, client);
        }

        private void ProcessClient(object clientObject)
        {
            TcpClient client = (TcpClient)clientObject;
            NetServerUser user = new NetServerUser();

            try
            {
                Console.WriteLine("Client {0} connected.", client.Client.RemoteEndPoint);

                NetworkStream stream = client.GetStream();

                user.BaseStream = stream;
                stream.ReadTimeout = Constants.Network.Timeout;

                int mode = stream.ReadByte();

                if (mode == this.encryptionMode)
                {
                    int position = 0;
                    byte[] buffer = new byte[(this.encryptionMode & (int)EncryptionMode.Rsa) * 128];

                    while (position < buffer.Length)
                    {
                        int bytesRead = stream.Read(buffer, position, buffer.Length - position);
                        position += bytesRead;

                        if (bytesRead == 0)
                        {
                            throw new Exception($"Connection to client {client.Client.RemoteEndPoint} lost.");
                        }
                    }

                    byte[] secret = this.rsaProvider.Decrypt(buffer, true);

                    byte[] key = new byte[16];
                    byte[] ivEncrypt = new byte[16];
                    byte[] ivDecrypt = new byte[16];

                    Array.Copy(secret, 0, key, 0, 16);
                    Array.Copy(secret, 16, ivDecrypt, 0, 16);
                    Array.Copy(secret, 32, ivEncrypt, 0, 16);

                    RijndaelManaged rijndaelEncrypt = new RijndaelManaged { Key = key, IV = ivEncrypt, Mode = CipherMode.CBC, Padding = PaddingMode.None };
                    ICryptoTransform encryptor = rijndaelEncrypt.CreateEncryptor();
                    rijndaelEncrypt.Dispose();
                    user.Encryptor = encryptor;

                    RijndaelManaged rijndaelDecrypt = new RijndaelManaged { Key = key, IV = ivDecrypt, Mode = CipherMode.CBC, Padding = PaddingMode.None };
                    ICryptoTransform decryptor = rijndaelDecrypt.CreateDecryptor();
                    rijndaelDecrypt.Dispose();
                    user.Decryptor = decryptor;

                    CryptoStream outStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write);
                    user.OutStream = outStream;

                    CryptoStream inStream = new CryptoStream(stream, decryptor, CryptoStreamMode.Read);
                    user.InStream = inStream;

                    MessageSerializer serializer = new MessageSerializer { Bytes = this.messageBufferPool.Get(), Stream = outStream};
                    user.SerializeBuffer = serializer.Bytes;

                    MessageDeserializer deserializer = new MessageDeserializer { Bytes = this.messageBufferPool.Get(), Stream = inStream };
                    user.DeserializeBuffer = deserializer.Bytes;

                    deserializer.ReadMessage(NetworkMethods.GetServerVersion);

                    serializer.Start(NetworkMethods.GetServerVersion);
                    serializer.WriteNextInt(deserializer.ReadNextInt() + 1);
                    serializer.WriteNextInt(Constants.Server.MinClientBuild);
                    serializer.Flush();
                    
                    stream.ReadTimeout = Timeout.Infinite;

                    this.RunMessageLoop(serializer, deserializer, user);

                    Console.WriteLine("Client {0} disconnected.", client.Client.RemoteEndPoint);
                }
                else
                {
                    Console.WriteLine("Client {0} tried to connect with unsupported encryption methods.", client.Client.RemoteEndPoint);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Client {0} did something stupid, I guess...", client.Client.RemoteEndPoint);
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Util.SafeDispose(user.OutStream, user.InStream, user.BaseStream, user.Encryptor, user.Decryptor);
                
                if (user.SerializeBuffer != null)
                {
                    this.messageBufferPool.Return(user.SerializeBuffer);
                }

                if (user.DeserializeBuffer != null)
                {
                    this.messageBufferPool.Return(user.DeserializeBuffer);
                }

                if (user.UploadUnits != null)
                {
                    foreach (UploadUnit unit in user.UploadUnits.Where(unit => this.fileTransfersByKey.ContainsKey(unit.Key)))
                    {
                        try
                        {
                            unit.FileStream.Dispose();
                        }
                        catch (Exception)
                        {
                            // No action needed.
                        }

                        this.fileTransfersByKey.Remove(unit.Key);
                    }
                }

                client.Close();
            }
        }

        private void RunMessageLoop(MessageSerializer serializer, MessageDeserializer deserializer, NetServerUser user)
        {
            string currentUser = null;

            user.UploadUnits = new List<UploadUnit>();

            while (true)
            {
                switch (deserializer.ReadMessage())
                {
                    case NetworkMethods.ConnectionClosed:
                    {
                        return;
                    }
                    case NetworkMethods.Login:
                    {
                        string userId = deserializer.ReadNextString();
                        string password = deserializer.ReadNextString();

                        serializer.Start(NetworkMethods.Login);

                        if (UpServer.Instance.Users.Verify(userId, password))
                        {
                            serializer.WriteNextBool(true);
                            currentUser = userId;
                        }
                        else
                        {
                            serializer.WriteNextBool(false);
                            currentUser = null;
                        }

                        serializer.Flush();

                        break;
                    }
                    case NetworkMethods.GetStorageInfo:
                    {
                        if (!UpServer.Instance.Users.HasUser(currentUser))
                        {
                            return;
                        }

                        serializer.Start(NetworkMethods.GetStorageInfo);
                        serializer.WriteNextLong(UpServer.Instance.Users.GetMaxCapacity(currentUser));
                        serializer.WriteNextLong(UpServer.Instance.Users.GetUsedCapacity(currentUser));
                        serializer.WriteNextInt(UpServer.Instance.Files.GetFiles(currentUser).Count);
                        serializer.Flush();

                        break;
                    }
                    case NetworkMethods.SetPassword:
                    {
                        if (!UpServer.Instance.Users.HasUser(currentUser))
                        {
                            return;
                        }

                        serializer.Start(NetworkMethods.SetPassword);
                        serializer.WriteNextBool(UpServer.Instance.Users.SetPassword(currentUser, deserializer.ReadNextString()));
                        serializer.Flush();

                        break;
                    }
                    case NetworkMethods.InitiateUpload:
                    {
                        if (!UpServer.Instance.Users.HasUser(currentUser))
                        {
                            return;
                        }

                        string fileName = deserializer.ReadNextString();
                        long fileSize = deserializer.ReadNextLong();

                        serializer.Start(NetworkMethods.InitiateUpload);

                        bool uploadAllowed = false;

                        // ReSharper disable once PossibleNullReferenceException
                        lock (currentUser)
                        {
                            if (UpServer.Instance.Users.GetMaxCapacity(currentUser) - UpServer.Instance.Users.GetUsedCapacity(currentUser) - user.ReservedBytes >= fileSize && UpServer.Instance.Files.IsValidFileName(fileName))
                            {
                                uploadAllowed = true;

                                user.ReservedBytes += fileSize;
                            }
                        }

                        if (uploadAllowed)
                        {
                            string key;

                            lock (this.fileTransfersByKey)
                            {
                                do
                                {
                                    key = Util.GetRandomString(8);
                                } while (this.fileTransfersByKey.ContainsKey(key));

                                UploadUnit unit = new UploadUnit { Key = key, User = currentUser, FileName = fileName, Size = fileSize, FileStream = new FileStream(Path.Combine(Constants.Server.FileTransferFolder, key + ".tmp"), FileMode.Create, FileAccess.Write) };

                                this.fileTransfersByKey.Add(key, unit);
                                user.UploadUnits.Add(unit);
                            }

                            serializer.WriteNextBool(true);
                            serializer.WriteNextString(key);
                        }
                        else
                        {
                            serializer.WriteNextBool(false);
                            serializer.WriteNextString("goobypls");
                        }

                        serializer.Flush();

                        break;
                    }
                    case NetworkMethods.UploadPacket:
                    {
                        string key = deserializer.ReadNextString();
                        int byteCount = deserializer.ReadNextInt();

                        UploadUnit unit;

                        if (!this.fileTransfersByKey.TryGetValue(key, out unit))
                        {
                            return;
                        }

                        unit.FileStream.Write(deserializer.Bytes, deserializer.Index, byteCount);
                        unit.Progress += byteCount;

                        //serializer.Start(NetworkMethods.UploadPacket);
                        //serializer.Flush();

                        break;
                    }
                    case NetworkMethods.FinishUpload:
                    {
                        string key = deserializer.ReadNextString();
                        UploadUnit unit;

                        if (!this.fileTransfersByKey.TryGetValue(key, out unit) || unit.Size != unit.Progress)
                        {
                            return;
                        }

                        unit.FileStream.Dispose();

                        string fileKey;

                        do
                        {
                            fileKey = Util.GetRandomString(8);
                        } while (UpServer.Instance.Files.HasFile(fileKey));

                        UpServer.Instance.Files.AddFile(fileKey, unit.FileName, currentUser, this.fileTransfersByKey[key].Size);

                        File.Move(Path.Combine(Constants.Server.FileTransferFolder, key + ".tmp"), Path.Combine(Constants.Server.FileStorageFolder, fileKey));

                        UpServer.Instance.Files.SetDownloadable(fileKey, true);

                        serializer.Start(NetworkMethods.FinishUpload);

                        serializer.WriteNextString(!string.IsNullOrEmpty(UpServer.Instance.Settings.OverrideAddress) ? $"{UpServer.Instance.Settings.OverrideAddress}/d?{fileKey}" : $"http://{UpServer.Instance.Settings.HostName}{(UpServer.Instance.Settings.WebPort == 80 ? string.Empty : ":" + UpServer.Instance.Settings.WebPort)}/d?{fileKey}");

                        serializer.Flush();

                        this.fileTransfersByKey.Remove(key);
                        user.UploadUnits.Remove(unit);
                        user.ReservedBytes -= unit.Size;

                        break;
                    }
                    case NetworkMethods.ListFiles:
                    {
                        if (!UpServer.Instance.Users.HasUser(currentUser))
                        {
                            return;
                        }

                        int offset = deserializer.ReadNextInt();
                        List<NanoDBLine> files = UpServer.Instance.Files.GetFiles(currentUser);

                        if (files == null)
                        {
                            return;
                        }

                        DateTime fromDate = deserializer.ReadNextDateTime();
                        DateTime toDate = deserializer.ReadNextDateTime();
                        long fromSize = deserializer.ReadNextLong();
                        long toSize = deserializer.ReadNextLong();
                        string filter = deserializer.ReadNextString();
                        int filterMatchMode = deserializer.ReadNextInt();

                        lock (files)
                        {
                            FileRegister fileReg = UpServer.Instance.Files;
                            int currentFileIndex = offset, writtenFiles = 0;

                            serializer.Start(NetworkMethods.ListFiles);

                            int startIndex = serializer.Index;

                            serializer.Index += 4;

                            while (writtenFiles < Constants.MaxFilesPerPacket && currentFileIndex < files.Count)
                            {
                                if (fileReg.SerializeFileInfo(files[currentFileIndex], serializer, fromDate, toDate, fromSize, toSize, filter, filterMatchMode))
                                {
                                    writtenFiles++;
                                }

                                currentFileIndex++;
                            }

                            serializer.WriteNextInt(currentFileIndex == files.Count ? -1 : currentFileIndex);
                            serializer.InsertInt(writtenFiles, startIndex);
                        }

                        serializer.Flush();

                        break;
                    }
                    case NetworkMethods.DeleteFile:
                    {
                        if (!UpServer.Instance.Users.HasUser(currentUser))
                        {
                            return;
                        }

                        string fileId = deserializer.ReadNextString();

                        serializer.Start(NetworkMethods.DeleteFile);

                        if (UpServer.Instance.Files.IsOwner(fileId, currentUser))
                        {
                            UpServer.Instance.Files.SetDownloadable(fileId, false);

                            int tries = 0, maxTries = 5;

                            while (tries < maxTries)
                            {
                                try
                                {
                                    File.Delete(Path.Combine(Constants.Server.FileStorageFolder, fileId));
                                    serializer.WriteNextBool(true);
                                    tries = int.MaxValue;

                                    break;
                                }
                                catch (Exception)
                                {
                                    tries++;
                                }
                            }

                            if (tries == maxTries)
                            {
                                serializer.WriteNextBool(true);
                            }
                        }
                        else
                        {
                            serializer.WriteNextBool(false);
                        }

                        serializer.Flush();

                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
        }
    }
}
