using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class EntityPositionPacket : IPacket
{
    public EntityPositionPacket(int entityId, short deltaX, short deltaY, short deltaZ, bool onGround)
    {
        EntityId = entityId;
        DeltaX = deltaX;
        DeltaY = deltaY;
        DeltaZ = deltaZ;
        OnGround = onGround;
    }

    public int EntityId { get; set; }
    public short DeltaX { get; set; }
    public short DeltaY { get; set; }
    public short DeltaZ { get; set; }
    public bool OnGround { get; set; }
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.CB_Play_RelEntityMove;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteShort(DeltaX);
        buffer.WriteShort(DeltaY);
        buffer.WriteShort(DeltaZ);
        buffer.WriteBool(OnGround);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var deltaX = buffer.ReadShort();
        var deltaY = buffer.ReadShort();
        var deltaZ = buffer.ReadShort();
        var onGround = buffer.ReadBool();

        return new EntityPositionPacket(
            entityId,
            deltaX, deltaY, deltaZ,
            onGround);
    }
}
#pragma warning restore CS1591
