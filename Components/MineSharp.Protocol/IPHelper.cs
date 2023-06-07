using MineSharp.Protocol.Exceptions;
using System.Net;

namespace MineSharp.Protocol;

internal static class IPHelper
{
    public static IPAddress ResolveHostname(string hostnameOrIp)
    {
        var type = Uri.CheckHostName(hostnameOrIp);
        string ip = type switch {
            UriHostNameType.Dns =>
                (Dns.GetHostEntry(hostnameOrIp).AddressList.FirstOrDefault()
                 ?? throw new MineSharpHostException($"Could not find ip for hostname ('{hostnameOrIp}')")).ToString(),

            UriHostNameType.IPv4 => hostnameOrIp,

            _ => throw new MineSharpHostException("Hostname not supported: " + hostnameOrIp)
        };

        return IPAddress.Parse(ip);
    }
}
