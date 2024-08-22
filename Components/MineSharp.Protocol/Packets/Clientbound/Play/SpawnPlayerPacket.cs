using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     SpawnPlayerPacket used for versions &lt;= 1.20.1
///     Merged with SpawnEntityPacket in 1.20.2
/// </summary>
/// <param name="EntityId">The ID of the entity.</param>
/// <param name="PlayerUuid">The UUID of the player.</param>
/// <param name="X">The X coordinate of the player.</param>
/// <param name="Y">The Y coordinate of the player.</param>
/// <param name="Z">The Z coordinate of the player.</param>
/// <param name="Yaw">The yaw of the player.</param>
/// <param name="Pitch">The pitch of the player.</param>
public sealed partial record SpawnPlayerPacket(
    int EntityId,
    Uuid PlayerUuid,
    double X,
    double Y,
    double Z,
    byte Yaw,
    byte Pitch
) : IPacketStatic<SpawnPlayerPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_NamedEntitySpawn;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteUuid(PlayerUuid);
        buffer.WriteDouble(X);
        buffer.WriteDouble(Y);
        buffer.WriteDouble(Z);
        buffer.WriteByte(Yaw);
        buffer.WriteByte(Pitch);
    }

    /// <inheritdoc />
    public static SpawnPlayerPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var entityId = buffer.ReadVarInt();
        var playerUuid = buffer.ReadUuid();
        var x = buffer.ReadDouble();
        var y = buffer.ReadDouble();
        var z = buffer.ReadDouble();
        var yaw = buffer.ReadByte();
        var pitch = buffer.ReadByte();
        return new SpawnPlayerPacket(entityId, playerUuid, x, y, z, yaw, pitch);
    }
}
