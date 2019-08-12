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
                    this.ProcessFileDownload(reqUrl, res);
                }
                else if (reqUrl.StartsWith("/i/"))
                {
                    this.ProcessFileInfo(reqUrl, res);
                }
                else if (reqUrl.StartsWith("/api/"))
                {
                    
                    string session = req.Cookies["session"]?.Value;
                    string user = this.sessions.GetUserFromSession(session, req.RemoteEndPoint?.Address);

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
                                        res.AddHeader("Set-Cookie", $"session={this.sessions.RegisterSession(loginUser, req.RemoteEndPoint?.Address)}; Max-Age=315619200; Path=/");

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
                    string user = this.sessions.GetUserFromSession(session, req.RemoteEndPoint?.Address);

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
