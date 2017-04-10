using System.Security.Cryptography;

namespace domi1819.Proton
{
    internal class ProtonServerConnection
    {
        public ICryptoTransform Encryptor { get; set; }

        public ICryptoTransform Decryptor { get; set; }
    }
}
