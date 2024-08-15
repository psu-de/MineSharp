using fNbt;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Tag Query Response packet
/// </summary>
/// <param name="TransactionId">The transaction ID</param>
/// <param name="Nbt">The NBT of the block or entity</param>
public sealed record TagQueryResponsePacket(int TransactionId, NbtTag? Nbt) : IPacketStatic<TagQueryResponsePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_NbtQueryResponse;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(TransactionId);
        buffer.WriteOptionalNbt(Nbt);
    }

    /// <inheritdoc />
    public static TagQueryResponsePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var transactionId = buffer.ReadVarInt();
        var nbt = buffer.ReadOptionalNbt();

        return new TagQueryResponsePacket(transactionId, nbt);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
