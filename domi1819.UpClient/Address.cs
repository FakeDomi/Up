namespace domi1819.UpClient
{
    internal struct Address
    {
        internal static Address Invalid = default(Address);

        internal string Host;

        internal int Port;

        internal Address(string host, int port)
        {
            this.Host = host;
            this.Port = port;
        }

        internal bool Equals(Address other)
        {
            return this.Host == other.Host && this.Port == other.Port;
        }

        internal static Address Parse(string address, int defaultPort)
        {
            string[] addressSplit = address.Split(':');

            if (addressSplit.Length == 1)
            {
                return new Address(addressSplit[0], defaultPort);
            }

            if (addressSplit.Length == 2)
            {
                if (int.TryParse(addressSplit[1], out int port) && port > 0 && port <= 65535)
                {
                    return new Address(addressSplit[0], port);
                }
            }

            return Invalid;
        }
    }
}
