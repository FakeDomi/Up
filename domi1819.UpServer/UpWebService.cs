using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using domi1819.UpCore.Utilities;

namespace domi1819.UpServer
{
    internal class UpWebService
    {
        private readonly byte[] icon;
        private readonly Dictionary<string, string> mimeDict;

        internal UpWebService()
        {
            this.icon = File.ReadAllBytes("favicon.ico");

            this.mimeDict = new Dictionary<string, string>();

            this.mimeDict.Add(".jpg", "image/jpeg");
            this.mimeDict.Add(".jpeg", "image/jpeg");
            this.mimeDict.Add(".png", "image/png");
            this.mimeDict.Add(".txt", "text/plain");
            this.mimeDict.Add(".xml", "text/plain");
            this.mimeDict.Add(".java", "text/plain");
            this.mimeDict.Add(".cs", "text/plain");
            this.mimeDict.Add(".cfg", "text/plain");
            this.mimeDict.Add(".conf", "text/plain");
            this.mimeDict.Add(".log", "text/plain");
            this.mimeDict.Add(".pdf", "application/pdf");

            Thread t = new Thread(this.Run);
            t.Start();
        }

        private void Run()
        {
            HttpListener listener = new HttpListener();

            string hostName = UpServer.Instance.Settings.HostName;

            listener.Prefixes.Add($"http://+:{UpServer.Instance.Settings.WebPort}/");

            try
            {
                listener.Start();

                Console.WriteLine($"Listening for HTTP connections ({hostName}:{UpServer.Instance.Settings.WebPort})");

                while (true)
                {
                    ThreadPool.QueueUserWorkItem(this.ProcessHttpRequest, listener.GetContext());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HTTP listener has been stopped:");
                Console.WriteLine(ex.ToString());
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
                    res.ContentLength64 = this.icon.Length;
                    res.OutputStream.Write(this.icon, 0, this.icon.Length);
                    res.OutputStream.Close();

                    res.Close();
                }
                else if (reqUrl.StartsWith("/d?"))
                {
                    this.ProcessDownload(reqUrl, res);
                }
                else if (reqUrl.StartsWith("/f?"))
                {
                    this.ProcessFileQuery(reqUrl, res);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void ProcessDownload(string reqUrl, HttpListenerResponse res)
        {
            string fileId = reqUrl.Replace("/d?", "").Replace("!", "");

            if (UpServer.Instance.Files.HasFile(fileId) && UpServer.Instance.Files.GetDownloadableFlag(fileId))
            {
                string fileName = UpServer.Instance.Files.GetFileName(fileId);

                using (FileStream fs = File.OpenRead(Path.Combine("filestor", fileId)))
                {
                    res.ContentLength64 = fs.Length;

                    //Console.WriteLine("Sending file " + fileId + " (" + fileName + ")| size=" + fs.Length + "b");

                    res.SendChunked = false;

                    string fileExt = Path.GetExtension(fileName) ?? string.Empty;

                    if (this.mimeDict.ContainsKey(fileExt) && !reqUrl.EndsWith("!"))
                    {
                        res.AddHeader("Content-disposition", "inline; filename=\"" + fileName + "\"");
                        res.ContentType = this.mimeDict[fileExt];
                    }
                    else
                    {
                        res.AddHeader("Content-disposition", "attachment; filename=\"" + fileName + "\"");
                        res.ContentType = "application/octet-stream";
                    }
                    
                    byte[] buffer = new byte[4 * 1024];

                    using (BinaryWriter bw = new BinaryWriter(res.OutputStream))
                    {
                        int read;

                        while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            bw.Write(buffer, 0, read);
                            bw.Flush();

                            //if (!UpServer.Instance.Files.GetDownloadableFlag(fileId))
                            //{
                            //    throw new Exception("abort download");
                            //}
                        }

                        res.OutputStream.Close();
                        bw.Close();
                    }

                    res.Close();

                    UpServer.Instance.Files.AddFileDownload(fileId);
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

        private void ProcessFileQuery(string reqUrl, HttpListenerResponse res)
        {
            string fileId = reqUrl.Replace("/f?", "");

            if (UpServer.Instance.Files.HasFile(fileId))
            {
                string fileName = UpServer.Instance.Files.GetFileName(fileId);
                int downloads = UpServer.Instance.Files.GetFileDownloads(fileId);
                long size = UpServer.Instance.Files.GetFileSize(fileId);
                string sizeText = Util.GetByteSizeText(size) + (size >= 1024 ? " (" + size + (size == 1 ? " byte)" : " bytes)") : string.Empty);
                string trafficCaused = Util.GetByteSizeText(size * (downloads + 1));
                string timeStamp = UpServer.Instance.Files.GetUploadDate(fileId).ToString("yyyy-MM-dd HH:mm:ss");

                byte[] bytes = Encoding.UTF8.GetBytes($"<html><head><title>^up - File Details</title><style type=\"text/css\">td{{padding:2px 8px 2px 8px;}}</style></head><body bgcolor=\"#2D2D30\"><font color=\"#F1F1F1\"><table style=\"color: #F1F1F1; border: 1px solid #434344; background: #252526\"><tr><td>File name</td><td>{fileName}</td></tr><tr><td>Uploaded on</td><td>{timeStamp}</td></tr><tr><td>File size</td><td>{sizeText}</td></tr><tr><td>Downloads</td><td>{downloads}</td></tr><tr><td>Traffic caused</td><td>{trafficCaused}</td></tr></table><br><address>UpServer {Constants.Version} at {UpServer.Instance.Settings.HostName}</address></body></html>");

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
            byte[] bytes = Encoding.UTF8.GetBytes($"<html><head><title>^up - File not found</title></head><body bgcolor=\"#2D2D30\"><font color=\"#F1F1F1\">The requested file {fileId} doesn't exist.<br><address>UpServer { Constants.Version } at {UpServer.Instance.Settings.HostName}</address></body></html>");

            res.ContentLength64 = bytes.Length;
            res.OutputStream.Write(bytes, 0, bytes.Length);
            res.OutputStream.Close();

            res.Close();
        }
    }
}
