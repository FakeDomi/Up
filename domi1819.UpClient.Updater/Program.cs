using System;
using System.Net;
using System.Net.Cache;
using System.Security.Cryptography;

namespace domi1819.UpClient.Updater
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            WebClient webClient = new WebClient { CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore) };

            Console.WriteLine("Downloading updates...");

            RSACryptoServiceProvider prov = new RSACryptoServiceProvider(4088);

            var pars = prov.ExportParameters(true);
        }
        
    }
}
