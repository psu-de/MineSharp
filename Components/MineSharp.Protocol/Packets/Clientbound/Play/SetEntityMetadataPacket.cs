using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Updates one or more metadata properties for an existing entity.
///     Any properties not included in the Metadata field are left unchanged.
/// </summary>
/// <param name="EntityId">The entity ID</param>
/// <param name="Metadata">The entity metadata</param>
public sealed record SetEntityMetadataPacket(int EntityId, EntityMetadata Metadata) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_EntityMetadata;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(EntityId);
        Metadata.Write(buffer, version);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var metadata = EntityMetadata.Read(buffer, version);

        return new SetEntityMetadataPacket(entityId, metadata);
    }
}
