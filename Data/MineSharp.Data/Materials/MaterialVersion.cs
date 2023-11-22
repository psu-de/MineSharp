using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Items;

namespace MineSharp.Data.Materials;

internal abstract class MaterialVersion
{
    public abstract IDictionary<Material, Dictionary<ItemType, float>> Palette { get; }
}
