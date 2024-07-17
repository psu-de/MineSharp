using fNbt;
using MineSharp.ChatComponent;
using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class DisguisedChatMessagePacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_ProfilelessChat;

    public Chat  Message  { get; set; }
    public int   ChatType { get; set; }
    public Chat  Name     { get; set; }
    public Chat? Target   { get; set; }


    public DisguisedChatMessagePacket(Chat message, int chatType, Chat name, Chat? target)
    {
        this.Message  = message;
        this.ChatType = chatType;
        this.Name     = name;
        this.Target   = target;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteChatComponent(this.Message);
        buffer.WriteVarInt(this.ChatType);
        buffer.WriteChatComponent(this.Name);
        buffer.WriteBool(this.Target != null);
        if (this.Target != null)
            buffer.WriteChatComponent(this.Target);
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
