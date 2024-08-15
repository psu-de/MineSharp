using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Configuration;

/// <summary>
///     Pong packet
/// </summary>
/// <param name="Id">The ID of the pong packet</param>
public sealed record PongPacket(int Id) : IPacketStatic<PongPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Configuration_Pong;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteInt(Id);
    }

    /// <inheritdoc />
    public static PongPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new PongPacket(buffer.ReadInt());
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}

