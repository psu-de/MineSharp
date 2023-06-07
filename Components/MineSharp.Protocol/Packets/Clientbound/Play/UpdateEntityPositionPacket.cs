using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class UpdateEntityPositionPacket : IPacket
{
    public static int Id => 0x2B;
    
    public int EntityId { get; set; }
    public short DeltaX { get; set; }
    public short DeltaY { get; set; }
    public short DeltaZ { get; set; }
    public bool OnGround { get; set; }

    public UpdateEntityPositionPacket(int entityId, short deltaX, short deltaY, short deltaZ, bool onGround)
    {
        this.EntityId = entityId;
        this.DeltaX = deltaX;
        this.DeltaY = deltaY;
        this.DeltaZ = deltaZ;
        this.OnGround = onGround;
    }

    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteVarInt(this.EntityId);
        buffer.WriteShort(this.DeltaX);
        buffer.WriteShort(this.DeltaY);
        buffer.WriteShort(this.DeltaZ);
        buffer.WriteBool(this.OnGround);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        var entityId = buffer.ReadVarInt();
        var deltaX = buffer.ReadShort();
        var deltaY = buffer.ReadShort();
        var deltaZ = buffer.ReadShort();
        var onGround = buffer.ReadBool();
        
        return new UpdateEntityPositionPacket(
            entityId,
            deltaX, deltaY, deltaZ,
            onGround);
    }
}
