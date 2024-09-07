using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using static MineSharp.Protocol.Packets.Clientbound.Play.ChunkBiomesPacket;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Chunk Biomes packet
/// </summary>
/// <param name="NumberOfChunks">Number of chunks</param>
/// <param name="ChunkBiomes">Array of chunk biome data</param>
public sealed record ChunkBiomesPacket(int NumberOfChunks, ChunkBiomeData[] ChunkBiomes) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_ChunkBiomes;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(NumberOfChunks);
        foreach (var chunk in ChunkBiomes)
        {
            buffer.WriteInt(chunk.ChunkZ);
            buffer.WriteInt(chunk.ChunkX);
            buffer.WriteVarInt(chunk.Size);
            buffer.WriteBytes(chunk.Data);
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var numberOfChunks = buffer.ReadVarInt();
        var chunkBiomeData = new ChunkBiomeData[numberOfChunks];
        for (int i = 0; i < numberOfChunks; i++)
        {
            var chunkZ = buffer.ReadInt();
            var chunkX = buffer.ReadInt();
            var size = buffer.ReadVarInt();
            var data = buffer.ReadBytes(size);
            chunkBiomeData[i] = new ChunkBiomeData(chunkZ, chunkX, size, data);
        }

        return new ChunkBiomesPacket(numberOfChunks, chunkBiomeData);
    }

    /// <summary>
    ///     Represents the chunk biome data
    /// </summary>
    /// <param name="ChunkZ">Chunk Z coordinate</param>
    /// <param name="ChunkX">Chunk X coordinate</param>
    /// <param name="Size">Size of data in bytes</param>
    /// <param name="Data">Chunk data structure</param>
    public sealed record ChunkBiomeData(int ChunkZ, int ChunkX, int Size, byte[] Data);
}
