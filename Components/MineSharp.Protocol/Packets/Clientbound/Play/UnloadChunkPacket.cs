using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Represents a packet to unload a chunk.
/// </summary>
/// <param name="X">The X coordinate of the chunk.</param>
/// <param name="Z">The Z coordinate of the chunk.</param>
public sealed record UnloadChunkPacket(int X, int Z) : IPacketStatic<UnloadChunkPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_UnloadChunk;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteInt(X);
        buffer.WriteInt(Z);
    }

    /// <inheritdoc />
    public static UnloadChunkPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var x = buffer.ReadInt();
        var z = buffer.ReadInt();
        return new UnloadChunkPacket(x, z);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}

