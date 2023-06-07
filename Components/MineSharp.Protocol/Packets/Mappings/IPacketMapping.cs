using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Mappings;

/// <summary>
/// A packet mapping is required when a packet in minecraft-data was renamed,
/// or split into multiple other packets.
/// </summary>
internal interface IPacketMapping
{
    /// <summary>
    /// The supported protocol versions by this mapper.
    /// </summary>
    public int[] SupportedVersions { get; }
    
    /// <summary>
    /// A mapping from old protocol ids to the newer packet ids (Clientbound packets).
    /// Only packets that were renamed in minecraft-data, or split into multiple other packets
    /// must be added. If just the id changed, the PacketMapper will be able to find out the new
    /// id via minecraft-data.
    /// </summary>
    public Dictionary<string, string> ClientboundMapping { get; }
    
    /// <summary>
    /// A mapping from old protocol ids to the newer packet ids (Serverbound packets).
    /// Only packets that were renamed in minecraft-data, or split into multiple other packets
    /// must be added. If just the id changed, the PacketMapper will be able to find out the new
    /// id via minecraft-data.
    /// </summary>
    public Dictionary<string, string> ServerboundMapping { get; }
    
}
