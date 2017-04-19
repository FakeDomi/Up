using System.Net.Sockets;
using System.Security.Cryptography;

namespace domi1819.UpServer.Server
{
    internal class Connection
    {
        public ICryptoTransform Encryptor { get; set; }

        public ICryptoTransform Decryptor { get; set; }

        public NetworkStream BaseStream { get; set; }

        public CryptoStream OutStream { get; set; }

        public CryptoStream InStream { get; set; }

        public byte[] WriterBuffer { get; set; }

        public byte[] ReaderBuffer { get; set; }

        public UploadUnit UploadUnit { get; set; }

        public string UserId { get; set; }
    }
}
