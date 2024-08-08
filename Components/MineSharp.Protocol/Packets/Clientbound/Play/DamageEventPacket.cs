using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Damage event packet
/// </summary>
/// <param name="EntityId">The ID of the entity taking damage</param>
/// <param name="SourceTypeId">The type of damage in the minecraft:damage_type registry</param>
/// <param name="SourceCauseId">The ID + 1 of the entity responsible for the damage, if present</param>
/// <param name="SourceDirectId">The ID + 1 of the entity that directly dealt the damage, if present</param>
/// <param name="HasSourcePosition">Indicates the presence of the source position fields</param>
/// <param name="SourcePositionX">The X coordinate of the source position, if present</param>
/// <param name="SourcePositionY">The Y coordinate of the source position, if present</param>
/// <param name="SourcePositionZ">The Z coordinate of the source position, if present</param>
public sealed record DamageEventPacket(
    int EntityId,
    int SourceTypeId,
    int SourceCauseId,
    int SourceDirectId,
    bool HasSourcePosition,
    double? SourcePositionX,
    double? SourcePositionY,
    double? SourcePositionZ) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_DamageEvent;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteVarInt(SourceTypeId);
        buffer.WriteVarInt(SourceCauseId);
        buffer.WriteVarInt(SourceDirectId);
        buffer.WriteBool(HasSourcePosition);
        if (HasSourcePosition)
        {
            buffer.WriteDouble(SourcePositionX.GetValueOrDefault());
            buffer.WriteDouble(SourcePositionY.GetValueOrDefault());
            buffer.WriteDouble(SourcePositionZ.GetValueOrDefault());
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var sourceTypeId = buffer.ReadVarInt();
        var sourceCauseId = buffer.ReadVarInt();
        var sourceDirectId = buffer.ReadVarInt();
        var hasSourcePosition = buffer.ReadBool();
        double? sourcePositionX = null;
        double? sourcePositionY = null;
        double? sourcePositionZ = null;

        if (hasSourcePosition)
        {
            sourcePositionX = buffer.ReadDouble();
            sourcePositionY = buffer.ReadDouble();
            sourcePositionZ = buffer.ReadDouble();
        }

        return new DamageEventPacket(
            entityId,
            sourceTypeId,
            sourceCauseId,
            sourceDirectId,
            hasSourcePosition,
            sourcePositionX,
            sourcePositionY,
            sourcePositionZ);
    }
}
