using System.Net;
using System.Net.Sockets;

namespace MineSharp.Protocol.Connection;

/// <summary>
/// Default tcp factory
/// </summary>
public class DefaultTcpFactory : IConnectionFactory
{
    /// <summary>
    /// Singleton instance
    /// </summary>
    public static readonly DefaultTcpFactory Instance = new ();

    private DefaultTcpFactory() 
    { }

    /// <summary>
    /// Create new tcp client and connect to the hostname:port
    /// </summary>
    /// <param name="address"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<TcpClient> CreateOpenConnection(IPAddress address, ushort port)
    {
        var client = new TcpClient();
        await client.ConnectAsync(address, port);
        
        return client;
    }

    /// <inheritdoc />
    public HttpClient CreateHttpClient()
    {
        return new();
    }
}
