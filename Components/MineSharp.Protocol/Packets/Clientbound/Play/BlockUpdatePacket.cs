using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Block update packet
/// </summary>
/// <param name="Location">The location of the block update</param>
/// <param name="StateId">The new state id</param>
public sealed record BlockUpdatePacket(Position Location, int StateId) : IPacketStatic<BlockUpdatePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_BlockChange;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WritePosition(Location);
        buffer.WriteVarInt(StateId);
    }

    /// <inheritdoc />
    public static BlockUpdatePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var location = buffer.ReadPosition();
        var stateId = buffer.ReadVarInt();
        return new BlockUpdatePacket(location, stateId);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
