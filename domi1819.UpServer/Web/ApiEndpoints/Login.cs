using domi1819.UpCore.Utilities;

namespace domi1819.UpServer.Web.ApiEndpoints
{
    internal class Login : ApiEndpoint
    {
        internal override bool NeedsAuthentication => false;

        internal override string Url => "/api/login";

        internal override void Process(Request request)
        {
            request.Sessions.InvalidateSession(request.Session);

            string loginUser = request.Reader.ReadLine();
            string pass = request.Reader.ReadLine();

            if (request.Users.Verify(loginUser, pass))
            {
                // the commented out line doesn't work on mono as they ignore the expiration date
                // res.SetCookie(new Cookie("session", this.sessions.RegisterSession(user), "/") { Expires = DateTime.Now.AddYears(10) });
                request.HttpResponse.AddHeader("Set-Cookie", $"session={request.Sessions.RegisterSession(loginUser, Http.GetRealIp(request.HttpRequest))}; Max-Age=315619200; Path=/");

                request.Writer.Write("ok");
            }
            else
            {
                request.Writer.Write("failed");
            }
        }
    }
}
