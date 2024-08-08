using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     The ChunkBatchReceived packet, used since 1.20.2.
///     https://wiki.vg/Protocol#Chunk_Batch_Received
/// </summary>
public sealed record ChunkBatchReceivedPacket(float ChunksPerTick) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_ChunkBatchReceived;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteFloat(ChunksPerTick);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new ChunkBatchReceivedPacket(buffer.ReadFloat());
    }
}
