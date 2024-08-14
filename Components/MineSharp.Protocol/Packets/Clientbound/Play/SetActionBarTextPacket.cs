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
public sealed record SetActionBarTextPacket(Chat ActionBarText) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_ActionBar;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteChatComponent(ActionBarText);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var actionBarText = buffer.ReadChatComponent();
        return new SetActionBarTextPacket(actionBarText);
    }
}
