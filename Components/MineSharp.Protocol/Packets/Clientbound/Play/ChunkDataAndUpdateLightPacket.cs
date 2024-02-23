using fNbt;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
/// <summary>
/// Chunk data and update light packet
/// </summary>
public class ChunkDataAndUpdateLightPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Play_MapChunk;

    /// <summary>
    /// X coordinate of the chunk
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Y coordinate of the chunk
    /// </summary>
    public int Z { get; set; }

    /// <summary>
    /// Heightmaps
    /// </summary>
    public NbtCompound Heightmaps { get; set; }

    /// <summary>
    /// Raw chunk data
    /// </summary>
    public byte[] ChunkData { get; set; }

    /// <summary>
    /// Array of BlockEntities
    /// </summary>
    public BlockEntity[] BlockEntities { get; set; }

    /// <summary>
    /// Whether to trust edges (only sent before 1.20)
    /// </summary>
    public bool? TrustEdges { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public long[] SkyLightMask { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public long[] BlockLightMask { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public long[] EmptySkyLightMask { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public long[] EmptyBlockLightMask { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public byte[][] SkyLight { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public byte[][] BlockLight { get; set; }

    /// <summary>
    /// Create a new instance
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
        int           x,
        int           z,
        NbtCompound   heightmaps,
        byte[]        chunkData,
        BlockEntity[] blockEntities,
        bool?         trustEdges,
        long[]        skyLightMask,
        long[]        blockLightMask,
        long[]        emptyBlockLightMask,
        long[]        emptySkyLightMask,
        byte[][]      skyLight,
        byte[][]      blockLight)
    {
        this.X                   = x;
        this.Z                   = z;
        this.Heightmaps          = heightmaps;
        this.ChunkData           = chunkData;
        this.BlockEntities       = blockEntities;
        this.TrustEdges          = trustEdges;
        this.SkyLightMask        = skyLightMask;
        this.BlockLightMask      = blockLightMask;
        this.EmptyBlockLightMask = emptyBlockLightMask;
        this.EmptySkyLightMask   = emptySkyLightMask;
        this.SkyLight            = skyLight;
        this.BlockLight          = blockLight;
    }

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol < ProtocolVersion.V_1_20 && this.TrustEdges == null)
            throw new ArgumentNullException(nameof(this.TrustEdges));

        buffer.WriteInt(this.X);
        buffer.WriteInt(this.Z);
        buffer.WriteNbt(this.Heightmaps);
        buffer.WriteVarInt(this.ChunkData.Length);
        buffer.WriteBytes(this.ChunkData);

        buffer.WriteVarInt(this.BlockEntities.Length);
        foreach (var entity in this.BlockEntities)
            buffer.WriteBlockEntity(entity);

        if (version.Version.Protocol < ProtocolVersion.V_1_20)
            buffer.WriteBool(this.TrustEdges!.Value);
        WriteLongArray(this.SkyLightMask, buffer);
        WriteLongArray(this.BlockLightMask, buffer);
        WriteLongArray(this.EmptySkyLightMask, buffer);
        WriteLongArray(this.EmptyBlockLightMask, buffer);

        buffer.WriteVarInt(this.SkyLight.Length);
        foreach (var array in this.SkyLight)
        {
            buffer.WriteVarInt(array.Length);
            buffer.WriteBytes(array);
        }


        buffer.WriteVarInt(this.BlockLight.Length);
        foreach (var array in this.BlockLight)
        {
            buffer.WriteVarInt(array.Length);
            buffer.WriteBytes(array);
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var x          = buffer.ReadInt();
        var z          = buffer.ReadInt();
        var heightmaps = buffer.ReadNbtCompound();
        var chunkData  = new byte[buffer.ReadVarInt()];
        buffer.ReadBytes(chunkData);

        var blockEntities = new BlockEntity[buffer.ReadVarInt()];
        for (int i = 0; i < blockEntities.Length; i++)
            blockEntities[i] = buffer.ReadBlockEntity();

        bool? trustEdges = null;
        if (version.Version.Protocol < ProtocolVersion.V_1_20)
            trustEdges = buffer.ReadBool();
        var skyLightMask        = ReadLongArray(buffer);
        var blockLightMask      = ReadLongArray(buffer);
        var emptySkyLightMask   = ReadLongArray(buffer);
        var emptyBlockLightMask = ReadLongArray(buffer);
        var skyLight            = new byte[buffer.ReadVarInt()][];
        for (int i = 0; i < skyLight.Length; i++)
        {
            skyLight[i] = new byte[buffer.ReadVarInt()];
            buffer.ReadBytes(skyLight[i]);
        }

        var blockLight = new byte[buffer.ReadVarInt()][];
        for (int i = 0; i < blockLight.Length; i++)
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

    private static void WriteLongArray(long[] array, PacketBuffer buffer)
    {
        buffer.WriteVarInt(array.Length);
        foreach (var l in array)
            buffer.WriteLong(l);
    }

    private static long[] ReadLongArray(PacketBuffer buffer)
    {
        int    length = buffer.ReadVarInt();
        long[] array  = new long[length];

        for (int i = 0; i < array.Length; i++)
            array[i] = buffer.ReadLong();

        return array;
    }
}
#pragma warning restore CS1591
