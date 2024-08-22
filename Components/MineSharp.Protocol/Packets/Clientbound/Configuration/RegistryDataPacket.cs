using fNbt;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;

/// <summary>
///     Registry data packet
///     See https://wiki.vg/Protocol#Registry_Data
/// </summary>
/// <param name="RegistryData">The registry data</param>
public sealed partial record RegistryDataPacket(NbtCompound RegistryData) : IPacketStatic<RegistryDataPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Configuration_RegistryData;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteNbt(RegistryData);
    }

    /// <inheritdoc />
    public static RegistryDataPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var registryData = buffer.ReadNbtCompound();
        registryData = registryData.NormalizeRegistryDataTopLevelIdentifiers();
        return new RegistryDataPacket(registryData);
    }
}

