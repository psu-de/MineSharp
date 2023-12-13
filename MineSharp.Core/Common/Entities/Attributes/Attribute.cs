namespace MineSharp.Core.Common.Entities.Attributes;

public class Attribute
{
    public string Key { get; set; }
    public double Base { get; set; }
    
    public Dictionary<UUID, Modifier> Modifiers { get; private set; }
    
    public Attribute(string key, double @base, Modifier[] modifiers)
    {
        this.Key = key;
        this.Base = @base;
        this.Modifiers = modifiers.ToDictionary(x => x.UUID);
    }

    public void AddModifier(Modifier modifier)
    {
        if (!this.Modifiers.TryAdd(modifier.UUID, modifier))
            this.Modifiers[modifier.UUID] = modifier;
    }

    public void RemoveModifier(UUID uuid)
    {
        this.Modifiers.Remove(uuid);
    }
    
    public double Value =>
        this.Modifiers.GroupBy(m => m.Value.Operation)
            .OrderBy(x => x.Key)
            .Aggregate(this.Base, (x, t) =>
            {
                var op = t.Key;
                var modifiers = t.Select(x => x.Value);
                return op switch {
                    ModifierOp.Add => modifiers.Aggregate(x, (y, t) => y += t.Amount),
                    ModifierOp.MultiplyBase => x * (1 + modifiers.Select(x => x.Amount).Sum()),
                    ModifierOp.Multiply => modifiers.Aggregate(x, (y, t) => y *= 1 + t.Amount),
                    _ => throw new NotSupportedException($"Modifier operation {op} not supported")
                };
            });
}
