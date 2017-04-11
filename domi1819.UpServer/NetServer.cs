using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using domi1819.NanoDB;
using domi1819.UpCore.Network;
using domi1819.UpCore.Utilities;
using domi1819.UpServer.Proton;
using domi1819.UpServer.Server;

namespace domi1819.UpServer
{
    internal class NetServer
    {
        private TcpListener listener;
        
        private ArrayPool<byte> messageBufferPool;
        private Dictionary<string, UploadUnitOld> fileTransfersByKey;
        private bool shutdown;

        private RSACryptoServiceProvider rsaCsp;
        private byte[] rsaModulus;
        private byte[] rsaExponent;
        private byte[] rsaFingerprint;

        internal void Start(int port, RsaKey rsaKey)
        {
            this.listener = new TcpListener(IPAddress.Any, port);

            this.rsaCsp = rsaKey.Csp;
            this.rsaModulus = rsaKey.Modulus;
            this.rsaExponent = rsaKey.Exponent;
            this.rsaFingerprint = rsaKey.Fingerprint;
            
            this.messageBufferPool = new ArrayPool<byte>(Constants.Network.MessageBufferSize);
            this.fileTransfersByKey = new Dictionary<string, UploadUnitOld>();
            
            new Thread(this.Run) { Name = "NetServerMain" }.Start();
        }

        private void Run()
        {
            this.listener.Start();

            while (!this.shutdown)
            {
                if (this.listener.Pending())
                {
                    ThreadPool.QueueUserWorkItem(this.ProcessClient, this.listener.AcceptTcpClient());
                }
                else
                {
                    Thread.Sleep(250);
                }
            }
        }

        private void ProcessClient(object clientObject)
        {
            TcpClient client = (TcpClient)clientObject;
            NetServerConnection connection = new NetServerConnection();

            try
            {
                Console.WriteLine("Client {0} connected.", client.Client.RemoteEndPoint); //TODO

                NetworkStream stream = client.GetStream();

                connection.BaseStream = stream;

                //stream.ReadTimeout = Constants.Network.Timeout;

                stream.Write(this.rsaFingerprint, 0, this.rsaFingerprint.Length);
                
                int mode = stream.ReadByte();

                if (mode == 0x01) // Send full key
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
                    connection.OutStream = outStream;
                    connection.InStream = inStream;
                    connection.WriterBuffer = this.messageBufferPool.Get();
                    connection.ReaderBuffer = this.messageBufferPool.Get();
                    
                    MessageSerializer serializer = new MessageSerializer { Bytes = connection.WriterBuffer, Stream = outStream };
                    MessageDeserializer deserializer = new MessageDeserializer { Bytes = connection.ReaderBuffer, Stream = inStream };

                    deserializer.ReadMessage(NetworkMethods.GetServerVersion);

                    serializer.Start(NetworkMethods.GetServerVersion);
                    serializer.WriteNextInt(deserializer.ReadNextInt() + 1);
                    serializer.WriteNextInt(Constants.Server.MinClientBuild);
                    serializer.Flush();

                    stream.ReadTimeout = Timeout.Infinite;

                    this.RunMessageLoop(serializer, deserializer, connection);

                    Console.WriteLine($"Client {client.Client.RemoteEndPoint} disconnected.");
                }
                else
                {
                    Console.WriteLine($"Client {client.Client.RemoteEndPoint} tried to connect with unknown request mode {mode}. Disconnected.");
                }
            }
            catch (Exception ex)
            {
                // TODO change, currently crashes when socket is not connected
                Console.WriteLine($"Client {client.Client.RemoteEndPoint} did something stupid, I guess...");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Util.SafeDispose(connection.OutStream, connection.InStream, connection.BaseStream, connection.Encryptor, connection.Decryptor);

                if (connection.WriterBuffer != null)
                {
                    this.messageBufferPool.Return(connection.WriterBuffer);
                }

                if (connection.ReaderBuffer != null)
                {
                    this.messageBufferPool.Return(connection.ReaderBuffer);
                }

                if (connection.UploadUnits != null)
                {
                    foreach (UploadUnitOld unit in connection.UploadUnits.Where(unit => this.fileTransfersByKey.ContainsKey(unit.Key)))
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

        private void RunMessageLoopNew(MessageContext context)
        {
            ProtonConnectionUser user = new ProtonConnectionUser();

            while (true)
            {
                
            }
        }

        private void RunMessageLoop(MessageSerializer serializer, MessageDeserializer deserializer, NetServerConnection user)
        {
            string currentUser = null;

            user.UploadUnits = new List<UploadUnitOld>();

            while (true)
            {
                switch (deserializer.ReadMessage())
                {
                    //TODO: Proper error handling

                    case NetworkMethods.ConnectionClosed:
                        return;

                    case NetworkMethods.Login:
                        this.ProcessLoginPacket(serializer, deserializer, out currentUser);
                        break;
                    
                    case NetworkMethods.GetStorageInfo:
                        this.ProcessGetStorageInfoPacket(serializer, currentUser);
                        break;
                    
                    case NetworkMethods.SetPassword:
                        this.ProcessSetPasswordPacket(serializer, deserializer, currentUser);
                        break;
                    
                    case NetworkMethods.InitiateUpload:
                        this.ProcessInitiateUploadPacket(serializer, deserializer, currentUser, user);
                        break;
                    
                    case NetworkMethods.UploadPacket:
                        this.ProcessUploadPacket(deserializer);
                        break;
                    
                    case NetworkMethods.FinishUpload:
                        this.ProcessFinishUploadPacket(serializer, deserializer, currentUser, user);
                        break;
                    
                    case NetworkMethods.ListFiles:
                        this.ProcessListFilesPacket(serializer, deserializer, currentUser);
                        break;
                    
                    case NetworkMethods.DeleteFile:
                        this.ProcessDeleteFilePacket(serializer, deserializer, currentUser);
                        break;
                    
                    case NetworkMethods.LinkFormat:
                        this.ProcessLinkFormatPacket(serializer);
                        break;
                }
            }
        }

        private void ProcessLoginPacket(MessageSerializer serializer, MessageDeserializer deserializer, out string currentUser)
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
        }

        private void ProcessGetStorageInfoPacket(MessageSerializer serializer, string currentUser)
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
        }

