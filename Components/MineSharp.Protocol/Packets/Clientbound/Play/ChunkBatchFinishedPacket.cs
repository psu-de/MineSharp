using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     The ChunkBatchFinished packet, used since 1.20.2.
///     https://wiki.vg/Protocol#Chunk_Batch_Finished
/// </summary>
public class ChunkBatchFinishedPacket : IPacket
{
    /// <summary>
    ///     Create a new ChunkBatchFinishedPacket instance
    /// </summary>
    /// <param name="batchSize"></param>
    public ChunkBatchFinishedPacket(int batchSize)
    {
        BatchSize = batchSize;
    }

    /// <summary>
    ///     The batch size
    /// </summary>
    public int BatchSize { get; set; }

    /// <inheritdoc />
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.CB_Play_ChunkBatchFinished;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(BatchSize);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new ChunkBatchFinishedPacket(
            buffer.ReadVarInt());
    }
}
