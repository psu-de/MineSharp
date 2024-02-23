using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class SetEntityVelocityPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_EntityVelocity;

    public int   EntityId  { get; set; }
    public short VelocityX { get; set; }
    public short VelocityY { get; set; }
    public short VelocityZ { get; set; }

    public SetEntityVelocityPacket(int entityId, short velocityX, short velocityY, short velocityZ)
    {
        this.EntityId  = entityId;
        this.VelocityX = velocityX;
        this.VelocityY = velocityY;
        this.VelocityZ = velocityZ;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.EntityId);
        buffer.WriteShort(this.VelocityX);
        buffer.WriteShort(this.VelocityY);
        buffer.WriteShort(this.VelocityZ);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId  = buffer.ReadVarInt();
        var velocityX = buffer.ReadShort();
        var velocityY = buffer.ReadShort();
        var velocityZ = buffer.ReadShort();
        return new SetEntityVelocityPacket(entityId, velocityX, velocityY, velocityZ);
    }
}
#pragma warning restore CS1591
