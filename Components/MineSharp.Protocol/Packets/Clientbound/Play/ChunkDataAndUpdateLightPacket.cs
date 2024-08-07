using fNbt;
using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
/// <summary>
///     Chunk data and update light packet
/// </summary>
public class ChunkDataAndUpdateLightPacket : IPacket
{
    /// <summary>
    ///     Create a new instance
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="heightmaps"></param>
    /// <param name="chunkData"></param>
    /// <param name="blockEntities"></param>
    /// <param name="trustEdges"></param>
    /// <param name="skyLightMask"></param>
    /// <param name="blockLightMask"></param>
    /// <param name="emptyBlockLightMask"></param>
    /// <param name="emptySkyLightMask"></param>
    /// <param name="skyLight"></param>
    /// <param name="blockLight"></param>
    public ChunkDataAndUpdateLightPacket(
        int x,
        int z,
        NbtCompound heightmaps,
        byte[] chunkData,
        BlockEntity[] blockEntities,
        bool? trustEdges,
        long[] skyLightMask,
        long[] blockLightMask,
        long[] emptyBlockLightMask,
        long[] emptySkyLightMask,
        byte[][] skyLight,
        byte[][] blockLight)
    {
        X = x;
        Z = z;
        Heightmaps = heightmaps;
        ChunkData = chunkData;
        BlockEntities = blockEntities;
        TrustEdges = trustEdges;
        SkyLightMask = skyLightMask;
        BlockLightMask = blockLightMask;
        EmptyBlockLightMask = emptyBlockLightMask;
        EmptySkyLightMask = emptySkyLightMask;
        SkyLight = skyLight;
        BlockLight = blockLight;
    }

    /// <summary>
    ///     X coordinate of the chunk
    /// </summary>
    public int X { get; set; }

    /// <summary>
    ///     Y coordinate of the chunk
    /// </summary>
    public int Z { get; set; }

    /// <summary>
    ///     Heightmaps
    /// </summary>
    public NbtCompound Heightmaps { get; set; }

    /// <summary>
    ///     Raw chunk data
    /// </summary>
    public byte[] ChunkData { get; set; }

    /// <summary>
    ///     Array of BlockEntities
    /// </summary>
    public BlockEntity[] BlockEntities { get; set; }

    /// <summary>
    ///     Whether to trust edges (only sent before 1.20)
    /// </summary>
    public bool? TrustEdges { get; set; }

    /// <summary>
    /// </summary>
    public long[] SkyLightMask { get; set; }

    /// <summary>
    /// </summary>
    public long[] BlockLightMask { get; set; }

    /// <summary>
    /// </summary>
    public long[] EmptySkyLightMask { get; set; }

    /// <summary>
    /// </summary>
    public long[] EmptyBlockLightMask { get; set; }

    /// <summary>
    /// </summary>
    public byte[][] SkyLight { get; set; }

    /// <summary>
    /// </summary>
    public byte[][] BlockLight { get; set; }

    /// <inheritdoc />
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.CB_Play_MapChunk;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol < ProtocolVersion.V_1_20 && TrustEdges == null)
        {
            throw new MineSharpPacketVersionException(nameof(TrustEdges), version.Version.Protocol);
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

        if (version.Version.Protocol < ProtocolVersion.V_1_20)
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
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
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
        if (version.Version.Protocol < ProtocolVersion.V_1_20)
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
}
#pragma warning restore CS1591
