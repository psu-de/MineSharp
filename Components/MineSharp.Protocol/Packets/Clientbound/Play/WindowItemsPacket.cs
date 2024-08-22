using MineSharp.Core.Common.Items;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Represents a packet containing window items.
/// </summary>
/// <param name="WindowId">The ID of the window.</param>
/// <param name="StateId">The state ID of the window.</param>
/// <param name="Items">The items in the window.</param>
/// <param name="SelectedItem">The selected item in the window.</param>
public sealed partial record WindowItemsPacket(byte WindowId, int StateId, Item?[] Items, Item? SelectedItem) : IPacketStatic<WindowItemsPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_WindowItems;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteByte(WindowId);
        buffer.WriteVarInt(StateId);
        buffer.WriteVarIntArray(Items, (buff, item) => buff.WriteOptionalItem(item));
        buffer.WriteOptionalItem(SelectedItem);
    }

    /// <inheritdoc />
    public static WindowItemsPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new WindowItemsPacket(
            buffer.ReadByte(),
            buffer.ReadVarInt(),
            buffer.ReadVarIntArray(buff => buff.ReadOptionalItem(data)),
            buffer.ReadOptionalItem(data));
    }
}

