using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public class ChatCommandPacket : IPacket
{
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

    public string Command { get; set; }
    public long Timestamp { get; set; }
    public long Salt { get; set; }
    public ArgumentSignature[] Signatures { get; set; }
    public bool? SignedPreview { get; set; }
    public ChatMessageItem[]? PreviousMessages { get; set; }
    public ChatMessageItem? LastRejectedMessage { get; set; }
    public int? MessageCount { get; set; }
    public byte[]? Acknowledged { get; set; }
    public PacketType Type => PacketType.SB_Play_ChatCommand;

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

    public class ArgumentSignature
    {
        public byte[] Signature;

        public ArgumentSignature(string argumentName, byte[] signature)
        {
            ArgumentName = argumentName;
            Signature = signature;
        }

        public string ArgumentName { get; set; }

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
    }
}
#pragma warning restore CS1591
