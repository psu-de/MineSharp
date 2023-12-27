using MineSharp.Core.Common.Items;
using MineSharp.Core.Common.Recipes;

namespace MineSharp.Data.Recipes;

/// <summary>
/// Provides recipe data.
/// </summary>
public class RecipeProvider
{
    private readonly RecipeData _data;

    internal RecipeProvider(RecipeData data)
    {
        this._data = data;
    }

    /// <summary>
    /// Get a list of recipes for a given Item type.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public Recipe[] GetRecipesForItem(ItemType item)
    {
        if (!this._data.Recipes.TryGetValue(item, out var recipes))
            return Array.Empty<Recipe>();

        return recipes;
    }
    
}