        private void ProcessSetPasswordPacket(MessageSerializer serializer, MessageDeserializer deserializer, string currentUser)
        {
            if (!UpServer.Instance.Users.HasUser(currentUser))
            {
                return;
            }

            serializer.Start(NetworkMethods.SetPassword);
            serializer.WriteNextBool(UpServer.Instance.Users.SetPassword(currentUser, deserializer.ReadNextString()));
            serializer.Flush();
        }

        private void ProcessInitiateUploadPacket(MessageSerializer serializer, MessageDeserializer deserializer, string currentUser, NetServerConnection user)
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

                    UploadUnitOld unit = new UploadUnitOld { Key = key, User = currentUser, FileName = fileName, Size = fileSize, FileStream = new FileStream(Path.Combine(UpServer.Instance.Config.FileTransferFolder, key + ".tmp"), FileMode.Create, FileAccess.Write) };

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

        }

        private void ProcessUploadPacket(MessageDeserializer deserializer)
        {
            string key = deserializer.ReadNextString();
            int byteCount = deserializer.ReadNextInt();

            UploadUnitOld unit;

            if (!this.fileTransfersByKey.TryGetValue(key, out unit))
            {
                return;
            }

            unit.FileStream.Write(deserializer.Bytes, deserializer.Index, byteCount);
            unit.Progress += byteCount;

            //serializer.Start(NetworkMethods.UploadPacket);
            //serializer.Flush();
        }

        private void ProcessFinishUploadPacket(MessageSerializer serializer, MessageDeserializer deserializer, string currentUser, NetServerConnection user)
        {
            string key = deserializer.ReadNextString();
            UploadUnitOld unit;

            if (!this.fileTransfersByKey.TryGetValue(key, out unit) || unit.Size != unit.Progress)
            {
                return;
            }

            unit.FileStream.Dispose();

            string fileId = UpServer.Instance.Files.GetNewFileId();

            UpServer.Instance.Files.AddFile(fileId, unit.FileName, currentUser, this.fileTransfersByKey[key].Size);

            File.Move(Path.Combine(UpServer.Instance.Config.FileTransferFolder, key + ".tmp"), Path.Combine(UpServer.Instance.Config.FileStorageFolder, fileId));

            UpServer.Instance.Files.SetDownloadable(fileId, true);

            serializer.Start(NetworkMethods.FinishUpload);

            serializer.WriteNextString(string.Format(UpServer.Instance.Files.GetLinkFormat(), fileId));

            serializer.Flush();

            this.fileTransfersByKey.Remove(key);

            user.UploadUnits.Remove(unit);
            user.ReservedBytes -= unit.Size;
        }

        private void ProcessListFilesPacket(MessageSerializer serializer, MessageDeserializer deserializer, string currentUser)
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
                FileManager fileReg = UpServer.Instance.Files;
                int currentFileIndex = offset, writtenFiles = 0;

                serializer.Start(NetworkMethods.ListFiles);

                int startIndex = serializer.Index;

                serializer.Index += 4;

                while (writtenFiles < Constants.Network.MaxFilesPerPacket && currentFileIndex < files.Count)
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

        }

        private void ProcessDeleteFilePacket(MessageSerializer serializer, MessageDeserializer deserializer, string currentUser)
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
                        File.Delete(Path.Combine(UpServer.Instance.Config.FileStorageFolder, fileId));
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
        }

        private void ProcessLinkFormatPacket(MessageSerializer serializer)
        {
            serializer.Start(NetworkMethods.LinkFormat);

            serializer.WriteNextString(UpServer.Instance.Files.GetLinkFormat());

            serializer.Flush();
        }
    }
}
