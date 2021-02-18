using System;
using System.Net;

namespace Domi.UpCore.Utilities
{
    public static class Http
    {
        public static IPAddress GetRealIp(HttpListenerRequest req)
        {
            if (req.RemoteEndPoint != null)
            {
                if (IPAddress.IsLoopback(req.RemoteEndPoint.Address) && 
                    (IPAddress.TryParse(req.Headers.Get("X-Real-IP"), out IPAddress address) || 
                     // ReSharper disable once AssignNullToNotNullAttribute
                     IPAddress.TryParse(req.Headers.Get("X-Forwarded-For")?.Split(',')[0].Trim(), out address)))
                {
                    return address;
                }

                return req.RemoteEndPoint.Address;
            }

            return null;
        }

        public static bool GetRange(string rangeHeader, out long start, out long end, long fileSize)
        {
            start = -1;
            end = -1;

            string[] separators = { "=", ", " };

            if (rangeHeader != null)
            {
                string[] parts = rangeHeader.Split(separators, StringSplitOptions.None);

                if (parts.Length == 2 && parts[0].Equals("bytes", StringComparison.InvariantCultureIgnoreCase))
                {
                    string[] ranges = parts[1].Split('-');
                    if (ranges.Length == 2)
                    {
                        if (ranges[1] == "" || long.TryParse(ranges[1], out end))
                        {
                            if (ranges[0] == "")
                            {
                                // Format: bytes=-XXX

                                // Can't be bytes=-
                                if (ranges[1] != "")
                                {
                                    start = fileSize - end;
                                    end = fileSize - 1;

                                    if (start >= 0)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else if (long.TryParse(ranges[0], out start))
                            {

                                if (ranges[1] == "")
                                {
                                    // Format: bytes=XXX-

                                    if (start < fileSize)
                                    {
                                        end = fileSize - 1;

                                        return true;
                                    }
                                }
                                else
                                {
                                    // Format: bytes=XXX-XXX
                                    // Parsing is very forgiving here (for now)

                                    if (start >= fileSize)
                                    {
                                        start = fileSize - 1;
                                    }

                                    if (end >= fileSize)
                                    {
                                        end = fileSize - 1;
                                    }

                                    if (start > end)
                                    {
                                        long swap = start;
                                        start = end;
                                        end = swap;
                                    }

                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
