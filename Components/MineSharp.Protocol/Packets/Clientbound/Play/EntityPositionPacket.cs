using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Entity position packet
/// </summary>
/// <param name="EntityId">The entity ID</param>
/// <param name="DeltaX">The change in X position</param>
/// <param name="DeltaY">The change in Y position</param>
/// <param name="DeltaZ">The change in Z position</param>
/// <param name="OnGround">Whether the entity is on the ground</param>
public sealed record EntityPositionPacket(int EntityId, short DeltaX, short DeltaY, short DeltaZ, bool OnGround) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_RelEntityMove;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteShort(DeltaX);
        buffer.WriteShort(DeltaY);
        buffer.WriteShort(DeltaZ);
        buffer.WriteBool(OnGround);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var deltaX = buffer.ReadShort();
        var deltaY = buffer.ReadShort();
        var deltaZ = buffer.ReadShort();
        var onGround = buffer.ReadBool();

        return new EntityPositionPacket(
            entityId,
            deltaX, deltaY, deltaZ,
            onGround);
    }
}

