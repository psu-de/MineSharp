using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
/// <summary>
/// Bundle delimiter packet
/// </summary>
public class BundleDelimiterPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Play_BundleDelimiter;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    { }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
        => new BundleDelimiterPacket();
}
#pragma warning restore CS1591
