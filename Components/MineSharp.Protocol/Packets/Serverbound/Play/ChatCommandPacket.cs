using MineSharp.Core;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public sealed record ChatCommandPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_ChatCommand;

    // Here is no non-argument constructor allowed
    // Do not use
#pragma warning disable CS8618
    private ChatCommandPacket()
#pragma warning restore CS8618
    {
    }

    /// <summary>
    ///     Constructor for 1.19 - 1.19.1
    /// </summary>
    /// <param name="command"></param>
    /// <param name="timestamp"></param>
    /// <param name="salt"></param>
    /// <param name="signatures"></param>
    /// <param name="signedPreview"></param>
    public ChatCommandPacket(string command, long timestamp, long salt, ArgumentSignature[] signatures,
                             bool signedPreview)
    {
        Command = command;
        Timestamp = timestamp;
        Salt = salt;
        Signatures = signatures;
        SignedPreview = signedPreview;
    }

    /// <summary>
    ///     Constructor for 1.19.2
    /// </summary>
    /// <param name="command"></param>
    /// <param name="timestamp"></param>
    /// <param name="salt"></param>
    /// <param name="signatures"></param>
    /// <param name="signedPreview"></param>
    /// <param name="previousMessages"></param>
    /// <param name="lastRejectedMessage"></param>
    public ChatCommandPacket(string command, long timestamp, long salt, ArgumentSignature[] signatures,
                             bool signedPreview,
                             ChatMessageItem[] previousMessages, ChatMessageItem? lastRejectedMessage)
    {
        Command = command;
        Timestamp = timestamp;
        Salt = salt;
        Signatures = signatures;
        SignedPreview = signedPreview;
        PreviousMessages = previousMessages;
        LastRejectedMessage = lastRejectedMessage;
    }

    /// <summary>
    ///     Constructor for >= 1.19.3
    /// </summary>
    /// <param name="command"></param>
    /// <param name="timestamp"></param>
    /// <param name="salt"></param>
    /// <param name="signatures"></param>
    /// <param name="messageCount"></param>
    /// <param name="acknowledged"></param>
    public ChatCommandPacket(string command, long timestamp, long salt, ArgumentSignature[] signatures,
                             int messageCount,
                             byte[] acknowledged)
    {
        Command = command;
        Timestamp = timestamp;
        Salt = salt;
        Signatures = signatures;
        MessageCount = messageCount;
        Acknowledged = acknowledged;
    }

    public string Command { get; init; }
    public long Timestamp { get; init; }
    public long Salt { get; init; }
    public ArgumentSignature[] Signatures { get; init; }
    public bool? SignedPreview { get; init; }
    public ChatMessageItem[]? PreviousMessages { get; init; }
    public ChatMessageItem? LastRejectedMessage { get; init; }
    public int? MessageCount { get; init; }
    public byte[]? Acknowledged { get; init; }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(Command);
        buffer.WriteLong(Timestamp);
        buffer.WriteLong(Salt);
        buffer.WriteVarIntArray(Signatures, (buffer, signature) => signature.Write(buffer, version));

        if (ProtocolVersion.IsBetween(version.Version.Protocol, ProtocolVersion.V_1_19, ProtocolVersion.V_1_19_2))
        {
            if (!SignedPreview.HasValue)
            {
                throw new MineSharpPacketVersionException(nameof(SignedPreview), version.Version.Protocol);
            }

            buffer.WriteBool(SignedPreview.Value!);
        }

        if (version.Version.Protocol >= ProtocolVersion.V_1_19_3)
        {
            if (Acknowledged == null)
            {
                throw new MineSharpPacketVersionException(nameof(Acknowledged), version.Version.Protocol);
            }

            if (!MessageCount.HasValue)
            {
                throw new MineSharpPacketVersionException(nameof(MessageCount), version.Version.Protocol);
            }

            buffer.WriteVarInt(MessageCount.Value);
            buffer.WriteBytes(Acknowledged);
            return;
        }

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

    private const int AfterMc1192AcknowledgedLength = 20;

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var command = buffer.ReadString();
        var timestamp = buffer.ReadLong();
        var salt = buffer.ReadLong();
        var signatures = buffer.ReadVarIntArray((buf) => ArgumentSignature.Read(buf, version));

        bool? signedPreview = null;
        if (ProtocolVersion.IsBetween(version.Version.Protocol, ProtocolVersion.V_1_19, ProtocolVersion.V_1_19_2))
        {
            signedPreview = buffer.ReadBool();
        }

        byte[]? acknowledged = null;
        int? messageCount = null;
        if (version.Version.Protocol >= ProtocolVersion.V_1_19_3)
        {
            messageCount = buffer.ReadVarInt();
            acknowledged = buffer.ReadBytes(AfterMc1192AcknowledgedLength);
        }

        ChatMessageItem[]? previousMessages = null;
        ChatMessageItem? lastRejectedMessage = null;
        if (version.Version.Protocol == ProtocolVersion.V_1_19_2)
        {
            previousMessages = buffer.ReadVarIntArray(ChatMessageItem.Read);
            var hasLastRejectedMessage = buffer.ReadBool();
            if (hasLastRejectedMessage)
            {
                lastRejectedMessage = ChatMessageItem.Read(buffer);
            }
        }

        return new ChatCommandPacket
        {
            Command = command,
            Timestamp = timestamp,
            Salt = salt,
            Signatures = signatures,
            SignedPreview = signedPreview,
            PreviousMessages = previousMessages,
            LastRejectedMessage = lastRejectedMessage,
            MessageCount = messageCount,
            Acknowledged = acknowledged
        };
    }

    public sealed record ArgumentSignature(string ArgumentName, byte[] Signature)
    {
        public void Write(PacketBuffer buffer, MinecraftData version)
        {
            buffer.WriteString(ArgumentName);

            if (version.Version.Protocol <= ProtocolVersion.V_1_19_2)
            {
                buffer.WriteVarInt(Signature.Length);
                buffer.WriteBytes(Signature);
            }
            else
            {
                buffer.WriteBytes(Signature);
            }
        }

        private const int AfterMc1192SignatureLength = 256;

        public static ArgumentSignature Read(PacketBuffer buffer, MinecraftData version)
        {
            var argumentName = buffer.ReadString();
            byte[] signature;
            if (version.Version.Protocol <= ProtocolVersion.V_1_19_2)
            {
                var length = buffer.ReadVarInt();
                signature = buffer.ReadBytes(length);
            }
            else
            {
                signature = buffer.ReadBytes(AfterMc1192SignatureLength);
            }
            return new ArgumentSignature(argumentName, signature);
        }
    }
}
#pragma warning restore CS1591
