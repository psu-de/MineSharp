using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Acknowledge Configuration packet sent by the client upon receiving a Start Configuration packet from the server.
/// </summary>
public sealed partial record AcknowledgeConfigurationPacket() : IPacketStatic<AcknowledgeConfigurationPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_ConfigurationAcknowledged;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        // No fields to write
    }

    /// <inheritdoc />
    public static AcknowledgeConfigurationPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        // No fields to read
        return new AcknowledgeConfigurationPacket();
    }
}
