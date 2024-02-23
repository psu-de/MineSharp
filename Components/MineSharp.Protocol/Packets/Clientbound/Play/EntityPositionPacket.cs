using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class EntityPositionPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_RelEntityMove;

    public int   EntityId { get; set; }
    public short DeltaX   { get; set; }
    public short DeltaY   { get; set; }
    public short DeltaZ   { get; set; }
    public bool  OnGround { get; set; }

    public EntityPositionPacket(int entityId, short deltaX, short deltaY, short deltaZ, bool onGround)
    {
        this.EntityId = entityId;
        this.DeltaX   = deltaX;
        this.DeltaY   = deltaY;
        this.DeltaZ   = deltaZ;
        this.OnGround = onGround;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.EntityId);
        buffer.WriteShort(this.DeltaX);
        buffer.WriteShort(this.DeltaY);
        buffer.WriteShort(this.DeltaZ);
        buffer.WriteBool(this.OnGround);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var deltaX   = buffer.ReadShort();
        var deltaY   = buffer.ReadShort();
        var deltaZ   = buffer.ReadShort();
        var onGround = buffer.ReadBool();

        return new EntityPositionPacket(
            entityId,
            deltaX, deltaY, deltaZ,
            onGround);
    }
}
#pragma warning restore CS1591
