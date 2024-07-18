using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class EntityPositionAndRotationPacket : IPacket
{
    public EntityPositionAndRotationPacket(int entityId, short deltaX, short deltaY, short deltaZ, sbyte yaw,
                                           sbyte pitch, bool onGround)
    {
        EntityId = entityId;
        DeltaX = deltaX;
        DeltaY = deltaY;
        DeltaZ = deltaZ;
        Yaw = yaw;
        Pitch = pitch;
        OnGround = onGround;
    }

    public int EntityId { get; set; }
    public short DeltaX { get; set; }
    public short DeltaY { get; set; }
    public short DeltaZ { get; set; }
    public sbyte Yaw { get; set; }
    public sbyte Pitch { get; set; }
    public bool OnGround { get; set; }
    public PacketType Type => PacketType.CB_Play_EntityMoveLook;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteShort(DeltaX);
        buffer.WriteShort(DeltaY);
        buffer.WriteShort(DeltaZ);
        buffer.WriteSByte(Yaw);
        buffer.WriteShort(Pitch);
        buffer.WriteBool(OnGround);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var deltaX = buffer.ReadShort();
        var deltaY = buffer.ReadShort();
        var deltaZ = buffer.ReadShort();
        var yaw = buffer.ReadSByte();
        var pitch = buffer.ReadSByte();
        var onGround = buffer.ReadBool();

        return new EntityPositionAndRotationPacket(
            entityId,
            deltaX, deltaY, deltaZ,
            yaw, pitch,
            onGround);
    }
}
#pragma warning restore CS1591
