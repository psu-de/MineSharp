using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public class EntityActionPacket : IPacket
{
    public PacketType Type => PacketType.SB_Play_EntityAction;

    public int          EntityId  { get; set; }
    public EntityAction Action    { get; set; }
    public int          JumpBoost { get; set; }

    public EntityActionPacket(int entityId, EntityAction action, int jumpBoost)
    {
        EntityId  = entityId;
        Action    = action;
        JumpBoost = jumpBoost;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.EntityId);
        buffer.WriteVarInt((int)this.Action);
        buffer.WriteVarInt(this.JumpBoost);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new EntityActionPacket(
            buffer.ReadVarInt(),
            (EntityAction)buffer.ReadVarInt(),
            buffer.ReadVarInt());
    }

    public enum EntityAction
    {
        StartSneaking         = 0,
        StopSneaking          = 1,
        LeaveBed              = 2,
        StartSprinting        = 3,
        StopSprinting         = 4,
        StartJumpWithHorse    = 5,
        StopJumpWithHorse     = 6,
        OpenVehicleInventory  = 7,
        StartFlyingWithElytra = 8
    }
}

#pragma warning restore CS1591
