namespace MineSharp.Core.Common.Enchantments;

public class Enchantment
{
    public readonly EnchantmentInfo Info;
    public int Level { get; set; }

    public Enchantment(EnchantmentInfo info, int level)
    {
        this.Info = info;
        this.Level = level;
    }
    
    public override string ToString() => $"Enchantment (Name={this.Info.Name} Id={this.Info.Id} Level={this.Level})";
}
