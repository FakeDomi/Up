using System;
using System.Net.Sockets;
using System.Security.Cryptography;
using domi1819.UpCore.Utilities;

namespace domi1819.UpServer.Server
{
    internal class Connection
    {
        public ICryptoTransform Encryptor { get; private set; }

        public ICryptoTransform Decryptor { get; private set; }

        public NetworkStream BaseStream { get; set; }

        public CryptoStream OutStream { get; private set; }

        public CryptoStream InStream { get; private set; }

        public byte[] WriterBuffer { get; private set; }

        public byte[] ReaderBuffer { get; private set; }

        public UploadUnit UploadUnit { get; set; }

        public string UserId { get; set; }

        public void InitializeSymmetricEncryption(byte[] secret, NetworkStream baseStream, ArrayPool<byte> arrayPool)
        {
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

            CryptoStream outStream = new CryptoStream(baseStream, encryptor, CryptoStreamMode.Write);
            CryptoStream inStream = new CryptoStream(baseStream, decryptor, CryptoStreamMode.Read);

            this.Encryptor = encryptor;
            this.Decryptor = decryptor;
            this.OutStream = outStream;
            this.InStream = inStream;
            this.WriterBuffer = arrayPool.Get();
            this.ReaderBuffer = arrayPool.Get();
        }

        public void Cleanup(ArrayPool<byte> arrayPool)
        {
            Util.SafeDispose(this.OutStream, this.InStream, this.BaseStream, this.Encryptor, this.Decryptor);

            arrayPool.Return(this.WriterBuffer);
            arrayPool.Return(this.ReaderBuffer);

            this.UploadUnit?.Cleanup();
        }
    }
}
