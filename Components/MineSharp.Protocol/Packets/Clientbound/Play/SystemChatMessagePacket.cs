using MineSharp.ChatComponent;
using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class SystemChatMessagePacket : IPacket
{
    /// <summary>
    ///     Constructor for 1.19.1
    /// </summary>
    /// <param name="content"></param>
    /// <param name="chatType"></param>
    public SystemChatMessagePacket(string content, int chatType)
    {
        Content = content;
        ChatType = chatType;
    }

    /// <summary>
    ///     Constructor for >= 1.19.2
    /// </summary>
    /// <param name="content"></param>
    /// <param name="isOverlay"></param>
    public SystemChatMessagePacket(string content, bool isOverlay)
    {
        Content = content;
        IsOverlay = isOverlay;
    }

    /// <summary>
    ///     Constructor for >= 1.20.3
    /// </summary>
    /// <param name="message"></param>
    /// <param name="isOverlay"></param>
    public SystemChatMessagePacket(Chat message, bool isOverlay)
    {
        Message = message;
        IsOverlay = isOverlay;
    }

    public string? Content { get; set; }
    public Chat? Message { get; set; }
    public int? ChatType { get; set; }
    public bool? IsOverlay { get; set; }
    public PacketType Type => PacketType.CB_Play_SystemChat;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol <= ProtocolVersion.V_1_20_2)
        {
            buffer.WriteString(Content
                               ?? throw new MineSharpPacketVersionException(nameof(Content), version.Version.Protocol));

            if (version.Version.Protocol == ProtocolVersion.V_1_19)
            {
                buffer.WriteVarInt(ChatType
                                   ?? throw new MineSharpPacketVersionException(
                                       nameof(ChatType), version.Version.Protocol));
            }
            else
            {
                buffer.WriteBool(IsOverlay
                                 ?? throw new MineSharpPacketVersionException(
                                     nameof(IsOverlay), version.Version.Protocol));
            }

            return;
        }

        buffer.WriteChatComponent(Message
                                  ?? throw new MineSharpPacketVersionException(
                                      nameof(Message), version.Version.Protocol));
        buffer.WriteBool(IsOverlay
                         ?? throw new MineSharpPacketVersionException(nameof(IsOverlay), version.Version.Protocol));
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol > ProtocolVersion.V_1_20_2)
        {
            return new SystemChatMessagePacket(buffer.ReadChatComponent(), buffer.ReadBool());
        }

        var content = buffer.ReadString();
        return version.Version.Protocol >= ProtocolVersion.V_1_19_2
            ? new(content, buffer.ReadBool())
            : new SystemChatMessagePacket(content, buffer.ReadVarInt());
    }
}
#pragma warning restore CS1591
