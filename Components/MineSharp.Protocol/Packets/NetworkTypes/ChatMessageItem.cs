using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.NetworkTypes;

/// <summary>
/// Represents a signed chat message
/// </summary>
public class ChatMessageItem
{
    /// <summary>
    /// Sender of the message
    /// </summary>
    public UUID? Sender { get; set; }
    
    /// <summary>
    /// Signature bytes
    /// </summary>
    public byte[]? Signature { get; set; }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="signature"></param>
    public ChatMessageItem(byte[]? signature)
    {
        this.Signature = signature;
    }
    
    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="signature"></param>
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

    /// <summary>
    /// Serialize the ChatMessageItem into buffer
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="version"></param>
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

    /// <summary>
    /// Deserialize a ChatMessageItem from the buffer
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="version"></param>
    /// <returns></returns>
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
