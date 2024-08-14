using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Sent when a player right clicks with a signed book. This tells the client to open the book GUI.
/// </summary>
/// <param name="Hand">The hand used to open the book.</param>
public sealed record OpenBookPacket(PlayerHand Hand) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_OpenBook;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt((int)Hand);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var hand = (PlayerHand)buffer.ReadVarInt();
        return new OpenBookPacket(hand);
    }
}
