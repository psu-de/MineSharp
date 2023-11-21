using fNbt;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;

public class RegistryDataPacket : IPacket
{
    public PacketType Type => PacketType.CB_Configuration_RegistryData;
    
    public NbtCompound RegistryData { get; set; }

    public RegistryDataPacket(NbtCompound registryData)
    {
        this.RegistryData = registryData;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteNbt(this.RegistryData);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new RegistryDataPacket(
            buffer.ReadNbt());
    }
}
