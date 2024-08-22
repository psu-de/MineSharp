using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Configuration;
#pragma warning disable CS1591
public sealed partial record FinishConfigurationPacket : IPacketStatic<FinishConfigurationPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Configuration_FinishConfiguration;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    { }

    /// <inheritdoc />
    public static FinishConfigurationPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new FinishConfigurationPacket();
    }
}
#pragma warning restore CS1591
