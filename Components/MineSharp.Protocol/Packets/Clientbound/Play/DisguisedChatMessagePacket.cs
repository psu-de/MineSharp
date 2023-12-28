using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class DisguisedChatMessagePacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_ProfilelessChat;

    public string Message { get; set; }
    public int ChatType { get; set; }
    public string Name { get; set; }
    public string? Target { get; set; }


    public DisguisedChatMessagePacket(string message, int chatType, string name, string? target)
    {
        this.Message = message;
        this.ChatType = chatType;
        this.Name = name;
        this.Target = target;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(this.Message);
        buffer.WriteVarInt(this.ChatType);
        buffer.WriteString(this.Name);
        buffer.WriteBool(this.Target != null);
        if (this.Target != null)
            buffer.WriteString(this.Target);
    }
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new DisguisedChatMessagePacket(
            buffer.ReadString(),
            buffer.ReadVarInt(),
            buffer.ReadString(),
            buffer.ReadBool() ? buffer.ReadString() : null);
    }
}
#pragma warning restore CS1591