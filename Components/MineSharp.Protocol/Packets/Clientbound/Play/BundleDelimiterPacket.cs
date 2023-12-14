using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class BundleDelimiterPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_BundleDelimiter;
    
    public void Write(PacketBuffer buffer, MinecraftData version)
    { }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
        => new BundleDelimiterPacket();
}