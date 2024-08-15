using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Clear Titles packet
/// </summary>
/// <param name="Reset">Whether to reset the client's current title information</param>
public sealed record ClearTitlesPacket(bool Reset) : IPacketStatic<ClearTitlesPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_ClearTitles;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteBool(Reset);
    }

    /// <inheritdoc />
    public static ClearTitlesPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var reset = buffer.ReadBool();
        return new ClearTitlesPacket(reset);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
