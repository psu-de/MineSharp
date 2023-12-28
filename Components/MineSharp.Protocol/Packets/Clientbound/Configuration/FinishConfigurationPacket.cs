using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;
#pragma warning disable CS1591
/// <summary>
/// Finish configuration packet
/// </summary>
public class FinishConfigurationPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Configuration_FinishConfiguration;
    
    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    { }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version) => new FinishConfigurationPacket();
}
#pragma warning restore CS1591