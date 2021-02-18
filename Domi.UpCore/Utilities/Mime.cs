using System.Collections.Generic;
using System.IO;

namespace Domi.UpCore.Utilities
{
    public static class Mime
    {
        private const byte Mask1 = 0b1000_0000;
        private const byte Mask2 = 0b1100_0000;
        private const byte Mask3 = 0b1110_0000;
        private const byte Mask4 = 0b1111_0000;
        private const byte Mask5 = 0b1111_1000;
        
        private const byte Length1 = 0b0000_0000;
        private const byte Length2 = 0b1100_0000;
        private const byte Length3 = 0b1110_0000;
        private const byte Length4 = 0b1111_0000;

        private const byte ContByte = 0b1000_0000;

        private static readonly Dictionary<string, string> MimeCache = new Dictionary<string, string>();

        /// <summary>
        /// Tries to analyze whether a given file is encoded in UTF-8 or an ASCII-based code page.
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
            int bytesRead;
            bool asciiMarker = false;

            using (FileStream fs = File.OpenRead(filePath))
            {
                bytesRead = fs.Read(bytes, 0, bytes.Length);
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

                // Single byte
                if (asciiMarker || (b & Mask1) == Length1)
                {
                    continue;
                }

                // Double byte
                if ((b & Mask3) == Length2)
                {
                    if (i + 1 < bytesRead)
                    {
                        byte b2 = bytes[i + 1];

                        if ((b2 & Mask2) == ContByte)
                        {
                            int codepoint = (b & ~Mask3) << 6 | (b2 & ~Mask2);

                            if (codepoint >= 0x0080 && codepoint < 0x0800)
                            {
                                i += 1;
                                continue;
                            }
                        }
                    }
                    else
                    {
                        goto utf8EndOfText;
                    }
                }
                // Triple byte
                else if ((b & Mask4) == Length3)
                {
                    if (i + 2 < bytesRead)
                    {
                        byte b2 = bytes[i + 1];
                        byte b3 = bytes[i + 2];

                        if ((b2 & Mask2) == ContByte && (b3 & Mask2) == ContByte)
                        {
                            int codepoint = (b & ~Mask3) << 12 | (b2 & ~Mask2) << 6 | (b3 & ~Mask2);

                            if (codepoint >= 0x0800 && codepoint < 0x10000)
                            {
                                i += 2;
                                continue;
                            }
                        }
                    }
                    else
                    {
                        goto utf8EndOfText;
                    }
                }
                // Quadruple byte
                else if ((b & Mask5) == Length4)
                {
                    if (i + 3 < bytesRead)
                    {
                        byte b2 = bytes[i + 1];
                        byte b3 = bytes[i + 2];
                        byte b4 = bytes[i + 3];

                        if ((b2 & Mask2) == ContByte && (b3 & Mask2) == ContByte && (b4 & Mask2) == ContByte)
                        {
                            int codepoint = (b & ~Mask3) << 18 | (b2 & ~Mask2) << 12 | (b3 & ~Mask2) << 6 | (b4 & ~Mask2);

                            if (codepoint >= 0x10000 && codepoint < 0x10FFFF)
                            {
                                i += 3;
                                continue;
                            }
                        }
                    }
                    else
                    {
                        goto utf8EndOfText;
                    }
                }

                asciiMarker = true;
            }

            if (asciiMarker)
            {
                mime = "text/plain";
                MimeCache.Add(cacheKey, mime);
                return true;
            }

            utf8EndOfText:
            mime = "text/plain; charset=utf-8";
            MimeCache.Add(cacheKey, mime);
            return true;
        }
    }
}
