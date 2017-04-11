using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace domi1819.UpServer
{
    internal class NetServerConnection
    {
        public ICryptoTransform Encryptor { get; set; }

        public ICryptoTransform Decryptor { get; set; }

        public NetworkStream BaseStream { get; set; }

        public CryptoStream OutStream { get; set; }

        public CryptoStream InStream { get; set; }

        public byte[] WriterBuffer { get; set; }

        public byte[] ReaderBuffer { get; set; }


        // TODO remove vvvvvv

        public List<UploadUnitOld> UploadUnits { get; set; }

        public long ReservedBytes { get; set; }
    }
}
