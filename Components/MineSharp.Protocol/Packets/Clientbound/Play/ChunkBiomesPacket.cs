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
public sealed partial record ChunkBiomesPacket(int NumberOfChunks, ChunkBiomeData[] ChunkBiomes) : IPacketStatic<ChunkBiomesPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_ChunkBiomes;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(NumberOfChunks);
        foreach (var chunk in ChunkBiomes)
        {
            chunk.Write(buffer);
        }
    }

    /// <inheritdoc />
    public static ChunkBiomesPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var numberOfChunks = buffer.ReadVarInt();
        var chunkBiomeData = new ChunkBiomeData[numberOfChunks];
        for (int i = 0; i < numberOfChunks; i++)
        {
            chunkBiomeData[i] = ChunkBiomeData.Read(buffer);
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
    public sealed record ChunkBiomeData(int ChunkZ, int ChunkX, int Size, byte[] Data) : ISerializable<ChunkBiomeData>
    {
        /// <inheritdoc />
        public void Write(PacketBuffer buffer)
        {
            buffer.WriteInt(ChunkZ);
            buffer.WriteInt(ChunkX);
            buffer.WriteVarInt(Size);
            buffer.WriteBytes(Data);
        }

        /// <inheritdoc />
        public static ChunkBiomeData Read(PacketBuffer buffer)
        {
            var chunkZ = buffer.ReadInt();
            var chunkX = buffer.ReadInt();
            var size = buffer.ReadVarInt();
            var data = buffer.ReadBytes(size);

            return new(chunkZ, chunkX, size, data);
        }
    }
}
