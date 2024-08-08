using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     SpawnPaintingPacket used for versions &lt;= 1.18.2
/// </summary>
/// <param name="EntityId">The ID of the entity.</param>
/// <param name="EntityUuid">The UUID of the entity.</param>
/// <param name="Title">The title of the painting.</param>
/// <param name="Location">The location of the painting.</param>
/// <param name="Direction">The direction the painting is facing.</param>
public sealed record SpawnPaintingPacket(
    int EntityId,
    Uuid EntityUuid,
    int Title,
    Position Location,
    sbyte Direction
) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_SpawnEntityPainting;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteUuid(EntityUuid);
        buffer.WriteVarInt(Title);
        buffer.WritePosition(Location);
        buffer.WriteSByte(Direction);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var entityUuid = buffer.ReadUuid();
        var title = buffer.ReadVarInt();
        var location = buffer.ReadPosition();
        var direction = buffer.ReadSByte();
        return new SpawnPaintingPacket(entityId, entityUuid, title, location, direction);
    }
}
