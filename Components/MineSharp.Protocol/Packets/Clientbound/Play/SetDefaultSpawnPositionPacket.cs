using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Set Default Spawn Position packet
/// </summary>
/// <param name="Location">The spawn location</param>
/// <param name="Angle">The angle at which to respawn at</param>
public sealed record SetDefaultSpawnPositionPacket(Position Location, float Angle) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_SpawnPosition;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WritePosition(Location);
        buffer.WriteFloat(Angle);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var location = buffer.ReadPosition();
        var angle = buffer.ReadFloat();

        return new SetDefaultSpawnPositionPacket(location, angle);
    }
}
