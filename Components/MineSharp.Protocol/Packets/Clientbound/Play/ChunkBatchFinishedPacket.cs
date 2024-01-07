using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
/// The ChunkBatchFinished packet, used since 1.20.2.
/// https://wiki.vg/Protocol#Chunk_Batch_Finished
/// </summary>
public class ChunkBatchFinishedPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Play_ChunkBatchFinished;

    /// <summary>
    /// The batch size
    /// </summary>
    public int BatchSize { get; set; }

    /// <summary>
    /// Create a new ChunkBatchFinishedPacket instance
    /// </summary>
    /// <param name="batchSize"></param>
    public ChunkBatchFinishedPacket(int batchSize)
    {
        this.BatchSize = batchSize;
    }

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.BatchSize);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new ChunkBatchFinishedPacket(
            buffer.ReadVarInt());
    }
}