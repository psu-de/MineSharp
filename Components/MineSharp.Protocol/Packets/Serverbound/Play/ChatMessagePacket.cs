using MineSharp.Core;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public sealed record ChatMessagePacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_ChatMessage;

    public ChatMessagePacket(
        string message,
        long timestamp,
        long salt,
        byte[]? signature,
        bool? signedPreview,
        ChatMessageItem[]? previousMessages,
        ChatMessageItem? lastRejectedMessage,
        int? messageCount,
        byte[]? acknowledged)
    {
        Message = message;
        Timestamp = timestamp;
        Salt = salt;
        Signature = signature;
        SignedPreview = signedPreview;
        PreviousMessages = previousMessages;
        LastRejectedMessage = lastRejectedMessage;
        MessageCount = messageCount;
        Acknowledged = acknowledged;

        if (acknowledged != null && acknowledged.Length != 3)
        {
            throw new ArgumentException($"{nameof(acknowledged)} must be 3 bytes long.");
        }
    }

    public ChatMessagePacket(string message, long timestamp, long salt, byte[] signature, bool signedPreview)
        : this(message, timestamp, salt, signature, signedPreview, null, null, null, null)
    { }

    public ChatMessagePacket(string message, long timestamp, long salt, byte[] signature, bool signedPreview,
                             ChatMessageItem[] previousMessages, ChatMessageItem? lastRejectedMessage)
        : this(message, timestamp, salt, signature, signedPreview, previousMessages, lastRejectedMessage, null, null)
    { }

    public ChatMessagePacket(string message, long timestamp, long salt, byte[]? signature, int messageCount,
                             byte[] acknowledged)
        : this(message, timestamp, salt, signature, null, null, null, messageCount, acknowledged)
    { }

    public string Message { get; init; }
    public long Timestamp { get; init; }
    public long Salt { get; init; }
    public byte[]? Signature { get; init; }
    public bool? SignedPreview { get; init; }
    public ChatMessageItem[]? PreviousMessages { get; init; }
    public ChatMessageItem? LastRejectedMessage { get; init; }
    public int? MessageCount { get; init; }
    public byte[]? Acknowledged { get; init; }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(Message);
        buffer.WriteLong(Timestamp);
        buffer.WriteLong(Salt);

        if (version.Version.Protocol >= ProtocolVersion.V_1_19_3)
        {
            var hasSignature = Signature != null;
            buffer.WriteBool(hasSignature);

            if (hasSignature)
            {
                if (Signature!.Length != 256)
                {
                    throw new ArgumentException("signature must exactly be 256 bytes long.");
                }

                buffer.WriteBytes(Signature);
            }

            if (MessageCount == null)
            {
                throw new MineSharpPacketVersionException(nameof(MessageCount), version.Version.Protocol);
            }

            buffer.WriteVarInt(MessageCount.Value);
            buffer.WriteBytes(Acknowledged);
            return;
        }

        // only 1.19-1.19.2
        if (Signature == null)
        {
            throw new MineSharpPacketVersionException(nameof(Signature), version.Version.Protocol);
        }

        if (SignedPreview == null)
        {
            throw new MineSharpPacketVersionException(nameof(SignedPreview), version.Version.Protocol);
        }

        buffer.WriteVarInt(Signature.Length);
        buffer.WriteBytes(Signature);

        buffer.WriteBool(SignedPreview.Value);

        if (version.Version.Protocol != ProtocolVersion.V_1_19_2)
        {
            return;
        }

        if (PreviousMessages == null)
        {
            throw new MineSharpPacketVersionException(nameof(PreviousMessages), version.Version.Protocol);
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

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var message = buffer.ReadString();
        var timestamp = buffer.ReadLong();
        var salt = buffer.ReadLong();
        byte[]? signature;
        bool? signedPreview;
        ChatMessageItem[]? previousMessages;
        ChatMessageItem? lastRejectedMessage;
        int? messageCount;
        byte[]? acknowledged;

        if (version.Version.Protocol >= ProtocolVersion.V_1_19_3)
        {
            var hasSignature = buffer.ReadBool();

            if (hasSignature)
            {
                signature = new byte[256];
                buffer.ReadBytes(signature);
            }
            else
            {
                signature = null;
            }

            messageCount = buffer.ReadVarInt();
            acknowledged = new byte[3];
            buffer.ReadBytes(acknowledged);

            return new ChatMessagePacket(message, timestamp, salt, signature, messageCount.Value, acknowledged);
        }

        // only 1.19-1.19.2
        signature = new byte[buffer.ReadVarInt()];
        buffer.ReadBytes(signature);

        signedPreview = buffer.ReadBool();

        if (version.Version.Protocol != ProtocolVersion.V_1_19_2)
        {
            return new ChatMessagePacket(message, timestamp, salt, signature, signedPreview.Value);
        }


        previousMessages = buffer.ReadVarIntArray(ChatMessageItem.Read);

        var hasLastRejectedMessage = buffer.ReadBool();
        if (hasLastRejectedMessage)
        {
            lastRejectedMessage = ChatMessageItem.Read(buffer);
        }
        else
        {
            lastRejectedMessage = null;
        }

        return new ChatMessagePacket(message, timestamp, salt, signature, signedPreview.Value, previousMessages,
                                     lastRejectedMessage);
    }
}
#pragma warning restore CS1591
