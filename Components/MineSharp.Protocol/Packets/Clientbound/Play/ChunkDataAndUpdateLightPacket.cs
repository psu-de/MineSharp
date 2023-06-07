using fNbt;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class ChunkDataAndUpdateLightPacket : IPacket
{
    public static int Id => 0x24;

    public int X { get; set; }
    public int Z { get; set; }
    public NbtCompound Heightmaps { get; set; }
    public byte[] ChunkData { get; set; }
    public BlockEntity[] BlockEntities { get; set; }
    public bool TrustEdges { get; set; }
    public long[] SkyLightMask { get; set; }
    public long[] BlockLightMask { get; set; }
    public long[] EmptySkyLightMask { get; set; }
    public long[] EmptyBlockLightMask { get; set; }
    public byte[][] SkyLight { get; set; }
    public byte[][] BlockLight { get; set; }

    public ChunkDataAndUpdateLightPacket(
        int x,
        int z,
        NbtCompound heightmaps,
        byte[] chunkData,
        BlockEntity[] blockEntities,
        bool trustEdges,
        long[] skyLightMask,
        long[] blockLightMask,
        long[] emptyBlockLightMask,
        long[] emptySkyLightMask,
        byte[][] skyLight,
        byte[][] blockLight)
    {
        this.X = x;
        this.Z = z;
        this.Heightmaps = heightmaps;
        this.ChunkData = chunkData;
        this.BlockEntities = blockEntities;
        this.TrustEdges = trustEdges;
        this.SkyLightMask = skyLightMask;
        this.BlockLightMask = blockLightMask;
        this.EmptyBlockLightMask = emptyBlockLightMask;
        this.EmptySkyLightMask = emptySkyLightMask;
        this.SkyLight = skyLight;
        this.BlockLight = blockLight;
    }

    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteInt(this.X);
        buffer.WriteInt(this.Z);
        buffer.WriteNbt(this.Heightmaps);
        buffer.WriteVarInt(this.ChunkData.Length);
        buffer.WriteBytes(this.ChunkData);
        
        buffer.WriteVarInt(this.BlockEntities.Length);
        foreach (var entity in this.BlockEntities)
            buffer.WriteBlockEntity(entity);
        
        buffer.WriteBool(this.TrustEdges);
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

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        var x = buffer.ReadInt();
        var z = buffer.ReadInt();
        var heightmaps = buffer.ReadNbt();
        var chunkData = new byte[buffer.ReadVarInt()];
        buffer.ReadBytes(chunkData);

        var blockEntities = new BlockEntity[buffer.ReadVarInt()];
        for (int i = 0; i < blockEntities.Length; i++)
            blockEntities[i] = buffer.ReadBlockEntity();

        var trustEdges = buffer.ReadBool();
        var skyLightMask = ReadLongArray(buffer);
        var blockLightMask = ReadLongArray(buffer);
        var emptySkyLightMask = ReadLongArray(buffer);
        var emptyBlockLightMask = ReadLongArray(buffer);
        var skyLight = new byte[buffer.ReadVarInt()][];
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
        int length = buffer.ReadVarInt();
        long[] array = new long[length];

        for (int i = 0; i < array.Length; i++)
            array[i] = buffer.ReadLong();

        return array;
    }
}
