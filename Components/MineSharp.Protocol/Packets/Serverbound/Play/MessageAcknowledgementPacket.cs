using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

public class MessageAcknowledgementPacket : IPacket
{
    public PacketType Type => PacketType.SB_Play_MessageAcknowledgement;
    
    public int? Count { get; set; }
    public ChatMessagePacket.MessageItem[]? PreviousMessages { get; set; }
    public ChatMessagePacket.MessageItem? LastRejectedMessage { get; set; }

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
    public MessageAcknowledgementPacket(ChatMessagePacket.MessageItem[]? previousMessages, ChatMessagePacket.MessageItem? lastRejectedMessage)
    {
        this.PreviousMessages = previousMessages;
        this.LastRejectedMessage = lastRejectedMessage;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol >= ProtocolVersion.V_1_19_3)
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
    
    public static IPacket Read(PacketBuffer buffer, MinecraftData version) => throw new NotImplementedException();
}
