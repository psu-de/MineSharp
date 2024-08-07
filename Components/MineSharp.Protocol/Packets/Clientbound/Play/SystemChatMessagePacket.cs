using MineSharp.ChatComponent;
using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
/// Packet for system messages displayed in chat or hotbar
/// See https://wiki.vg/Protocol#System_Chat_Message
/// </summary>
public abstract class SystemChatMessagePacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.CB_Play_SystemChat;
    
    /// <summary>
    /// The message
    /// </summary>
    public required Chat Message { get; init; }

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
    public class Before192 : SystemChatMessagePacket
    {
        /// <summary>
        /// Position of the message
        /// 0: chat (chat box), 1: system message (chat box), 2: game info (above hotbar), 3: say command, 4: msg command, 5: team msg command, 6: emote command, 7: tellraw command
        /// </summary>
        public required int ChatType { get; init; }

        /// <inheritdoc />
        public override void Write(PacketBuffer buffer, MinecraftData data)
        {
            buffer.WriteChatComponent(Message);
            buffer.WriteVarInt(ChatType);
        }

        internal static Before192 _Read(PacketBuffer buffer, MinecraftData version)
        {
            return new() { Message = buffer.ReadChatComponent(), ChatType = buffer.ReadInt() };
        }
    }

    /// <summary>
    /// <inheritdoc cref="SystemChatMessagePacket"/><br/>
    /// 
    /// Used since Minecraft Java 1.19.2
    /// </summary>
    public class Since192 : SystemChatMessagePacket
    {
        /// <summary>
        /// If true, the message is displayed on the action bar
        /// </summary>
        public required bool IsOverlay { get; init; }
        
        /// <inheritdoc />
        public override void Write(PacketBuffer buffer, MinecraftData version)
        {
            buffer.WriteChatComponent(Message);
            buffer.WriteBool(IsOverlay);
        }

        internal static Since192 _Read(PacketBuffer buffer, MinecraftData version)
        {
            return new() { Message = buffer.ReadChatComponent(), IsOverlay = buffer.ReadBool() };
        }
    }
}

