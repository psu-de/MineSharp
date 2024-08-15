using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Entity rotation packet
/// </summary>
/// <param name="EntityId">The entity ID</param>
/// <param name="Yaw">The yaw rotation</param>
/// <param name="Pitch">The pitch rotation</param>
/// <param name="OnGround">Whether the entity is on the ground</param>
public sealed record EntityRotationPacket(int EntityId, sbyte Yaw, sbyte Pitch, bool OnGround) : IPacketStatic<EntityRotationPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_EntityLook;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteSByte(Yaw);
        buffer.WriteSByte(Pitch);
        buffer.WriteBool(OnGround);
    }

    /// <inheritdoc />
    public static EntityRotationPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var entityId = buffer.ReadVarInt();
        var yaw = buffer.ReadSByte();
        var pitch = buffer.ReadSByte();
        var onGround = buffer.ReadBool();

        return new EntityRotationPacket(entityId, yaw, pitch, onGround);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}

