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
public sealed record UseItemPacket(PlayerHand Hand, int? SequenceId = null) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_UseItem;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt((int)Hand);

        if (version.Version.Protocol >= ProtocolVersion.V_1_19)
        {
            buffer.WriteVarInt((int)SequenceId!);
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var hand = (PlayerHand)buffer.ReadVarInt();

        if (version.Version.Protocol < ProtocolVersion.V_1_19)
        {
            return new UseItemPacket(hand);
        }

        return new UseItemPacket(hand, buffer.ReadVarInt());
    }
}
