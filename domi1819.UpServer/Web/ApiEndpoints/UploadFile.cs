using System;
using System.IO;
using domi1819.UpCore.Utilities;

namespace domi1819.UpServer.Web.ApiEndpoints
{
    internal class UploadFile : ApiEndpoint
    {
        internal override string Url => "/api/upload-file";

        internal override void Process(Request request)
        {
            string fileName = request.HttpRequest.Headers[Headers.FileName];
            long fileSize = request.HttpRequest.ContentLength64;

            if (fileName == null || !request.Files.IsValidFileName(fileName))
            {
                request.SetError("invalid filename");
                return;
            }

            if (fileSize > request.Users.GetFreeCapacity(request.User))
            {
                request.SetError("not enough storage capacity");
                return;
            }

            request.Users.AddTransferStorage(request.User, fileSize);
            string tempFile;

            do
            {
                tempFile = Path.Combine(UpServer.Instance.Config.FileTransferFolder, $"{Util.GetRandomString(8)}.tmp");
            } while (File.Exists(tempFile));

            FileStream fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write);

            byte[] block = new byte[4096];
            long remainingBytes = fileSize;
            Stream input = request.HttpRequest.InputStream;

            try
            {
                while (remainingBytes > 0)
                {
                    int read = input.Read(block, 0, (int)Math.Min(block.Length, remainingBytes));

                    if (read == 0)
                    {
                        // client sent too few bytes, i don't even know what to do at this point
                        throw new Exception("concern");
                    }

                    fs.Write(block, 0, read);
                    remainingBytes -= read;
                }
            }
            catch
            {
                request.Users.RemoveTransferStorage(request.User, fileSize);

                fs.Dispose();

                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }

                throw;
            }

            fs.Dispose();
            string fileId = request.Files.GetNewFileId();
            request.Files.AddFile(fileId, fileName, request.User, fileSize);

            File.Move(tempFile, Path.Combine(UpServer.Instance.Config.FileStorageFolder, fileId));

            request.Users.RemoveTransferStorage(request.User, fileSize);
            request.Files.SetDownloadable(fileId, true);

            request.HttpResponse.AddHeader(Headers.FileLink, string.Format(request.Files.GetLinkFormat(), fileId));
        }
    }
}
