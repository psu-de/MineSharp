using MineSharp.Core.Common;
using MineSharp.Core.Common.Items;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public class WindowClickPacket : IPacket
{
    public WindowClickPacket(byte windowId, int stateId, short slot, sbyte mouseButton, int mode, Slot[] changedSlots,
                             Item? selectedItem)
    {
        WindowId = windowId;
        StateId = stateId;
        Slot = slot;
        MouseButton = mouseButton;
        Mode = mode;
        ChangedSlots = changedSlots;
        SelectedItem = selectedItem;
    }

    public byte WindowId { get; set; }
    public int StateId { get; set; }
    public short Slot { get; set; }
    public sbyte MouseButton { get; set; }
    public int Mode { get; set; }
    public Slot[] ChangedSlots { get; set; }
    public Item? SelectedItem { get; set; }
    public PacketType Type => PacketType.SB_Play_WindowClick;


    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteByte(WindowId);
        buffer.WriteVarInt(StateId);
        buffer.WriteShort(Slot);
        buffer.WriteSByte(MouseButton);
        buffer.WriteVarInt(Mode);
        buffer.WriteVarIntArray(ChangedSlots, (buff, slot) => buff.WriteSlot(slot));
        buffer.WriteOptionalItem(SelectedItem);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new WindowClickPacket(
            buffer.ReadByte(),
            buffer.ReadVarInt(),
            buffer.ReadShort(),
            buffer.ReadSByte(),
            buffer.ReadVarInt(),
            buffer.ReadVarIntArray(buff => buff.ReadSlot(version)),
            buffer.ReadOptionalItem(version));
    }
}
#pragma warning restore CS1591
