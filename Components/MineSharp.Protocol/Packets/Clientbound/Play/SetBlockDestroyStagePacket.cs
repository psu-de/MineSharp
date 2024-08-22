using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Set Block Destroy Stage packet
/// </summary>
/// <param name="EntityId">The ID of the entity breaking the block</param>
/// <param name="Location">Block Position</param>
/// <param name="DestroyStage">0–9 to set it, any other value to remove it</param>
public sealed partial record SetBlockDestroyStagePacket(int EntityId, Position Location, byte DestroyStage) : IPacketStatic<SetBlockDestroyStagePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_BlockBreakAnimation;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WritePosition(Location);
        buffer.WriteByte(DestroyStage);
    }

    /// <inheritdoc />
    public static SetBlockDestroyStagePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var entityId = buffer.ReadVarInt();
        var location = buffer.ReadPosition();
        var destroyStage = buffer.ReadByte();

        return new SetBlockDestroyStagePacket(
            entityId,
            location,
            destroyStage);
    }
}
