using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     SpawnLivingEntityPacket used for versions &lt;= 1.18.2
/// </summary>
/// <param name="EntityId">The ID of the entity.</param>
/// <param name="EntityUuid">The UUID of the entity.</param>
/// <param name="EntityType">The type of the entity.</param>
/// <param name="X">The X coordinate of the entity.</param>
/// <param name="Y">The Y coordinate of the entity.</param>
/// <param name="Z">The Z coordinate of the entity.</param>
/// <param name="Yaw">The yaw of the entity.</param>
/// <param name="Pitch">The pitch of the entity.</param>
/// <param name="HeadPitch">The head pitch of the entity.</param>
/// <param name="VelocityX">The X velocity of the entity.</param>
/// <param name="VelocityY">The Y velocity of the entity.</param>
/// <param name="VelocityZ">The Z velocity of the entity.</param>
public sealed record SpawnLivingEntityPacket(
    int EntityId,
    Uuid EntityUuid,
    int EntityType,
    double X,
    double Y,
    double Z,
    byte Yaw,
    byte Pitch,
    byte HeadPitch,
    short VelocityX,
    short VelocityY,
    short VelocityZ
) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_SpawnEntityLiving;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteUuid(EntityUuid);
        buffer.WriteVarInt(EntityType);
        buffer.WriteDouble(X);
        buffer.WriteDouble(Y);
        buffer.WriteDouble(Z);
        buffer.WriteByte(Yaw);
        buffer.WriteByte(Pitch);
        buffer.WriteByte(HeadPitch);
        buffer.WriteShort(VelocityX);
        buffer.WriteShort(VelocityY);
        buffer.WriteShort(VelocityZ);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new SpawnLivingEntityPacket(
            buffer.ReadVarInt(),
            buffer.ReadUuid(),
            buffer.ReadVarInt(),
            buffer.ReadDouble(),
            buffer.ReadDouble(),
            buffer.ReadDouble(),
            buffer.ReadByte(),
            buffer.ReadByte(),
            buffer.ReadByte(),
            buffer.ReadShort(),
            buffer.ReadShort(),
            buffer.ReadShort()
        );
    }
}
