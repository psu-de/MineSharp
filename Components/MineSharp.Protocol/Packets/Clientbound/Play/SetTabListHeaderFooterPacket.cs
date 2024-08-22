using MineSharp.ChatComponent;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Set Tab List Header And Footer packet
/// </summary>
/// <param name="Header">The header text component</param>
/// <param name="Footer">The footer text component</param>
public sealed partial record SetTabListHeaderFooterPacket(Chat Header, Chat Footer) : IPacketStatic<SetTabListHeaderFooterPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_PlayerlistHeader;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteChatComponent(Header);
        buffer.WriteChatComponent(Footer);
    }

    /// <inheritdoc />
    public static SetTabListHeaderFooterPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var header = buffer.ReadChatComponent();
        var footer = buffer.ReadChatComponent();

        return new SetTabListHeaderFooterPacket(header, footer);
    }
}
