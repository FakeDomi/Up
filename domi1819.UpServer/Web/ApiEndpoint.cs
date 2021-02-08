using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace domi1819.UpServer.Web
{
    internal abstract class ApiEndpoint
    {
        internal static Entries Endpoints { get; } = new Entries();

        internal virtual bool NeedsAuthentication => true;

        internal abstract string Url { get; }

        internal abstract void Process(Request request);

        internal class Entries
        {
            private readonly Dictionary<string, ApiEndpoint> dictionary = new Dictionary<string, ApiEndpoint>();

            internal ApiEndpoint this[string url] => this.dictionary.TryGetValue(url, out ApiEndpoint endpoint) ? endpoint : null;

            internal Entries()
            {
                foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(ApiEndpoint))))
                {
                    ApiEndpoint endpoint = (ApiEndpoint)Activator.CreateInstance(type);
                    this.dictionary.Add(endpoint.Url, endpoint);
                }
            }
        }
    }
}
