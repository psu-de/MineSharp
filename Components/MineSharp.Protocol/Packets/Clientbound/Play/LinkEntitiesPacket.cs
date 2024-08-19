using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Packet sent when an entity has been leashed to another entity.
/// </summary>
/// <param name="AttachedEntityId">The attached entity's ID</param>
/// <param name="HoldingEntityId">The ID of the entity holding the lead. Set to -1 to detach.</param>
public sealed record LinkEntitiesPacket(int AttachedEntityId, int HoldingEntityId) : IPacketStatic<LinkEntitiesPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_AttachEntity;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteInt(AttachedEntityId);
        buffer.WriteInt(HoldingEntityId);
    }

    /// <inheritdoc />
    public static LinkEntitiesPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var attachedEntityId = buffer.ReadInt();
        var holdingEntityId = buffer.ReadInt();

        return new LinkEntitiesPacket(attachedEntityId, holdingEntityId);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
