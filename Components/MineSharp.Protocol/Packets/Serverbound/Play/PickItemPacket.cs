using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Used to swap out an empty space on the hotbar with the item in the given inventory slot.
///     The Notchian client uses this for pick block functionality (middle click) to retrieve items from the inventory.
///     
///     The server first searches the player's hotbar for an empty slot, starting from the current slot and looping around to the slot before it.
///     If there are no empty slots, it starts a second search from the current slot and finds the first slot that does not contain an enchanted item.
///     If there still are no slots that meet that criteria, then the server uses the currently selected slot.
///     
///     After finding the appropriate slot, the server swaps the items and sends 3 packets:
///     
///         Set Container Slot with window ID set to -2, updating the chosen hotbar slot.
///         Set Container Slot with window ID set to -2, updating the slot where the picked item used to be.
///         Set Held Item, switching to the newly chosen slot.
/// </summary>
/// <param name="SlotToUse">The slot to use</param>
public sealed record PickItemPacket(int SlotToUse) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_PickItem;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(SlotToUse);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var slotToUse = buffer.ReadVarInt();

        return new PickItemPacket(slotToUse);
    }
}
