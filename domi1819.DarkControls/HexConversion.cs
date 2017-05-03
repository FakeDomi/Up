using System.Drawing;

namespace domi1819.DarkControls
{
    internal static class HexConversion
    {
        internal static string ToHexString(this Color color)
        {
            return $"#{(color.A == 255 ? "" : ToHex(color.A))}{ToHex(color.R)}{ToHex(color.G)}{ToHex(color.B)}";
        }

        private static string ToHex(byte value)
        {
            return $"{GetHexChar(value >> 4)}{GetHexChar(value)}";
        }

        private static char GetHexChar(int value)
        {
            int loNibble = value & 0x0F;

            return (char)(loNibble + (loNibble < 10 ? '0' : 'A' - 10));
        }
    }
}
