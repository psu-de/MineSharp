using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class EntityRotationPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_EntityLook;
    
    public int EntityId { get; set; }
    public sbyte Yaw { get; set; }
    public sbyte Pitch { get; set; }
    public bool OnGround { get; set; }

    public EntityRotationPacket(int entityId, sbyte yaw, sbyte pitch, bool onGround)
    {
        this.EntityId = entityId;
        this.Yaw = yaw;
        this.Pitch = pitch;
        this.OnGround = onGround;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.EntityId);
        buffer.WriteSByte(this.Yaw);
        buffer.WriteSByte(this.Pitch);
        buffer.WriteBool(this.OnGround);
    }
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var yaw = buffer.ReadSByte();
        var pitch = buffer.ReadSByte();
        var onGround = buffer.ReadBool();

        return new EntityRotationPacket(entityId, yaw, pitch, onGround);
    }
}
#pragma warning restore CS1591