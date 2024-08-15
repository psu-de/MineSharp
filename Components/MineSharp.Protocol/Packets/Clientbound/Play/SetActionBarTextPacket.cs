using MineSharp.ChatComponent;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Displays a message above the hotbar. Equivalent to System Chat Message with Overlay set to true,
///     except that chat message blocking isn't performed. Used by the Notchian server only to implement the /title command.
/// </summary>
/// <param name="ActionBarText">The text to display in the action bar</param>
public sealed record SetActionBarTextPacket(Chat ActionBarText) : IPacketStatic<SetActionBarTextPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_ActionBar;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteChatComponent(ActionBarText);
    }

    /// <inheritdoc />
    public static SetActionBarTextPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var actionBarText = buffer.ReadChatComponent();
        return new SetActionBarTextPacket(actionBarText);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
