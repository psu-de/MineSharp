using MineSharp.Core.Serialization;

namespace MineSharp.Core.Common.Entities.Attributes;

/// <summary>
///     A modifier for an attribute
/// </summary>
/// <param name="Uuid">The uuid associated with this Modifier. This is a constant value from Minecraft java.</param>
/// <param name="Amount"></param>
/// <param name="Operation"></param>
public sealed record Modifier(Uuid Uuid, double Amount, ModifierOp Operation) : ISerializable<Modifier>
{
    /// <inheritdoc />
    public void Write(PacketBuffer buffer)
    {
        buffer.WriteUuid(Uuid);
        buffer.WriteDouble(Amount);
        buffer.WriteByte((byte)Operation);
    }

    /// <inheritdoc />
    public static Modifier Read(PacketBuffer buffer)
    {
        var uuid = buffer.ReadUuid();
        var amount = buffer.ReadDouble();
        var operation = buffer.ReadByte();

        return new(uuid, amount, (ModifierOp)operation);
    }
}
