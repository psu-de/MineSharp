using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class UpdateEntityPositionAndRotationPacket : IPacket
{
    public static int Id => 0x2C;
    public int EntityId { get; set; }
    public short DeltaX { get; set; }
    public short DeltaY { get; set; }
    public short DeltaZ { get; set; }
    public sbyte Yaw { get; set; }
    public sbyte Pitch { get; set; }
    public bool OnGround { get; set; }

    public UpdateEntityPositionAndRotationPacket(int entityId, short deltaX, short deltaY, short deltaZ, sbyte yaw, sbyte pitch, bool onGround)
    {
        this.EntityId = entityId;
        this.DeltaX = deltaX;
        this.DeltaY = deltaY;
        this.DeltaZ = deltaZ;
        this.Yaw = yaw;
        this.Pitch = pitch;
        this.OnGround = onGround;
    }

    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteVarInt(this.EntityId);
        buffer.WriteShort(this.DeltaX);
        buffer.WriteShort(this.DeltaY);
        buffer.WriteShort(this.DeltaZ);
        buffer.WriteSByte(this.Yaw);
        buffer.WriteShort(this.Pitch);
        buffer.WriteBool(this.OnGround);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        var entityId = buffer.ReadVarInt();
        var deltaX = buffer.ReadShort();
        var deltaY = buffer.ReadShort();
        var deltaZ = buffer.ReadShort();
        var yaw = buffer.ReadSByte();
        var pitch = buffer.ReadSByte();
        var onGround = buffer.ReadBool();
        
        return new UpdateEntityPositionAndRotationPacket(
            entityId,
            deltaX, deltaY, deltaZ,
            yaw, pitch,
            onGround);
    }
}
