using System.Net.Sockets;

namespace MineSharp.Protocol;

/// <summary>
/// Interface to provide a tcp client for <see cref="MinecraftClient"/>
/// </summary>
public interface ITcpClientFactory
{
    /// <summary>
    /// Create an open tcp client
    /// </summary>
    /// <param name="hostname"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    public TcpClient CreateOpenConnection(string hostname, ushort port);
}
