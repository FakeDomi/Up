namespace Domi.UpServer.Web.ApiEndpoints
{
    internal class GetSessions : ApiEndpoint
    {
        internal override string Url => "/api/get-sessions";

        internal override void Process(Request request)
        {
            foreach (string sessionEntry in request.Sessions.GetSessionsFromUser(request.User))
            {
                Sessions.SessionData data = request.Sessions.GetData(sessionEntry);

                request.Writer.WriteLine($"{sessionEntry};{data.FirstLogin.ToString(UpWebService.DateFormat)};{data.LastActivity.ToString(UpWebService.DateFormat)};{(object)data.LastIp ?? "unknown"}");
            }
        }
    }
}
