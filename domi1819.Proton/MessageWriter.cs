using System;
using System.IO;
using System.Text;

namespace domi1819.Proton
{
    public class MessageWriter
    {
        private int offset;
        private byte[] bytes;
        private Stream stream;

        public void WriteNextBool(bool b)
        {
            this.bytes[this.offset] = b ? (byte)1 : (byte)0;
            this.offset++;
        }

        public void WriteNextInt(int i)
        {
            this.WriteInt(i, this.offset);
            this.offset += 4;
        }

        public void WriteNextLong(long l)
        {
            this.WriteLong(l, this.offset);
            this.offset += 8;
        }

        public void WriteNextString(string str)
        {
            int length = Encoding.UTF8.GetBytes(str, 0, str.Length, this.bytes, this.offset + 4);

            this.WriteNextInt(length);
            this.offset += length;
        }

        public void WriteNextDateTime(DateTime dateTime)
        {
            this.WriteNextInt(dateTime.Year);

            this.bytes[this.offset] = (byte)dateTime.Month;
            this.bytes[this.offset + 1] = (byte)dateTime.Day;
            this.bytes[this.offset + 2] = (byte)dateTime.Hour;
            this.bytes[this.offset + 3] = (byte)dateTime.Minute;
            this.bytes[this.offset + 4] = (byte)dateTime.Second;

            this.offset += 5;
        }

        private void WriteInt(int i, int index)
        {
            this.bytes[index] = (byte)(i >> 24);
            this.bytes[index + 1] = (byte)(i >> 16);
            this.bytes[index + 2] = (byte)(i >> 8);
            this.bytes[index + 3] = (byte)i;
        }

        private void WriteLong(long l, int index)
        {
            this.bytes[index] = (byte)(l >> 56);
            this.bytes[index + 1] = (byte)(l >> 48);
            this.bytes[index + 2] = (byte)(l >> 40);
            this.bytes[index + 3] = (byte)(l >> 32);
            this.bytes[index + 4] = (byte)(l >> 24);
            this.bytes[index + 5] = (byte)(l >> 16);
            this.bytes[index + 6] = (byte)(l >> 8);
            this.bytes[index + 7] = (byte)l;
        }

        private void Expand(int blockSize)
        {
            while (this.offset % blockSize > 0)
            {
                this.bytes[this.offset] = 0;
                this.offset++;
            }
        }
    }
}
