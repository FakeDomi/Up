using Domi.UpClient.Forms;
using Domi.UpCore.Utilities;

namespace Domi.UpClient
{
    internal static class InfoFormDefaults
    {
        internal static InfoForm PasswordChanged => new InfoForm("Success!", "Password changed.", Constants.Client.InfoSuccessTimeout);

        internal static InfoForm NewPasswordRejected => new InfoForm("Server rejected password!", "Make sure the password is not too short / long.", Constants.Client.InfoErrorTimeout);

        internal static InfoForm LoginFailed => new InfoForm("Login failed!", "Invalid username / password.", Constants.Client.InfoErrorTimeout);

        internal static InfoForm ServerNotTrusted => new InfoForm("Connection failed!", "Server key not trusted.", Constants.Client.InfoErrorTimeout);
    }
}
