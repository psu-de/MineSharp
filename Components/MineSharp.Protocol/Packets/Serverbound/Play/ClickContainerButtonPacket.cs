using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Used when clicking on window buttons. Until 1.14, this was only used by enchantment tables.
/// </summary>
/// <param name="WindowId">The ID of the window sent by Open Screen.</param>
/// <param name="ButtonId">Meaning depends on window type; see below.</param>
public sealed partial record ClickContainerButtonPacket(byte WindowId, byte ButtonId) : IPacketStatic<ClickContainerButtonPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_EnchantItem;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteByte(WindowId);
        buffer.WriteByte(ButtonId);
    }

    /// <inheritdoc />
    public static ClickContainerButtonPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var windowId = buffer.ReadByte();
        var buttonId = buffer.ReadByte();

        return new(windowId, buttonId);
    }

    // TODO: Add all the meanings of the properties
    // depends on the type of container but we only get the window ID here
    // so we need static methods that have the container type as a parameter
    // https://wiki.vg/index.php?title=Protocol&oldid=19208#Click_Container_Button
}
