namespace domi1819.UpServer.Web.ApiEndpoints
{
    internal class EndSession : ApiEndpoint
    {
        internal override string Url => "/api/end-session";
        
        internal override void Process(Request request)
        {
            string sessionToEnd = request.Reader.ReadLine();

            if (request.Sessions.HasSession(request.User, sessionToEnd))
            {
                request.Sessions.InvalidateSession(sessionToEnd);

                request.Writer.Write(sessionToEnd == request.Session ? "redirect" : "ok");
            }
        }
    }
}
