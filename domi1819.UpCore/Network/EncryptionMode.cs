using System;

namespace domi1819.UpCore.Network
{
    [Flags]
    public enum EncryptionMode
    {
        Rsa1024 = 0x01,
        Rsa2048 = 0x02,
        Rsa4096 = 0x04,
        Aes128 = 0x10,
        Aes192 = 0x20,
        Aes256 = 0x40,
        Rsa = Rsa1024 | Rsa2048 | Rsa4096
    }
}
