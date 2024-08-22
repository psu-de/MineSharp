﻿using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public sealed partial record TeleportEntityPacket(
    int EntityId,
    double X,
    double Y,
    double Z,
    sbyte Yaw,
    sbyte Pitch,
    bool OnGround
) : IPacketStatic<TeleportEntityPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_EntityTeleport;

    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteDouble(X);
        buffer.WriteDouble(Y);
        buffer.WriteDouble(Z);
        buffer.WriteSByte(Yaw);
        buffer.WriteSByte(Pitch);
        buffer.WriteBool(OnGround);
    }

    public static TeleportEntityPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var entityId = buffer.ReadVarInt();
        var x = buffer.ReadDouble();
        var y = buffer.ReadDouble();
        var z = buffer.ReadDouble();
        var yaw = buffer.ReadSByte();
        var pitch = buffer.ReadSByte();
        var onGround = buffer.ReadBool();

        return new TeleportEntityPacket(
            entityId, x, y, z, yaw, pitch, onGround);
    }
}
#pragma warning restore CS1591
