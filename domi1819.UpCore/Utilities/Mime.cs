using System.Collections.Generic;
using System.IO;
using System.Text;

namespace domi1819.UpCore.Utilities
{
    public static class Mime
    {
        private static readonly Decoder Utf8Decoder = Encoding.GetEncoding(Encoding.UTF8.CodePage, new EncoderExceptionFallback(), new DecoderExceptionFallback()).GetDecoder();
        private static readonly Dictionary<string, string> MimeCache = new Dictionary<string, string>();

        /// <summary>
        /// Tries to analyze whether a given text file is encoded in UTF-8, UTF-16BE, UTF-16LE or an ASCII-based code page.
        /// </summary>
        /// <param name="filePath">The path of the file to check.</param>
        /// <param name="cacheKey">A key that is unique to this file. For Up it's the file id.</param>
        /// <param name="mime">The mime type of the text file, or null.</param>
        /// <returns>True if the file has been detected as a text file, false otherwise.</returns>
        public static bool GuessTextType(string filePath, string cacheKey, out string mime)
        {
            if (MimeCache.TryGetValue(cacheKey, out mime))
            {
                return mime != null;
            }

            byte[] bytes = new byte[Constants.Server.SniffBytes];

            using (FileStream fs = File.OpenRead(filePath))
            {
                int bytesRead = fs.Read(bytes, 0, bytes.Length);

                try
                {
                    Utf8Decoder.GetCharCount(bytes, 0, bytesRead);

                    mime = "text/plain; charset=utf-8";
                    MimeCache.Add(cacheKey, mime);
                    return true;
                }
                catch (DecoderFallbackException)
                {
                    if (bytesRead >= 2)
                    {
                        if (bytes[0] == 0xfe && bytes[1] == 0xff)
                        {
                            mime = "text/plain; charset=utf-16be";
                            MimeCache.Add(cacheKey, mime);
                            return true;
                        }
                        else if (bytes[0] == 0xff && bytes[1] == 0xfe)
                        {
                            mime = "text/plain; charset=utf-16le";
                            MimeCache.Add(cacheKey, mime);
                            return true;
                        }
                    }

                    for (int i = 0; i < bytesRead; i++)
                    {
                        byte b = bytes[i];
                        switch (b)
                        {
                            case 0x00:
                            case 0x01:
                            case 0x02:
                            case 0x03:
                            case 0x04:
                            case 0x05:
                            case 0x06:
                            case 0x07:
                            case 0x08:
                            case 0x0E:
                            case 0x0F:
                            case 0x10:
                            case 0x11:
                            case 0x12:
                            case 0x13:
                            case 0x14:
                            case 0x15:
                            case 0x16:
                            case 0x17:
                            case 0x18:
                            case 0x19:
                            case 0x1A:
                            case 0x1B:
                            case 0x1C:
                            case 0x1D:
                            case 0x1E:
                            case 0x1F:
                            case 0x7F:
                                mime = null;
                                MimeCache.Add(cacheKey, null);
                                return false;
                        }
                    }

                    mime = "text/plain";
                    MimeCache.Add(cacheKey, mime);
                    return true;
                }
            }
        }
    }
}
