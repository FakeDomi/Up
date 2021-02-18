using System.IO;
using System.Net;

namespace Domi.UpServer.Web
{
    internal readonly ref struct Request
    {
        internal readonly string Session;
        internal readonly string User;
        internal readonly Sessions Sessions;
        internal readonly UserManager Users;
        internal readonly FileManager Files;
        internal readonly StreamReader Reader;
        internal readonly StreamWriter Writer;
        internal readonly HttpListenerRequest HttpRequest;
        internal readonly HttpListenerResponse HttpResponse;

        internal Request(string session, string user, Sessions sessions, UserManager users, FileManager files, StreamReader reader, StreamWriter writer, HttpListenerRequest req, HttpListenerResponse res)
        {
            this.Session = session;
            this.User = user;
            this.Sessions = sessions;
            this.Users = users;
            this.Files = files;
            this.Reader = reader;
            this.Writer = writer;
            this.HttpRequest = req;
            this.HttpResponse = res;
        }

        internal void SetError(string errorMessage)
        {
            this.HttpResponse.AddHeader(Headers.Result, Results.Error);
            this.HttpResponse.AddHeader(Headers.ErrorMessage, errorMessage);
        }
    }
}
