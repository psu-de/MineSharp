using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
/// Packet to use an item
/// </summary>
public class UseItemPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.SB_Play_UseItem;

    /// <summary>
    /// The Hand used
    /// </summary>
    public PlayerHand Hand { get; set; }

    /// <summary>
    /// Sequence id used to synchronize server and client.
    /// Only used for versions &gt;= 1.19
    /// </summary>
    public int? SequenceId { get; set; }

    /// <summary>
    /// Constructor for 1.18-1.18.2
    /// </summary>
    /// <param name="hand"></param>
    public UseItemPacket(PlayerHand hand)
    {
        this.Hand = hand;
    }

    /// <summary>
    /// Constructor for &gt;= 1.19
    /// </summary>
    /// <param name="hand"></param>
    /// <param name="sequenceId"></param>
    public UseItemPacket(PlayerHand hand, int sequenceId) : this(hand)
    {
        this.SequenceId = sequenceId;
    }

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt((int)this.Hand);

        if (version.Version.Protocol >= ProtocolVersion.V_1_19)
            buffer.WriteVarInt((int)this.SequenceId!);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var hand = (PlayerHand)buffer.ReadVarInt();

        if (version.Version.Protocol < ProtocolVersion.V_1_19)
            return new UseItemPacket(hand);

        return new UseItemPacket(hand, buffer.ReadVarInt());
    }
}
