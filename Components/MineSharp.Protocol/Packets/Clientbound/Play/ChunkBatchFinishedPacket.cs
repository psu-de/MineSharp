using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     The ChunkBatchFinished packet, used since 1.20.2.
///     https://wiki.vg/Protocol#Chunk_Batch_Finished
/// </summary>
/// <param name="BatchSize">The batch size</param>
public sealed record ChunkBatchFinishedPacket(int BatchSize) : IPacketStatic<ChunkBatchFinishedPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_ChunkBatchFinished;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(BatchSize);
    }

    /// <inheritdoc />
    public static ChunkBatchFinishedPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new ChunkBatchFinishedPacket(
            buffer.ReadVarInt());
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}

