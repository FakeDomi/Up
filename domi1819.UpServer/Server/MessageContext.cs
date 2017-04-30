using System;
using domi1819.UpCore.Network;

namespace domi1819.UpServer.Server
{
    internal class MessageContext
    {
        internal bool Disconnect { get; set; }

        internal bool ShouldPush { get; set; }

        public MessageDeserializer MessageReader { get; }

        public MessageSerializer MessageWriter { get; }

        internal MessageContext(MessageDeserializer reader, MessageSerializer writer)
        {
            this.MessageReader = reader;
            this.MessageWriter = writer;
        }

        internal int FetchMessage()
        {
            this.ShouldPush = true;
            return this.MessageReader.ReadMessage();
        }

        internal void PushMessage()
        {
            this.MessageWriter.Flush();
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
    }
}
