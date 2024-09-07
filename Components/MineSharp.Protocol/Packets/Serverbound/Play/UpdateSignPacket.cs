using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Update Sign packet sent from the client to the server when the "Done" button is pushed after placing a sign.
/// </summary>
/// <param name="Location">Block Coordinates</param>
/// <param name="IsFrontText">Whether the updated text is in front or on the back of the sign</param>
/// <param name="Line1">First line of text in the sign</param>
/// <param name="Line2">Second line of text in the sign</param>
/// <param name="Line3">Third line of text in the sign</param>
/// <param name="Line4">Fourth line of text in the sign</param>
public sealed record UpdateSignPacket(Position Location, bool IsFrontText, string Line1, string Line2, string Line3, string Line4) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_UpdateSign;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WritePosition(Location);
        buffer.WriteBool(IsFrontText);
        buffer.WriteString(Line1);
        buffer.WriteString(Line2);
        buffer.WriteString(Line3);
        buffer.WriteString(Line4);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var location = buffer.ReadPosition();
        var isFrontText = buffer.ReadBool();
        var line1 = buffer.ReadString();
        var line2 = buffer.ReadString();
        var line3 = buffer.ReadString();
        var line4 = buffer.ReadString();

        return new UpdateSignPacket(
            location,
            isFrontText,
            line1, line2, line3, line4);
    }
}
