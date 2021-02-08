using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using domi1819.UpCore.Utilities;
using domi1819.UpServer.Console;

namespace domi1819.UpServer.Web
{
    internal class UpWebService
    {
        // ReSharper disable once InconsistentNaming
        private const int ERROR_OPERATION_ABORTED = 995;
        private const int MonoErrorListenerClosed = 500;

        internal const string DateFormat = "yyyy-MM-dd HH:mm:ss";

        private static readonly Dictionary<string, string> MimeDict = new Dictionary<string, string>();

        private Thread dispatcherThread;
        private HttpListener listener;

        private readonly ServerConfig config;
        private readonly FileManager files;
        private readonly UserManager users;

        private readonly CachedFiles cachedFiles;
        private readonly Sessions sessions;

        private readonly Regex fileLinkRegex = new Regex(@"^\/[a-z0-9]{8}[!+]?(\/.+)?$", RegexOptions.Compiled);

        internal UpWebService(UpServer upServer)
        {
            this.config = upServer.Config;
            this.files = upServer.Files;
            this.users = upServer.Users;

            this.cachedFiles = new CachedFiles(this.config.WebFolder, !this.config.WebInterfaceEnabled);
            this.sessions = new Sessions();

            MimeDict.Add(".flac", "audio/flac");
            MimeDict.Add(".mp3", "audio/mpeg");
            MimeDict.Add(".ogg", "audio/ogg");
            MimeDict.Add(".oga", "audio/ogg");
            MimeDict.Add(".opus", "audio/ogg");
            MimeDict.Add(".wav", "audio/wav");

            MimeDict.Add(".pdf", "application/pdf");

            MimeDict.Add(".apng", "image/apng");
            MimeDict.Add(".bmp", "image/bmp");
            MimeDict.Add(".gif", "image/gif");
            MimeDict.Add(".ico", "image/x-icon");
            MimeDict.Add(".cur", "image/x-icon");
            MimeDict.Add(".jpg", "image/jpeg");
            MimeDict.Add(".jpeg", "image/jpeg");
            MimeDict.Add(".jfif", "image/jpeg");
            MimeDict.Add(".pjpeg", "image/jpeg");
            MimeDict.Add(".pjp", "image/jpeg");
            MimeDict.Add(".png", "image/png");
            MimeDict.Add(".svg", "image/svg+xml");
            MimeDict.Add(".webp", "image/webp");

            MimeDict.Add(".mp4", "video/mp4");
            MimeDict.Add(".m4a", "video/mp4");
            MimeDict.Add(".ogv", "video/ogg");
            MimeDict.Add(".webm", "video/webm");
        }

        internal void Start()
        {
            string prefix = $"http://{this.config.HttpServerListenerName}:{this.config.HttpServerPort}/";

            try
            {
                this.listener = new HttpListener();
                this.listener.Prefixes.Add(prefix);
                this.listener.Start();

                UpConsole.WriteLineRestoreCommand($"Listening for HTTP connections ({this.config.HttpServerListenerName}:{this.config.HttpServerPort})");

                this.dispatcherThread = new Thread(this.Run) { Name = "Up HTTP Server Dispatcher" };
                this.dispatcherThread.Start();
            }
            catch (HttpListenerException ex)
            {
                UpConsole.WriteLineRestoreCommand($"HTTP listener has been stopped: {ex.Message}");
                UpConsole.WriteLineRestoreCommand($"ErrorCode = {ex.ErrorCode}");
                UpConsole.WriteLineRestoreCommand("");
                UpConsole.WriteLineRestoreCommand("If you are on Windows, you need to either run UpServer as admin,");
                UpConsole.WriteLineRestoreCommand("or grant permission to the HTTP prefix UpServer is using with the command:");
                UpConsole.WriteLineRestoreCommand($"netsh http add urlacl url={prefix} user=YOUR_DOMAIN\\your_user");
                UpConsole.WriteLineRestoreCommand("");
            }
        }

