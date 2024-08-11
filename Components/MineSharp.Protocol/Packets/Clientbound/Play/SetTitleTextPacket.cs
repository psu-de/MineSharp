using MineSharp.ChatComponent;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Packet sent by the server to set the title text in the client.
/// </summary>
/// <param name="TitleText">The title text to be displayed</param>
public sealed record SetTitleTextPacket(Chat TitleText) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_SetTitleText;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteChatComponent(TitleText);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var titleText = buffer.ReadChatComponent();
        return new SetTitleTextPacket(titleText);
    }
}
