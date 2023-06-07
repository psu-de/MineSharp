using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class SetEntityVelocityPacket : IPacket
{
    public static int Id => 0x54;

    public int EntityId { get; set; }
    public short VelocityX { get; set; }
    public short VelocityY { get; set; }
    public short VelocityZ { get; set; }

    public SetEntityVelocityPacket(int entityId, short velocityX, short velocityY, short velocityZ)
    {
        this.EntityId = entityId;
        this.VelocityX = velocityX;
        this.VelocityY = velocityY;
        this.VelocityZ = velocityZ;
    }

    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteVarInt(this.EntityId);
        buffer.WriteShort(this.VelocityX);
        buffer.WriteShort(this.VelocityY);
        buffer.WriteShort(this.VelocityZ);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        var entityId = buffer.ReadVarInt();
        var velocityX = buffer.ReadShort();
        var velocityY = buffer.ReadShort();
        var velocityZ = buffer.ReadShort();
        return new SetEntityVelocityPacket(entityId, velocityX, velocityY, velocityZ);
    }
}
