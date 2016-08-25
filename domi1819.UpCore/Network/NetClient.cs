using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography;
using domi1819.Crypto;
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

        private readonly object messageLock = new object();
        private readonly object connectLock = new object();

        private int connectHandles;

        public string Address { get; set; }
        public int Port { get; set; }
        public bool Connected { get; private set; }

        public NetClient(string address, int port)
        {
            this.Address = address;
            this.Port = port;
        }

        public void ClaimConnectHandle()
        {
            lock (this.connectLock)
            {
                if (!this.Connected)
                {
                    this.Connect();
                    this.connectHandles = 0;
                }

                this.connectHandles++;
            }
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

        public void Connect()
        {
            lock (this.messageLock)
            {
                this.client = new TcpClient();

                this.client.Connect(this.Address, this.Port);

                NetworkStream netStream = this.client.GetStream();

                RSACryptoServiceProvider rsaProvider = Rsa.GetProvider("public.key");
                RNGCryptoServiceProvider rngProvider = new RNGCryptoServiceProvider();

                byte[] secret = new byte[48];

                rngProvider.GetBytes(secret);

                byte[] encryptedKey = rsaProvider.Encrypt(secret, true);

                netStream.WriteByte((byte)(EncryptionMode.Aes128 | EncryptionMode.Rsa4096));
                netStream.Write(encryptedKey, 0, encryptedKey.Length);

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
                this.serializer.WriteNextInt(Constants.DefaultPort);
                this.serializer.Flush();

                this.deserializer.ReadMessage(NetworkMethods.GetServerVersion);

                if (this.deserializer.ReadNextInt() != Constants.DefaultPort + 1)
                {
                    throw new Exception("The connection could not be established: Connection test failed.");
                }

                if (this.deserializer.ReadNextInt() > Constants.Build)
                {
                    throw new Exception("This client is too outdated to communicate with the server. Consider updating.");
                }

                this.Connected = true;
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
                string transferKey = this.deserializer.ReadNextString();

                return shouldUpload ? transferKey : null;
            }
        }

        public void UploadPacket(string key, byte[] bytes, int start, int count)
        {
            this.CheckConnected();

            lock (this.messageLock)
            {
                this.serializer.Start(NetworkMethods.UploadPacket);
                this.serializer.WriteNextString(key);
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
                this.serializer.WriteNextString(key);
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

        private void CheckConnected()
        {
            if (!this.Connected)
            {
                throw new Exception("NetClient not connected. Connect first.");
            }
        }
    }
}
