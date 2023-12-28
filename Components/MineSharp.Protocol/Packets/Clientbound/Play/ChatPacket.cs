using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591

/// <summary>
/// ChatPacket only used for versions &lt;= 1.18.2.
/// It was replaced by multiple packets in 1.19
/// </summary>
public class ChatPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Play_Chat;
    
    /// <summary>
    /// The chat message
    /// </summary>
    public string Message { get; set; }
    
    /// <summary>
    /// The position of the chat message
    /// </summary>
    public byte Position { get; set; }
    
    /// <summary>
    /// The UUID of the message sender
    /// </summary>
    public UUID Sender { get; set; }
    
    /// <summary>
    /// Create a new instance
    /// </summary>
    /// <param name="message"></param>
    /// <param name="position"></param>
    /// <param name="sender"></param>
    public ChatPacket(string message, byte position, UUID sender)
    {
        this.Message = message;
        this.Position = position;
        this.Sender = sender;
    }

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(this.Message);
        buffer.WriteByte(this.Position);
        buffer.WriteUuid(this.Sender);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new ChatPacket(
            buffer.ReadString(),
            buffer.ReadByte(),
            buffer.ReadUuid());
    }
}
#pragma warning restore CS1591