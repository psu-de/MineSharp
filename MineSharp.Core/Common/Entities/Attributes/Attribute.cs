using System.Diagnostics;

namespace MineSharp.Core.Common.Entities.Attributes;

/// <summary>
///     Entity Attribute
/// </summary>
public class Attribute
{
    /// <summary>
    ///     Create a new Attribute
    /// </summary>
    /// <param name="key"></param>
    /// <param name="base"></param>
    /// <param name="modifiers"></param>
    public Attribute(string key, double @base, Modifier[] modifiers)
    {
        Key = key;
        Base = @base;
        Modifiers = modifiers.ToDictionary(x => x.Uuid);
    }

    /// <summary>
    ///     The name of this Attribute
    /// </summary>
    public string Key { get; }

    /// <summary>
    ///     The base value of this attribute
    /// </summary>
    public double Base { get; }

    /// <summary>
    ///     The modifiers active for this attribute. Indexed by their UUID
    /// </summary>
    public Dictionary<Uuid, Modifier> Modifiers { get; }

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
}
