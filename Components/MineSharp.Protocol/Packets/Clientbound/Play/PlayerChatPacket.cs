using MineSharp.ChatComponent;
using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;
using static MineSharp.Protocol.Packets.Clientbound.Play.PlayerChatPacket;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public sealed record PlayerChatPacket(IChatMessageBody Body) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_PlayerChat;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        Body.Write(buffer, version);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol == ProtocolVersion.V_1_19)
        {
            return new PlayerChatPacket(V119Body.Read(buffer));
        }

        if (version.Version.Protocol >= ProtocolVersion.V_1_19_2)
        {
            return new PlayerChatPacket(V11923Body.Read(buffer, version));
        }

        throw new NotImplementedException();
    }

    public interface IChatMessageBody
    {
        void Write(PacketBuffer buffer, MinecraftData version);
    }

    /// <summary>
    /// Represents the body of a chat message in version 1.19.
    /// </summary>
    /// <param name="SignedChat">The signed chat message.</param>
    /// <param name="UnsignedChat">The unsigned chat message, if available.</param>
    /// <param name="MessageType">The type of the message.</param>
    /// <param name="Sender">The UUID of the sender.</param>
    /// <param name="SenderName">The name of the sender.</param>
    /// <param name="SenderTeamName">The name of the sender's team, if available.</param>
    /// <param name="Timestamp">The timestamp of the message.</param>
    /// <param name="Salt">The salt value for the message.</param>
    /// <param name="Signature">The signature of the message.</param>
    public sealed record V119Body(
        Chat SignedChat,
        Chat? UnsignedChat,
        int MessageType,
        Uuid Sender,
        Chat SenderName,
        Chat? SenderTeamName,
        long Timestamp,
        long Salt,
        byte[] Signature
    ) : IChatMessageBody
    {
        /// <inheritdoc />
        public void Write(PacketBuffer buffer, MinecraftData version)
        {
            buffer.WriteChatComponent(SignedChat);

            var hasUnsignedChat = UnsignedChat != null;
            buffer.WriteBool(hasUnsignedChat);
            if (hasUnsignedChat)
            {
                buffer.WriteChatComponent(UnsignedChat!);
            }

            buffer.WriteVarInt(MessageType);
            buffer.WriteUuid(Sender);

            var hasTeamName = SenderTeamName != null;
            buffer.WriteBool(hasTeamName);
            if (hasTeamName)
            {
                buffer.WriteChatComponent(SenderTeamName!);
            }

            buffer.WriteLong(Timestamp);
            buffer.WriteLong(Salt);
            buffer.WriteVarInt(Signature.Length);
            buffer.WriteBytes(Signature);
        }

        /// <summary>
        /// Reads a <see cref="V119Body"/> from the given <see cref="PacketBuffer"/>.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <returns>The read <see cref="V119Body"/>.</returns>
        public static V119Body Read(PacketBuffer buffer)
        {
            var signedChat = buffer.ReadChatComponent();

            var hasUnsignedChat = buffer.ReadBool();
            Chat? unsignedChat = null;
            if (hasUnsignedChat)
            {
                unsignedChat = buffer.ReadChatComponent();
            }

            var messageType = buffer.ReadVarInt();
            var sender = buffer.ReadUuid();
            var senderName = buffer.ReadChatComponent();

            var hasSenderTeamName = buffer.ReadBool();
            Chat? senderTeamName = null;
            if (hasSenderTeamName)
            {
                senderTeamName = buffer.ReadChatComponent();
            }

            var timestamp = buffer.ReadLong();
            var salt = buffer.ReadLong();
            var signature = new byte[buffer.ReadVarInt()];
            buffer.ReadBytes(signature);

            return new V119Body(signedChat, unsignedChat, messageType, sender, senderName, senderTeamName, timestamp, salt, signature);
        }
    }

    public sealed record V11923Body : IChatMessageBody
    {
        // Here is no non-argument constructor allowed
        // Do not use
#pragma warning disable CS8618
        private V11923Body()
#pragma warning restore CS8618
        {
        }

        private V11923Body(byte[]? previousSignature, Uuid sender, int? index, byte[]? signature, string plainMessage,
                           Chat? formattedMessage, long timestamp, long salt, ChatMessageItem[] previousMessages,
                           Chat? unsignedContent, int filterType, long[]? filterTypeMask, int type, Chat networkName,
                           Chat? networkTargetName)
        {
            PreviousSignature = previousSignature;
            Sender = sender;
            Index = index;
            Signature = signature;
            PlainMessage = plainMessage;
            FormattedMessage = formattedMessage;
            Timestamp = timestamp;
            Salt = salt;
            PreviousMessages = previousMessages;
            UnsignedContent = unsignedContent;
            FilterType = filterType;
            FilterTypeMask = filterTypeMask;
            Type = type;
            NetworkName = networkName;
            NetworkTargetName = networkTargetName;
        }

        /// <summary>
        ///     Constructor for 1.19.2
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
        public V11923Body(byte[]? previousSignature, Uuid sender, byte[] signature, string plainMessage,
                          Chat? formattedMessage,
                          long timestamp, long salt, ChatMessageItem[] previousMessages, Chat? unsignedContent,
                          int filterType,
                          long[]? filterTypeMask, int type, Chat networkName, Chat? networkTargetName)
        {
            PreviousSignature = previousSignature;
            Sender = sender;
            Signature = signature;
            PlainMessage = plainMessage;
            FormattedMessage = formattedMessage;
            Timestamp = timestamp;
            Salt = salt;
            PreviousMessages = previousMessages;
            UnsignedContent = unsignedContent;
            FilterType = filterType;
            FilterTypeMask = filterTypeMask;
            Type = type;
            NetworkName = networkName;
            NetworkTargetName = networkTargetName;
        }

        /// <summary>
        ///     Constructor for 1.19.3
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
        public V11923Body(
            Uuid sender,
            int index,
            byte[]? signature,
            string plainMessage,
            long timestamp,
            long salt,
            ChatMessageItem[] previousMessages,
            Chat? unsignedContent,
            int filterType,
            long[]? filterTypeMask,
            int type,
            Chat networkName,
            Chat? networkTargetName)
        {
            Sender = sender;
            Index = index;
            Signature = signature;
            PlainMessage = plainMessage;
            Timestamp = timestamp;
            Salt = salt;
            PreviousMessages = previousMessages;
            UnsignedContent = unsignedContent;
            FilterType = filterType;
            FilterTypeMask = filterTypeMask;
            Type = type;
            NetworkName = networkName;
            NetworkTargetName = networkTargetName;
        }

        public byte[]? PreviousSignature { get; init; }
        public Uuid Sender { get; init; }
        public int? Index { get; init; }
        public byte[]? Signature { get; init; }
        public string PlainMessage { get; init; }
        public Chat? FormattedMessage { get; init; }
        public long Timestamp { get; init; }
        public long Salt { get; init; }
        public ChatMessageItem[] PreviousMessages { get; init; }
        public Chat? UnsignedContent { get; init; }
        public int FilterType { get; init; }
        public long[]? FilterTypeMask { get; init; }
        public int Type { get; init; }
        public Chat NetworkName { get; init; }
        public Chat? NetworkTargetName { get; init; }

        public void Write(PacketBuffer buffer, MinecraftData version)
        {
            if (version.Version.Protocol == ProtocolVersion.V_1_19_2)
            {
                var hasPreviousSignature = PreviousSignature != null;
                buffer.WriteBool(hasPreviousSignature);
                if (hasPreviousSignature)
                {
                    buffer.WriteVarInt(PreviousSignature!.Length);
                    buffer.WriteBytes(PreviousSignature);
                }
            }

            buffer.WriteUuid(Sender);

            if (version.Version.Protocol == ProtocolVersion.V_1_19_2)
            {
                buffer.WriteVarInt(Signature!.Length);
                buffer.WriteBytes(Signature);
            }
            else
            {
                var hasSignature = Signature != null;
                buffer.WriteBool(hasSignature);
                if (hasSignature)
                {
                    buffer.WriteBytes(Signature);
                }
            }

            buffer.WriteString(PlainMessage);

            if (version.Version.Protocol == ProtocolVersion.V_1_19_2)
            {
                var hasFormattedMessage = FormattedMessage != null;
                buffer.WriteBool(hasFormattedMessage);
                if (hasFormattedMessage)
                {
                    buffer.WriteChatComponent(FormattedMessage!);
                }
            }

            buffer.WriteLong(Timestamp);
            buffer.WriteLong(Salt);
            buffer.WriteVarIntArray(PreviousMessages, (buf, val) => val.Write(buf));

            var hasUnsignedContent = UnsignedContent != null;
            buffer.WriteBool(hasUnsignedContent);
            if (hasUnsignedContent)
            {
                buffer.WriteChatComponent(UnsignedContent!);
            }

            buffer.WriteVarInt(FilterType);
            if (FilterType == 2)
            {
                buffer.WriteVarIntArray(FilterTypeMask!, (buffer, l) => buffer.WriteLong(l));
            }

            buffer.WriteVarInt(Type);
            buffer.WriteChatComponent(NetworkName);

            var hasNetworkTargetName = NetworkTargetName != null;
            buffer.WriteBool(hasNetworkTargetName);
            if (hasNetworkTargetName)
            {
                buffer.WriteChatComponent(NetworkTargetName!);
            }
        }

        public static V11923Body Read(PacketBuffer buffer, MinecraftData version)
        {
            byte[]? previousSignature = null;
            Uuid sender;
            int? index = null;
            byte[]? signature = null;
            string plainMessage;
            Chat? formattedMessage = null;
            long timestamp;
            long salt;
            ChatMessageItem[] previousMessages;
            Chat? unsignedContent;
            int filterType;
            long[]? filterTypeMask = null;
            int type;
            Chat networkName;
            Chat? networkTargetName = null;

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
                {
                    formattedMessage = buffer.ReadChatComponent();
                }
            }

            timestamp = buffer.ReadLong();
            salt = buffer.ReadLong();
            previousMessages = buffer.ReadVarIntArray(ChatMessageItem.Read);

            var hasUnsignedContent = buffer.ReadBool();
            unsignedContent = null;
            if (hasUnsignedContent)
            {
                unsignedContent = buffer.ReadChatComponent();
            }

            filterType = buffer.ReadVarInt();
            filterTypeMask = null;
            if (filterType == 2)
            {
                buffer.ReadVarIntArray(buffer => buffer.ReadLong());
            }

            type = buffer.ReadVarInt();
            networkName = buffer.ReadChatComponent();

            var hasNetworkTargetName = buffer.ReadBool();
            if (hasNetworkTargetName)
            {
                networkTargetName = buffer.ReadChatComponent();
            }

            return new(previousSignature, sender, index, signature, plainMessage, formattedMessage, timestamp, salt,
                       previousMessages, unsignedContent, filterType, filterTypeMask, type, networkName,
                       networkTargetName);
        }
    }
}
#pragma warning restore CS1591
