using MineSharp.Core.Common.Items;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Packet used to set slots in creative inventory (https://wiki.vg/Protocol#Set_Creative_Mode_Slot)
/// </summary>
/// <param name="SlotIndex">The slot index</param>
/// <param name="Item">The clicked Item</param>
public sealed record SetCreativeSlotPacket(short SlotIndex, Item? Item) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
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
