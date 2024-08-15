using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;

/// <summary>
///     Finish configuration packet
///     See https://wiki.vg/Protocol#Finish_Configuration
/// </summary>
public sealed record FinishConfigurationPacket : IPacketStatic<FinishConfigurationPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Configuration_FinishConfiguration;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    { }

    /// <inheritdoc />
    public static FinishConfigurationPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new FinishConfigurationPacket();
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
