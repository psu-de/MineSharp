using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

public class ChatMessagePacket : IPacket
{
    public PacketType Type => PacketType.SB_Play_ChatMessage;

    public string Message { get; set; }
    public long Timestamp { get; set; }
    public long Salt { get; set; }
    public byte[]? Signature { get; set; }
    public bool? SignedPreview { get; set; }
    public ChatMessageItem[]? PreviousMessages { get; set; }
    public ChatMessageItem? LastRejectedMessage { get; set; }
    public int? MessageCount { get; set; }
    public byte[]? Acknowledged { get; set; }

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
        this.Message = message;
        this.Timestamp = timestamp;
        this.Salt = salt;
        this.Signature = signature;
        this.SignedPreview = signedPreview;
        this.PreviousMessages = previousMessages;
        this.LastRejectedMessage = lastRejectedMessage;
        this.MessageCount = messageCount;
        this.Acknowledged = acknowledged;

        if (acknowledged != null && acknowledged.Length != 3)
        {
            throw new ArgumentException($"{nameof(acknowledged)} must be 3 bytes long.");
        }
    }

    public ChatMessagePacket(string message, long timestamp, long salt, byte[] signature, bool signedPreview)
        : this(message, timestamp, salt, signature, signedPreview, null, null, null, null)
    { }

    public ChatMessagePacket(string message, long timestamp, long salt, byte[] signature, bool signedPreview, ChatMessageItem[] previousMessages, ChatMessageItem? lastRejectedMessage)
        : this(message, timestamp, salt, signature, signedPreview, previousMessages, lastRejectedMessage, null, null)
    { }

    public ChatMessagePacket(string message, long timestamp, long salt, byte[]? signature, int messageCount, byte[] acknowledged)
        : this(message, timestamp, salt, signature, null, null, null, messageCount, acknowledged)
    { }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(this.Message);
        buffer.WriteLong(this.Timestamp);
        buffer.WriteLong(this.Salt);

        if (version.Version.Protocol >= ProtocolVersion.V_1_19_3)
        {
            bool hasSignature = this.Signature != null;
            buffer.WriteBool(hasSignature);

            if (hasSignature)
            {
                if (this.Signature!.Length != 256)
                {
                    throw new PacketVersionException("Signature must exactly be 256 bytes long.");
                }

                buffer.WriteBytes(this.Signature);
            }

            if (this.MessageCount == null)
                throw new PacketVersionException($"Expected {nameof(MessageCount)} to be set for versions >= 1.19.3");

            buffer.WriteVarInt(this.MessageCount.Value);
            buffer.WriteBytes(this.Acknowledged);
            return;
        }

        // only 1.19-1.19.2
        if (this.Signature == null)
            throw new PacketVersionException($"Expected field {this.Signature} to be set for versions 1.19-1.19.2.");

        if (this.SignedPreview == null)
            throw new PacketVersionException($"Expected field {this.SignedPreview} to be set for versions 1.19-1.19.2.");

        buffer.WriteVarInt(this.Signature.Length);
        buffer.WriteBytes(this.Signature);

        buffer.WriteBool(this.SignedPreview.Value);

        if (version.Version.Protocol != ProtocolVersion.V_1_19_2)
            return;

        if (this.PreviousMessages == null)
            throw new PacketVersionException($"Expected field {this.PreviousMessages} to be set for versions 1.19.2.");

        buffer.WriteVarIntArray(this.PreviousMessages, (buf, val) => val.Write(buf, version));

        var hasLastRejectedMessage = this.LastRejectedMessage != null;
        buffer.WriteBool(hasLastRejectedMessage);

        if (!hasLastRejectedMessage)
            return;

        this.LastRejectedMessage!.Write(buffer, version);
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
            bool hasSignature = buffer.ReadBool();

            if (hasSignature)
            {
                signature = new byte[256];
                buffer.ReadBytes(signature);
            }
            else signature = null;

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


        previousMessages = buffer.ReadVarIntArray(buff => ChatMessageItem.Read(buff, version));

        var hasLastRejectedMessage = buffer.ReadBool();
        if (hasLastRejectedMessage)
        {
            lastRejectedMessage = ChatMessageItem.Read(buffer, version);
        }
        else lastRejectedMessage = null;

        return new ChatMessagePacket(message, timestamp, salt, signature, signedPreview.Value, previousMessages, lastRejectedMessage);
    }
}
