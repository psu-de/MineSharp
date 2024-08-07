using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public class MessageAcknowledgementPacket : IPacket
{
    /**
     * Constructor for >= 1.19.3
     */
    public MessageAcknowledgementPacket(int count)
    {
        Count = count;
    }

    /**
     * Constructor for 1.19.2
     */
    public MessageAcknowledgementPacket(ChatMessageItem[]? previousMessages, ChatMessageItem? lastRejectedMessage)
    {
        PreviousMessages = previousMessages;
        LastRejectedMessage = lastRejectedMessage;
    }

    public int? Count { get; set; }
    public ChatMessageItem[]? PreviousMessages { get; set; }
    public ChatMessageItem? LastRejectedMessage { get; set; }
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.SB_Play_MessageAcknowledgement;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol >= ProtocolVersion.V_1_19_3)
        {
            if (Count == null)
            {
                throw new MineSharpPacketVersionException(nameof(Count), version.Version.Protocol);
            }

            buffer.WriteVarInt(Count.Value);
            return;
        }

        if (PreviousMessages == null)
        {
            throw new MineSharpPacketVersionException(nameof(PreviousMessages), version.Version.Protocol);
        }

        buffer.WriteVarIntArray(PreviousMessages, (buf, val) => val.Write(buf, version));

        var hasLastRejectedMessage = LastRejectedMessage != null;
        buffer.WriteBool(hasLastRejectedMessage);
        if (!hasLastRejectedMessage)
        {
            return;
        }

        LastRejectedMessage!.Write(buffer, version);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        throw new NotImplementedException();
    }
}
#pragma warning restore CS1591
