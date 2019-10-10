using System;
using System.Collections.Generic;
using System.Text;

namespace domi1819.UpCore.Mime
{
    public static class MimeSniffer
    {
        private static readonly byte[] mask444 = new byte[] { 0xff, 0xff, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff };
        private static readonly byte[] mask446 = new byte[] { 0xff, 0xff, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };

        private static List<Pattern> patterns = new List<Pattern>
        {
            new MaskedPattern(new byte[] { 0x46, 0x4f, 0x52, 0x4d, 0x00, 0x00, 0x00, 0x00, 0x41, 0x49, 0x46, 0x46 }, mask444, MimeType.AudioAiff),
            new SimplePattern(new byte[] { 0x2e, 0x73, 0x6e, 0x64 }, MimeType.AudioBasic),
            new SimplePattern(new byte[] { 0x66, 0x4c, 0x61, 0x43 }, MimeType.AudioFlac),
            new SimplePattern(new byte[] { 0x4d, 0x54, 0x68, 0x64, 0x00, 0x00, 0x00, 0x06 }, MimeType.AudioMidi),
            new SimplePattern(new byte[] { 0x49, 0x44, 0x33 }, MimeType.AudioMpeg),
            new SimplePattern(new byte[] { 0xff, 0xfb }, MimeType.AudioMpeg),
            new MaskedPattern(new byte[] { 0x52, 0x49, 0x46, 0x46, 0x00, 0x00, 0x00, 0x00, 0x57, 0x41, 0x56, 0x45 }, mask444, MimeType.AudioWave),

            new SimplePattern(new byte[] { 0x74, 0x74, 0x63, 0x66 }, MimeType.FontCollection),
            new SimplePattern(new byte[] { 0x4f, 0x54, 0x54, 0x4f }, MimeType.FontOtf),
            new SimplePattern(new byte[] { 0x00, 0x01, 0x00, 0x00 }, MimeType.FontTtf),
            new SimplePattern(new byte[] { 0x77, 0x4f, 0x46, 0x46 }, MimeType.FontWoff),
            new SimplePattern(new byte[] { 0x77, 0x4f, 0x46, 0x32 }, MimeType.FontWoff2),

            new SimplePattern(new byte[] { 0x00, 0x00, 0x01, 0x00 }, MimeType.ImageXIcon),
            new SimplePattern(new byte[] { 0x00, 0x00, 0x02, 0x00 }, MimeType.ImageXIcon),
            new BmpPattern(),
            new SimplePattern(new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 }, MimeType.ImageGif),
            new SimplePattern(new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }, MimeType.ImageGif),
            new MaskedPattern(new byte[] { 0x52, 0x49, 0x46, 0x46, 0x00, 0x00, 0x00, 0x00, 0x57, 0x45, 0x42, 0x50, 0x56, 0x50 }, mask446, MimeType.ImageWebP),
            
            new MaskedPattern(new byte[] { 0x52, 0x49, 0x46, 0x46, 0x00, 0x00, 0x00, 0x00, 0x41, 0x56, 0x49, 0x20 }, mask444, MimeType.VideoAvi),
            new Mp4Pattern(),
            new SimplePattern(new byte[] { 0x1a, 0x45, 0xdf, 0xa3}, MimeType.VideoWebm), // also includes matroska containers
            
