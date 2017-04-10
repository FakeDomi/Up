using System;
using System.IO;
using System.Text;

namespace domi1819.Proton
{
    public class MessageReader
    {
        private int offset;

        internal byte[] Bytes { get; set; }

        internal Stream Stream { get; set; }

        public bool ReadNextBool()
        {
            this.offset++;
            return this.Bytes[this.offset - 1] != 0;
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

            string result = Encoding.UTF8.GetString(this.Bytes, this.offset, length);

            this.offset += length;

            return result;
        }

        public DateTime ReadNextDateTime()
        {
            int year = this.ReadNextInt();
            int month = this.Bytes[this.offset];
            int day = this.Bytes[this.offset + 1];
            int hour = this.Bytes[this.offset + 2];
            int minute = this.Bytes[this.offset + 3];
            int second = this.Bytes[this.offset + 4];

            this.offset += 5;

            return new DateTime(year, month, day, hour, minute, second);
        }

        private int ReadInt(int index)
        {
            return this.Bytes[index] << 24 | this.Bytes[index + 1] << 16 | this.Bytes[index + 2] << 8 | this.Bytes[index + 3];
        }

        private long ReadLong(int index)
        {
            return (long)this.Bytes[index] << 56 | (long)this.Bytes[index + 1] << 48 | (long)this.Bytes[index + 2] << 40 | (long)this.Bytes[index + 3] << 32 | (long)this.Bytes[index + 4] << 24 | (long)this.Bytes[index + 5] << 16 | (long)this.Bytes[index + 6] << 8 | this.Bytes[index + 7];
        }
    }
}
