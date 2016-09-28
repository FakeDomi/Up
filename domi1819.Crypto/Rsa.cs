using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace domi1819.Crypto
{
    public class Rsa
    {
        private const int RsaPrivateParamCount = 8;
        private const int RsaPublicParamCount = 2;
        private const int RsaParamLengthSize = 4;

        public static void GenerateKeyPair(string privateKeyFile, string publicKeyFile, int keySize)
        {
            using (RSACryptoServiceProvider rsaCryptoProvider = new RSACryptoServiceProvider(keySize))
            {
                RSAParameters rsaParams = rsaCryptoProvider.ExportParameters(true);
                byte[][] keyParams = { rsaParams.Modulus, rsaParams.Exponent, rsaParams.P, rsaParams.Q, rsaParams.DP, rsaParams.DQ, rsaParams.InverseQ, rsaParams.D };
                byte[] sizes = new byte[RsaParamLengthSize];

                using (FileStream privateKeyWriter = new FileStream(privateKeyFile, FileMode.Create, FileAccess.Write))
                {
                    int index = 0;
                    
                    Split(keySize, sizes);

                    WriteFormatted(0x01, ref index, privateKeyWriter);
                    WriteFormatted(sizes, ref index, privateKeyWriter);
                    
                    foreach (byte[] param in keyParams)
                    {
                        Split(param.Length, sizes);

                        WriteFormatted(sizes, ref index, privateKeyWriter);
                        WriteFormatted(param, ref index, privateKeyWriter);
                    }
                }

                using (FileStream publicKeyWriter = new FileStream(publicKeyFile, FileMode.Create, FileAccess.Write))
                {
                    int index = 0;
                    
                    Split(keySize, sizes);
                    
                    WriteFormatted(0x00, ref index, publicKeyWriter);
                    WriteFormatted(sizes, ref index, publicKeyWriter);

                    for (int i = 0; i < RsaPublicParamCount; i++)
                    {
                        Split(keyParams[i].Length, sizes);

                        WriteFormatted(sizes, ref index, publicKeyWriter);
                        WriteFormatted(keyParams[i], ref index, publicKeyWriter);
                    }
                }
            }
        }

        public static RSACryptoServiceProvider GetProvider(string keyFilePath)
        {
            RSACryptoServiceProvider rsaProvider;
            byte[][] rsaParams;
            bool privateKey = false;

            using (FileStream stream = new FileStream(keyFilePath, FileMode.Open, FileAccess.Read))
            {
                int index = 0;
                int keyType = ReadFormatted(ref index, stream);

                if (keyType == 0x01)
                {
                    privateKey = true;
                }
                else if (keyType != 0x00)
                {
                    throw new CryptographicException("Invalid key type \"{0}\". Public (0) or private (1) key required.", keyType.ToString());
                }
                
                byte[] size = new byte[RsaParamLengthSize];
                
                ReadFormatted(size, ref index, stream);
                
                rsaParams = new byte[privateKey ? RsaPrivateParamCount : RsaPublicParamCount][];
                rsaProvider = new RSACryptoServiceProvider(Unsplit(size));

                for (int i = 0; i < rsaParams.Length; i++)
                {
                    ReadFormatted(size, ref index, stream);
                    rsaParams[i] = new byte[Unsplit(size)];
                    ReadFormatted(rsaParams[i], ref index, stream);
                }
            }

            rsaProvider.ImportParameters(privateKey ? new RSAParameters { Modulus = rsaParams[0], Exponent = rsaParams[1], P = rsaParams[2], Q = rsaParams[3], DP = rsaParams[4], DQ = rsaParams[5], InverseQ = rsaParams[6], D = rsaParams[7] } : new RSAParameters { Modulus = rsaParams[0], Exponent = rsaParams[1] });

            return rsaProvider;
        }

        private static void WriteFormatted(byte[] data, ref int index, Stream stream)
        {
            foreach (byte b in data)
            {
                WriteFormatted(b, ref index, stream);
            }
        }

        private static void WriteFormatted(byte data, ref int index, Stream stream)
        {
            if (index == 16)
            {
                stream.WriteByte(10);
                index = 0;
            }

            stream.WriteByte(GetHexChar(data >> 4));
            stream.WriteByte(GetHexChar(data));
            stream.WriteByte(32);

            index++;
        }

        private static void ReadFormatted(byte[] output, ref int index, Stream stream)
        {
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = ReadFormatted(ref index, stream);
            }
        }

        private static byte ReadFormatted(ref int index, Stream stream)
        {
            if (index == 16)
            {
                stream.ReadByte();
                index = 0;
            }

            index++;

            byte retValue = (byte)(GetHexValue(stream.ReadByte()) << 4 | GetHexValue(stream.ReadByte()));
            stream.ReadByte();

            return retValue;
        }

        private static byte GetHexChar(int value)
        {
            int loNibble = value & 0x0F;

            return (byte)(loNibble + (loNibble < 10 ? 48 : 65 - 10));
        }

        private static byte GetHexValue(int chr)
        {
            if (chr < 48 || chr > 70 || (chr > 57 && chr < 65))
            {
                throw new CryptographicException($"Invalid char {chr} in hex key file.");
            }

            return (byte)((chr >= 65 ? (chr - 7) : chr) - 48);
        }

        private static void Split(int value, IList<byte> target)
        {
            target[0] = (byte)(value >> 24);
            target[1] = (byte)(value >> 16);
            target[2] = (byte)(value >> 8);
            target[3] = (byte)value;
        }

        private static int Unsplit(IList<byte> source)
        {
            return (source[0] << 24) | (source[1] << 16) | (source[2] << 8) | source[3];
        }
    }
}
