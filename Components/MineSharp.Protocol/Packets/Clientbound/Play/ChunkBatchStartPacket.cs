using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     The ChunkBatchStart Packet, used since 1.20.2
///     https://wiki.vg/Protocol#Chunk_Batch_Start
/// </summary>
public sealed partial record ChunkBatchStartPacket : IPacketStatic<ChunkBatchStartPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_ChunkBatchStart;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    { }

    /// <inheritdoc />
    public static ChunkBatchStartPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new ChunkBatchStartPacket();
    }
}