        internal void Stop()
        {
            this.listener.Abort();
        }

        internal void ReloadCachedFiles()
        {
            this.cachedFiles.Reload();
        }

        private void Run()
        {
            try
            {
                while (true)
                {
                    ThreadPool.QueueUserWorkItem(this.ProcessHttpRequest, this.listener.GetContext());
                }
            }
            catch (HttpListenerException ex) when (ex.ErrorCode == ERROR_OPERATION_ABORTED || ex.ErrorCode == MonoErrorListenerClosed)
            {
                // HTTP listener has been stopped.
            }
        }

        private void ProcessHttpRequest(object context)
        {
            try
            {
                HttpListenerContext ctx = (HttpListenerContext)context;
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse res = ctx.Response;

                string reqUrl = req.Url.AbsolutePath;

                if (reqUrl.StartsWith("/api/"))
                {
                    using (StreamReader reader = new StreamReader(req.InputStream))
                    using (StreamWriter writer = new StreamWriter(res.OutputStream))
                    {
                        string session = reader.ReadLine();
                        string user = this.sessions.GetUserFromSession(session, Http.GetRealIp(req));

                        ApiEndpoint endpoint = ApiEndpoint.Endpoints[reqUrl];

                        if (endpoint != null)
                        {
                            if (endpoint.NeedsAuthentication && user == null)
                            {
                                res.StatusCode = (int)HttpStatusCode.Forbidden;
                            }
                            else
                            {
                                endpoint.Process(new Request(session, user, this.sessions, this.users, this.files, reader, writer, req, res));
                            }
                        }
                        else
                        {
                            res.StatusCode = (int)HttpStatusCode.NotFound;
                        }
                    }
                }
                else
                {
                    if (reqUrl == "/")
                    {
                        reqUrl = "/login";
                    }

                    string session = req.Cookies["session"]?.Value;
                    string user = this.sessions.GetUserFromSession(session, Http.GetRealIp(req));

                    if (reqUrl == "/login" && user != null)
                    {
                        res.Redirect("/home");
                    }
                    else if (reqUrl == "/home" && user == null)
                    {
                        res.Redirect("/login");
                    }
                    else if (reqUrl == "/files" && user == null)
                    {
                        res.Redirect("/login");
                    }
                    else if (reqUrl == "/sessions" && user == null)
                    {
                        res.Redirect("/login");
                    }
                    else
                    {
                        byte[] data = this.cachedFiles[reqUrl];

                        if (data != null)
                        {
                            res.ContentLength64 = data.Length;
                            res.OutputStream.Write(data, 0, data.Length);
                            res.OutputStream.Close();
                        }
                        else
                        {
                            if (this.fileLinkRegex.IsMatch(reqUrl))
                            {
                                this.ProcessFileDownload(req, reqUrl, res);
                            }
                            else
                            {
                                res.StatusCode = (int)HttpStatusCode.NotFound;
                            }
                        }
                    }
                }

                res.Close();
            }
            catch (Exception ex)
            {
                // ERROR_DEV_NOT_EXIST
                if ((ex as HttpListenerException)?.ErrorCode == 55)
                {
                    return;
                }

                if (!(ex is IOException))
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

                    UpConsole.WriteLineRestoreCommand(ex.ToString());
                }
            }
        }

