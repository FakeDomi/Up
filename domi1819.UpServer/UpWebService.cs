using System;
using System.Collections.Generic;
using System.Globalization;
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
        private static readonly Dictionary<string, string> OtherMimeCache = new Dictionary<string, string>();

        private Thread dispatcherThread;

        private readonly ServerConfig config;
        private readonly FileManager files;
        private readonly UserManager users;

        private readonly CachedFiles cachedFiles;
        private readonly Sessions sessions;

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
            MimeDict.Add(".wav", "audio/wave");

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
            this.dispatcherThread = new Thread(this.Run) { Name = "Up HTTP Server Dispatcher" };
            this.dispatcherThread.Start();
        }

        internal void Stop()
        {
            this.dispatcherThread.Abort();
        }

        internal void ReloadCachedFiles()
        {
            this.cachedFiles.Reload();
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
            catch (HttpListenerException ex)
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

                string reqUrl = req.Url.AbsolutePath;

                if (reqUrl.StartsWith("/d/"))
                {
                    this.ProcessFileDownload(req, reqUrl, res);
                }
                else if (reqUrl.StartsWith("/i/"))
                {
                    this.ProcessFileInfo(reqUrl, res);
                }
                else if (reqUrl.StartsWith("/api/"))
                {
                    
                    string session = req.Cookies["session"]?.Value;
                    string user = this.sessions.GetUserFromSession(session, Http.GetRealIp(req));

                    switch (reqUrl)
                    {
                        case "/api/login":
                            this.sessions.InvalidateSession(req.Cookies["session"]?.Value);

                            using (StreamReader reader = new StreamReader(req.InputStream))
                            {
                                string loginUser = reader.ReadLine();
                                string pass = reader.ReadLine();

                                using (StreamWriter writer = new StreamWriter(res.OutputStream))
                                {
                                    if (this.users.Verify(loginUser, pass))
                                    {
                                        // the commented out line doesn't work on mono as they ignore the expiration date
                                        // res.SetCookie(new Cookie("session", this.sessions.RegisterSession(user), "/") { Expires = DateTime.Now.AddYears(10) });
                                        res.AddHeader("Set-Cookie", $"session={this.sessions.RegisterSession(loginUser, Http.GetRealIp(req))}; Max-Age=315619200; Path=/");

                                        writer.Write("ok");
                                    }
                                    else
                                    {
                                        writer.Write("failed");
                                    }
                                }
                            }

                            break;

                        case "/api/get-sessions":
                            if (user != null)
                            {
                                const string format = "yyyy-MM-dd HH:mm:ss";

                                using (StreamWriter writer = new StreamWriter(res.OutputStream))
                                {
                                    writer.NewLine = "\n";

                                    foreach (string sessionEntry in this.sessions.GetSessionsFromUser(user))
                                    {
                                        Sessions.SessionData data = this.sessions.GetData(sessionEntry);

                                        writer.WriteLine($"{sessionEntry};{data.FirstLogin.ToString(format)};{data.LastActivity.ToString(format)};{(object)data.LastIp ?? "unknown"}");
                                    }
                                }
                            }
                            else
                            {
                                res.StatusCode = (int)HttpStatusCode.Forbidden;
                            }

                            break;

                        case "/api/end-session":
                            if (user != null)
                            {
                                using (StreamReader reader = new StreamReader(res.OutputStream))
                                {
                                    string s = reader.ReadLine();

                                    if (this.sessions.HasSession(user, s))
                                    {
                                        this.sessions.InvalidateSession(s);

                                        using (StreamWriter writer = new StreamWriter(res.OutputStream))
                                        {
                                            writer.Write(s == session ? "redirect" : "ok");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                res.StatusCode = (int)HttpStatusCode.Forbidden;
                            }

                            break;
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
                    else if (reqUrl == "/logout")
                    {
                        this.sessions.InvalidateSession(req.Cookies["session"]?.Value);

                        res.SetCookie(new Cookie("session", "", "/") { Expired = true });
                        res.Redirect("/login");
                    }
                    else if (reqUrl == "/sessions" && user == null)
                    {
                        res.Redirect("/login");
                    }
                    else
                    {
                        byte[] data = this.cachedFiles[reqUrl];

                        if (data == null)
                        {
                            res.StatusCode = (int)HttpStatusCode.NotFound;
                        }
                        else
                        {
                            res.ContentLength64 = data.Length;
                            res.OutputStream.Write(data, 0, data.Length);
                            res.OutputStream.Close();
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
            // Link format: /d/12345678
            string fileId = reqUrl.Substring(3, Constants.Server.FileIdLength);

            if (this.files.FileExists(fileId) && this.files.GetDownloadableFlag(fileId))
            {
                string fileName = this.files.GetFileName(fileId);

                using (FileStream fileStream = File.OpenRead(Path.Combine(this.config.FileStorageFolder, fileId)))
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
                    bool forcedDownload = reqUrl.EndsWith("!");

                    if (!forcedDownload && (MimeDict.TryGetValue(fileExt, out string mime) || this.GuessTextMimeType(fileId, out mime)))
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


        private static Decoder Utf8 = Encoding.GetEncoding(Encoding.UTF8.CodePage, new EncoderExceptionFallback(), new DecoderExceptionFallback()).GetDecoder();

        /// <summary>
        /// Tries to analyze whether a given text file is encoded in UTF-8, UTF-16BE, UTF-16LE or an ASCII-based code page.
        /// </summary>
        /// <param name="fileId">The id of the file to check.</param>
        /// <param name="mime">The mime type of the text file, or null.</param>
        /// <returns>True if the file has been detected as a text file, false otherwise.</returns>
        private bool GuessTextMimeType(string fileId, out string mime)
        {
            if (OtherMimeCache.TryGetValue(fileId, out mime))
            {
                return mime != null;
            }

            byte[] bytes = new byte[Constants.Server.SniffBytes];

            using (FileStream fs = File.OpenRead(Path.Combine(this.config.FileStorageFolder, fileId)))
            {
                int bytesRead = fs.Read(bytes, 0, bytes.Length);

                try
                {
                    Utf8.GetCharCount(bytes, 0, bytesRead);

                    mime = "text/plain; charset=utf-8";
                    OtherMimeCache.Add(fileId, mime);
                    return true;
                }
                catch (DecoderFallbackException)
                {
                    if (bytesRead >= 2)
                    {
                        if (bytes[0] == 0xfe && bytes[1] == 0xff)
                        {
                            mime = "text/plain; charset=utf-16be";
                            OtherMimeCache.Add(fileId, mime);
                            return true;
                        }
                        else if (bytes[0] == 0xff && bytes[1] == 0xfe)
                        {
                            mime = "text/plain; charset=utf-16le";
                            OtherMimeCache.Add(fileId, mime);
                            return true;
                        }
                    }

                    for (int i = 0; i < bytesRead; i++)
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
                                mime = null;
                                OtherMimeCache.Add(fileId, mime);
                                return false;
                        }
                    }

                    mime = "text/plain";
                    OtherMimeCache.Add(fileId, mime);
                    return true;
                }
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

        private class CachedFiles
        {
            private readonly string[] fileNamesMinimal = { "favicon.ico" };
            private readonly string[] fileNames = { "favicon.ico", "login.html", "home.html", "sessions.html", "style.css", "custom-font" };

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

        private class Sessions
        {
            private readonly Dictionary<string, string> sessionToUser = new Dictionary<string, string>();
            private readonly Dictionary<string, List<string>> userToSessions = new Dictionary<string, List<string>>();
            private readonly Dictionary<string, SessionData> sessionData = new Dictionary<string, SessionData>();

            internal void InvalidateSession(string session)
            {
                if (session != null && this.sessionToUser.ContainsKey(session))
                {
                    string user = this.sessionToUser[session];

                    if (this.userToSessions.ContainsKey(user))
                    {
                        this.userToSessions[user].Remove(session);
                    }

                    this.sessionToUser.Remove(session);
                    this.sessionData.Remove(session);
                }
            }

            internal string GetUserFromSession(string session, IPAddress address)
            {
                if (session == null || !this.sessionToUser.ContainsKey(session))
                {
                    return null;
                }

                SessionData data = this.sessionData[session];

                data.LastActivity = DateTime.Now;
                data.LastIp = address ?? data.LastIp;

                this.sessionData[session] = data;

                return this.sessionToUser[session];
            }

            internal List<string> GetSessionsFromUser(string user)
            {
                return user != null && this.userToSessions.ContainsKey(user) ? this.userToSessions[user] : new List<string>();
            }

            internal bool HasSession(string user, string session)
            {
                return this.userToSessions.ContainsKey(user) && this.userToSessions[user].Contains(session);
            }

            internal string RegisterSession(string user, IPAddress address)
            {
                string session = Util.GetRandomString(10);

                while (this.sessionToUser.ContainsKey(session))
                {
                    session = Util.GetRandomString(10);
                }
                
                if (!this.userToSessions.ContainsKey(user))
                {
                    this.userToSessions.Add(user, new List<string>());
                }
                
                this.sessionToUser.Add(session, user);
                this.userToSessions[user].Add(session);
                this.sessionData.Add(session, new SessionData(address));

                return session;
            }

            internal SessionData GetData(string session)
            {
                return this.sessionData[session];
            }

            internal struct SessionData
            {
                internal DateTime FirstLogin { get; }

                internal DateTime LastActivity { get; set; }

                internal IPAddress LastIp { get; set; }

                internal SessionData(IPAddress address)
                {
                    this.FirstLogin = DateTime.Now;
                    this.LastActivity = DateTime.Now;
                    this.LastIp = address;
                }
            }
        }
    }
}