            new SimplePattern(new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2d }, MimeType.ApplicationPdf),
            new SimplePattern(new byte[] { 0x4f, 0x67, 0x67, 0x53, 0x00 }, MimeType.ApplicationOgg),
            new SimplePattern(new byte[] { 0x25, 0x21, 0x50, 0x53, 0x2d, 0x41, 0x64, 0x6f, 0x62, 0x65, 0x2d }, MimeType.ApplicationPostScript),
            new SimplePattern(new byte[] { 0x00, 0x61, 0x73, 0x6d }, MimeType.ApplicationWasm),
            new SimplePattern(new byte[] { 0x1f, 0x8b, 0x08 }, MimeType.ApplicationXGzip),
            new SimplePattern(new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1a, 0x07, 0x01, 0x00 }, MimeType.ApplicationXRarCompressed),
            new SimplePattern(new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1a, 0x07, 0x00 }, MimeType.ApplicationXRarCompressed),
            new SimplePattern(new byte[] { 0x50, 0x4b, 0x03, 0x04 }, MimeType.ApplicationZip)
        };

        private static Decoder Utf8 = Encoding.GetEncoding(Encoding.UTF8.CodePage, new EncoderExceptionFallback(), new DecoderExceptionFallback()).GetDecoder();

        public static MimeType GetMimeType(byte[] bytes, int length, long fileSize)
        {
            foreach(Pattern pattern in patterns)
            {
                if (pattern.Matches(bytes, length, fileSize))
                {
                    return pattern.MimeType;
                }
            }

            try
            {
                Utf8.GetCharCount(bytes, 0, length);
                return MimeType.TextPlainUtf8;
            }
            catch (DecoderFallbackException)
            {
                if (bytes.Length >= 2 && length >= 2)
                {
                    if (bytes[0] == 0xfe && bytes[1] == 0xff)
                    {
                        return MimeType.TextPlainUtf16Be;
                    }
                    else if (bytes[0] == 0xff && bytes[1] == 0xfe)
                    {
                        return MimeType.TextPlainUtf16Le;
                    }
                }

                for (int i = 0; i < Math.Min(bytes.Length, length); i++)
                {
                    byte b = bytes[i];
                    switch (b)
                    {
                        case 0x00:
                        case 0x01:
                        case 0x02:
                        case 0x03:
                        case 0x04:
                        case 0x05:
                        case 0x06:
                        case 0x07:
                        case 0x08:
                        case 0x0E:
                        case 0x0F:
                        case 0x10:
                        case 0x11:
                        case 0x12:
                        case 0x13:
                        case 0x14:
                        case 0x15:
                        case 0x16:
                        case 0x17:
                        case 0x18:
                        case 0x19:
                        case 0x1A:
                        case 0x1B:
                        case 0x1C:
                        case 0x1D:
                        case 0x1E:
                        case 0x1F:
                        case 0x7F:
                            return MimeType.ApplicationOctetStream;
                    }
                }

                return MimeType.TextPlain;
            }
        }

        private interface Pattern
        {
            bool Matches(byte[] bytes, int length, long fileSize);

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

            public virtual bool Matches(byte[] bytes, int length, long fileSize)
            {
                if (bytes.Length >= this.magic.Length && length > this.magic.Length)
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

        private class MaskedPattern : Pattern
        {
            private byte[] magic;
            private byte[] mask;

            public MimeType MimeType { get; }

            public MaskedPattern(byte[] magic, byte[] mask, MimeType mineType)
            {
                this.magic = magic;
                this.mask = mask;
                this.MimeType = mineType;
            }

            public bool Matches(byte[] bytes, int length, long fileSize)
            {
                if (bytes.Length >= this.magic.Length && length >= this.magic.Length)
                {
                    for (int i = 0; i < this.magic.Length; i++)
                    {
                        if (this.magic[i] != (bytes[i] & this.mask[i]))
                        {
                            return false;
                        }
                    }

                    return true;
                }

                return false;
            }
        }

        private class BmpPattern : SimplePattern
        {
            public BmpPattern() : base(new byte[] { 0x42, 0x4d }, MimeType.ImageBmp)
            {
            }

            public override bool Matches(byte[] bytes, int length, long fileSize)
            {
                return base.Matches(bytes, length, fileSize) && bytes.Length >= 6 && length >= 6 && (bytes[2] | bytes[3] << 8 | bytes[4] << 16 | bytes[5] << 24) == fileSize;
            }
        }

        private class Mp4Pattern : Pattern
        {
            public MimeType MimeType => MimeType.VideoMp4;

            public bool Matches(byte[] bytes, int length, long fileSize)
            {
                // https://mimesniff.spec.whatwg.org/#signature-for-mp4

                if (bytes.Length >= 12 && length >= 12)
                {
                    int boxSize = bytes[0] << 24 | bytes[1] << 16 | bytes[2] << 8 | bytes[3];

                    if (bytes.Length >= boxSize && length >= boxSize && boxSize % 4 == 0 && 
                        bytes[4] == 0x66 && bytes[5] == 0x74 && bytes[6] == 0x79 && bytes[7] == 0x70)
                    {
                        for(int offset = 8; offset < boxSize; offset += 4)
                        {
                            if (offset != 12 &&
                                bytes[offset] == 0x6d && bytes[offset + 1] == 0x70 && bytes[offset + 2] == 0x34)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
        }
    }
}
