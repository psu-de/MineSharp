using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class UpdateEntityRotationPacket : IPacket
{
    public static int Id => 0x2D;
    
    public int EntityId { get; set; }
    public sbyte Yaw { get; set; }
    public sbyte Pitch { get; set; }
    public bool OnGround { get; set; }

    public UpdateEntityRotationPacket(int entityId, sbyte yaw, sbyte pitch, bool onGround)
    {
        this.EntityId = entityId;
        this.Yaw = yaw;
        this.Pitch = pitch;
        this.OnGround = onGround;
    }

    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteVarInt(this.EntityId);
        buffer.WriteSByte(this.Yaw);
        buffer.WriteSByte(this.Pitch);
        buffer.WriteBool(this.OnGround);
    }
    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        var entityId = buffer.ReadVarInt();
        var yaw = buffer.ReadSByte();
        var pitch = buffer.ReadSByte();
        var onGround = buffer.ReadBool();

        return new UpdateEntityRotationPacket(entityId, yaw, pitch, onGround);
    }
}
