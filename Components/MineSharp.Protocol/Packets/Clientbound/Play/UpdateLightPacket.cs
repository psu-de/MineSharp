using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using static MineSharp.Protocol.Packets.Clientbound.Play.UpdateLightPacket;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Updates light levels for a chunk.
/// </summary>
/// <param name="ChunkX">Chunk coordinate (block coordinate divided by 16, rounded down)</param>
/// <param name="ChunkZ">Chunk coordinate (block coordinate divided by 16, rounded down)</param>
/// <param name="SkyLightMask">BitSet for sky light sections</param>
/// <param name="BlockLightMask">BitSet for block light sections</param>
/// <param name="EmptySkyLightMask">BitSet for empty sky light sections</param>
/// <param name="EmptyBlockLightMask">BitSet for empty block light sections</param>
/// <param name="SkyLightArrays">Array of sky light data</param>
/// <param name="BlockLightArrays">Array of block light data</param>
public sealed record UpdateLightPacket(
    int ChunkX,
    int ChunkZ,
    BitSet SkyLightMask,
    BitSet BlockLightMask,
    BitSet EmptySkyLightMask,
    BitSet EmptyBlockLightMask,
    LightArray[] SkyLightArrays,
    LightArray[] BlockLightArrays
) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_UpdateLight;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(ChunkX);
        buffer.WriteVarInt(ChunkZ);
        buffer.WriteBitSet(SkyLightMask);
        buffer.WriteBitSet(BlockLightMask);
        buffer.WriteBitSet(EmptySkyLightMask);
        buffer.WriteBitSet(EmptyBlockLightMask);
        buffer.WriteVarInt(SkyLightArrays.Length);
        foreach (var array in SkyLightArrays)
        {
            array.Write(buffer);
        }
        buffer.WriteVarInt(BlockLightArrays.Length);
        foreach (var array in BlockLightArrays)
        {
            array.Write(buffer);
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var chunkX = buffer.ReadVarInt();
        var chunkZ = buffer.ReadVarInt();
        var skyLightMask = buffer.ReadBitSet();
        var blockLightMask = buffer.ReadBitSet();
        var emptySkyLightMask = buffer.ReadBitSet();
        var emptyBlockLightMask = buffer.ReadBitSet();
        var skyLightArrayCount = buffer.ReadVarInt();
        var skyLightArrays = new LightArray[skyLightArrayCount];
        for (int i = 0; i < skyLightArrayCount; i++)
        {
            skyLightArrays[i] = LightArray.Read(buffer);
        }
        var blockLightArrayCount = buffer.ReadVarInt();
        var blockLightArrays = new LightArray[blockLightArrayCount];
        for (int i = 0; i < blockLightArrayCount; i++)
        {
            blockLightArrays[i] = LightArray.Read(buffer);
        }

        return new UpdateLightPacket(
            chunkX,
            chunkZ,
            skyLightMask,
            blockLightMask,
            emptySkyLightMask,
            emptyBlockLightMask,
            skyLightArrays,
            blockLightArrays
        );
    }

    /// <summary>
    ///     Represents a light array.
    /// </summary>
    /// <param name="Length">Length of the array in bytes (always 2048)</param>
    /// <param name="Data">The light data</param>
    public sealed record LightArray(int Length, byte[] Data) : ISerializable<LightArray>
    {
        /// <inheritdoc />
        public void Write(PacketBuffer buffer)
        {
            buffer.WriteVarInt(Length);
            buffer.WriteBytes(Data);
        }

        /// <inheritdoc />
        public static LightArray Read(PacketBuffer buffer)
        {
            var length = buffer.ReadVarInt();
            var data = buffer.ReadBytes(length);
            return new LightArray(length, data);
        }
    }
}
