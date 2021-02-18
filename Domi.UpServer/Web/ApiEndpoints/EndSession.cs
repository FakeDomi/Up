namespace Domi.UpServer.Web.ApiEndpoints
{
    internal class EndSession : ApiEndpoint
    {
        internal override string Url => "/api/end-session";
        
        internal override void Process(Request request)
        {
            string sessionToEnd = request.HttpRequest.Headers[Headers.EndSession];

            if (request.Sessions.HasSession(request.User, sessionToEnd))
            {
                request.Sessions.InvalidateSession(sessionToEnd);
                request.HttpResponse.Headers.Add(Headers.Result, sessionToEnd == request.Session ? Results.Redirect : Results.Ok);
            }
        }
    }
}
