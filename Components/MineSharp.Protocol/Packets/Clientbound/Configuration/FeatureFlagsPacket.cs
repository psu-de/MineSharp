using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;

public class FeatureFlagsPacket : IPacket
{
    public PacketType Type => PacketType.CB_Configuration_FeatureFlags;
    
    public string[] FeatureFlags { get; set; }
    
    public FeatureFlagsPacket(string[] featureFlags)
    {
        this.FeatureFlags = featureFlags;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarIntArray(this.FeatureFlags, (buff, str) => buff.WriteString(str));
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new FeatureFlagsPacket(
            buffer.ReadVarIntArray(buff => buff.ReadString()));
    }
}
