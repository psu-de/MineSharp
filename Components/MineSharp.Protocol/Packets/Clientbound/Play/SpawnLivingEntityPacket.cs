using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
/// <summary>
///     SpawnLivingEntityPacket used for versions &lt;= 1.18.2
/// </summary>
public class SpawnLivingEntityPacket : IPacket
{
    public SpawnLivingEntityPacket(int entityId, Uuid entityUuid, int entityType, double x, double y, double z,
                                   byte yaw, byte pitch,
                                   byte headPitch, short velocityX, short velocityY, short velocityZ)
    {
        EntityId = entityId;
        EntityUuid = entityUuid;
        EntityType = entityType;
        X = x;
        Y = y;
        Z = z;
        Yaw = yaw;
        Pitch = pitch;
        HeadPitch = headPitch;
        VelocityX = velocityX;
        VelocityY = velocityY;
        VelocityZ = velocityZ;
    }


    public int EntityId { get; set; }
    public Uuid EntityUuid { get; set; }
    public int EntityType { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public byte Yaw { get; set; }
    public byte Pitch { get; set; }
    public byte HeadPitch { get; set; }
    public short VelocityX { get; set; }
    public short VelocityY { get; set; }
    public short VelocityZ { get; set; }
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.CB_Play_SpawnEntityLiving;

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
            buffer.ReadShort());
    }
}
#pragma warning restore CS1591
