using System.Net;
using System.Net.Sockets;

namespace MineSharp.Protocol.Connection;

/// <summary>
///     Interface to provide a tcp client for <see cref="MinecraftClient" />
/// </summary>
public interface IConnectionFactory
{
    /// <summary>
    ///     Create an open tcp client
    /// </summary>
    /// <param name="address"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    public Task<TcpClient> CreateOpenConnection(IPAddress address, ushort port);

    /// <summary>
    ///     Create an HTTP client
    /// </summary>
    /// <returns></returns>
    public HttpClient CreateHttpClient();
}
