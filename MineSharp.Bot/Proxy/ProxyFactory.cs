using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using MineSharp.Protocol.Connection;
using Starksoft.Aspen.Proxy;

namespace MineSharp.Bot.Proxy;

/// <summary>
///     A Proxy provider
/// </summary>
public class ProxyFactory : IConnectionFactory
{
    /// <summary>
    ///     Specifies the type of a proxy
    /// </summary>
#pragma warning disable CS1591
    public enum ProxyType { Http, Socks4, Socks4A, Socks5 }
#pragma warning restore CS1591
    private static readonly ProxyClientFactory Factory = new();

    /// <summary>
    ///     Hostname of the proxy
    /// </summary>
    public readonly string Hostname;

    /// <summary>
    ///     If authentication is required, the password for the proxy
    /// </summary>
    public readonly string? Password;

    /// <summary>
    ///     Port of the Proxy
    /// </summary>
    public readonly int Port;

    private readonly IProxyClient proxyClient;

    /// <summary>
    ///     If authentication is required, the username for the proxy
    /// </summary>
    public readonly string? Username;

    private HttpClientHandler? httpClientHandler;

    /// <summary>
    ///     The type of the proxy
    /// </summary>
    public ProxyType Type;

    /// <summary>
    ///     Create a new ProxyProvider with authentication
    /// </summary>
    /// <param name="type"></param>
    /// <param name="hostname"></param>
    /// <param name="port"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public ProxyFactory(ProxyType type, string hostname, int port, string username, string password)
        : this(type, hostname, port)
    {
        Username = username;
        Password = password;
    }

    /// <summary>
    ///     Create a new ProxyProvider without authentication
    /// </summary>
    /// <param name="type"></param>
    /// <param name="hostname"></param>
    /// <param name="port"></param>
    public ProxyFactory(ProxyType type, string hostname, int port)
    {
        Type = type;
        Hostname = hostname;
        Port = port;

        var socketType = Type switch
        {
            ProxyType.Http => Starksoft.Aspen.Proxy.ProxyType.Http,
            ProxyType.Socks4 => Starksoft.Aspen.Proxy.ProxyType.Socks4,
            ProxyType.Socks4A => Starksoft.Aspen.Proxy.ProxyType.Socks4a,
            ProxyType.Socks5 => Starksoft.Aspen.Proxy.ProxyType.Socks5,
            _ => throw new UnreachableException()
        };

        proxyClient = Username is null
            ? Factory.CreateProxyClient(socketType, Hostname, Port)
            : Factory.CreateProxyClient(socketType, Hostname, Port, Username, Password);
    }

    /// <summary>
    ///     Create a new TcpClient instance and connect to the given hostname and port
    /// </summary>
    /// <param name="address"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    public Task<TcpClient> CreateOpenConnection(IPAddress address, ushort port)
    {
        var ip = address.ToString();
        return Task.FromResult(proxyClient.CreateConnection(ip, port));
    }

    /// <summary>
    ///     Create a proxied http client
    /// </summary>
    /// <returns></returns>
    /// <exception cref="UnreachableException"></exception>
    public HttpClient CreateHttpClient()
    {
        if (httpClientHandler is not null)
        {
            return new(httpClientHandler);
        }

        var protocol = Type switch
        {
            ProxyType.Http => "http://",
            ProxyType.Socks4 => "socks://",
            ProxyType.Socks4A => "socks4a://",
            ProxyType.Socks5 => "socks5://",
            _ => throw new UnreachableException()
        };

        var proxyUrl = $"{protocol}{Hostname}:{Port}";
        var proxy = new WebProxy(proxyUrl);

        if (Username is not null && Password is not null)
        {
            proxy.Credentials = new NetworkCredential(Username, Password);
        }

        httpClientHandler = new() { Proxy = proxy };
        return new(httpClientHandler);
    }
}
