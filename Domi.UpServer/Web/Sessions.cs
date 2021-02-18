using System;
using System.Collections.Generic;
using System.Net;
using Domi.UpCore.Utilities;

namespace Domi.UpServer.Web
{
    internal class Sessions
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
