using MineSharp.ChatComponent;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Packet sent by the server to set the subtitle text.
/// </summary>
/// <param name="SubtitleText">The subtitle text to be displayed.</param>
public sealed record SetSubtitleTextPacket(Chat SubtitleText) : IPacketStatic<SetSubtitleTextPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_SetTitleSubtitle;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteChatComponent(SubtitleText);
    }

    /// <inheritdoc />
    public static SetSubtitleTextPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var subtitleText = buffer.ReadChatComponent();
        return new SetSubtitleTextPacket(subtitleText);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
