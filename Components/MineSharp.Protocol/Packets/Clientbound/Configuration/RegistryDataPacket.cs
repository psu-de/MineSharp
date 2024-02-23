using fNbt;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;
#pragma warning disable CS1591
/// <summary>
/// Registry data packet
/// </summary>
public class RegistryDataPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Configuration_RegistryData;

    /// <summary>
    /// The registry data
    /// </summary>
    public NbtCompound RegistryData { get; set; }

    /// <summary>
    /// Create a new instance
    /// </summary>
    /// <param name="registryData"></param>
    public RegistryDataPacket(NbtCompound registryData)
    {
        this.RegistryData = registryData;
    }

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteNbt(this.RegistryData);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new RegistryDataPacket(
            buffer.ReadNbtCompound());
    }
}
#pragma warning restore CS1591
