using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace domi1819.UpCore.Mime
{
    class MimeSniffer
    {
        public static MimeType GetMimeType(byte[] bytes)
        {


            return MimeType.ApplicationOctetStream;
        }

        private interface Pattern
        {
            bool matches(byte[] bytes);

            MimeType MimeType { get; }
        }

        private class SimplePattern : Pattern
        {
            private byte[] magic;

            public MimeType MimeType { get; }

            public SimplePattern(byte[] magic, MimeType mineType)
            {
                this.magic = magic;
                this.MimeType = mineType;
            }

            public bool matches(byte[] bytes)
            {
                if (bytes.Length >= this.magic.Length)
                {
                    for (int i = 0; i < this.magic.Length; i++)
                    {
                        if (this.magic[i] != bytes[i])
                        {
                            return false;
                        }
                    }

                    return true;
                }

                return false;
            }
        }
    }
}
