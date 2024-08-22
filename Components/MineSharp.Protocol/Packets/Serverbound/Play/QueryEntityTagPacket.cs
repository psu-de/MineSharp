using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Query Entity Tag packet.
///     Used when F3+I is pressed while looking at an entity.
/// </summary>
/// <param name="TransactionId">An incremental ID so that the client can verify that the response matches.</param>
/// <param name="EntityId">The ID of the entity to query.</param>
public sealed record QueryEntityTagPacket(int TransactionId, int EntityId) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_QueryEntityNbt;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(TransactionId);
        buffer.WriteVarInt(EntityId);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var transactionId = buffer.ReadVarInt();
        var entityId = buffer.ReadVarInt();

        return new QueryEntityTagPacket(transactionId, entityId);
    }
}
