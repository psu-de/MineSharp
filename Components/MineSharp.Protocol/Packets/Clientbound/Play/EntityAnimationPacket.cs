using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using static MineSharp.Protocol.Packets.Clientbound.Play.EntityAnimationPacket;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Entity animation packet
/// </summary>
/// <param name="EntityId">The entity ID</param>
/// <param name="Animation">The animation ID</param>
public sealed record EntityAnimationPacket(int EntityId, EntityAnimation Animation) : IPacketStatic<EntityAnimationPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_Animation;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteByte((byte)Animation);
    }

    /// <inheritdoc />
    public static EntityAnimationPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var entityId = buffer.ReadVarInt();
        var animation = (EntityAnimation)buffer.ReadByte();

        return new EntityAnimationPacket(entityId, animation);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }

    /// <summary>
    ///     Enum representing different types of entity animations.
    /// </summary>
    public enum EntityAnimation
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        SwingMainArm = 0,
        LeaveBed = 2,
        SwingOffhand = 3,
        CriticalEffect = 4,
        MagicCriticalEffect = 5
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
