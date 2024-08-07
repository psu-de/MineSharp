using MineSharp.ChatComponent;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class DisguisedChatMessagePacket : IPacket
{
    public DisguisedChatMessagePacket(Chat message, int chatType, Chat name, Chat? target)
    {
        Message = message;
        ChatType = chatType;
        Name = name;
        Target = target;
    }

    public Chat Message { get; set; }
    public int ChatType { get; set; }
    public Chat Name { get; set; }
    public Chat? Target { get; set; }
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.CB_Play_ProfilelessChat;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteChatComponent(Message);
        buffer.WriteVarInt(ChatType);
        buffer.WriteChatComponent(Name);
        buffer.WriteBool(Target != null);
        if (Target != null)
        {
            buffer.WriteChatComponent(Target);
        }
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new DisguisedChatMessagePacket(
            buffer.ReadChatComponent(),
            buffer.ReadVarInt(),
            buffer.ReadChatComponent(),
            buffer.ReadBool() ? buffer.ReadChatComponent() : null);
    }
}
#pragma warning restore CS1591
