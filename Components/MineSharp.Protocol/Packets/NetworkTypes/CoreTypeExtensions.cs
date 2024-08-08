using MineSharp.Core.Common;
using MineSharp.Core.Common.Items;
using MineSharp.Core.Serialization;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.NetworkTypes;

internal static class CoreTypeExtensions
{
    public static void WriteOptionalItem(this PacketBuffer buffer, Item? item)
    {
        var present = item != null;
        buffer.WriteBool(present);

        if (!present)
        {
            return;
        }

        buffer.WriteVarInt(item!.Info.Id);
        buffer.WriteByte(item!.Count);
        buffer.WriteOptionalNbt(item!.Metadata);
    }

    public static Item? ReadOptionalItem(this PacketBuffer buffer, MinecraftData data)
    {
        var present = buffer.ReadBool();

        if (!present)
        {
            return null;
        }

        return new(
            data.Items.ById(buffer.ReadVarInt())!,
            buffer.ReadByte(),
            null,
            buffer.ReadOptionalNbtCompound());
    }

    public static void WriteSlot(this PacketBuffer buffer, Slot slot)
    {
        buffer.WriteShort(slot.SlotIndex);
        buffer.WriteOptionalItem(slot.Item);
    }

    public static Slot ReadSlot(this PacketBuffer buffer, MinecraftData data)
    {
        var slotIndex = buffer.ReadShort();
        var item = buffer.ReadOptionalItem(data);

        return new(item, slotIndex);
    }
}
