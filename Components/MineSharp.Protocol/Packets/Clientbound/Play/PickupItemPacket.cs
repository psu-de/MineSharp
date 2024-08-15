using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Sent by the server when someone picks up an item lying on the ground.
///     Its sole purpose appears to be the animation of the item flying towards you.
/// </summary>
/// <param name="CollectedEntityId">The ID of the collected entity</param>
/// <param name="CollectorEntityId">The ID of the collector entity</param>
/// <param name="PickupItemCount">The number of items picked up. Seems to be 1 for XP orbs, otherwise the number of items in the stack.</param>
public sealed record PickupItemPacket(int CollectedEntityId, int CollectorEntityId, int PickupItemCount) : IPacketStatic<PickupItemPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_Collect;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(CollectedEntityId);
        buffer.WriteVarInt(CollectorEntityId);
        buffer.WriteVarInt(PickupItemCount);
    }

    /// <inheritdoc />
    public static PickupItemPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var collectedEntityId = buffer.ReadVarInt();
        var collectorEntityId = buffer.ReadVarInt();
        var pickupItemCount = buffer.ReadVarInt();

        return new PickupItemPacket(
            collectedEntityId,
            collectorEntityId,
            pickupItemCount);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
