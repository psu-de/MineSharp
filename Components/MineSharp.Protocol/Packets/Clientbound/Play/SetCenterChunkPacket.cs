using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Sets the center position of the client's chunk loading area.
/// </summary>
/// <param name="ChunkX">Chunk X coordinate of the loading area center.</param>
/// <param name="ChunkZ">Chunk Z coordinate of the loading area center.</param>
public sealed record SetCenterChunkPacket(int ChunkX, int ChunkZ) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_UpdateViewPosition;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(ChunkX);
        buffer.WriteVarInt(ChunkZ);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var chunkX = buffer.ReadVarInt();
        var chunkZ = buffer.ReadVarInt();

        return new SetCenterChunkPacket(chunkX, chunkZ);
    }
}
