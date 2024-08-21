using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Status;

/// <summary>
///     Packet for server status response
/// </summary>
/// <param name="Response">The server response</param>
public sealed partial record StatusResponsePacket(string Response) : IPacketStatic<StatusResponsePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Status_ServerInfo;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteString(Response);
    }

    /// <inheritdoc />
    public static StatusResponsePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new StatusResponsePacket(buffer.ReadString());
    }
}
