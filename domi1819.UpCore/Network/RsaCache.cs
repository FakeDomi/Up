using System;
using System.IO;
using System.Text;
using domi1819.UpCore.Crypto;
using domi1819.UpCore.Utilities;

namespace domi1819.UpCore.Network
{
    public class RsaCache : IDisposable
    {
        public string Path { get; }

        public RsaKey Key { get; set; }

        public string ServerAddress { get; set; }

        private string GetKeyPath(string serverAddress) => System.IO.Path.Combine(this.Path, Blake2.Hash(Encoding.UTF8.GetBytes(serverAddress), 24).ToHexString(6) + ".key");

        public RsaCache(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            this.Path = path;
        }

        public bool LoadKey(string serverAddress)
        {
            if (serverAddress == null)
            {
                throw new ArgumentNullException(nameof(serverAddress));
            }

            if (this.ServerAddress == serverAddress && this.Key?.Csp != null)
            {
                return true;
            }

            this.Key?.Dispose();
            this.Key = null;
            this.ServerAddress = null;

            string keyPath = this.GetKeyPath(serverAddress);

            if (File.Exists(keyPath))
            {
                this.Key = RsaKey.FromFile(keyPath);
            }

            return this.Key != null;
        }

        public void SaveKey()
        {
            Rsa.WriteKey(new[] { this.Key.Modulus, this.Key.Exponent }, this.GetKeyPath(this.ServerAddress), false);
        }

        public void Dispose()
        {
            this.Key?.Dispose();
        }
    }
}
