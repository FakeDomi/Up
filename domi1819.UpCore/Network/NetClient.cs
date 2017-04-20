using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography;
using domi1819.UpCore.Utilities;

namespace domi1819.UpCore.Network
{
    public class NetClient
    {
        private TcpClient client;

        private CryptoStream inStream;
        private CryptoStream outStream;

        private MessageSerializer serializer;
        private MessageDeserializer deserializer;
        
        private readonly RsaCache rsaCache;
        
        private readonly object messageLock = new object();
        private readonly object connectLock = new object();

        private int connectHandles;

        public string Host { private get; set; }
        public int Port { private get; set; }

        public bool Connected { get; private set; }

        public delegate bool AddItemCallback(string fileId, string fileName, long fileSize, DateTime updateDate, int downloads);

        public NetClient(string host, int port, RsaCache rsaCache)
        {
            this.Host = host;
            this.Port = port;

            this.rsaCache = rsaCache;
        }

        public bool ClaimConnectHandle()
        {
            lock (this.connectLock)
            {
                if (!this.Connected)
                {
                    if (!this.Connect())
                    {
                        return false;
                    }

                    this.connectHandles = 0;
                }

                this.connectHandles++;
            }

            return true;
        }

        public void ReleaseConnectHandle()
        {
            lock (this.connectLock)
            {
                this.connectHandles--;

                if (this.connectHandles == 0)
                {
                    this.Disconnect();
                }
            }
        }

        public bool Connect()
        {
            lock (this.messageLock)
            {
                this.client = new TcpClient();
                this.client.Connect(this.Host, this.Port);

                NetworkStream netStream = this.client.GetStream();
                byte[] keyFingerprint = new byte[Constants.Encryption.FingerprintSize];
                string serverAddress = $"{this.Host}:{this.Port}";

                netStream.Read(keyFingerprint, 0, keyFingerprint.Length);

                if (!this.rsaCache.LoadKey(serverAddress))
                {
                    // TODO: No valid key found.
                    this.client.Close();
                    return false;
                }

                RsaKey rsaKey = this.rsaCache.Key;
                byte[] serverFingerprint = rsaKey.Fingerprint;

                for (int i = 0; i < keyFingerprint.Length; i++)
                {
                    if (keyFingerprint[i] != serverFingerprint[i])
                    {
                        // TODO: Server key fingerprint doesn't match local key fingerprint.
                        this.client.Close();
                        return false;
                    }
                }

                netStream.WriteByte(0x00);

                RSACryptoServiceProvider rsaCsp = rsaKey.Csp;
                RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

                byte[] secret = new byte[48];

                rngCsp.GetBytes(secret);

                byte[] encryptedSecret = rsaCsp.Encrypt(secret, true);
                
                netStream.Write(encryptedSecret, 0, encryptedSecret.Length);

                byte[] key = new byte[16];
                byte[] ivEncrypt = new byte[16];
                byte[] ivDecrypt = new byte[16];

                Array.Copy(secret, 0, key, 0, 16);
                Array.Copy(secret, 16, ivEncrypt, 0, 16);
                Array.Copy(secret, 32, ivDecrypt, 0, 16);

                RijndaelManaged rijndaelEncrypt = new RijndaelManaged { Key = key, IV = ivEncrypt, Mode = CipherMode.CBC, Padding = PaddingMode.None };
                RijndaelManaged rijndaelDecrypt = new RijndaelManaged { Key = key, IV = ivDecrypt, Mode = CipherMode.CBC, Padding = PaddingMode.None };

                this.outStream = new CryptoStream(netStream, rijndaelEncrypt.CreateEncryptor(), CryptoStreamMode.Write);
                this.inStream = new CryptoStream(netStream, rijndaelDecrypt.CreateDecryptor(), CryptoStreamMode.Read) /*{ ReadTimeout = Constants.Network.Timeout }*/;

                this.serializer = new MessageSerializer { Bytes = new byte[65536], Stream = this.outStream };
                this.deserializer = new MessageDeserializer { Bytes = new byte[65536], Stream = this.inStream };

                this.serializer.Start(NetworkMethods.GetServerVersion);
                this.serializer.WriteNextInt(Constants.Server.DefaultPort);
                this.serializer.Flush();

                this.deserializer.ReadMessage(NetworkMethods.GetServerVersion);

                if (this.deserializer.ReadNextInt() != Constants.Server.DefaultPort + 1)
                {
                    throw new Exception("The connection could not be established: Connection test failed.");
                }

                if (this.deserializer.ReadNextInt() > Constants.Build)
                {
                    throw new Exception("This client is too outdated to communicate with the server. Consider updating.");
                }

                this.Connected = true;
            }

            return true;
        }

