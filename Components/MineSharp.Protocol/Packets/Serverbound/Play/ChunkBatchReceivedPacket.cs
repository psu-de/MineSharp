using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     The ChunkBatchReceived packet, used since 1.20.2.
///     https://wiki.vg/Protocol#Chunk_Batch_Received
/// </summary>
public sealed record ChunkBatchReceivedPacket(float ChunksPerTick) : IPacketStatic<ChunkBatchReceivedPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_ChunkBatchReceived;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteFloat(ChunksPerTick);
    }

    /// <inheritdoc />
    public static ChunkBatchReceivedPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new ChunkBatchReceivedPacket(buffer.ReadFloat());
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
