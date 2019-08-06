using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using domi1819.UpCore.Utilities;
using domi1819.UpServer.Console;

namespace domi1819.UpServer
{
    internal class UpWebService
    {
        private static readonly Dictionary<string, string> MimeDict = new Dictionary<string, string>();

        private static byte[] iconData;

        private Thread dispatcherThread;

        private readonly ServerConfig config;
        private readonly FileManager files;

        internal UpWebService(UpServer upServer)
        {
            this.config = upServer.Config;
            this.files = upServer.Files;

            if (File.Exists(Path.Combine(this.config.DataFolder, "favicon.ico")))
            {
                iconData = File.ReadAllBytes(Path.Combine(this.config.DataFolder, "favicon.ico"));
            }

            MimeDict.Add(".jpg", "image/jpeg");
            MimeDict.Add(".jpeg", "image/jpeg");
            MimeDict.Add(".png", "image/png");
            MimeDict.Add(".txt", "text/plain; charset=UTF-8");
            MimeDict.Add(".xml", "text/plain; charset=UTF-8");
            MimeDict.Add(".java", "text/plain; charset=UTF-8");
            MimeDict.Add(".cs", "text/plain; charset=UTF-8");
            MimeDict.Add(".cfg", "text/plain; charset=UTF-8");
            MimeDict.Add(".conf", "text/plain; charset=UTF-8");
            MimeDict.Add(".log", "text/plain; charset=UTF-8");
            MimeDict.Add(".pdf", "application/pdf");
            MimeDict.Add(".mp3", "audio/mpeg");
            MimeDict.Add(".mp4", "video/mp4");
            MimeDict.Add(".webm", "video/webm");
        }

        internal void Start()
        {
            this.dispatcherThread = new Thread(this.Run) { Name = "Up HTTP Server Dispatcher" };
            this.dispatcherThread.Start();
        }

        internal void Stop()
        {
            this.dispatcherThread.Abort();
        }

        private void Run()
        {
            HttpListener listener = new HttpListener();
            string prefix = $"http://{this.config.HttpServerListenerName}:{this.config.HttpServerPort}/";

            listener.Prefixes.Add(prefix);

            try
            {
                listener.Start();

                UpConsole.WriteLineRestoreCommand($"Listening for HTTP connections ({this.config.HostName}:{this.config.HttpServerPort})");

                while (true)
                {
                    ThreadPool.QueueUserWorkItem(this.ProcessHttpRequest, listener.GetContext());
                }
            }
            catch (Exception ex)
            {
                UpConsole.WriteLineRestoreCommand($"HTTP listener has been stopped: {ex.Message}");
                UpConsole.WriteLineRestoreCommand("");
                UpConsole.WriteLineRestoreCommand("If you are on Windows, you need to either run UpServer as admin,");
                UpConsole.WriteLineRestoreCommand("or grant permission to the HTTP prefix UpServer is using with the command:");
                UpConsole.WriteLineRestoreCommand($"netsh http add urlacl url={prefix} user=YOUR_DOMAIN\\your_user");
                UpConsole.WriteLineRestoreCommand("");
            }
        }

        private void ProcessHttpRequest(object context)
        {
            try
            {
                HttpListenerContext ctx = (HttpListenerContext)context;
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse res = ctx.Response;
                string reqUrl = req.RawUrl;

                if (reqUrl.StartsWith("/favicon.ico"))
                {
                    if (iconData == null)
                    {
                        res.StatusCode = (int)HttpStatusCode.NotFound;
                        res.Close();

                        return;
                    }

                    res.ContentLength64 = iconData.Length;
                    res.OutputStream.Write(iconData, 0, iconData.Length);
                    res.OutputStream.Close();

                    res.Close();
                }
                else if (reqUrl.StartsWith("/d/"))
                {
                    this.ProcessFileDownload(reqUrl, res);
                }
                else if (reqUrl.StartsWith("/i/"))
                {
                    this.ProcessFileInfo(reqUrl, res);
                }
            }
            catch (Exception ex)
            {
                UpConsole.WriteLineRestoreCommand(ex.ToString());
            }
        }

