using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Protocol.Exceptions;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

public class MessageAcknowledgementPacket : IPacket
{
    public static int Id => 0x03;
    
    public int? Count { get; set; }
    public ChatMessagePacket.V1_19.MessageItem[]? PreviousMessages { get; set; }
    public ChatMessagePacket.V1_19.MessageItem? LastRejectedMessage { get; set; }

    /**
     * Constructor for >= 1.19.3
     */
    public MessageAcknowledgementPacket(int count)
    {
        this.Count = count;
    }

    /**
     * Constructor for 1.19.2
     */
    public MessageAcknowledgementPacket(ChatMessagePacket.V1_19.MessageItem[]? previousMessages, ChatMessagePacket.V1_19.MessageItem? lastRejectedMessage)
    {
        this.PreviousMessages = previousMessages;
        this.LastRejectedMessage = lastRejectedMessage;
    }

    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        if (version.Protocol.Version >= ProtocolVersion.V_1_19_3)
        {
            if (Count == null)
            {
                throw new PacketVersionException($"Expected {nameof(Count)} to be set for versions >= 1.19.3");
            }
            
            buffer.WriteVarInt(this.Count.Value);
            return;
        }

        if (PreviousMessages == null)
        {
            throw new PacketVersionException($"Expected {nameof(PreviousMessages)} to be set for versions >= 1.19.3");
        }
        
        buffer.WriteVarIntArray(this.PreviousMessages, (buf, val) => val.Write(buf));

        var hasLastRejectedMessage = this.LastRejectedMessage != null;
        buffer.WriteBool(hasLastRejectedMessage);
        if (!hasLastRejectedMessage)
            return;
        this.LastRejectedMessage!.Write(buffer);
    }
    
    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName) => throw new NotImplementedException();
}
