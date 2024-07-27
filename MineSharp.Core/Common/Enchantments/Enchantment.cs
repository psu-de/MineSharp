namespace MineSharp.Core.Common.Enchantments;

/// <summary>
///     An enchantment
/// </summary>
/// <param name="info"></param>
/// <param name="level"></param>
public class Enchantment(EnchantmentInfo info, int level)
{
    /// <summary>
    ///     Descriptor of this enchantment
    /// </summary>
    public readonly EnchantmentInfo Info = info;

    /// <summary>
    ///     The level of this enchantment
    /// </summary>
    public int Level { get; set; } = level;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Enchantment (Name={Info.Name} Id={Info.Id} Level={Level})";
    }
}
