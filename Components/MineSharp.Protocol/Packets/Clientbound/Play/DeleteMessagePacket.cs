using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Packet sent by the server to remove a message from the client's chat.
///     This only works for messages with signatures; system messages cannot be deleted with this packet.
/// </summary>
/// <param name="MessageId">The message ID + 1, used for validating message signature.</param>
/// <param name="Signature">The previous message's signature. Always 256 bytes and not length-prefixed.</param>
public sealed record DeleteMessagePacket(int MessageId, byte[]? Signature) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_HideMessage;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(MessageId);
        if (MessageId == 0 && Signature != null)
        {
            buffer.WriteBytes(Signature);
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var messageId = buffer.ReadVarInt();
        byte[]? signature = null;
        if (messageId == 0)
        {
            signature = buffer.ReadBytes(256);
        }

        return new DeleteMessagePacket(messageId, signature);
    }
}
