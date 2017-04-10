using System;
using System.IO;
using System.Text;

namespace domi1819.Proton
{
    public class MessageReader
    {
        private int offset;
        private byte[] bytes;
        private Stream stream;

        public bool ReadNextBool()
        {
            this.offset++;
            return this.bytes[this.offset - 1] != 0;
        }

        public int ReadNextInt()
        {
            int value = this.ReadInt(this.offset);
            this.offset += 4;

            return value;
        }

        public long ReadNextLong()
        {
            long value = this.ReadLong(this.offset);
            this.offset += 8;

            return value;
        }

        public string ReadNextString()
        {
            int length = this.ReadNextInt();

            string result = Encoding.UTF8.GetString(this.bytes, this.offset, length);

            this.offset += length;

            return result;
        }

        public DateTime ReadNextDateTime()
        {
            int year = this.ReadNextInt();
            int month = this.bytes[this.offset];
            int day = this.bytes[this.offset + 1];
            int hour = this.bytes[this.offset + 2];
            int minute = this.bytes[this.offset + 3];
            int second = this.bytes[this.offset + 4];

            this.offset += 5;

            return new DateTime(year, month, day, hour, minute, second);
        }

        private int ReadInt(int index)
        {
            return this.bytes[index] << 24 | this.bytes[index + 1] << 16 | this.bytes[index + 2] << 8 | this.bytes[index + 3];
        }

        private long ReadLong(int index)
        {
            return (long)this.bytes[index] << 56 | (long)this.bytes[index + 1] << 48 | (long)this.bytes[index + 2] << 40 | (long)this.bytes[index + 3] << 32 | (long)this.bytes[index + 4] << 24 | (long)this.bytes[index + 5] << 16 | (long)this.bytes[index + 6] << 8 | this.bytes[index + 7];
        }
    }
}
