using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Sent by the client to indicate that it has performed certain actions: sneaking (crouching), sprinting, exiting a bed, jumping with a horse, and opening a horse's inventory while riding it.
///     
/// Leave bed is only sent when the "Leave Bed" button is clicked on the sleep GUI, not when waking up in the morning.
/// 
/// Open vehicle inventory is only sent when pressing the inventory key (default: E) while on a horse or chest boat — all other methods of opening such an inventory (involving right-clicking or shift-right-clicking it) do not use this packet.
/// </summary>
/// <param name="EntityId">Player ID</param>
/// <param name="Action">The ID of the action</param>
/// <param name="JumpBoost">Only used by the “start jump with horse” action, in which case it ranges from 0 to 100. In all other cases it is 0.</param>
public sealed record EntityActionPacket(int EntityId, EntityActionPacket.EntityAction Action, int JumpBoost) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_EntityAction;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteVarInt((int)Action);
        buffer.WriteVarInt(JumpBoost);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new EntityActionPacket(
            buffer.ReadVarInt(),
            (EntityAction)buffer.ReadVarInt(),
            buffer.ReadVarInt());
    }


#pragma warning disable CS1591
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
#pragma warning restore CS1591
}
