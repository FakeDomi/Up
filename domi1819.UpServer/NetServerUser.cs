using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace domi1819.UpServer
{
    internal class NetServerUser
    {
        internal NetworkStream BaseStream { get; set; }
        internal CryptoStream OutStream { get; set; }
        internal CryptoStream InStream { get; set; }
        internal ICryptoTransform Encryptor { get; set; }
        internal ICryptoTransform Decryptor { get; set; }
        internal byte[] SerializeBuffer { get; set; }
        internal byte[] DeserializeBuffer { get; set; }

        internal List<UploadUnit> UploadUnits { get; set; }
        internal long ReservedBytes { get; set; }
    }
}