        private void ProcessFileDownload(string reqUrl, HttpListenerResponse res)
        {
            // Link format: /d/12345678
            string fileId = reqUrl.Substring(3, Constants.Server.FileIdLength);

            if (this.files.FileExists(fileId) && this.files.GetDownloadableFlag(fileId))
            {
                string fileName = this.files.GetFileName(fileId);

                using (FileStream fileStream = File.OpenRead(Path.Combine(this.config.FileStorageFolder, fileId)))
                {
                    res.ContentLength64 = fileStream.Length;

                    string fileExt = Path.GetExtension(fileName)?.ToLowerInvariant() ?? string.Empty;

                    if (MimeDict.ContainsKey(fileExt) && !reqUrl.EndsWith("!"))
                    {
                        res.AddHeader("Content-Disposition", "inline; filename=\"" + fileName + "\"");
                        res.ContentType = MimeDict[fileExt];
                    }
                    else
                    {
                        res.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
                        res.ContentType = "application/octet-stream";
                    }

                    Stream outStream = res.OutputStream;
                    byte[] buffer = new byte[4 * 1024];
                    int bytesRead;

                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        outStream.Write(buffer, 0, bytesRead);

                        //if (!this.files.GetDownloadableFlag(fileId))
                        //{
                        //    throw new Exception("abort download");
                        //}
                    }

                    res.OutputStream.Close();
                    res.Close();

                    this.files.IncrementDownloadCount(fileId);
                }
            }
            else
            {
                byte[] bytes = Encoding.UTF8.GetBytes("<html><head><title>^up - File not found</title></head><body bgcolor=\"#2D2D30\"><font color=\"#F1F1F1\">The requested file " + fileId + " doesn't exist.<br><address>UpServer " + Constants.Version + " at " + "localhost" + "</address></body></html>");

                res.ContentLength64 = bytes.Length;
                res.OutputStream.Write(bytes, 0, bytes.Length);
                res.OutputStream.Close();

                res.Close();
            }
        }

        private void ProcessFileInfo(string reqUrl, HttpListenerResponse res)
        {
            // Link format: /i/12345678
            string fileId = reqUrl.Substring(3, Constants.Server.FileIdLength);

            if (this.files.FileExists(fileId))
            {
                string fileName = this.files.GetFileName(fileId);
                int downloads = this.files.GetFileDownloads(fileId);
                long size = this.files.GetFileSize(fileId);
                string sizeText = Util.GetByteSizeText(size) + (size >= 1024 ? " (" + size + (size == 1 ? " byte)" : " bytes)") : string.Empty);
                string trafficCaused = Util.GetByteSizeText(size * (downloads + 1));
                string timeStamp = this.files.GetUploadDate(fileId).ToString("yyyy-MM-dd HH:mm:ss");

                byte[] bytes = Encoding.UTF8.GetBytes($"<html><head><title>^up - File Details</title><style type=\"text/css\">td{{padding:2px 8px 2px 8px;}}</style></head><body bgcolor=\"#2D2D30\"><font color=\"#F1F1F1\"><table style=\"color: #F1F1F1; border: 1px solid #434344; background: #252526\"><tr><td>File name</td><td>{fileName}</td></tr><tr><td>Uploaded on</td><td>{timeStamp}</td></tr><tr><td>File size</td><td>{sizeText}</td></tr><tr><td>Downloads</td><td>{downloads}</td></tr><tr><td>Traffic caused</td><td>{trafficCaused}</td></tr></table><br><address>UpServer {Constants.Version} at {this.config.HostName}</address></body></html>");

                res.ContentLength64 = bytes.Length;
                res.OutputStream.Write(bytes, 0, bytes.Length);
                res.OutputStream.Close();

                res.Close();
            }
            else
            {
                this.SendFileNotFound(res, fileId);
            }
        }

        private void SendFileNotFound(HttpListenerResponse res, string fileId)
        {
            byte[] bytes = Encoding.UTF8.GetBytes($"<html><head><title>^up - File not found</title></head><body bgcolor=\"#2D2D30\"><font color=\"#F1F1F1\">The requested file {fileId} doesn't exist.<br><address>UpServer { Constants.Version } at {this.config.HostName}</address></body></html>");

            res.ContentLength64 = bytes.Length;
            res.OutputStream.Write(bytes, 0, bytes.Length);
            res.OutputStream.Close();

            res.Close();
        }
    }
}
