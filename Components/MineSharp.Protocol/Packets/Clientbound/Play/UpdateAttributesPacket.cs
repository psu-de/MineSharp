using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using Attribute = MineSharp.Core.Common.Entities.Attributes.Attribute;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public sealed record UpdateAttributesPacket(
    int EntityId,
    Attribute[] Attributes
) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_EntityUpdateAttributes;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteVarIntArray(Attributes, (buffer, attribute) => attribute.Write(buffer));
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var attributes = buffer.ReadVarIntArray(Attribute.Read);

        return new UpdateAttributesPacket(entityId, attributes);
    }
}
#pragma warning restore CS1591
