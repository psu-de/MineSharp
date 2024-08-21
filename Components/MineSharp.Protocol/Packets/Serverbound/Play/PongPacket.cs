using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Pong Packet https://wiki.vg/Protocol#Ping_Response_.28play.29
/// </summary>
/// <param name="Id"></param>
public sealed partial record PongPacket(int Id) : IPacketStatic<PongPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_Pong;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteInt(Id);
    }

    /// <inheritdoc />
    public static PongPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var id = buffer.ReadInt();

        return new PongPacket(id);
    }
}
