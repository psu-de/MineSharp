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
public sealed partial record SetEntityMetadataPacket(int EntityId, EntityMetadata Metadata) : IPacketStatic<SetEntityMetadataPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_EntityMetadata;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(EntityId);
        Metadata.Write(buffer, data);
    }

    /// <inheritdoc />
    public static SetEntityMetadataPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var entityId = buffer.ReadVarInt();
        var metadata = EntityMetadata.Read(buffer, data);

        return new SetEntityMetadataPacket(entityId, metadata);
    }
}
