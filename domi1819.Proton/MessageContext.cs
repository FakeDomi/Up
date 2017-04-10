using System;

namespace domi1819.Proton
{
    public class MessageContext
    {
        public bool Cancel { get; set; }

        public MessageReader MessageReader { get; private set; }

        public MessageWriter MessageWriter { get; private set; }

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
