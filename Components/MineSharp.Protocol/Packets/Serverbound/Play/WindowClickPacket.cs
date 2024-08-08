using MineSharp.Core.Common;
using MineSharp.Core.Common.Items;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public sealed record WindowClickPacket(
    byte WindowId,
    int StateId,
    short Slot,
    sbyte MouseButton,
    int Mode,
    Slot[] ChangedSlots,
    Item? SelectedItem
) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_WindowClick;

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
            buffer.ReadOptionalItem(version)
        );
    }
}
#pragma warning restore CS1591
