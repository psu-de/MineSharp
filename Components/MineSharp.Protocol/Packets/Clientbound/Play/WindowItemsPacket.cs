using MineSharp.Core.Common;
using MineSharp.Core.Common.Items;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class WindowItemsPacket : IPacket
{
    public WindowItemsPacket(byte windowId, int stateId, Item?[] items, Item? selectedItem)
    {
        WindowId = windowId;
        StateId = stateId;
        Items = items;
        SelectedItem = selectedItem;
    }

    public byte WindowId { get; set; }
    public int StateId { get; set; }
    public Item?[] Items { get; set; }
    public Item? SelectedItem { get; set; }
    public PacketType Type => PacketType.CB_Play_WindowItems;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteByte(WindowId);
        buffer.WriteVarInt(StateId);
        buffer.WriteVarIntArray(Items, (buff, item) => buff.WriteOptionalItem(item));
        buffer.WriteOptionalItem(SelectedItem);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new WindowItemsPacket(
            buffer.ReadByte(),
            buffer.ReadVarInt(),
            buffer.ReadVarIntArray(buff => buff.ReadOptionalItem(version)),
            buffer.ReadOptionalItem(version));
    }
}
#pragma warning restore CS1591
