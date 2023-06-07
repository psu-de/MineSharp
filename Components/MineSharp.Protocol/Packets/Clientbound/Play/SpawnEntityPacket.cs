using MineSharp.Core.Common;
using MineSharp.Data;
using System.Diagnostics;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class SpawnEntityPacket : IPacket
{
    private const string PACKET_LIVING_ENTITY = "spawn_entity_living";
    private const string PACKET_PAINTING = "spawn_entity_painting";

    public static int Id => 0x01;

    public int EntityId { get; set; }
    public UUID ObjectUuid { get; set; }
    public int Type { get; set; }
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

    public SpawnEntityPacket(int entityId, UUID objectUuid, int type, double x, double y, double z, sbyte pitch, sbyte yaw, sbyte headPitch, int objectData, short velocityX, short velocityY, short velocityZ)
    {
        this.EntityId = entityId;
        this.ObjectUuid = objectUuid;
        this.Type = type;
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.Pitch = pitch;
        this.Yaw = yaw;
        this.HeadPitch = headPitch;
        this.ObjectData = objectData;
        this.VelocityX = velocityX;
        this.VelocityY = velocityY;
        this.VelocityZ = velocityZ;
    }

    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        throw new NotImplementedException();
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        if (version.Protocol.Version < ProtocolVersion.V_1_19 && packetName == PACKET_PAINTING)
        {
            return ReadBefore119SpawnPainting(buffer, version);
        }

        var entityId = buffer.ReadVarInt();
        var objectUuid = buffer.ReadUuid();
        var type = buffer.ReadVarInt();
        var x = buffer.ReadDouble();
        var y = buffer.ReadDouble();
        var z = buffer.ReadDouble();
        var pitch = buffer.ReadSByte();
        var yaw = buffer.ReadSByte();
        sbyte headPitch = 0;
        int objectData = 0;
        if (version.Protocol.Version >= ProtocolVersion.V_1_19)
        {
            headPitch = buffer.ReadSByte();
            objectData = buffer.ReadVarInt();
        }
        else
        {
            // spawn_entity_living packet does not have object data field.
            if (packetName == PACKET_LIVING_ENTITY)
            {
                headPitch = buffer.ReadSByte();
                // for some reason, for spawn_entity_living yaw and pitch are switched
                (pitch, yaw) = (yaw, pitch);
            }
            else
                objectData = buffer.ReadInt();
        }

        var velocityX = buffer.ReadShort();
        var velocityY = buffer.ReadShort();
        var velocityZ = buffer.ReadShort();

        return new SpawnEntityPacket(entityId, objectUuid, type, x, y, z, pitch, yaw, headPitch, objectData, velocityX, velocityY, velocityZ);
    }

    private static IPacket ReadBefore119SpawnPainting(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var entityUuid = buffer.ReadUuid();
        var motive = buffer.ReadVarInt();
        var location = new Position(buffer.ReadULong());
        var direction = buffer.ReadByte();

        // for some reason the direction enum values got changed
        // from 1.18 to 1.19
        direction = direction switch {
            0 => 3,
            1 => 4,
            2 => 2,
            3 => 5,
            _ => throw new UnreachableException()
        };

        // TODO: What to do with motive? 
        // since 1.19 it seems, the motive is sent in
        // another SetEntityMetadata packet

        var paintingId = version.Entities.GetByName("painting").Id;
        return new SpawnEntityPacket(
            entityId, entityUuid, paintingId, location.X, location.Y, location.Z, 0, 0, 0, direction, 0, 0, 0);
    }
}
