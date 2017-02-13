using System;
using System.Drawing;

namespace domi1819.UpCore.Utilities
{
    public static class ExtensionConverters
    {
        public static string Pad(this int value, int length)
        {
            return value.ToString().PadLeft(length, '0');
        }

        public static string ToHex(this byte value)
        {
            return $"{Util.GetHexChar(value >> 4)}{Util.GetHexChar(value)}";
        }

        public static string ToHexString(this Color color)
        {
            return $"#{(color.A == 255 ? "" : color.A.ToHex())}{color.R.ToHex()}{color.G.ToHex()}{color.B.ToHex()}";
        }

        public static string ToHexString(this byte[] bytes, int groupedChars = 0, int limit = -1, char separator = '-')
        {
            char[] chars;

            if (limit < 0 || limit > bytes.Length)
            {
                limit = bytes.Length;
            }

            if (groupedChars == 0 || groupedChars >= limit * 2)
            {
                chars = new char[limit * 2];

                for (int i = 0; i < limit; i++)
                {
                    chars[i * 2] = Util.GetHexChar(bytes[i] >> 4);
                    chars[i * 2 + 1] = Util.GetHexChar(bytes[i]);
                }
            }
            else
            {
                chars = new char[limit * 2 + ((limit * 2 - 1) / groupedChars)];

                int position = 0;
                int count = groupedChars;
                int i = 0;

                foreach (byte b in bytes)
                {
                    if (i >= limit)
                    {
                        break;
                    }

                    if (count > 1)
                    {
                        chars[position] = Util.GetHexChar(b >> 4);
                        chars[position + 1] = Util.GetHexChar(b);

                        count -= 2;
                        position += 2;
                    }
                    else if (count == 1)
                    {
                        chars[position] = Util.GetHexChar(b >> 4);
                        chars[position + 1] = separator;
                        chars[position + 2] = Util.GetHexChar(b);

                        count = groupedChars - 1;
                        position += 3;
                    }
                    else if (count == 0)
                    {
                        chars[position] = separator;
                        chars[position + 1] = Util.GetHexChar(b >> 4);

                        if (groupedChars == 1)
                        {
                            chars[position + 2] = separator;
                            chars[position + 3] = Util.GetHexChar(b);

                            count = 0;
                            position += 4;
                        }
                        else
                        {
                            chars[position + 2] = Util.GetHexChar(b);

                            count = groupedChars - 2;
                            position += 3;
                        }
                    }

                    i++;
                }
            }

            return new string(chars);
        }
        
        public static string FormatString(this DateTime dateTime)
        {
            return $"{dateTime.Year}-{dateTime.Month.Pad(2)}-{dateTime.Day.Pad(2)} {dateTime.Hour.Pad(2)}:{dateTime.Minute.Pad(2)}:{dateTime.Second.Pad(2)}";
        }
    }
}
