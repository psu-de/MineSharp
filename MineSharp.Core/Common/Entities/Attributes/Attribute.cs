using System.Diagnostics;
using MineSharp.Core.Serialization;

namespace MineSharp.Core.Common.Entities.Attributes;

/// <summary>
///     Entity Attribute
/// </summary>
public sealed record Attribute : ISerializable<Attribute>
{
    /// <summary>
    ///     Create a new Attribute
    /// </summary>
    /// <param name="key">The name of this Attribute</param>
    /// <param name="base">The base value of this attribute</param>
    /// <param name="modifiers">The modifiers active for this attribute. Indexed by their UUID</param>
    public Attribute(Identifier key, double @base, Modifier[] modifiers)
    {
        Key = key;
        Base = @base;
        Modifiers = modifiers.ToDictionary(x => x.Uuid);
    }

    /// <summary>
    ///     The name of this Attribute
    /// </summary>
    public Identifier Key { get; init; }

    /// <summary>
    ///     The base value of this attribute
    /// </summary>
    public double Base { get; init; }

    /// <summary>
    ///     The modifiers active for this attribute. Indexed by their UUID
    /// </summary>
    public Dictionary<Uuid, Modifier> Modifiers { get; } // must never be set from outside because we do not want uncontrollable Dictionary changes

    /// <summary>
    ///     Calculate the Multiplier of this attribute with all modifiers.
    /// </summary>
    public double Value =>
        Modifiers.GroupBy(m => m.Value.Operation)
                 .OrderBy(x => x.Key)
                 .Aggregate(Base, (x, t) =>
                  {
                      var op = t.Key;
                      var modifiers = t.Select(x => x.Value);
                      return op switch
                      {
                          ModifierOp.Add => modifiers.Aggregate(x, (y, t) => y += t.Amount),
                          ModifierOp.MultiplyBase => x * (1 + modifiers.Select(x => x.Amount).Sum()),
                          ModifierOp.Multiply => modifiers.Aggregate(x, (y, t) => y *= 1 + t.Amount),
                          _ => throw new UnreachableException()
                      };
                  });

    /// <summary>
    ///     Add a new modifier to this attribute
    /// </summary>
    /// <param name="modifier"></param>
    public void AddModifier(Modifier modifier)
    {
        if (!Modifiers.TryAdd(modifier.Uuid, modifier))
        {
            Modifiers[modifier.Uuid] = modifier;
        }
    }

    /// <summary>
    ///     Remove the modifier with the given uuid
    /// </summary>
    /// <param name="uuid"></param>
    public void RemoveModifier(Uuid uuid)
    {
        Modifiers.Remove(uuid);
    }

    /// <inheritdoc/>
    public void Write(PacketBuffer buffer)
    {
        buffer.WriteIdentifier(Key);
        buffer.WriteDouble(Value);
        buffer.WriteVarIntArray(Modifiers.Values, (buffer, modifier) => modifier.Write(buffer));
    }

    /// <inheritdoc/>
    public static Attribute Read(PacketBuffer buffer)
    {
        var key = buffer.ReadIdentifier();
        var value = buffer.ReadDouble();
        var modifiers = buffer.ReadVarIntArray(Modifier.Read);

        return new(key, value, modifiers);
    }
}
