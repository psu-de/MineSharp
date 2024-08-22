﻿using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Entity position and rotation packet
/// </summary>
/// <param name="EntityId">The entity ID</param>
/// <param name="DeltaX">The change in X position</param>
/// <param name="DeltaY">The change in Y position</param>
/// <param name="DeltaZ">The change in Z position</param>
/// <param name="Yaw">The yaw rotation</param>
/// <param name="Pitch">The pitch rotation</param>
/// <param name="OnGround">Whether the entity is on the ground</param>
public sealed partial record EntityPositionAndRotationPacket(int EntityId, short DeltaX, short DeltaY, short DeltaZ, sbyte Yaw, sbyte Pitch, bool OnGround) : IPacketStatic<EntityPositionAndRotationPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_EntityMoveLook;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteShort(DeltaX);
        buffer.WriteShort(DeltaY);
        buffer.WriteShort(DeltaZ);
        buffer.WriteSByte(Yaw);
        buffer.WriteShort(Pitch);
        buffer.WriteBool(OnGround);
    }

    /// <inheritdoc />
    public static EntityPositionAndRotationPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var entityId = buffer.ReadVarInt();
        var deltaX = buffer.ReadShort();
        var deltaY = buffer.ReadShort();
        var deltaZ = buffer.ReadShort();
        var yaw = buffer.ReadSByte();
        var pitch = buffer.ReadSByte();
        var onGround = buffer.ReadBool();

        return new EntityPositionAndRotationPacket(
            entityId,
            deltaX, deltaY, deltaZ,
            yaw, pitch,
            onGround);
    }
}
