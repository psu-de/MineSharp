using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public sealed record EntityActionPacket(int EntityId, EntityActionPacket.EntityAction Action, int JumpBoost) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_EntityAction;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteVarInt((int)Action);
        buffer.WriteVarInt(JumpBoost);
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
        StartSneaking = 0,
        StopSneaking = 1,
        LeaveBed = 2,
        StartSprinting = 3,
        StopSprinting = 4,
        StartJumpWithHorse = 5,
        StopJumpWithHorse = 6,
        OpenVehicleInventory = 7,
        StartFlyingWithElytra = 8
    }
}
#pragma warning restore CS1591
