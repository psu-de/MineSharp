using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using static MineSharp.Protocol.Packets.Clientbound.Play.LookAtPacket;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Used to rotate the client player to face the given location or entity.
/// </summary>
/// <param name="FeetOrEyes">Values are feet=0, eyes=1. If set to eyes, aims using the head position; otherwise aims using the feet position.</param>
/// <param name="TargetX">X coordinate of the point to face towards.</param>
/// <param name="TargetY">Y coordinate of the point to face towards.</param>
/// <param name="TargetZ">Z coordinate of the point to face towards.</param>
/// <param name="IsEntity">If true, additional information about an entity is provided.</param>
/// <param name="EntityId">The entity to face towards. Only if IsEntity is true.</param>
/// <param name="EntityFeetOrEyes">Whether to look at the entity's eyes or feet. Same values and meanings as FeetOrEyes, just for the entity's head/feet. Only if IsEntity is true.</param>
public sealed record LookAtPacket(LookAtPosition FeetOrEyes, double TargetX, double TargetY, double TargetZ, bool IsEntity, int? EntityId, LookAtPosition? EntityFeetOrEyes) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_FacePlayer;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt((int)FeetOrEyes);
        buffer.WriteDouble(TargetX);
        buffer.WriteDouble(TargetY);
        buffer.WriteDouble(TargetZ);
        buffer.WriteBool(IsEntity);

        if (IsEntity)
        {
            buffer.WriteVarInt(EntityId.GetValueOrDefault());
            buffer.WriteVarInt((int)EntityFeetOrEyes.GetValueOrDefault());
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var feetOrEyes = (LookAtPosition)buffer.ReadVarInt();
        var targetX = buffer.ReadDouble();
        var targetY = buffer.ReadDouble();
        var targetZ = buffer.ReadDouble();
        var isEntity = buffer.ReadBool();

        int? entityId = null;
        LookAtPosition? entityFeetOrEyes = null;

        if (isEntity)
        {
            entityId = buffer.ReadVarInt();
            entityFeetOrEyes = (LookAtPosition)buffer.ReadVarInt();
        }

        return new LookAtPacket(
            feetOrEyes,
            targetX, targetY, targetZ,
            isEntity,
            entityId, entityFeetOrEyes);
    }

    /// <summary>
    ///     Enum representing the position to look at (feet or eyes).
    /// </summary>
    public enum LookAtPosition
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Feet = 0,
        Eyes = 1
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}

