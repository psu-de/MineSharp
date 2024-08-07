using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     The ChunkBatchReceived packet, used since 1.20.2.
///     https://wiki.vg/Protocol#Chunk_Batch_Received
/// </summary>
public class ChunkBatchReceivedPacket : IPacket
{
    /// <summary>
    ///     Create a new ChunkBatchReceivedPacket instance
    /// </summary>
    /// <param name="chunksPerTick"></param>
    public ChunkBatchReceivedPacket(float chunksPerTick)
    {
        ChunksPerTick = chunksPerTick;
    }

    /// <summary>
    ///     ChunksPerTick
    /// </summary>
    public float ChunksPerTick { get; set; }

    /// <inheritdoc />
    public PacketType Type => StaticType;
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
