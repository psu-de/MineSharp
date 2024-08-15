using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Sets the center position of the client's chunk loading area.
/// </summary>
/// <param name="ChunkX">Chunk X coordinate of the loading area center.</param>
/// <param name="ChunkZ">Chunk Z coordinate of the loading area center.</param>
public sealed record SetCenterChunkPacket(int ChunkX, int ChunkZ) : IPacketStatic<SetCenterChunkPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_UpdateViewPosition;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(ChunkX);
        buffer.WriteVarInt(ChunkZ);
    }

    /// <inheritdoc />
    public static SetCenterChunkPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var chunkX = buffer.ReadVarInt();
        var chunkZ = buffer.ReadVarInt();

        return new SetCenterChunkPacket(chunkX, chunkZ);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
