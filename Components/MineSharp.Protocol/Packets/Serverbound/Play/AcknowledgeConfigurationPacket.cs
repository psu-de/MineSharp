using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Acknowledge Configuration packet sent by the client upon receiving a Start Configuration packet from the server.
/// </summary>
public sealed record AcknowledgeConfigurationPacket() : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_ConfigurationAcknowledged;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        // No fields to write
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        // No fields to read
        return new AcknowledgeConfigurationPacket();
    }
}
