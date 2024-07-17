using DnsClient;
using DnsClient.Protocol;
using MineSharp.Auth.Exceptions;
using MineSharp.Protocol.Exceptions;
using System.Net;

namespace MineSharp.Protocol;

internal static class IPHelper
{
    private static readonly LookupClient Client = new LookupClient();

    public static IPAddress ResolveHostname(string hostnameOrIp, ref ushort port)
    {
        var type = Uri.CheckHostName(hostnameOrIp);
        return type switch
        {
            UriHostNameType.Dns  => _ResolveHostname(hostnameOrIp, ref port),
            UriHostNameType.IPv4 => IPAddress.Parse(hostnameOrIp),

            _ => throw new MineSharpHostException($"unknown hostname: '{hostnameOrIp}'")
        };
    }

    private static IPAddress _ResolveHostname(string hostname, ref ushort port)
    {
        if (port != 25565 || hostname == "localhost")
            return DnsLookup(hostname);

        var result = Client.Query($"_minecraft._tcp.{hostname}", QueryType.SRV);

        if (result.HasError)
            return DnsLookup(hostname);

        var srvRecord = result.Answers
                              .OfType<SrvRecord>()
                              .FirstOrDefault();

        if (srvRecord == null)
            return DnsLookup(hostname); // No SRV record, fallback to hostname

        var serviceName = srvRecord.Target.Value;
        if (serviceName.EndsWith('.'))
            serviceName = serviceName.Substring(0, serviceName.Length - 1);

        var ip = DnsLookup(serviceName);
        port = srvRecord.Port;

        return ip;
    }

    private static IPAddress DnsLookup(string hostname)
    {
        return Client.GetHostEntry(hostname).AddressList.FirstOrDefault()
            ?? throw new MineSharpHostException($"ip not found for hostname '{hostname}'");
    }
}
