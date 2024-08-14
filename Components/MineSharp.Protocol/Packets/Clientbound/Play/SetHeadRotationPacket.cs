using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Changes the direction an entity's head is facing.
/// </summary>
/// <param name="EntityId">The entity ID</param>
/// <param name="HeadYaw">New angle, not a delta</param>
public sealed record SetHeadRotationPacket(int EntityId, byte HeadYaw) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_EntityHeadRotation;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteByte(HeadYaw);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var headYaw = buffer.ReadByte();

        return new SetHeadRotationPacket(entityId, headYaw);
    }
}
