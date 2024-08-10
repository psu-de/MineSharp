using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Serialization;

namespace MineSharp.Protocol.Packets.NetworkTypes;

/// <summary>
///     Represents a signed chat message
/// </summary>
public class ChatMessageItem : ISerializable<ChatMessageItem>
{
    /// <summary>
    ///     Creates a new instance
    /// </summary>
    /// <param name="signature"></param>
    public ChatMessageItem(byte[]? signature)
    {
        Signature = signature;
    }

    /// <summary>
    ///     Creates a new instance
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="signature"></param>
    public ChatMessageItem(Uuid sender, byte[] signature)
    {
        Sender = sender;
        Signature = signature;
    }

    private ChatMessageItem(Uuid? sender, byte[]? signature)
    {
        Sender = sender;
        Signature = signature;
    }

    /// <summary>
    ///     Sender of the message
    /// </summary>
    public Uuid? Sender { get; set; }

    /// <summary>
    ///     Signature bytes
    /// </summary>
    public byte[]? Signature { get; set; }

    /// <summary>
    ///     Serialize the ChatMessageItem into buffer
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="version"></param>
    public void Write(PacketBuffer buffer)
    {
        if (buffer.ProtocolVersion == ProtocolVersion.V_1_19_2)
        {
            buffer.WriteUuid(Sender!.Value);
            buffer.WriteVarInt(Signature!.Length);
            buffer.WriteBytes(Signature.AsSpan());
        }
        else
        {
            buffer.WriteVarInt(Signature == null ? -1 : 0);
            if (Signature != null)
            {
                buffer.WriteBytes(Signature.AsSpan());
            }
        }
    }

    /// <summary>
    ///     Deserialize a ChatMessageItem from the buffer
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static ChatMessageItem Read(PacketBuffer buffer)
    {
        Uuid? uuid = null;
        byte[]? signature = null;

        if (buffer.ProtocolVersion == ProtocolVersion.V_1_19_2)
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
                signature = new byte[256];
                buffer.ReadBytes(signature);
            }
        }


        return new(uuid, signature);
    }
}
