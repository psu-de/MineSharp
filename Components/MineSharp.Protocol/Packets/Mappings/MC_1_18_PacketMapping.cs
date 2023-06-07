namespace MineSharp.Protocol.Packets.Mappings;

internal class MC_1_18_PacketMapping : IPacketMapping
{
    public int[] SupportedVersions { get; } = { 757, 758 };
    
    // Mapping from old clientbound id to new clientbound id
    public Dictionary<string, string> ClientboundMapping { get; } = new Dictionary<string, string>() 
    {
        { "spawn_entity_living", "spawn_entity" },
        { "spawn_entity_painting", "spawn_entity" },
        { "chat", "player_chat" }
    };

    public Dictionary<string, string> ServerboundMapping { get; } = new Dictionary<string, string>()
    {
        { "chat", "chat_message" },
    };
}
