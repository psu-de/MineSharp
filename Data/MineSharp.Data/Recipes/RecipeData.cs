using MineSharp.Core.Common.Items;
using MineSharp.Core.Common.Recipes;

namespace MineSharp.Data.Recipes;

internal abstract class RecipeData
{
    public abstract Dictionary<ItemType, Recipe[]> Recipes { get; }
}
