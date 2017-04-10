using System;
using System.IO;

namespace domi1819.Proton
{
    public class MessageContext
    {
        public bool Disconnect { get; set; }

        public bool ShouldPushResponse { get; set; }

        public MessageReader MessageReader { get; }

        public MessageWriter MessageWriter { get; }

        public MessageContext(Stream readerStream, byte[] readerBytes, Stream writerStream, byte[] writerBytes)
        {
            this.MessageReader = new MessageReader { Stream = readerStream, Bytes = readerBytes };
            this.MessageWriter = new MessageWriter { Stream = writerStream, Bytes = writerBytes };
        }

        public bool ReadNextBool()
        {
            return this.MessageReader.ReadNextBool();
        }

        public void WriteNextBool(bool b)
        {
            this.MessageWriter.WriteNextBool(b);
        }

        public int ReadNextInt()
        {
            return this.MessageReader.ReadNextInt();
        }

        public void WriteNextInt(int i)
        {
            this.MessageWriter.WriteNextInt(i);
        }

        public long ReadNextLong()
        {
            return this.MessageReader.ReadNextLong();
        }

        public void WriteNextLong(long l)
        {
            this.MessageWriter.WriteNextLong(l);
        }

        public string ReadNextString()
        {
            return this.MessageReader.ReadNextString();
        }

        public void WriteNextString(string str)
        {
            this.MessageWriter.WriteNextString(str);
        }

        public DateTime ReadNextDateTime()
        {
            return this.MessageReader.ReadNextDateTime();
        }

        public void WriteNextDateTime(DateTime dateTime)
        {
            this.MessageWriter.WriteNextDateTime(dateTime);
        }

        internal int FetchRequestMessage()
        {
            this.ShouldPushResponse = true;

            return 0;
        }

        internal void PushResponseMessage()
        {
            if (this.ShouldPushResponse)
            {
                
            }
        }
    }
}
