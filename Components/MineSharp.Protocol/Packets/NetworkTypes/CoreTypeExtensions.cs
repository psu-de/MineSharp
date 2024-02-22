using MineSharp.Core.Common;
using MineSharp.Core.Common.Items;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.NetworkTypes;

internal static class CoreTypeExtensions
{
    public static void WriteOptionalItem(this PacketBuffer buffer, Item? item)
    {
        var present = item != null;
        buffer.WriteBool(present);

        if (!present)
            return;
        
        buffer.WriteVarInt(item!.Info.Id);
        buffer.WriteByte(item!.Count);
        buffer.WriteNbt(item!.Metadata);
    }

    public static Item? ReadOptionalItem(this PacketBuffer buffer, MinecraftData data)
    {
        var present = buffer.ReadBool();

        if (!present)
            return null;

        return new Item(
            data.Items.ById(buffer.ReadVarInt())!,
            buffer.ReadByte(),
            null,
            buffer.ReadNbtCompound());
    }
    
    public static void WriteSlot(this PacketBuffer buffer, Slot slot)
    {
        buffer.WriteShort(slot.SlotIndex);
        buffer.WriteOptionalItem(slot.Item);
    }
    
    public static Slot ReadSlot(this PacketBuffer buffer, MinecraftData data)
    {
        var slotIndex = buffer.ReadShort();
        Item? item = buffer.ReadOptionalItem(data);
        
        return new Slot(item, slotIndex);
    }
}
