using MineSharp.ChatComponent;
using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class SystemChatMessagePacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_SystemChat;

    public string? Content   { get; set; }
    public Chat?   Message   { get; set; }
    public int?    ChatType  { get; set; }
    public bool?   IsOverlay { get; set; }

    /// <summary>
    /// Constructor for 1.19.1
    /// </summary>
    /// <param name="content"></param>
    /// <param name="chatType"></param>
    public SystemChatMessagePacket(string content, int chatType)
    {
        this.Content  = content;
        this.ChatType = chatType;
    }

    /// <summary>
    /// Constructor for >= 1.19.2
    /// </summary>
    /// <param name="content"></param>
    /// <param name="isOverlay"></param>
    public SystemChatMessagePacket(string content, bool isOverlay)
    {
        this.Content   = content;
        this.IsOverlay = isOverlay;
    }
    
    /// <summary>
    /// Constructor for >= 1.20.3
    /// </summary>
    /// <param name="message"></param>
    /// <param name="isOverlay"></param>
    public SystemChatMessagePacket(Chat message, bool isOverlay)
    {
        this.Message = message;
        this.IsOverlay = isOverlay;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol <= ProtocolVersion.V_1_20_2)
        {
            buffer.WriteString(this.Content 
                            ?? throw new InvalidOperationException($"Content must be set for protocol version {version.Version.Protocol}"));

            if (version.Version.Protocol == ProtocolVersion.V_1_19)
            {
                buffer.WriteVarInt(this.ChatType 
                                ?? throw new InvalidOperationException($"ChatType must be set for protocol version {version.Version.Protocol}"));
            }
            else
            {
                buffer.WriteBool(this.IsOverlay 
                              ?? throw new InvalidOperationException($"IsOverlay must be set for protocol version {version.Version.Protocol}"));
            }

            return;
        }
        
        buffer.WriteChatComponent(this.Message 
                               ?? throw new InvalidCastException($"Message must be set for protocol version {version.Version.Protocol}"));
        buffer.WriteBool(this.IsOverlay 
                      ?? throw new InvalidOperationException($"IsOverlay must be set for protocol version {version.Version.Protocol}"));
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol > ProtocolVersion.V_1_20_2)
        {
            return new SystemChatMessagePacket(buffer.ReadChatComponent(), buffer.ReadBool());
        }
        
        var content = buffer.ReadString();
        return version.Version.Protocol >= ProtocolVersion.V_1_19_2 
            ? new SystemChatMessagePacket(content, buffer.ReadBool()) 
            : new SystemChatMessagePacket(content, buffer.ReadVarInt());

    }
}
#pragma warning restore CS1591
