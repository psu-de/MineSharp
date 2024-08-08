using MineSharp.ChatComponent;
using MineSharp.Core;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
/// Packet for system messages displayed in chat or hotbar
/// See https://wiki.vg/Protocol#System_Chat_Message
/// </summary>
public abstract record SystemChatMessagePacket(Chat Message) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_SystemChat;

    /// <inheritdoc />
    public abstract void Write(PacketBuffer buffer, MinecraftData version);

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return version.Version.Protocol switch
        {
            >= ProtocolVersion.V_1_19_2 => Since192._Read(buffer, version),
            < ProtocolVersion.V_1_19_2 => Before192._Read(buffer, version)
        };
    }

    /// <summary>
    /// <inheritdoc cref="SystemChatMessagePacket"/><br/>
    /// 
    /// Used before Minecraft Java 1.19.2
    /// </summary>
    public sealed record Before192(Chat Message, int ChatType) : SystemChatMessagePacket(Message)
    {
        /// <inheritdoc />
        public override void Write(PacketBuffer buffer, MinecraftData data)
        {
            buffer.WriteChatComponent(Message);
            buffer.WriteVarInt(ChatType);
        }

        internal static Before192 _Read(PacketBuffer buffer, MinecraftData version)
        {
            return new(buffer.ReadChatComponent(), buffer.ReadInt());
        }
    }

    /// <summary>
    /// <inheritdoc cref="SystemChatMessagePacket"/><br/>
    /// 
    /// Used since Minecraft Java 1.19.2
    /// </summary>
    public sealed record Since192(Chat Message, bool IsOverlay) : SystemChatMessagePacket(Message)
    {
        /// <inheritdoc />
        public override void Write(PacketBuffer buffer, MinecraftData version)
        {
            buffer.WriteChatComponent(Message);
            buffer.WriteBool(IsOverlay);
        }

        internal static Since192 _Read(PacketBuffer buffer, MinecraftData version)
        {
            return new(buffer.ReadChatComponent(), buffer.ReadBool());
        }
    }
}
