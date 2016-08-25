using System;
using System.IO;
using System.Text;
using domi1819.UpCore.Utilities;

namespace domi1819.UpCore.Network
{
    public class MessageDeserializer
    {
        public int Index { get; set; }
        public byte[] Bytes { get; set; }
        public Stream Stream { get; set; }

        public int ReadMessage(int expectedMessage = -1)
        {
            int blockSize = Constants.Encryption.AesBlockSize;
            int headerSize = Constants.Network.MessageHeaderSize;
            
            if (!this.ReadBytes(headerSize, 0))
            {
                return NetworkMethods.ConnectionClosed;
            }

            int messageLength = this.ReadInt(0);
            int messageId = this.ReadInt(4);

            if (messageLength < headerSize || messageLength >= this.Bytes.Length)
            {
                throw new Exception($"Invalid message size {messageLength}.");
            }

            if (expectedMessage >= 0 && messageId != expectedMessage)
            {
                throw new Exception("Received unexpected message.");
            }

            if (!this.ReadBytes(messageLength - headerSize + (messageLength % blockSize == 0 ? 0 : blockSize - messageLength % blockSize), headerSize))
            {
                return NetworkMethods.ConnectionClosed;
            }

            this.Index = headerSize;

            return messageId;
        }

        public bool ReadNextBool()
        {
            this.Index++;
            return this.Bytes[this.Index - 1] != 0;
        }

        public int ReadNextInt()
        {
            int value = this.ReadInt(this.Index);
            this.Index += 4;

            return value;
        }

        public long ReadNextLong()
        {
            long value = this.ReadLong(this.Index);
            this.Index += 8;

            return value;
        }

        public string ReadNextString()
        {
            int length = this.ReadNextInt();

            string result = Encoding.UTF8.GetString(this.Bytes, this.Index, length);

            this.Index += length;

            return result;
        }

        public int ReadNextByteArray(byte[] arr, int start)
        {
            int count = this.ReadNextInt();

            for (int i = 0; i < count; i++)
            {
                arr[start + i] = this.Bytes[this.Index];
                this.Index++;
            }

            return count;
        }

        public DateTime ReadNextDateTime()
        {
            int year = this.ReadNextInt();
            int month = this.Bytes[this.Index];
            int day = this.Bytes[this.Index + 1];
            int hour = this.Bytes[this.Index + 2];
            int minute = this.Bytes[this.Index + 3];
            int second = this.Bytes[this.Index + 4];

            this.Index += 5;

            return new DateTime(year, month, day, hour, minute, second);
        }

        private bool ReadBytes(int count, int position)
        {
            int totalRead = 0;
            int pos = position;

            while (count - totalRead > 0)
            {
                int bytesRead = this.Stream.Read(this.Bytes, pos, count - totalRead);

                if (bytesRead == 0)
                {
                    return false;
                }

                pos += bytesRead;
                totalRead += bytesRead;
            }

            return true;
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
