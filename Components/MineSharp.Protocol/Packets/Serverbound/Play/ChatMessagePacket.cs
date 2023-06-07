using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Protocol.Exceptions;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

public class ChatMessagePacket : IPacket
{
    public static int Id => 0x05;
        
    public string Message { get; set; }
    public V1_19? V1_19Body { get; set; }

    public ChatMessagePacket(string message)
    {
        this.Message = message;
    }

    public ChatMessagePacket(string message, V1_19 body)
    {
        this.Message = message;
        this.V1_19Body = body;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteString(this.Message);

        if (version.Protocol.Version < ProtocolVersion.V_1_19)
            return;

        if (this.V1_19Body == null)
        {
            throw new PacketVersionException($"Expected to field {V1_19Body} set for versions >= 1.19.");
        }

        this.V1_19Body.Write(buffer, version);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        var message = buffer.ReadString();
        if (version.Protocol.Version < ProtocolVersion.V_1_19)
        {
            return new ChatMessagePacket(message);
        }
        
        var body = V1_19.Read(buffer, version);
        return new ChatMessagePacket(message, body);
    }

    public class V1_19
    {
        public long Timestamp { get; set; }
        public long Salt { get; set; }
        public byte[]? Signature { get; set; }
        public bool? SignedPreview { get; set; }
        public MessageItem[]? PreviousMessages { get; set; }
        public MessageItem? LastRejectedMessage { get; set; }
        public int? MessageCount { get; set; }
        public byte[]? Acknowledged { get; set; }

        public V1_19(long timestamp, long salt, byte[]? signature, bool? signedPreview, MessageItem[]? previousMessages, MessageItem? lastRejectedMessage, int? messageCount, byte[]? acknowledged)
        {
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

        public V1_19(long timestamp, long salt, byte[] signature, bool signedPreview)
            : this(timestamp, salt, signature, signedPreview, null, null, null, null)
        { }

        public V1_19(long timestamp, long salt, byte[] signature, bool signedPreview, MessageItem[] previousMessages, MessageItem? lastRejectedMessage)
            : this(timestamp, salt, signature, signedPreview, previousMessages, lastRejectedMessage, null, null)
        { }

        public V1_19(long timestamp, long salt, byte[]? signature, int messageCount, byte[] acknowledged)
            : this(timestamp, salt, signature, null, null, null, messageCount, acknowledged)
        { }

        public void Write(PacketBuffer buffer, MinecraftData version)
        {
            if (version.Protocol.Version < ProtocolVersion.V_1_19)
            {
                throw new InvalidOperationException();
            }
            
            buffer.WriteLong(this.Timestamp);
            buffer.WriteLong(this.Salt);

            if (version.Protocol.Version >= ProtocolVersion.V_1_19_3)
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

            if (version.Protocol.Version != ProtocolVersion.V_1_19_2)
                return;

            if (this.PreviousMessages == null)
                throw new PacketVersionException($"Expected field {this.PreviousMessages} to be set for versions 1.19.2.");
            
            buffer.WriteVarIntArray(this.PreviousMessages, (buf, val) => val.Write(buf));

            var hasLastRejectedMessage = this.LastRejectedMessage != null;
            buffer.WriteBool(hasLastRejectedMessage);

            if (!hasLastRejectedMessage)
                return;
            
            this.LastRejectedMessage!.Write(buffer);
        }

        public static V1_19 Read(PacketBuffer buffer, MinecraftData version)
        {
            if (version.Protocol.Version < ProtocolVersion.V_1_19)
            {
                throw new InvalidOperationException();
            }

            var timestamp = buffer.ReadLong();
            var salt = buffer.ReadLong();
            byte[]? signature;
            bool? signedPreview;
            MessageItem[]? previousMessages;
            MessageItem? lastRejectedMessage;
            int? messageCount;
            byte[]? acknowledged;

            if (version.Protocol.Version >= ProtocolVersion.V_1_19_3)
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
                
                return new V1_19(timestamp, salt, signature, messageCount.Value, acknowledged);
            }
            
            // only 1.19-1.19.2
            signature = new byte[buffer.ReadVarInt()];
            buffer.ReadBytes(signature);
            
            signedPreview = buffer.ReadBool();

            if (version.Protocol.Version != ProtocolVersion.V_1_19_2)
            {
                return new V1_19(timestamp, salt, signature, signedPreview.Value);
            }

            
            previousMessages = buffer.ReadVarIntArray(MessageItem.Read);

            var hasLastRejectedMessage = buffer.ReadBool();
            if (hasLastRejectedMessage)
            {
                lastRejectedMessage = MessageItem.Read(buffer);
            }
            else lastRejectedMessage = null;

            return new V1_19(timestamp, salt, signature, signedPreview.Value, previousMessages, lastRejectedMessage);
        }


        public class MessageItem
        {
            public UUID Sender { get; set; }
            public byte[] Signature { get; set; }

            public MessageItem(UUID sender, byte[] signature)
            {
                this.Sender = sender;
                this.Signature = signature;
            }

            public void Write(PacketBuffer buffer)
            {
                buffer.WriteUuid(this.Sender);
                buffer.WriteVarInt(this.Signature.Length);
                buffer.WriteBytes(this.Signature.AsSpan());
            }

            public static MessageItem Read(PacketBuffer buffer)
            {
                var sender = buffer.ReadUuid();
                var signature = new byte[buffer.ReadVarInt()];
                buffer.ReadBytes(signature);

                return new MessageItem(sender, signature);
            }
        }
    }
}
