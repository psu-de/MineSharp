using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Packet to use an item
/// </summary>
public class UseItemPacket : IPacket
{
    /// <summary>
    ///     Constructor for 1.18-1.18.2
    /// </summary>
    /// <param name="hand"></param>
    public UseItemPacket(PlayerHand hand)
    {
        Hand = hand;
    }

    /// <summary>
    ///     Constructor for &gt;= 1.19
    /// </summary>
    /// <param name="hand"></param>
    /// <param name="sequenceId"></param>
    public UseItemPacket(PlayerHand hand, int sequenceId) : this(hand)
    {
        SequenceId = sequenceId;
    }

    /// <summary>
    ///     The Hand used
    /// </summary>
    public PlayerHand Hand { get; set; }

    /// <summary>
    ///     Sequence id used to synchronize server and client.
    ///     Only used for versions &gt;= 1.19
    /// </summary>
    public int? SequenceId { get; set; }

    /// <inheritdoc />
    public PacketType Type => StaticType;
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
