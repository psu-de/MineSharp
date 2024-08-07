using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Pong Packet https://wiki.vg/Protocol#Ping_Response_.28play.29
/// </summary>
/// <param name="id"></param>
public class PongPacket(int id) : IPacket
{
    /// <summary>
    ///     Pong id
    /// </summary>
    public int Id { get; set; } = id;

    /// <inheritdoc />
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.SB_Play_Pong;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteInt(Id);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new PongPacket(
            buffer.ReadInt());
    }
}
