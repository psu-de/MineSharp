using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Sent when a player right clicks with a signed book. This tells the client to open the book GUI.
/// </summary>
/// <param name="Hand">The hand used to open the book.</param>
public sealed record OpenBookPacket(PlayerHand Hand) : IPacketStatic<OpenBookPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_OpenBook;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt((int)Hand);
    }

    /// <inheritdoc />
    public static OpenBookPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var hand = (PlayerHand)buffer.ReadVarInt();
        return new OpenBookPacket(hand);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
