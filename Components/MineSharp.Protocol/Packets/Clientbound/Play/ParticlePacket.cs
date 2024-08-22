﻿using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
/// Represents a packet for particle effects in the game.
/// </summary>
/// <param name="ParticleId">The ID of the particle.</param>
/// <param name="LongDistance">Indicates if the particle is long distance.</param>
/// <param name="X">The X coordinate of the particle.</param>
/// <param name="Y">The Y coordinate of the particle.</param>
/// <param name="Z">The Z coordinate of the particle.</param>
/// <param name="OffsetX">The X offset of the particle.</param>
/// <param name="OffsetY">The Y offset of the particle.</param>
/// <param name="OffsetZ">The Z offset of the particle.</param>
/// <param name="MaxSpeed">The maximum speed of the particle.</param>
/// <param name="ParticleCount">The number of particles.</param>
/// <param name="Data">
///     Data depends on the particle id.
///     Will be an empty buffer for most particles.
/// </param>
public sealed partial record ParticlePacket(
    int ParticleId,
    bool LongDistance,
    double X,
    double Y,
    double Z,
    float OffsetX,
    float OffsetY,
    float OffsetZ,
    float MaxSpeed,
    int ParticleCount,
    IParticleData? Data
) : IPacketStatic<ParticlePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_WorldParticles;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(ParticleId);
        buffer.WriteBool(LongDistance);
        buffer.WriteDouble(X);
        buffer.WriteDouble(Y);
        buffer.WriteDouble(Z);
        buffer.WriteFloat(OffsetX);
        buffer.WriteFloat(OffsetY);
        buffer.WriteFloat(OffsetZ);
        buffer.WriteFloat(MaxSpeed);
        buffer.WriteVarInt(ParticleCount);
        Data?.Write(buffer, data);
    }

    /// <inheritdoc />
    public static ParticlePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var particleId = buffer.ReadVarInt();
        var longDistance = buffer.ReadBool();
        var x = buffer.ReadDouble();
        var y = buffer.ReadDouble();
        var z = buffer.ReadDouble();
        var offsetX = buffer.ReadFloat();
        var offsetY = buffer.ReadFloat();
        var offsetZ = buffer.ReadFloat();
        var maxSpeed = buffer.ReadFloat();
        var particleCount = buffer.ReadVarInt();
        var particleData = ParticleDataRegistry.Read(buffer, data, particleId);

        return new ParticlePacket(particleId, longDistance, x, y, z, offsetX, offsetY, offsetZ, maxSpeed, particleCount, particleData);
    }
}
