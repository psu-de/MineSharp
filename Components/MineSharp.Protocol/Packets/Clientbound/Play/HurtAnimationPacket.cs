using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Hurt Animation packet
/// </summary>
/// <param name="EntityId">The ID of the entity taking damage</param>
/// <param name="Yaw">The direction the damage is coming from in relation to the entity</param>
public sealed record HurtAnimationPacket(int EntityId, float Yaw) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_HurtAnimation;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteFloat(Yaw);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var yaw = buffer.ReadFloat();

        return new HurtAnimationPacket(entityId, yaw);
    }
}
