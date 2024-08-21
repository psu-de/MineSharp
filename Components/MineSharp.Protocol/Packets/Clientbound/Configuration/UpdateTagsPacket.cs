using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;

/// <summary>
///     Update Tags (configuration) packet
/// </summary>
/// <param name="Registries">Array of registries with their tags</param>
public sealed partial record UpdateTagsPacket(Registry[] Registries) : IPacketStatic<UpdateTagsPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Configuration_Tags;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarIntArray(Registries, (buffer, registry) => registry.Write(buffer));
    }

    /// <inheritdoc />
    public static UpdateTagsPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var registries = buffer.ReadVarIntArray(Registry.Read);

        return new UpdateTagsPacket(registries);
    }
}
