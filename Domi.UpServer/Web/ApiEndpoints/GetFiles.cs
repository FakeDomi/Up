using System;
using System.Collections.Generic;
using Domi.NanoDB;
using Domi.UpCore.Utilities;

namespace Domi.UpServer.Web.ApiEndpoints
{
    internal class GetFiles : ApiEndpoint
    {
        internal override string Url => "/api/get-files";

        internal override void Process(Request request)
        {
            List<NanoDBLine> userFiles = request.Files.GetFiles(request.User);
            const string separator = "\n";

            request.Writer.Write(request.Files.GetLinkFormat());
            request.Writer.Write(separator);

            foreach (NanoDBLine line in userFiles)
            {
                request.Writer.Write(line[FileManager.Index.FileId]);
                request.Writer.Write(separator);
                request.Writer.Write(Util.GetByteSizeText((long)line[FileManager.Index.FileSize]));
                request.Writer.Write(separator);
                request.Writer.Write(line[FileManager.Index.Downloads]);
                request.Writer.Write("x");
                request.Writer.Write(separator);
                request.Writer.Write(((DateTime)line[FileManager.Index.UploadDate]).ToString(UpWebService.DateFormat));
                request.Writer.Write(separator);
                request.Writer.Write(line[FileManager.Index.FileName]);
                request.Writer.Write(separator);
            }
        }
    }
}
