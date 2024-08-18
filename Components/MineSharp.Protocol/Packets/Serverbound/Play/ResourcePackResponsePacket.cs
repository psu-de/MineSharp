using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Resource Pack Response packet
/// </summary>
/// <param name="Uuid">The unique identifier of the resource pack</param>
/// <param name="Result">The result of the resource pack response</param>
public sealed record ResourcePackResponsePacket(Uuid Uuid, ResourcePackResult Result) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_ResourcePackReceive;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteUuid(Uuid);
        buffer.WriteVarInt((int)Result);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var uuid = buffer.ReadUuid();
        var result = (ResourcePackResult)buffer.ReadVarInt();

        return new ResourcePackResponsePacket(uuid, result);
    }
}
