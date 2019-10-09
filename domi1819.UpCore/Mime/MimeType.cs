using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace domi1819.UpCore.Mime
{
    public class MimeType
    {
        private static List<MimeType> mimeTypes = new List<MimeType>(48);

        public static readonly MimeType AudioAiff;
        public static readonly MimeType AudioBasic;
        public static readonly MimeType AudioFlac;
        public static readonly MimeType AudioMidi;
        public static readonly MimeType AudioMpeg;
        public static readonly MimeType AudioWave;

        public static readonly MimeType ApplicationOgg;
        public static readonly MimeType ApplicationOctetStream;
        public static readonly MimeType ApplicationPdf;
        public static readonly MimeType ApplicationPostScript;
        public static readonly MimeType ApplicationWasm;
        public static readonly MimeType ApplicationXGzip;
        public static readonly MimeType ApplicationXRarCompressed;
        public static readonly MimeType ApplicationZip;

        public static readonly MimeType FontCollection;
        public static readonly MimeType FontOtf;
        public static readonly MimeType FontTtf;
        public static readonly MimeType FontWoff;
        public static readonly MimeType FontWoff2;

        public static readonly MimeType ImageBmp;
        public static readonly MimeType ImageGif;
        public static readonly MimeType ImageJpeg;
        public static readonly MimeType ImagePng;
        public static readonly MimeType ImageXIcon;
        public static readonly MimeType ImageWebP;

        public static readonly MimeType TextPlain;
        public static readonly MimeType TextPlainUtf8;
        public static readonly MimeType TextPlainUtf16Be;
        public static readonly MimeType TextPlainUtf16Le;

        public static readonly MimeType VideoAvi;
        public static readonly MimeType VideoMp4;
        public static readonly MimeType VideoWebm;

        public string Type { get; }

        public int Id { get; }

        public MimeType(string type)
        {
            this.Type = type;
            this.Id = mimeTypes.Count;
            mimeTypes.Add(this);
        }

        public override string ToString()
        {
            return this.Type;
        }

        static MimeType()
        {
            AudioAiff = new MimeType("audio/aiff");
            AudioBasic = new MimeType("audio/basic");
            AudioFlac = new MimeType("audio/flac");
            AudioMidi = new MimeType("audio/midi");
            AudioMpeg = new MimeType("audio/mpeg");
            AudioWave = new MimeType("audio/wave");

            ApplicationOgg = new MimeType("application/ogg");
            ApplicationOctetStream = new MimeType("application/octet-stream");
            ApplicationPdf = new MimeType("application/pdf");
            ApplicationPostScript = new MimeType("application/postscript");
            ApplicationWasm = new MimeType("application/wasm");
            ApplicationXGzip = new MimeType("application/x-gzip");
            ApplicationXRarCompressed = new MimeType("application/x-rar-compressed");
            ApplicationZip = new MimeType("application/zip");

            FontCollection = new MimeType("font/collection");
            FontOtf = new MimeType("font/otf");
            FontTtf = new MimeType("font/ttf");
            FontWoff = new MimeType("font/woff");
            FontWoff2 = new MimeType("font/woff2");

            ImageBmp = new MimeType("image/bmp");
            ImageGif = new MimeType("image/gif");
            ImageJpeg = new MimeType("image/jpeg");
            ImagePng = new MimeType("image/png");
            ImageXIcon = new MimeType("image/x-icon");
            ImageWebP = new MimeType("image/webp");

            TextPlain = new MimeType("text/plain");
            TextPlainUtf8 = new MimeType("text/plain; charset=utf-8");
            TextPlainUtf16Be = new MimeType("text/plain; charset=utf-16be");
            TextPlainUtf16Le = new MimeType("text/plain; charset=utf-16le");

            VideoAvi = new MimeType("video/avi");
            VideoMp4 = new MimeType("video/mp4");
            VideoWebm = new MimeType("video/webm");
        }
    }
}
