namespace MineSharp.Protocol.Packets.Mappings;

public class MC_1_19_PacketMapping : IPacketMapping
{
    public int[] SupportedVersions { get; } = { 759, 760, 761, 762 };
    
    public Dictionary<string, string> ClientboundMapping { get; } = new Dictionary<string, string>();
    public Dictionary<string, string> ServerboundMapping { get; } = new Dictionary<string, string>();
}
