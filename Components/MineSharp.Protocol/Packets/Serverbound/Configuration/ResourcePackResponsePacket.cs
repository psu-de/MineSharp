using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Serverbound.Configuration;
/// <summary>
///     Resource pack response packet
/// </summary>
/// <param name="Result">The result of the resource pack response</param>
public sealed partial record ResourcePackResponsePacket(ResourcePackResult Result) : IPacketStatic<ResourcePackResponsePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Configuration_ResourcePackReceive;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt((int)Result);
    }

    /// <inheritdoc />
    public static ResourcePackResponsePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var result = (ResourcePackResult)buffer.ReadVarInt();

        return new ResourcePackResponsePacket(result);
    }
}
