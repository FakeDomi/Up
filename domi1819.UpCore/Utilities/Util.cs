using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace domi1819.UpCore.Utilities
{
    public static class Util
    {
        private static readonly string[] SizeUnits = { " B", " KB", " MB", " GB", " TB", " PB" };
        private static readonly string[] RevSizeUnits = { "", "k", "m", "g", "t", "p" };
        private static readonly char[] RandomStringElements = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        private static readonly Random Random = new Random();

        public static string GetByteSizeText(long bytes)
        {
            int exp = 0;
            double bytesL = bytes;

            while (bytesL >= 1024 && exp < SizeUnits.Length)
            {
                bytesL /= 1024;
                exp++;
            }

            if (bytesL < 10)
            {
                return (long)(bytesL * 100) / 100D + SizeUnits[exp];
            }

            if (bytesL < 100)
            {
                return (long)(bytesL * 10) / 10D + SizeUnits[exp];
            }

            return (long)bytesL + SizeUnits[exp];
        }

        public static long? GetRevSize(string input)
        {
            string str = input.ToLower();
            int exp = 0;

            for (int i = 0; i < RevSizeUnits.Length; i++)
            {
                if (input.Contains(RevSizeUnits[i]))
                {
                    exp = i;
                }
            }

            Match match = Regex.Match(str, @"\d+");
            long longValue;

            if (match.Success && long.TryParse(match.Value, out longValue))
            {
                return longValue * (long)Math.Pow(1024, exp);
            }

            return null;
        }

        public static string GetRandomString(int length)
        {
            char[] chars = new char[length];

            for (int i = 0; i < length; i++)
            {
                chars[i] = RandomStringElements[Random.Next(RandomStringElements.Length)];
            }

            return new string(chars);
        }

        public static byte[] Hash(string text, string salt)
        {
            byte[] result;

            using (SHA256Managed sha = new SHA256Managed())
            {
                result = sha.ComputeHash(Encoding.UTF8.GetBytes(text + "$s:" + salt));
            }

            return result;
        }

        public static void SafeDispose(params IDisposable[] objects)
        {
            foreach (IDisposable disposableObj in objects.Where(disposableObj => disposableObj != null))
            {
                try
                {
                    disposableObj.Dispose();
                }
                catch (Exception)
                {
                    // Blah...
                }
            }
        }

        public static string CreateTempFolder()
        {
            string path;

            do
            {
                path = Path.Combine(Path.GetTempPath(), $"up_{GetRandomString(4)}");
            } while (Directory.Exists(path));

            Directory.CreateDirectory(path);

            return path;
        }

        public static char GetHexChar(int value)
        {
            int loNibble = value & 0x0F;

            return (char)(loNibble + (loNibble < 10 ? '0' : 'A' - 10));
        }
    }
}
