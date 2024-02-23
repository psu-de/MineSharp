using MineSharp.Protocol;
using System.Net.Sockets;
using Starksoft.Aspen.Proxy;
using System.Diagnostics;
using System.Net;

namespace MineSharp.Bot.Proxy;

/// <summary>
/// A Proxy provider
/// </summary>
public class ProxyFactory : ITcpClientFactory
{
    private static readonly ProxyClientFactory Factory = new ProxyClientFactory();

    /// <summary>
    /// Specifies the type of a proxy
    /// </summary>
#pragma warning disable CS1591
    public enum ProxyType { None, Http, Socks4, Socks4A, Socks5, }
#pragma warning restore CS1591

    /// <summary>
    /// The type of the proxy
    /// </summary>
    public ProxyType Type;

    /// <summary>
    /// Hostname of the proxy
    /// </summary>
    public readonly string Hostname;

    /// <summary>
    /// Port of the Proxy
    /// </summary>
    public readonly int Port;

    /// <summary>
    /// If authentication is required, the username for the proxy
    /// </summary>
    public readonly string? Username;

    /// <summary>
    /// If authentication is required, the password for the proxy
    /// </summary>
    public readonly string? Password;

    private IProxyClient?      proxyClient;
    private HttpClientHandler? httpClientHandler;

    /// <summary>
    /// Create an empty ProxyProvider
    /// </summary>
    public ProxyFactory()
        : this(ProxyType.None, string.Empty, 0)
    { }

    /// <summary>
    /// Create a new ProxyProvider with authentication
    /// </summary>
    /// <param name="type"></param>
    /// <param name="hostname"></param>
    /// <param name="port"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public ProxyFactory(ProxyType type, string hostname, int port, string username, string password)
        : this(type, hostname, port)
    {
        this.Username = username;
        this.Password = password;
    }

    /// <summary>
    /// Create a new ProxyProvider without authentication 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="hostname"></param>
    /// <param name="port"></param>
    public ProxyFactory(ProxyType type, string hostname, int port)
    {
        this.Type     = type;
        this.Hostname = hostname;
        this.Port     = port;

        if (this.Type == ProxyType.None)
            return;

        var socketType = this.Type switch
        {
            ProxyType.Http    => Starksoft.Aspen.Proxy.ProxyType.Http,
            ProxyType.Socks4  => Starksoft.Aspen.Proxy.ProxyType.Socks4,
            ProxyType.Socks4A => Starksoft.Aspen.Proxy.ProxyType.Socks4a,
            ProxyType.Socks5  => Starksoft.Aspen.Proxy.ProxyType.Socks5,
            _                 => throw new UnreachableException()
        };

        this.proxyClient = this.Username is null
            ? Factory.CreateProxyClient(socketType, this.Hostname, this.Port)
            : Factory.CreateProxyClient(socketType, this.Hostname, this.Port, this.Username, this.Password);
    }

    /// <summary>
    /// Create a new TcpClient instance and connect to the given hostname and port
    /// </summary>
    /// <param name="hostname"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    public TcpClient CreateOpenConnection(string hostname, ushort port)
    {
        return this.proxyClient is null
            ? new TcpClient(hostname, port)
            : this.proxyClient.CreateConnection(hostname, port);
    }

    /// <summary>
    /// Create a proxied http client
    /// </summary>
    /// <returns></returns>
    /// <exception cref="UnreachableException"></exception>
    public HttpClient CreateHttpClient()
    {
        if (this.Type == ProxyType.None)
            return new HttpClient();

        if (this.httpClientHandler is not null)
            return new HttpClient(this.httpClientHandler);

        var protocol = this.Type switch
        {
            ProxyType.Http    => "http://",
            ProxyType.Socks4  => "socks://",
            ProxyType.Socks4A => "socks4a://",
            ProxyType.Socks5  => "socks5://",
            _                 => throw new UnreachableException()
        };

        var proxyUrl = $"{protocol}{this.Hostname}:{this.Port}";
        var proxy    = new WebProxy(proxyUrl);

        if (this.Username is not null && this.Password is not null)
            proxy.Credentials = new NetworkCredential(this.Username, this.Password);

        this.httpClientHandler = new HttpClientHandler() { Proxy = proxy };
        return new HttpClient(this.httpClientHandler);
    }
}
