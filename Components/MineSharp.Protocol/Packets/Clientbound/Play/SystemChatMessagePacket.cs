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
public abstract partial record SystemChatMessagePacket(Chat Message) : IPacketStatic<SystemChatMessagePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_SystemChat;

    /// <inheritdoc />
    public abstract void Write(PacketBuffer buffer, MinecraftData data);

    /// <inheritdoc />
    public static SystemChatMessagePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return data.Version.Protocol switch
        {
            >= ProtocolVersion.V_1_19_1 => Since191._Read(buffer, data),
            < ProtocolVersion.V_1_19_1 => Before191._Read(buffer, data)
        };
    }

    /// <summary>
    /// <inheritdoc cref="SystemChatMessagePacket"/><br/>
    /// 
    /// Used before Minecraft Java 1.19.1
    /// </summary>
    public sealed record Before191(Chat Message, int ChatType) : SystemChatMessagePacket(Message)
    {
        /// <inheritdoc />
        public override void Write(PacketBuffer buffer, MinecraftData data)
        {
            buffer.WriteChatComponent(Message);
            buffer.WriteVarInt(ChatType);
        }

        internal static Before191 _Read(PacketBuffer buffer, MinecraftData data)
        {
            return new(buffer.ReadChatComponent(), buffer.ReadInt());
        }
    }

    /// <summary>
    /// <inheritdoc cref="SystemChatMessagePacket"/><br/>
    /// 
    /// Used since Minecraft Java 1.19.1
    /// </summary>
    public sealed record Since191(Chat Message, bool IsOverlay) : SystemChatMessagePacket(Message)
    {
        /// <inheritdoc />
        public override void Write(PacketBuffer buffer, MinecraftData data)
        {
            buffer.WriteChatComponent(Message);
            buffer.WriteBool(IsOverlay);
        }

        internal static Since191 _Read(PacketBuffer buffer, MinecraftData data)
        {
            return new(buffer.ReadChatComponent(), buffer.ReadBool());
        }
    }
}
