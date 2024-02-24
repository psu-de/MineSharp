using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Core.Common.Entities.Attributes;
using MineSharp.Data.Protocol;
using Attribute = MineSharp.Core.Common.Entities.Attributes.Attribute;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class UpdateAttributesPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_EntityUpdateAttributes;

    public int         EntityId   { get; set; }
    public Attribute[] Attributes { get; set; }

    public UpdateAttributesPacket(int entityId, Attribute[] attributes)
    {
        this.EntityId   = entityId;
        this.Attributes = attributes;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.EntityId);
        buffer.WriteVarIntArray(this.Attributes, this.WriteAttribute);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId   = buffer.ReadVarInt();
        var attributes = buffer.ReadVarIntArray(ReadAttribute);

        return new UpdateAttributesPacket(entityId, attributes);
    }

    private void WriteAttribute(PacketBuffer buffer, Attribute attribute)
    {
        buffer.WriteString(attribute.Key);
        buffer.WriteDouble(attribute.Value);
        buffer.WriteVarIntArray(attribute.Modifiers.Values, this.WriteModifier);
    }

    private void WriteModifier(PacketBuffer buffer, Modifier modifier)
    {
        buffer.WriteUuid(modifier.UUID);
        buffer.WriteDouble(modifier.Amount);
        buffer.WriteByte((byte)modifier.Operation);
    }

    private static Attribute ReadAttribute(PacketBuffer buffer)
    {
        var key       = buffer.ReadString();
        var value     = buffer.ReadDouble();
        var modifiers = buffer.ReadVarIntArray(ReadModifier);

        return new Attribute(key, value, modifiers);
    }

    private static Modifier ReadModifier(PacketBuffer buffer)
    {
        var uuid      = buffer.ReadUuid();
        var amount    = buffer.ReadDouble();
        var operation = buffer.ReadByte();

        return new Modifier(uuid, amount, (ModifierOp)operation);
    }
}
#pragma warning restore CS1591
