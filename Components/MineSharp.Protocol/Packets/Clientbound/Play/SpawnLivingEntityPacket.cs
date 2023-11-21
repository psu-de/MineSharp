using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
/// SpawnLivingEntityPacket used for versions <= 1.18.2
/// </summary>
public class SpawnLivingEntityPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_SpawnEntityLiving;
    
    
    public int EntityId { get; set; }
    public UUID EntityUuid { get; set; }
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

    public SpawnLivingEntityPacket(int entityId, UUID entityUuid, int entityType, double x, double y, double z, byte yaw, byte pitch, byte headPitch, short velocityX, short velocityY, short velocityZ)
    {
        this.EntityId = entityId;
        this.EntityUuid = entityUuid;
        this.EntityType = entityType;
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.Yaw = yaw;
        this.Pitch = pitch;
        this.HeadPitch = headPitch;
        this.VelocityX = velocityX;
        this.VelocityY = velocityY;
        this.VelocityZ = velocityZ;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.EntityId);
        buffer.WriteUuid(this.EntityUuid);
        buffer.WriteVarInt(this.EntityType);
        buffer.WriteDouble(this.X);
        buffer.WriteDouble(this.Y);
        buffer.WriteDouble(this.Z);
        buffer.WriteByte(this.Yaw);
        buffer.WriteByte(this.Pitch);
        buffer.WriteByte(this.HeadPitch);
        buffer.WriteShort(this.VelocityX);
        buffer.WriteShort(this.VelocityY);
        buffer.WriteShort(this.VelocityZ);
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
