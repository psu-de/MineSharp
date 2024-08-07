using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class SpawnEntityPacket : IPacket
{
    public SpawnEntityPacket(int entityId, Uuid objectUuid, int entityType, double x, double y, double z, sbyte pitch,
                             sbyte yaw,
                             sbyte headPitch, int objectData, short velocityX, short velocityY, short velocityZ)
    {
        EntityId = entityId;
        ObjectUuid = objectUuid;
        EntityType = entityType;
        X = x;
        Y = y;
        Z = z;
        Pitch = pitch;
        Yaw = yaw;
        HeadPitch = headPitch;
        ObjectData = objectData;
        VelocityX = velocityX;
        VelocityY = velocityY;
        VelocityZ = velocityZ;
    }

    public int EntityId { get; set; }
    public Uuid ObjectUuid { get; set; }
    public int EntityType { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public sbyte Pitch { get; set; }
    public sbyte Yaw { get; set; }
    public sbyte HeadPitch { get; set; }
    public int ObjectData { get; set; }
    public short VelocityX { get; set; }
    public short VelocityY { get; set; }
    public short VelocityZ { get; set; }
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.CB_Play_SpawnEntity;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        throw new NotImplementedException();
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var objectUuid = buffer.ReadUuid();
        var type = buffer.ReadVarInt();
        var x = buffer.ReadDouble();
        var y = buffer.ReadDouble();
        var z = buffer.ReadDouble();
        var pitch = buffer.ReadSByte();
        var yaw = buffer.ReadSByte();
        sbyte headPitch = 0;
        var objectData = 0;
        if (version.Version.Protocol >= ProtocolVersion.V_1_19)
        {
            headPitch = buffer.ReadSByte();
            objectData = buffer.ReadVarInt();
        }
        else
        {
            objectData = buffer.ReadInt();
        }

        var velocityX = buffer.ReadShort();
        var velocityY = buffer.ReadShort();
        var velocityZ = buffer.ReadShort();

        return new SpawnEntityPacket(entityId, objectUuid, type, x, y, z, pitch, yaw, headPitch, objectData, velocityX,
                                     velocityY,
                                     velocityZ);
    }
}
#pragma warning restore CS1591
