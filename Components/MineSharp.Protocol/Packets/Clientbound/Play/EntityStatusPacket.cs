using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Entity status packet
/// </summary>
/// <param name="EntityId">The entity ID</param>
/// <param name="Status">The status byte</param>
public sealed record EntityStatusPacket(int EntityId, sbyte Status) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_EntityStatus;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteSByte(Status);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var status = buffer.ReadSByte();

        return new EntityStatusPacket(entityId, status);
    }

    // TODO: Add all the meanings of the status
    // not all statuses are valid for all entities
    // https://wiki.vg/Entity_statuses
}
