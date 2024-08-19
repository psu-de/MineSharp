using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Sent by the client when the player swings their arm.
/// </summary>
/// <param name="Hand">The hand used by the player.</param>
public sealed record SwingArmPacket(PlayerHand Hand) : IPacketStatic<SwingArmPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_ArmAnimation;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt((int)Hand);
    }

    /// <inheritdoc />
    public static SwingArmPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var hand = (PlayerHand)buffer.ReadVarInt();

        return new SwingArmPacket(hand);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