        private void ProcessFileDownload(HttpListenerRequest req, string reqUrl, HttpListenerResponse res)
        {
            // Link format: /12345678[!|+][/filename.ext]

            int fileIdLength = Constants.Server.FileIdLength;
            string fileId = reqUrl.Substring(1, fileIdLength);

            if (this.files.FileExists(fileId) && this.files.GetDownloadableFlag(fileId))
            {
                string fileName = this.files.GetFileName(fileId);
                bool forcedDownload = reqUrl.Length > fileIdLength && (reqUrl[fileIdLength] == '!' || reqUrl[fileIdLength] == '+');

                if (reqUrl.Length <= fileIdLength + 1)
                {
                    res.Redirect(string.Format(this.files.GetLinkFormat(), $"{fileId}{(forcedDownload ? "!" : "")}/{fileName}"));
                    return;
                }

                string storagePath = Path.Combine(this.config.FileStorageFolder, fileId);

                using (FileStream fileStream = File.OpenRead(storagePath))
                {
                    if (Http.GetRange(req.Headers.Get("Range"), out long start, out long end, fileStream.Length))
                    {
                        res.ContentLength64 = end - start + 1;
                        res.AddHeader("Content-Range", $"bytes {start}-{end}/{fileStream.Length}");
                        res.StatusCode = (int)HttpStatusCode.PartialContent;

                        fileStream.Seek(start, SeekOrigin.Begin);
                    }
                    else
                    {
                        res.ContentLength64 = fileStream.Length;
                        res.AddHeader("Accept-Ranges", "bytes");
                    }

                    string fileExt = Path.GetExtension(fileName)?.ToLowerInvariant() ?? string.Empty;

                    if (!forcedDownload && (MimeDict.TryGetValue(fileExt, out string mime) || Mime.GuessTextType(storagePath, fileId, out mime)))
                    {
                        res.AddHeader("Content-Disposition", $"inline; filename=\"{fileName}\"");
                        res.ContentType = mime;
                    }
                    else
                    {
                        res.AddHeader("Content-Disposition", $"attachment; filename=\"{fileName}\"");
                        res.ContentType = "application/octet-stream";
                    }

                    Stream outStream = res.OutputStream;
                    byte[] buffer = new byte[4 * 1024];
                    long bytesToSend = res.ContentLength64;
                    int bytesRead;

                    while (bytesToSend > 0 && (bytesRead = fileStream.Read(buffer, 0, (int)Math.Min(buffer.Length, bytesToSend))) > 0)
                    {
                        outStream.Write(buffer, 0, bytesRead);
                        bytesToSend -= bytesRead;

                        //if (!this.files.GetDownloadableFlag(fileId))
                        //{
                        //    throw new Exception("abort download");
                        //}
                    }

                    res.OutputStream.Close();

                    this.files.IncrementDownloadCount(fileId);

                    return;
                }
            }

            this.SendFileNotFound(res, fileId);
        }

        private void SendFileNotFound(HttpListenerResponse res, string fileId)
        {
            byte[] bytes = Encoding.UTF8.GetBytes($"<html><head><title>^up - File not found</title></head><body bgcolor=\"#2D2D30\"><font color=\"#F1F1F1\">The requested file {fileId} doesn't exist.<br><address>UpServer { Constants.Version } at {this.config.HostName}</address></body></html>");

            res.ContentLength64 = bytes.Length;
            res.OutputStream.Write(bytes, 0, bytes.Length);
            res.OutputStream.Close();

        }

        private class CachedFiles
        {
            private readonly string[] fileNamesMinimal = { "favicon.ico" };
            private readonly string[] fileNames = { "favicon.ico", "login.html", "home.html", "files.html", "sessions.html", "logout.html", "style.css", "custom-font", "logo.png" };

            private readonly string path;
            private readonly bool minimal;
            
            private readonly Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();

            internal byte[] this[string url]
            {
                get
                {
                    lock (this.files)
                    {
                        return this.files.ContainsKey(url) ? this.files[url] : null;
                    }
                }
            } 

            internal CachedFiles(string path, bool minimal)
            {
                this.path = path;
                this.minimal = minimal;

                this.Reload();
            }

            public void Reload()
            {
                lock (this.files)
                {
                    this.files.Clear();
                    
                    foreach (string fileName in this.minimal ? this.fileNamesMinimal : this.fileNames)
                    {
                        string filePath = Path.Combine(this.path, fileName);

                        if (File.Exists(filePath))
                        {
                            this.files.Add("/" + fileName.Replace(".html", ""), File.ReadAllBytes(filePath));
                        }
                    }
                }
            }
        }
    }
}
