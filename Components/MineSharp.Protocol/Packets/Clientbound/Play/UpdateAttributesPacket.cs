using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using Attribute = MineSharp.Core.Common.Entities.Attributes.Attribute;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public sealed partial record UpdateAttributesPacket(
    int EntityId,
    Attribute[] Attributes
) : IPacketStatic<UpdateAttributesPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_EntityUpdateAttributes;

    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteVarIntArray(Attributes, (buffer, attribute) => attribute.Write(buffer));
    }

    public static UpdateAttributesPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var entityId = buffer.ReadVarInt();
        var attributes = buffer.ReadVarIntArray(Attribute.Read);

        return new UpdateAttributesPacket(entityId, attributes);
    }
}
#pragma warning restore CS1591
