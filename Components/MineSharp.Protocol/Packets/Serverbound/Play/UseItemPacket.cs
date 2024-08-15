using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Packet to use an item
/// </summary>
/// <param name="Hand">The Hand used</param>
/// <param name="SequenceId">
///     Sequence id used to synchronize server and client.
///     Only used for versions &gt;= 1.19
/// </param>
public sealed record UseItemPacket(PlayerHand Hand, int? SequenceId = null) : IPacketStatic<UseItemPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_UseItem;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt((int)Hand);

        if (data.Version.Protocol >= ProtocolVersion.V_1_19_0)
        {
            buffer.WriteVarInt((int)SequenceId!);
        }
    }

    /// <inheritdoc />
    public static UseItemPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var hand = (PlayerHand)buffer.ReadVarInt();

        if (data.Version.Protocol < ProtocolVersion.V_1_19_0)
        {
            return new UseItemPacket(hand);
        }

        return new UseItemPacket(hand, buffer.ReadVarInt());
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
