using MineSharp.Core.Common;
using MineSharp.Core.Common.Items;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class WindowItemsPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_WindowItems;

    public byte    WindowId     { get; set; }
    public int     StateId      { get; set; }
    public Item?[] Items        { get; set; }
    public Item?   SelectedItem { get; set; }

    public WindowItemsPacket(byte windowId, int stateId, Item?[] items, Item? selectedItem)
    {
        this.WindowId     = windowId;
        this.StateId      = stateId;
        this.Items        = items;
        this.SelectedItem = selectedItem;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteByte(this.WindowId);
        buffer.WriteVarInt(this.StateId);
        buffer.WriteVarIntArray(this.Items, (buff, item) => buff.WriteOptionalItem(item));
        buffer.WriteOptionalItem(this.SelectedItem);
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
