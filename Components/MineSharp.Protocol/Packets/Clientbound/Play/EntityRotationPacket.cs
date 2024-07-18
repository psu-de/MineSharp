using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class EntityRotationPacket : IPacket
{
    public EntityRotationPacket(int entityId, sbyte yaw, sbyte pitch, bool onGround)
    {
        EntityId = entityId;
        Yaw = yaw;
        Pitch = pitch;
        OnGround = onGround;
    }

    public int EntityId { get; set; }
    public sbyte Yaw { get; set; }
    public sbyte Pitch { get; set; }
    public bool OnGround { get; set; }
    public PacketType Type => PacketType.CB_Play_EntityLook;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteSByte(Yaw);
        buffer.WriteSByte(Pitch);
        buffer.WriteBool(OnGround);
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
