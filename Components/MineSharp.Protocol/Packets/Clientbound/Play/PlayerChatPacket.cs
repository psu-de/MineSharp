using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using System.Runtime.CompilerServices;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class PlayerChatPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_PlayerChat;

    public IChatMessageBody Body { get; set; }

    public PlayerChatPacket(IChatMessageBody body)
    {
        this.Body = body;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        this.Body.Write(buffer, version);
    }
    
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol == ProtocolVersion.V_1_19)
            return new PlayerChatPacket(V1_19Body.Read(buffer));

        if (version.Version.Protocol >= ProtocolVersion.V_1_19_2)
            return new PlayerChatPacket(V1_19_2_3Body.Read(buffer, version));

        throw new NotImplementedException();
    }

    public interface IChatMessageBody
    {
        void Write(PacketBuffer buffer, MinecraftData data);
    }

    public class V1_19Body : IChatMessageBody
    {
        public string SignedChat { get; set; }
        public string? UnsignedChat { get; set; }
        public int MessageType { get; set; }
        public UUID Sender { get; set; }
        public string SenderName { get; set; }
        public string? SenderTeamName { get; set; }
        public long Timestamp { get; set; }
        public long Salt { get; set; }
        public byte[] Signature { get; set; }

        public V1_19Body(string signedChat, string? unsignedChat, int messageType, UUID sender, string senderName, string? senderTeamName, long timestamp, long salt, byte[] signature)
        {
            this.SignedChat = signedChat;
            this.UnsignedChat = unsignedChat;
            this.MessageType = messageType;
            this.Sender = sender;
            this.SenderName = senderName;
            this.SenderTeamName = senderTeamName;
            this.Timestamp = timestamp;
            this.Salt = salt;
            this.Signature = signature;
        }

        public void Write(PacketBuffer buffer, MinecraftData data)
        {
            buffer.WriteString(this.SignedChat);
            
            bool hasUnsignedChat = this.UnsignedChat != null;
            buffer.WriteBool(hasUnsignedChat);
            if (hasUnsignedChat)
                buffer.WriteString(this.UnsignedChat!);
            
            buffer.WriteVarInt(this.MessageType);
            buffer.WriteUuid(this.Sender);

            var hasTeamName = this.SenderTeamName != null;
            if (hasTeamName)
                buffer.WriteString(this.SenderTeamName!);
            
            buffer.WriteLong(this.Timestamp);
            buffer.WriteLong(this.Salt);
            buffer.WriteVarInt(this.Signature.Length);
            buffer.WriteBytes(this.Signature);
        }

        public static V1_19Body Read(PacketBuffer buffer)
        {
            var signedChat = buffer.ReadString();
            
            var hasUnsignedChat = buffer.ReadBool();
            string? unsignedChat = null;
            if (hasUnsignedChat)
                unsignedChat = buffer.ReadString();

            var messageType = buffer.ReadVarInt();
            var sender = buffer.ReadUuid();
            var senderName = buffer.ReadString();
            
            var hasSenderTeamName = buffer.ReadBool();
            string? senderTeamName = null;
            if (hasSenderTeamName)
                senderTeamName = buffer.ReadString();

            var timestamp = buffer.ReadLong();
            var salt = buffer.ReadLong();
            byte[] signature = new byte[buffer.ReadVarInt()];
            buffer.ReadBytes(signature);

            return new V1_19Body(signedChat, unsignedChat, messageType, sender, senderName, senderTeamName, timestamp, salt, signature);
        }
    }

    public class V1_19_2_3Body : IChatMessageBody
    {
        public byte[]? PreviousSignature { get; set; }
        public UUID Sender { get; set; }
        public int? Index { get; set; }
        public byte[]? Signature { get; set; }
        public string PlainMessage { get; set; }
        public string? FormattedMessage { get; set; }
        public long Timestamp { get; set; }
        public long Salt { get; set; }
        public Serverbound.Play.ChatMessagePacket.MessageItem[] PreviousMessages { get; set; }
        public string? UnsignedContent { get; set; }
        public int FilterType { get; set; }
        public long[]? FilterTypeMask { get; set; }
        public int Type { get; set; }
        public string NetworkName { get; set; }
        public string? NetworkTargetName { get; set; }

        private V1_19_2_3Body(byte[]? previousSignature, UUID sender, int? index, byte[]? signature, string plainMessage, string? formattedMessage, long timestamp, long salt, Serverbound.Play.ChatMessagePacket.MessageItem[] previousMessages, string? unsignedContent, int filterType, long[]? filterTypeMask, int type, string networkName, string? networkTargetName)
        {
            this.PreviousSignature = previousSignature;
            this.Sender = sender;
            this.Index = index;
            this.Signature = signature;
            this.PlainMessage = plainMessage;
            this.FormattedMessage = formattedMessage;
            this.Timestamp = timestamp;
            this.Salt = salt;
            this.PreviousMessages = previousMessages;
            this.UnsignedContent = unsignedContent;
            this.FilterType = filterType;
            this.FilterTypeMask = filterTypeMask;
            this.Type = type;
            this.NetworkName = networkName;
            this.NetworkTargetName = networkTargetName;
        }

        /// <summary>
        /// Constructor for 1.19.2
        /// </summary>
        /// <param name="previousSignature"></param>
        /// <param name="sender"></param>
        /// <param name="signature"></param>
        /// <param name="plainMessage"></param>
        /// <param name="formattedMessage"></param>
        /// <param name="timestamp"></param>
        /// <param name="salt"></param>
        /// <param name="previousMessages"></param>
        /// <param name="unsignedContent"></param>
        /// <param name="filterType"></param>
        /// <param name="filterTypeMask"></param>
        /// <param name="type"></param>
        /// <param name="networkName"></param>
        /// <param name="networkTargetName"></param>
        public V1_19_2_3Body(byte[]? previousSignature, UUID sender, byte[] signature, string plainMessage, string? formattedMessage, long timestamp, long salt, Protocol.Packets.Serverbound.Play.ChatMessagePacket.MessageItem[] previousMessages, string? unsignedContent, int filterType, long[]? filterTypeMask, int type, string networkName, string? networkTargetName)
        {
            this.PreviousSignature = previousSignature;
            this.Sender = sender;
            this.Signature = signature;
            this.PlainMessage = plainMessage;
            this.FormattedMessage = formattedMessage;
            this.Timestamp = timestamp;
            this.Salt = salt;
            this.PreviousMessages = previousMessages;
            this.UnsignedContent = unsignedContent;
            this.FilterType = filterType;
            this.FilterTypeMask = filterTypeMask;
            this.Type = type;
            this.NetworkName = networkName;
            this.NetworkTargetName = networkTargetName;
        }

        /// <summary>
        /// Constructor for 1.19.3
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="index"></param>
        /// <param name="signature"></param>
        /// <param name="plainMessage"></param>
        /// <param name="timestamp"></param>
        /// <param name="salt"></param>
        /// <param name="previousMessages"></param>
        /// <param name="unsignedContent"></param>
        /// <param name="filterType"></param>
        /// <param name="filterTypeMask"></param>
        /// <param name="type"></param>
        /// <param name="networkName"></param>
        /// <param name="networkTargetName"></param>
        public V1_19_2_3Body(
            UUID sender,
            int index,
            byte[]? signature,
            string plainMessage,
            long timestamp,
            long salt,
            Serverbound.Play.ChatMessagePacket.MessageItem[] previousMessages,
            string? unsignedContent,
            int filterType,
            long[]? filterTypeMask,
            int type,
            string networkName,
            string? networkTargetName)
        {
            this.Sender = sender;
            this.Index = index;
            this.Signature = signature;
            this.PlainMessage = plainMessage;
            this.Timestamp = timestamp;
            this.Salt = salt;
            this.PreviousMessages = previousMessages;
            this.UnsignedContent = unsignedContent;
            this.FilterType = filterType;
            this.FilterTypeMask = filterTypeMask;
            this.Type = type;
            this.NetworkName = networkName;
            this.NetworkTargetName = networkTargetName;
        }

        public void Write(PacketBuffer buffer, MinecraftData data)
        {
            if (data.Version.Protocol == ProtocolVersion.V_1_19_2)
            {
                var hasPreviousSignature = this.PreviousSignature != null;
                buffer.WriteBool(hasPreviousSignature);
                if (hasPreviousSignature)
                {
                    buffer.WriteVarInt(this.PreviousSignature!.Length);
                    buffer.WriteBytes(this.PreviousSignature);
                }
            }
            
            buffer.WriteUuid(this.Sender);

            if (data.Version.Protocol == ProtocolVersion.V_1_19_2)
            {
                buffer.WriteVarInt(this.Signature!.Length);
                buffer.WriteBytes(this.Signature);
            }
            else
            {
                var hasSignature = this.Signature != null;
                buffer.WriteBool(hasSignature);
                if (hasSignature)
                    buffer.WriteBytes(this.Signature);
            }
            buffer.WriteString(this.PlainMessage);

            if (data.Version.Protocol == ProtocolVersion.V_1_19_2)
            {
                var hasFormattedMessage = this.FormattedMessage != null;
                buffer.WriteBool(hasFormattedMessage);
                if (hasFormattedMessage)
                    buffer.WriteString(this.FormattedMessage!);
            }

            buffer.WriteLong(this.Timestamp);
            buffer.WriteLong(this.Salt);
            buffer.WriteVarIntArray(this.PreviousMessages, (buf, val) => val.Write(buf));

            var hasUnsignedContent = this.UnsignedContent != null;
            buffer.WriteBool(hasUnsignedContent);
            if (hasUnsignedContent)
                buffer.WriteString(this.UnsignedContent!);

            buffer.WriteVarInt(this.FilterType);
            if (this.FilterType == 2)
                buffer.WriteVarIntArray(this.FilterTypeMask!, (buffer, l) => buffer.WriteLong(l));
            buffer.WriteVarInt(this.Type);
            buffer.WriteString(this.NetworkName);

            bool hasNetworkTargetName = this.NetworkTargetName != null;
            buffer.WriteBool(hasNetworkTargetName);
            if (hasNetworkTargetName)
                buffer.WriteString(this.NetworkTargetName!);
        }

        public static V1_19_2_3Body Read(PacketBuffer buffer, MinecraftData version)
        {
            byte[]? previousSignature = null;
            UUID sender;
            int? index = null;
            byte[]? signature = null;
            string plainMessage;
            string? formattedMessage = null;
            long timestamp;
            long salt;
            Serverbound.Play.ChatMessagePacket.MessageItem[] previousMessages;
            string? unsignedContent;
            int filterType;
            long[]? filterTypeMask = null;
            int type;
            string networkName;
            string? networkTargetName = null;

            if (version.Version.Protocol == ProtocolVersion.V_1_19_2)
            {
                var hasPreviousSignature = buffer.ReadBool();
                if (hasPreviousSignature)
                {
                    previousSignature = new byte[buffer.ReadVarInt()];
                    buffer.ReadBytes(previousSignature);
                }
            }

            sender = buffer.ReadUuid();

            if (version.Version.Protocol == ProtocolVersion.V_1_19_2)
            {
                signature = new byte[buffer.ReadVarInt()];
                buffer.ReadBytes(signature);
            }
            else
            {
                index = buffer.ReadVarInt();
                
                var hasSignature = buffer.ReadBool();
                if (hasSignature)
                {
                    signature = new byte[256];
                    buffer.ReadBytes(signature);
                }
            }
            plainMessage = buffer.ReadString();

            if (version.Version.Protocol == ProtocolVersion.V_1_19_2)
            {
                var hasFormattedMessage = buffer.ReadBool();
                formattedMessage = null;
                if (hasFormattedMessage)
                    formattedMessage = buffer.ReadString();
            }
            
            timestamp = buffer.ReadLong();
            salt = buffer.ReadLong();
            previousMessages = buffer.ReadVarIntArray(Serverbound.Play.ChatMessagePacket.MessageItem.Read);

            var hasUnsignedContent = buffer.ReadBool();
            unsignedContent = null;
            if (hasUnsignedContent)
                unsignedContent = buffer.ReadString();

            filterType = buffer.ReadVarInt();
            filterTypeMask = null;
            if (filterType == 2)
                buffer.ReadVarIntArray(buffer => buffer.ReadLong());
            type = buffer.ReadVarInt();
            networkName = buffer.ReadString();

            bool hasNetworkTargetName = buffer.ReadBool();
            if (hasNetworkTargetName)
                networkTargetName = buffer.ReadString();

            return new V1_19_2_3Body(previousSignature, sender, index, signature, plainMessage, formattedMessage, timestamp, salt, previousMessages, unsignedContent, filterType, filterTypeMask, type, networkName,
                networkTargetName);
        }
    }
}
