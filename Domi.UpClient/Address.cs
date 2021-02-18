namespace Domi.UpClient
{
    /// <summary>
    /// A type designed to hold a full address to some TCP/UDP server.
    /// </summary>
    internal struct Address
    {
        /// <summary>
        /// The default value of the Address type that marks an invalid Address.
        /// </summary>
        internal static readonly Address Invalid = default(Address);

        /// <summary>
        /// The host part of this Address. Can be an IP number, a hostname or a DNS name.
        /// </summary>
        internal string Host { get; }

        /// <summary>
        /// The port of this Address. Ranges from 1 - 65535.
        /// </summary>
        internal int Port { get; }

        /// <summary>
        /// Create a new Address with the given parameters.
        /// </summary>
        /// <param name="host">The host part.</param>
        /// <param name="port">The port.</param>
        private Address(string host, int port)
        {
            this.Host = host;
            this.Port = port;
        }

        /// <summary>
        /// Check two Addresses for equality in terms of host part and port.
        /// </summary>
        /// <param name="other">The other Address to use for comparing.</param>
        /// <returns>True if both Addresses are equal, false otherwise.</returns>
        internal bool Equals(Address other)
        {
            return this.Host == other.Host && this.Port == other.Port;
        }

        /// <summary>
        /// Parse a full address string into an Address, with a default port if none is found in the address string.
        /// Examples: google.com; 172.16.0.1:8080
        /// </summary>
        /// <param name="fullAddress">The address input string.</param>
        /// <param name="defaultPort">The default port to use if none is specified in the input string.</param>
        /// <returns>An Address or Address.Invalid when parsing failed.</returns>
        internal static Address Parse(string fullAddress, int defaultPort)
        {
            string[] addressSplit = fullAddress.Split(':');

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
