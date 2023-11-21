using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;

public class FinishConfigurationPacket : IPacket
{
    public PacketType Type => PacketType.CB_Configuration_FinishConfiguration;
    
    public void Write(PacketBuffer buffer, MinecraftData version)
    { }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version) => new FinishConfigurationPacket();
}
