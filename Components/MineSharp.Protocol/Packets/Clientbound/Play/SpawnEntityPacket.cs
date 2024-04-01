using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class SpawnEntityPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_SpawnEntity;

    public int    EntityId   { get; set; }
    public UUID   ObjectUuid { get; set; }
    public int    EntityType { get; set; }
    public double X          { get; set; }
    public double Y          { get; set; }
    public double Z          { get; set; }
    public sbyte  Pitch      { get; set; }
    public sbyte  Yaw        { get; set; }
    public sbyte  HeadPitch  { get; set; }
    public int    ObjectData { get; set; }
    public short  VelocityX  { get; set; }
    public short  VelocityY  { get; set; }
    public short  VelocityZ  { get; set; }

    public SpawnEntityPacket(int   entityId,  UUID objectUuid, int   entityType, double x, double y, double z, sbyte pitch, sbyte yaw,
                             sbyte headPitch, int  objectData, short velocityX,  short  velocityY, short velocityZ)
    {
        this.EntityId   = entityId;
        this.ObjectUuid = objectUuid;
        this.EntityType = entityType;
        this.X          = x;
        this.Y          = y;
        this.Z          = z;
        this.Pitch      = pitch;
        this.Yaw        = yaw;
        this.HeadPitch  = headPitch;
        this.ObjectData = objectData;
        this.VelocityX  = velocityX;
        this.VelocityY  = velocityY;
        this.VelocityZ  = velocityZ;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        throw new NotImplementedException();
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var   entityId   = buffer.ReadVarInt();
        var   objectUuid = buffer.ReadUuid();
        var   type       = buffer.ReadVarInt();
        var   x          = buffer.ReadDouble();
        var   y          = buffer.ReadDouble();
        var   z          = buffer.ReadDouble();
        var   pitch      = buffer.ReadSByte();
        var   yaw        = buffer.ReadSByte();
        sbyte headPitch  = 0;
        int   objectData = 0;
        if (version.Version.Protocol >= ProtocolVersion.V_1_19)
        {
            headPitch  = buffer.ReadSByte();
            objectData = buffer.ReadVarInt();
        }
        else
        {
            objectData = buffer.ReadInt();
        }

        var velocityX = buffer.ReadShort();
        var velocityY = buffer.ReadShort();
        var velocityZ = buffer.ReadShort();

        return new SpawnEntityPacket(entityId, objectUuid, type, x, y, z, pitch, yaw, headPitch, objectData, velocityX, velocityY,
            velocityZ);
    }
}
#pragma warning restore CS1591
