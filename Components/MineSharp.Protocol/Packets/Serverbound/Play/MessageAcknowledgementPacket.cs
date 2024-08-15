using MineSharp.Core;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public sealed record MessageAcknowledgementPacket : IPacketStatic<MessageAcknowledgementPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_MessageAcknowledgement;

    // Here is no non-argument constructor allowed
    // Do not use
    private MessageAcknowledgementPacket()
    {
    }

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

    public int? Count { get; init; }
    public ChatMessageItem[]? PreviousMessages { get; init; }
    public ChatMessageItem? LastRejectedMessage { get; init; }

    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        if (data.Version.Protocol >= ProtocolVersion.V_1_19_3)
        {
            if (Count == null)
            {
                throw new MineSharpPacketVersionException(nameof(Count), data.Version.Protocol);
            }

            buffer.WriteVarInt(Count.Value);
            return;
        }

        if (PreviousMessages == null)
        {
            throw new MineSharpPacketVersionException(nameof(PreviousMessages), data.Version.Protocol);
        }

        buffer.WriteVarIntArray(PreviousMessages, (buf, val) => val.Write(buf));

        var hasLastRejectedMessage = LastRejectedMessage != null;
        buffer.WriteBool(hasLastRejectedMessage);
        if (!hasLastRejectedMessage)
        {
            return;
        }

        LastRejectedMessage!.Write(buffer);
    }

    public static MessageAcknowledgementPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        // TODO: implement this method
        throw new NotImplementedException();
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
#pragma warning restore CS1591
