using fNbt;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;

/// <summary>
///     Registry data packet
///     See https://wiki.vg/Protocol#Registry_Data
/// </summary>
public class RegistryDataPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Configuration_RegistryData;
    
    /// <summary>
    ///     The registry data
    /// </summary>
    public required NbtCompound RegistryData { get; init; }
    
    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteNbt(RegistryData);
    }
    
    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new RegistryDataPacket() { RegistryData = buffer.ReadNbtCompound() };
    }
}
