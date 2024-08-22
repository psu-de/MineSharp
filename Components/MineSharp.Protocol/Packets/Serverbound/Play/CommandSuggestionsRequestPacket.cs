using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Command Suggestions Request packet
/// </summary>
/// <param name="TransactionId">The id of the transaction</param>
/// <param name="Text">All text behind the cursor without the '/'</param>
public sealed partial record CommandSuggestionsRequestPacket(int TransactionId, string Text) : IPacketStatic<CommandSuggestionsRequestPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_TabComplete;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(TransactionId);
        buffer.WriteString(Text);
    }

    /// <inheritdoc />
    public static CommandSuggestionsRequestPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var transactionId = buffer.ReadVarInt();
        var text = buffer.ReadString();

        return new(transactionId, text);
    }
}
