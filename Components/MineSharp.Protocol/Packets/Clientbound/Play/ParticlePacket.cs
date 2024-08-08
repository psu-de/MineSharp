using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

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
public sealed record ParticlePacket(
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
    PacketBuffer Data
) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_WorldParticles;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
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
        buffer.WriteBytes(Data.GetBuffer());
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
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
        var byteBuffer = buffer.RestBuffer();
        var data = new PacketBuffer(byteBuffer, buffer.ProtocolVersion);

        return new ParticlePacket(particleId, longDistance, x, y, z, offsetX, offsetY, offsetZ, maxSpeed, particleCount, data);
    }
}
