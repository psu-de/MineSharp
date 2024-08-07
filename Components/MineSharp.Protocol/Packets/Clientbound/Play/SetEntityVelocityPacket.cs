using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class SetEntityVelocityPacket : IPacket
{
    public SetEntityVelocityPacket(int entityId, short velocityX, short velocityY, short velocityZ)
    {
        EntityId = entityId;
        VelocityX = velocityX;
        VelocityY = velocityY;
        VelocityZ = velocityZ;
    }

    public int EntityId { get; set; }
    public short VelocityX { get; set; }
    public short VelocityY { get; set; }
    public short VelocityZ { get; set; }
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.CB_Play_EntityVelocity;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteShort(VelocityX);
        buffer.WriteShort(VelocityY);
        buffer.WriteShort(VelocityZ);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var velocityX = buffer.ReadShort();
        var velocityY = buffer.ReadShort();
        var velocityZ = buffer.ReadShort();
        return new SetEntityVelocityPacket(entityId, velocityX, velocityY, velocityZ);
    }
}
#pragma warning restore CS1591
