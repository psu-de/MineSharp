using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;


/// <summary>
/// ChatPacket only used for versions <= 1.18.2.
/// It was replaced by multiple packets in 1.19
/// </summary>
public class ChatPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_Chat;
    
    public string Message { get; set; }
    public byte Position { get; set; }
    public UUID Sender { get; set; }
    
    
    public ChatPacket(string message, byte position, UUID sender)
    {
        this.Message = message;
        this.Position = position;
        this.Sender = sender;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(this.Message);
        buffer.WriteByte(this.Position);
        buffer.WriteUuid(this.Sender);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new ChatPacket(
            buffer.ReadString(),
            buffer.ReadByte(),
            buffer.ReadUuid());
    }
}
