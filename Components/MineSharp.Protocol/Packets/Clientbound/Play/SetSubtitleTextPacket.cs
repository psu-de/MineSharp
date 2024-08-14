using MineSharp.ChatComponent;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Packet sent by the server to set the subtitle text.
/// </summary>
/// <param name="SubtitleText">The subtitle text to be displayed.</param>
public sealed record SetSubtitleTextPacket(Chat SubtitleText) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_SetTitleSubtitle;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteChatComponent(SubtitleText);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var subtitleText = buffer.ReadChatComponent();
        return new SetSubtitleTextPacket(subtitleText);
    }
}
