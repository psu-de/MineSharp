using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
/// The ChunkBatchReceived packet, used since 1.20.2.
/// https://wiki.vg/Protocol#Chunk_Batch_Received
/// </summary>
public class ChunkBatchReceivedPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.SB_Play_ChunkBatchReceived;
    
    /// <summary>
    /// ChunksPerTick
    /// </summary>
    public float ChunksPerTick { get; set; }

    /// <summary>
    /// Create a new ChunkBatchReceivedPacket instance
    /// </summary>
    /// <param name="chunksPerTick"></param>
    public ChunkBatchReceivedPacket(float chunksPerTick)
    {
        this.ChunksPerTick = chunksPerTick;
    }
    
    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteFloat(this.ChunksPerTick);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
        => new ChunkBatchReceivedPacket(buffer.ReadFloat());
}