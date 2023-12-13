namespace MineSharp.Core.Common.Entities.Attributes;

public class Attribute
{
    public string Key { get; set; }
    public double Base { get; set; }
    
    private Dictionary<UUID, Modifier> modifiers { get; set; }
    
    public Attribute(string key, double @base, Modifier[] modifiers)
    {
        this.Key = key;
        this.Base = @base;
        this.modifiers = modifiers.ToDictionary(x => x.UUID);
    }

    public void AddModifier(Modifier modifier)
    {
        if (!this.modifiers.TryAdd(modifier.UUID, modifier))
            this.modifiers[modifier.UUID] = modifier;
    }

    public void RemoveModifier(UUID uuid)
    {
        this.modifiers.Remove(uuid);
    }
    
    public double Value =>
        this.modifiers.GroupBy(m => m.Value.Operation)
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
