using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Bundle delimiter packet
/// </summary>
public sealed record BundleDelimiterPacket : IPacketStatic<BundleDelimiterPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_BundleDelimiter;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    { }

    /// <inheritdoc />
    public static BundleDelimiterPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new BundleDelimiterPacket();
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
