using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;
#pragma warning disable CS1591
/// <summary>
///     Keep alive packet in Configuration
/// </summary>
public class KeepAlivePacket : IPacket
{
    /// <summary>
    ///     Create a new instance
    /// </summary>
    /// <param name="keepAliveId"></param>
    public KeepAlivePacket(long keepAliveId)
    {
        KeepAliveId = keepAliveId;
    }

    /// <summary>
    ///     The keep alive id
    /// </summary>
    public long KeepAliveId { get; set; }

    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Configuration_KeepAlive;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteLong(KeepAliveId);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new KeepAlivePacket(buffer.ReadLong());
    }
}
#pragma warning restore CS1591
