using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Applies a cooldown period to all items with the given type.
/// </summary>
/// <param name="ItemId">Numeric ID of the item to apply a cooldown to.</param>
/// <param name="CooldownTicks">Number of ticks to apply a cooldown for, or 0 to clear the cooldown.</param>
public sealed record SetCooldownPacket(int ItemId, int CooldownTicks) : IPacketStatic<SetCooldownPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_SetCooldown;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(ItemId);
        buffer.WriteVarInt(CooldownTicks);
    }

    /// <inheritdoc />
    public static SetCooldownPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var itemId = buffer.ReadVarInt();
        var cooldownTicks = buffer.ReadVarInt();

        return new SetCooldownPacket(itemId, cooldownTicks);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
