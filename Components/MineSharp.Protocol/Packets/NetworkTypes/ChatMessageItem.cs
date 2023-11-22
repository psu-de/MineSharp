using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.NetworkTypes;

public class ChatMessageItem
{
    public UUID? Sender { get; set; }
    public byte[]? Signature { get; set; }

    public ChatMessageItem(byte[]? signature)
    {
        this.Signature = signature;
    }
    
    public ChatMessageItem(UUID sender, byte[] signature)
    {
        this.Sender = sender;
        this.Signature = signature;
    }

    private ChatMessageItem(UUID? sender, byte[]? signature)
    {
        this.Sender = sender;
        this.Signature = signature;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol == ProtocolVersion.V_1_19_2)
        {
            buffer.WriteUuid(this.Sender!.Value);
            buffer.WriteVarInt(this.Signature!.Length);
            buffer.WriteBytes(this.Signature.AsSpan());
        }
        else
        {
            buffer.WriteVarInt(this.Signature == null ? -1 : 0);
            if (this.Signature != null)
                buffer.WriteBytes(this.Signature.AsSpan());
        }
        
    }

    public static ChatMessageItem Read(PacketBuffer buffer, MinecraftData version)
    {
        UUID? uuid = null;
        byte[]? signature = null;

        if (version.Version.Protocol == ProtocolVersion.V_1_19_2)
        {
            uuid = buffer.ReadUuid();
            signature = new byte[buffer.ReadVarInt()];
            buffer.ReadBytes(signature);
        }
        else
        {
            var length = buffer.ReadVarInt();
            if (length == 0)
            {
                signature = new byte[length];
                buffer.ReadBytes(signature);
            }
        }
        

        return new ChatMessageItem(uuid, signature);
    }
}