        public RsaKey FetchKey()
        {
            lock (this.messageLock)
            {
                TcpClient tempClient = new TcpClient();

                tempClient.Connect(this.Host, this.Port);

                NetworkStream netStream = tempClient.GetStream();
                byte[] keyFingerprint = new byte[Constants.Encryption.FingerprintSize];
                
                netStream.Read(keyFingerprint, 0, keyFingerprint.Length);
                netStream.WriteByte(0x01);
                
                byte[] rsaModulus = new byte[Constants.Encryption.RsaModulusBytes];
                byte[] rsaExponent = new byte[Constants.Encryption.RsaExponentBytes];

                netStream.Read(rsaModulus, 0, rsaModulus.Length);
                netStream.Read(rsaExponent, 0, rsaExponent.Length);

                return RsaKey.FromParams(rsaModulus, rsaExponent);
            }
        }

        public void Disconnect()
        {
            this.client?.Close();
            this.Connected = false;
        }
        
        public bool Login(string userId, string password)
        {
            this.CheckConnected();

            lock (this.messageLock)
            {
                this.serializer.Start(NetworkMethods.Login);
                this.serializer.WriteNextString(userId);
                this.serializer.WriteNextString(password);
                this.serializer.Flush();

                this.deserializer.ReadMessage(NetworkMethods.Login);

                return this.deserializer.ReadNextBool();
            }
        }

        public bool Login(Config.Config config)
        {
            return this.Login(config.UserId, config.Password);
        }

        public StorageInfo GetStorageInfo()
        {
            this.CheckConnected();

            lock (this.messageLock)
            {
                this.serializer.Start(NetworkMethods.GetStorageInfo);
                this.serializer.Flush();

                this.deserializer.ReadMessage(NetworkMethods.GetStorageInfo);

                return new StorageInfo { MaxCapacity = this.deserializer.ReadNextLong(), UsedCapacity = this.deserializer.ReadNextLong(), FileCount = this.deserializer.ReadNextInt() };
            }
        }

        public bool SetPassword(string newPassword)
        {
            this.CheckConnected();

            lock (this.messageLock)
            {
                this.serializer.Start(NetworkMethods.SetPassword);
                this.serializer.WriteNextString(newPassword);
                this.serializer.Flush();

                this.deserializer.ReadMessage(NetworkMethods.SetPassword);

                return this.deserializer.ReadNextBool();
            }
        }

        public string InitiateUpload(string fileName, long fileSize)
        {
            this.CheckConnected();

            lock (this.messageLock)
            {
                this.serializer.Start(NetworkMethods.InitiateUpload);
                this.serializer.WriteNextString(fileName);
                this.serializer.WriteNextLong(fileSize);
                this.serializer.Flush();

                this.deserializer.ReadMessage(NetworkMethods.InitiateUpload);

                bool shouldUpload = this.deserializer.ReadNextBool();
                
                return shouldUpload ? "12345678" : null; //TODO: Server doesn't need transfer keys anymore
            }
        }

        public void UploadPacket(string key, byte[] bytes, int start, int count)
        {
            this.CheckConnected();

            lock (this.messageLock)
            {
                this.serializer.Start(NetworkMethods.UploadPacket);
                //this.serializer.WriteNextString(key);
                this.serializer.WriteNextInt(count);

                Array.Copy(bytes, start, this.serializer.Bytes, this.serializer.Index, count - start);

                this.serializer.Index += count - start;
                
                this.serializer.Flush();

                //this.deserializer.ReadMessage(NetworkMethods.UploadPacket);
            }
        }

