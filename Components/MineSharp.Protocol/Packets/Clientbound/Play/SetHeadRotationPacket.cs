using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Changes the direction an entity's head is facing.
/// </summary>
/// <param name="EntityId">The entity ID</param>
/// <param name="HeadYaw">New angle, not a delta</param>
public sealed record SetHeadRotationPacket(int EntityId, byte HeadYaw) : IPacketStatic<SetHeadRotationPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_EntityHeadRotation;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteByte(HeadYaw);
    }

    /// <inheritdoc />
    public static SetHeadRotationPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var entityId = buffer.ReadVarInt();
        var headYaw = buffer.ReadByte();

        return new SetHeadRotationPacket(entityId, headYaw);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
