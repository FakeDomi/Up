using System;
using System.IO;
using System.Text;
using System.Xml;

namespace domi1819.UpCore.Utilities
{
    public class FancyXmlWriter : XmlWriter
    {
        private readonly XmlWriter xmlWriter;

        public override WriteState WriteState => this.xmlWriter.WriteState;

        public FancyXmlWriter(TextWriter writer)
        {
            this.xmlWriter = Create(writer, new XmlWriterSettings { Encoding = Encoding.UTF8, NewLineChars = Environment.NewLine, Indent = true });
        }

        public override void WriteStartDocument()
        {
            this.xmlWriter.WriteStartDocument();
        }

        public override void WriteStartDocument(bool standalone)
        {
            this.xmlWriter.WriteStartDocument(standalone);
        }

        public override void WriteEndDocument()
        {
            this.xmlWriter.WriteEndDocument();
        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            this.xmlWriter.WriteDocType(name, pubid, sysid, subset);
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            this.xmlWriter.WriteStartElement(prefix, localName, ns);
        }

        public override void WriteEndElement()
        {
            this.xmlWriter.WriteFullEndElement();
        }

        public override void WriteFullEndElement()
        {
            this.xmlWriter.WriteFullEndElement();
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            this.xmlWriter.WriteStartAttribute(prefix, localName, ns);
        }

        public override void WriteEndAttribute()
        {
            this.xmlWriter.WriteEndAttribute();
        }

        public override void WriteCData(string text)
        {
            this.xmlWriter.WriteCData(text);
        }

        public override void WriteComment(string text)
        {
            this.xmlWriter.WriteComment(text);
        }

        public override void WriteProcessingInstruction(string name, string text)
        {
            this.xmlWriter.WriteProcessingInstruction(name, text);
        }

        public override void WriteEntityRef(string name)
        {
            this.xmlWriter.WriteEntityRef(name);
        }

        public override void WriteCharEntity(char ch)
        {
            this.xmlWriter.WriteCharEntity(ch);
        }

        public override void WriteWhitespace(string ws)
        {
            this.xmlWriter.WriteWhitespace(ws);
        }

        public override void WriteString(string text)
        {
            this.xmlWriter.WriteString(text);
        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            this.xmlWriter.WriteSurrogateCharEntity(lowChar, highChar);
        }

        public override void WriteChars(char[] buffer, int index, int count)
        {
            this.xmlWriter.WriteChars(buffer, index, count);
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            this.xmlWriter.WriteRaw(buffer, index, count);
        }

        public override void WriteRaw(string data)
        {
            this.xmlWriter.WriteRaw(data);
        }

        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            this.xmlWriter.WriteBase64(buffer, index, count);
        }

        public override void Close()
        {
            this.xmlWriter.Close();
        }

        public override void Flush()
        {
            this.xmlWriter.Flush();
        }

        public override string LookupPrefix(string ns)
        {
            return this.xmlWriter.LookupPrefix(ns);
        }
    }
}
