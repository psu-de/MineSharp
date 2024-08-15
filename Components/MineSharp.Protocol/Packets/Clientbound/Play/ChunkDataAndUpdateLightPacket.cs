using fNbt;
using MineSharp.Core;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Chunk data and update light packet
/// </summary>
/// <param name="X">X coordinate of the chunk</param>
/// <param name="Z">Y coordinate of the chunk</param>
/// <param name="Heightmaps">Heightmaps</param>
/// <param name="ChunkData">Raw chunk data</param>
/// <param name="BlockEntities">Array of BlockEntities</param>
/// <param name="TrustEdges">Whether to trust edges (only sent before 1.20)</param>
/// <param name="SkyLightMask"></param>
/// <param name="BlockLightMask"></param>
/// <param name="EmptySkyLightMask"></param>
/// <param name="EmptyBlockLightMask"></param>
/// <param name="SkyLight"></param>
/// <param name="BlockLight"></param>
public sealed record ChunkDataAndUpdateLightPacket(
    int X,
    int Z,
    NbtCompound Heightmaps,
    byte[] ChunkData,
    BlockEntity[] BlockEntities,
    bool? TrustEdges,
    long[] SkyLightMask,
    long[] BlockLightMask,
    long[] EmptySkyLightMask,
    long[] EmptyBlockLightMask,
    byte[][] SkyLight,
    byte[][] BlockLight) : IPacketStatic<ChunkDataAndUpdateLightPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_MapChunk;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        if (data.Version.Protocol < ProtocolVersion.V_1_20_0 && TrustEdges == null)
        {
            throw new MineSharpPacketVersionException(nameof(TrustEdges), data.Version.Protocol);
        }

        buffer.WriteInt(X);
        buffer.WriteInt(Z);
        buffer.WriteNbt(Heightmaps);
        buffer.WriteVarInt(ChunkData.Length);
        buffer.WriteBytes(ChunkData);

        buffer.WriteVarInt(BlockEntities.Length);
        foreach (var entity in BlockEntities)
        {
            buffer.WriteBlockEntity(entity);
        }

        if (data.Version.Protocol < ProtocolVersion.V_1_20_0)
        {
            buffer.WriteBool(TrustEdges!.Value);
        }

        buffer.WriteLongArray(SkyLightMask);
        buffer.WriteLongArray(BlockLightMask);
        buffer.WriteLongArray(EmptySkyLightMask);
        buffer.WriteLongArray(EmptyBlockLightMask);

        buffer.WriteVarInt(SkyLight.Length);
        foreach (var array in SkyLight)
        {
            buffer.WriteVarInt(array.Length);
            buffer.WriteBytes(array);
        }

        buffer.WriteVarInt(BlockLight.Length);
        foreach (var array in BlockLight)
        {
            buffer.WriteVarInt(array.Length);
            buffer.WriteBytes(array);
        }
    }

    /// <inheritdoc />
    public static ChunkDataAndUpdateLightPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var x = buffer.ReadInt();
        var z = buffer.ReadInt();
        var heightmaps = buffer.ReadNbtCompound();
        var chunkData = new byte[buffer.ReadVarInt()];
        buffer.ReadBytes(chunkData);

        var blockEntities = new BlockEntity[buffer.ReadVarInt()];
        for (var i = 0; i < blockEntities.Length; i++)
        {
            blockEntities[i] = buffer.ReadBlockEntity();
        }

        bool? trustEdges = null;
        if (data.Version.Protocol < ProtocolVersion.V_1_20_0)
        {
            trustEdges = buffer.ReadBool();
        }

        var skyLightMask = buffer.ReadLongArray();
        var blockLightMask = buffer.ReadLongArray();
        var emptySkyLightMask = buffer.ReadLongArray();
        var emptyBlockLightMask = buffer.ReadLongArray();
        var skyLight = new byte[buffer.ReadVarInt()][];
        for (var i = 0; i < skyLight.Length; i++)
        {
            skyLight[i] = new byte[buffer.ReadVarInt()];
            buffer.ReadBytes(skyLight[i]);
        }

        var blockLight = new byte[buffer.ReadVarInt()][];
        for (var i = 0; i < blockLight.Length; i++)
        {
            blockLight[i] = new byte[buffer.ReadVarInt()];
            buffer.ReadBytes(blockLight[i]);
        }

        return new ChunkDataAndUpdateLightPacket(
            x,
            z,
            heightmaps,
            chunkData,
            blockEntities,
            trustEdges,
            skyLightMask,
            blockLightMask,
            emptySkyLightMask,
            emptyBlockLightMask,
            skyLight,
            blockLight);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
