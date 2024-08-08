using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Status;
#pragma warning disable CS1591
/// <summary>
///     Packet for server status response
/// </summary>
/// <param name="Response">The server response</param>
public sealed record StatusResponsePacket(string Response) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Status_ServerInfo;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(Response);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new StatusResponsePacket(buffer.ReadString());
    }
}
#pragma warning restore CS1591
