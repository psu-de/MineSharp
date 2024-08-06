using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class ParticlePacket : IPacket
{
    public ParticlePacket(int particleId, bool longDistance, double x, double y, double z, float offsetX, float offsetY, float offsetZ, float maxSpeed, int particleCount, PacketBuffer data)
    {
        ParticleId = particleId;
        LongDistance = longDistance;
        X = x;
        Y = y;
        Z = z;
        OffsetX = offsetX;
        OffsetY = offsetY;
        OffsetZ = offsetZ;
        MaxSpeed = maxSpeed;
        ParticleCount = particleCount;
        Data = data;
    }

    public int ParticleId { get; set; }
    public bool LongDistance { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public float OffsetX { get; set; }
    public float OffsetY { get; set; }
    public float OffsetZ { get; set; }
    public float MaxSpeed { get; set; }
    public int ParticleCount { get; set; }
    /// <summary>
    ///     Data depends on the particle id.
    ///     Will be an empty buffer for most particles.
    /// </summary>
    public PacketBuffer Data { get; set; }
    public PacketType Type => PacketType.CB_Play_WorldParticles;

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
        var byteBuffer = new byte[buffer.ReadableBytes];
        buffer.ReadBytes(byteBuffer);
        var data = new PacketBuffer(byteBuffer, buffer.ProtocolVersion);

        return new ParticlePacket(particleId, longDistance, x, y, z, offsetX, offsetY, offsetZ, maxSpeed, particleCount, data);
    }

}
#pragma warning restore CS1591
