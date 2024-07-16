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
    public PacketType Type => PacketType.SB_Play_ChatCommand;

    public string              Command             { get; set; }
    public long                Timestamp           { get; set; }
    public long                Salt                { get; set; }
    public ArgumentSignature[] Signatures          { get; set; }
    public bool?               SignedPreview       { get; set; }
    public ChatMessageItem[]?  PreviousMessages    { get; set; }
    public ChatMessageItem?    LastRejectedMessage { get; set; }
    public int?                MessageCount        { get; set; }
    public byte[]?             Acknowledged        { get; set; }

    /// <summary>
    /// Constructor for 1.19 - 1.19.1 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="timestamp"></param>
    /// <param name="salt"></param>
    /// <param name="signatures"></param>
    /// <param name="signedPreview"></param>
    public ChatCommandPacket(string command, long timestamp, long salt, ArgumentSignature[] signatures, bool signedPreview)
    {
        this.Command       = command;
        this.Timestamp     = timestamp;
        this.Salt          = salt;
        this.Signatures    = signatures;
        this.SignedPreview = signedPreview;
    }

    /// <summary>
    /// Constructor for 1.19.2
    /// </summary>
    /// <param name="command"></param>
    /// <param name="timestamp"></param>
    /// <param name="salt"></param>
    /// <param name="signatures"></param>
    /// <param name="signedPreview"></param>
    /// <param name="previousMessages"></param>
    /// <param name="lastRejectedMessage"></param>
    public ChatCommandPacket(string            command, long timestamp, long salt, ArgumentSignature[] signatures, bool signedPreview,
                             ChatMessageItem[] previousMessages, ChatMessageItem? lastRejectedMessage)
    {
        this.Command             = command;
        this.Timestamp           = timestamp;
        this.Salt                = salt;
        this.Signatures          = signatures;
        this.SignedPreview       = signedPreview;
        this.PreviousMessages    = previousMessages;
        this.LastRejectedMessage = lastRejectedMessage;
    }

    /// <summary>
    /// Constructor for >= 1.19.3
    /// </summary>
    /// <param name="command"></param>
    /// <param name="timestamp"></param>
    /// <param name="salt"></param>
    /// <param name="signatures"></param>
    /// <param name="messageCount"></param>
    /// <param name="acknowledged"></param>
    public ChatCommandPacket(string command, long timestamp, long salt, ArgumentSignature[] signatures, int messageCount,
                             byte[] acknowledged)
    {
        this.Command      = command;
        this.Timestamp    = timestamp;
        this.Salt         = salt;
        this.Signatures   = signatures;
        this.MessageCount = messageCount;
        this.Acknowledged = acknowledged;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(this.Command);
        buffer.WriteLong(this.Timestamp);
        buffer.WriteLong(this.Salt);
        buffer.WriteVarIntArray(this.Signatures, (buffer, signature) => signature.Write(buffer, version));

        if (ProtocolVersion.IsBetween(version.Version.Protocol, ProtocolVersion.V_1_19, ProtocolVersion.V_1_19_2))
        {
            if (!this.SignedPreview.HasValue)
            {
                throw new PacketVersionException($"{nameof(ChatCommandPacket)} must have SignedPreview set for versions 1.19-1.19.2");
            }

            buffer.WriteBool(this.SignedPreview.Value!);
        }

        if (version.Version.Protocol >= ProtocolVersion.V_1_19_3)
        {
            if (this.Acknowledged == null)
            {
                throw new PacketVersionException(
                    $"{nameof(ChatCommandPacket)} must have {nameof(this.Acknowledged)} set for versions >= 1.19.3");
            }

            if (!this.MessageCount.HasValue)
            {
                throw new PacketVersionException(
                    $"{nameof(ChatCommandPacket)} must have {nameof(this.MessageCount)} set for versions >= 1.19.3");
            }

            buffer.WriteVarInt(this.MessageCount.Value);
            buffer.WriteBytes(this.Acknowledged);
            return;
        }

        if (version.Version.Protocol != ProtocolVersion.V_1_19_2)
            return;

        if (this.PreviousMessages == null)
        {
            throw new PacketVersionException(
                $"{nameof(ChatCommandPacket)} must have {nameof(this.PreviousMessages)} set for version 1.19.2");
        }

        buffer.WriteVarIntArray(this.PreviousMessages, (buf, val) => val.Write(buf, version));

        var hasLastRejectedMessage = this.LastRejectedMessage != null;
        buffer.WriteBool(hasLastRejectedMessage);

        if (!hasLastRejectedMessage)
            return;

        this.LastRejectedMessage!.Write(buffer, version);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
        => throw new NotImplementedException();

    public class ArgumentSignature
    {
        public string ArgumentName { get; set; }
        public byte[] Signature;

        public ArgumentSignature(string argumentName, byte[] signature)
        {
            this.ArgumentName = argumentName;
            this.Signature    = signature;
        }

        public void Write(PacketBuffer buffer, MinecraftData version)
        {
            buffer.WriteString(this.ArgumentName);

            if (version.Version.Protocol <= ProtocolVersion.V_1_19_2)
            {
                buffer.WriteVarInt(this.Signature.Length);
                buffer.WriteBytes(this.Signature);
            }
            else
            {
                buffer.WriteBytes(this.Signature);
            }
        }
    }
}
#pragma warning restore CS1591
