using MineSharp.Core.Common;
using MineSharp.Core.Common.Items;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Packet used to set slots in creative inventory (https://wiki.vg/Protocol#Set_Creative_Mode_Slot)
/// </summary>
public class SetCreativeSlotPacket : IPacket
{
    /// <summary>
    ///     Constructor
    /// </summary>
    public SetCreativeSlotPacket(short slotIndex, Item? item)
    {
        SlotIndex = slotIndex;
        Item = item;
    }

    /// <summary>
    ///     The inventory slot index
    /// </summary>
    public short SlotIndex { get; set; }

    /// <summary>
    ///     The clicked item
    /// </summary>
    public Item? Item { get; set; }

    /// <inheritdoc />
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.SB_Play_SetCreativeSlot;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteShort(SlotIndex);
        buffer.WriteOptionalItem(Item);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var index = buffer.ReadShort();
        var item = buffer.ReadOptionalItem(version);
        return new SetCreativeSlotPacket(index, item);
    }
}
