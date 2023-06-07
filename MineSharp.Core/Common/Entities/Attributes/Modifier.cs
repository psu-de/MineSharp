namespace MineSharp.Core.Common.Entities.Attributes;

public class Modifier
{
    public UUID UUID { get; set; }
    public double Amount { get; set; }
    public ModifierOp Operation { get; set; }

    public Modifier(UUID uuid, double amount, ModifierOp operation)
    {
        this.UUID = uuid;
        this.Amount = amount;
        this.Operation = operation;
    }
}