        public string FinishUpload(string key)
        {
            this.CheckConnected();

            lock (this.messageLock)
            {
                this.serializer.Start(NetworkMethods.FinishUpload);
                //this.serializer.WriteNextString(key);
                this.serializer.Flush();

                this.deserializer.ReadMessage(NetworkMethods.FinishUpload);

                return this.deserializer.ReadNextString();
            }
        }

        public int ListFiles(List<FileDetails> fileList, int offset, DateTime fromDate, DateTime toDate, long fromSize, long toSize, string fileNameFilter, int filterMatchMode)
        {
            this.CheckConnected();

            lock (this.messageLock)
            {
                this.serializer.Start(NetworkMethods.ListFiles);
                this.serializer.WriteNextInt(offset);
                this.serializer.WriteNextDateTime(fromDate);
                this.serializer.WriteNextDateTime(toDate);
                this.serializer.WriteNextLong(fromSize);
                this.serializer.WriteNextLong(toSize);
                this.serializer.WriteNextString(fileNameFilter);
                this.serializer.WriteNextInt(filterMatchMode);
                this.serializer.Flush();

                this.deserializer.ReadMessage(NetworkMethods.ListFiles);

                int maxRead = this.deserializer.ReadNextInt();

                for (int i = 0; i < maxRead; i++)
                {
                    fileList.Add(new FileDetails { FileId = this.deserializer.ReadNextString(), FileName = this.deserializer.ReadNextString(), FileSize = this.deserializer.ReadNextLong(), UploadDate = this.deserializer.ReadNextDateTime(), Downloads = this.deserializer.ReadNextInt() });
                }

                return this.deserializer.ReadNextInt();
            }
        }

        public int ListFiles(AddItemCallback addItemCallback, int offset, DateTime fromDate, DateTime toDate, long fromSize, long toSize, string fileNameFilter, int filterMatchMode)
        {
            this.CheckConnected();

            lock (this.messageLock)
            {
                this.serializer.Start(NetworkMethods.ListFiles);
                this.serializer.WriteNextInt(offset);
                this.serializer.WriteNextDateTime(fromDate);
                this.serializer.WriteNextDateTime(toDate);
                this.serializer.WriteNextLong(fromSize);
                this.serializer.WriteNextLong(toSize);
                this.serializer.WriteNextString(fileNameFilter);
                this.serializer.WriteNextInt(filterMatchMode);
                this.serializer.Flush();

                this.deserializer.ReadMessage(NetworkMethods.ListFiles);

                int maxRead = this.deserializer.ReadNextInt();

                for (int i = 0; i < maxRead; i++)
                {
                    addItemCallback.Invoke(this.deserializer.ReadNextString(), this.deserializer.ReadNextString(), this.deserializer.ReadNextLong(), this.deserializer.ReadNextDateTime(), this.deserializer.ReadNextInt());
                }

                return this.deserializer.ReadNextInt();
            }
        }

        public bool DeleteFile(string fileId)
        {
            this.CheckConnected();

            lock (this.messageLock)
            {
                this.serializer.Start(NetworkMethods.DeleteFile);
                this.serializer.WriteNextString(fileId);
                this.serializer.Flush();

                this.deserializer.ReadMessage(NetworkMethods.DeleteFile);

                return this.deserializer.ReadNextBool();
            }
        }

        public string GetLinkFormat()
        {
            this.CheckConnected();

            lock (this.messageLock)
            {
                this.serializer.Start(NetworkMethods.LinkFormat);
                this.serializer.Flush();

                this.deserializer.ReadMessage(NetworkMethods.LinkFormat);

                return this.deserializer.ReadNextString();
            }
        }

        private void CheckConnected()
        {
            if (!this.Connected)
            {
                throw new Exception("NetClient not connected. Connect first.");
            }
        }
    }
}
