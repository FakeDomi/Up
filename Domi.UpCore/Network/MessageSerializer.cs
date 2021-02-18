using System;
using System.IO;
using System.Text;
using Domi.UpCore.Utilities;

namespace Domi.UpCore.Network
{
    public class MessageSerializer
    {
        public int Index { get; set; }
        public byte[] Bytes { get; set; }
        public Stream Stream { get; set; }

        public void Start(int method)
        {
            this.WriteInt(method, 4);
            this.Index = 8;
        }

        public void WriteNextBool(bool b)
        {
            this.Bytes[this.Index] = b ? (byte)1 : (byte)0;
            this.Index++;
        }

        public void WriteNextInt(int i)
        {
            this.WriteInt(i, this.Index);
            this.Index += 4;
        }

        public void WriteNextLong(long l)
        {
            this.WriteLong(l, this.Index);
            this.Index += 8;
        }

        public void WriteNextString(string str)
        {
            int length = Encoding.UTF8.GetBytes(str, 0, str.Length, this.Bytes, this.Index + 4);

            this.WriteNextInt(length);
            this.Index += length;
        }

        public void WriteNextByteArray(byte[] arr, int start, int count)
        {
            this.WriteNextInt(count - start);

            for (int i = start; i < start + count; i++)
            {
                this.Bytes[this.Index] = arr[i];
                this.Index++;
            }
        }

        public void WriteNextDateTime(DateTime dateTime)
        {
            this.WriteNextInt(dateTime.Year);

            this.Bytes[this.Index] = (byte)dateTime.Month;
            this.Bytes[this.Index + 1] = (byte)dateTime.Day;
            this.Bytes[this.Index + 2] = (byte)dateTime.Hour;
            this.Bytes[this.Index + 3] = (byte)dateTime.Minute;
            this.Bytes[this.Index + 4] = (byte)dateTime.Second;

            this.Index += 5;
        }

        public void InsertInt(int i, int index)
        {
            this.WriteInt(i, index);
        }

        public void Expand(int blockSize)
        {
            while (this.Index % blockSize > 0)
            {
                this.Bytes[this.Index] = 0;
                this.Index++;
            }
        }

        public void Flush()
        {
            int size = this.Index;

            this.WriteInt(size, 0);
            this.Expand(Constants.Encryption.AesBlockSize);

            this.Stream.Write(this.Bytes, 0, this.Index);
        }

        private void WriteInt(int i, int index)
        {
            this.Bytes[index] = (byte)(i >> 24);
            this.Bytes[index + 1] = (byte)(i >> 16);
            this.Bytes[index + 2] = (byte)(i >> 8);
            this.Bytes[index + 3] = (byte)i;
        }

        private void WriteLong(long l, int index)
        {
            this.Bytes[index] = (byte)(l >> 56);
            this.Bytes[index + 1] = (byte)(l >> 48);
            this.Bytes[index + 2] = (byte)(l >> 40);
            this.Bytes[index + 3] = (byte)(l >> 32);
            this.Bytes[index + 4] = (byte)(l >> 24);
            this.Bytes[index + 5] = (byte)(l >> 16);
            this.Bytes[index + 6] = (byte)(l >> 8);
            this.Bytes[index + 7] = (byte)l;
        }
    }
}
