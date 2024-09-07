using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Query Block Entity Tag packet
///     Used when F3+I is pressed while looking at a block.
/// </summary>
/// <param name="TransactionId">An incremental ID so that the client can verify that the response matches.</param>
/// <param name="Location">The location of the block to check.</param>
public sealed record QueryBlockEntityTagPacket(int TransactionId, Position Location) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_QueryBlockNbt;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(TransactionId);
        buffer.WritePosition(Location);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var transactionId = buffer.ReadVarInt();
        var location = buffer.ReadPosition();

        return new QueryBlockEntityTagPacket(transactionId, location);
    }
}
