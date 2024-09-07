using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Clear Titles packet
/// </summary>
/// <param name="Reset">Whether to reset the client's current title information</param>
public sealed record ClearTitlesPacket(bool Reset) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_ClearTitles;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteBool(Reset);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var reset = buffer.ReadBool();
        return new ClearTitlesPacket(reset);
    }
}
