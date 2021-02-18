using System;
using System.IO;
using System.Security.Cryptography;
using Domi.UpCore.Crypto;
using Domi.UpCore.Utilities;

namespace Domi.UpCore.Network
{
    public class RsaKey : IDisposable
    {
        public byte[] Fingerprint { get; private set; }

        public byte[] Modulus { get; private set; }

        public byte[] Exponent { get; private set; }

        public RSACryptoServiceProvider Csp { get; private set; }

        public void Dispose()
        {
            this.Csp?.Dispose();
        }

        public static RsaKey FromFile(string path)
        {
            if (File.Exists(path))
            {
                RsaKey key = new RsaKey();

                try
                {
                    key.Csp = Rsa.GetProvider(path);
                }
                catch (Exception)
                {
                    return null;
                }

                RSAParameters rsaParams = key.Csp.ExportParameters(false);

                key.Fingerprint = GetFingerprint(rsaParams.Modulus, rsaParams.Exponent);

                key.Modulus = rsaParams.Modulus;
                key.Exponent = rsaParams.Exponent;

                return key;
            }

            return null;
        }

        public static RsaKey FromParams(byte[] modulus, byte[] exponent)
        {
            RsaKey key = new RsaKey { Csp = new RSACryptoServiceProvider() };

            key.Csp.ImportParameters(new RSAParameters { Modulus = modulus, Exponent = exponent });

            key.Fingerprint = GetFingerprint(modulus, exponent);
            key.Modulus = modulus;
            key.Exponent = exponent;

            return key;
        }

        private static byte[] GetFingerprint(byte[] modulus, byte[] exponent)
        {
            byte[] keyBytes = new byte[modulus.Length + exponent.Length];

            Array.Copy(modulus, 0, keyBytes, 0, modulus.Length);
            Array.Copy(exponent, 0, keyBytes, modulus.Length, exponent.Length);

            return Blake2.Hash(keyBytes, (byte)Constants.Encryption.FingerprintSize);
        }
    }
}
