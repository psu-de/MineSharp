using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Configuration;
#pragma warning disable CS1591
public class FinishConfigurationPacket : IPacket
{
    public PacketType Type => PacketType.SB_Configuration_FinishConfiguration;

    public void Write(PacketBuffer buffer, MinecraftData version)
    { }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version) => new FinishConfigurationPacket();
}
#pragma warning restore CS1591
