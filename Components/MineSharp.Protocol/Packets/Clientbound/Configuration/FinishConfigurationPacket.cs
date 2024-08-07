using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;

/// <summary>
///     Finish configuration packet
///     See https://wiki.vg/Protocol#Finish_Configuration
/// </summary>
public class FinishConfigurationPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.CB_Configuration_FinishConfiguration;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    { }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new FinishConfigurationPacket();
    }
}
