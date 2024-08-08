using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     The ChunkBatchFinished packet, used since 1.20.2.
///     https://wiki.vg/Protocol#Chunk_Batch_Finished
/// </summary>
/// <param name="BatchSize">The batch size</param>
public sealed record ChunkBatchFinishedPacket(int BatchSize) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
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

