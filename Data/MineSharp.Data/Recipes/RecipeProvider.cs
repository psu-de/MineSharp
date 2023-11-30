using MineSharp.Core.Common.Items;
using MineSharp.Core.Common.Recipes;

namespace MineSharp.Data.Recipes;

public class RecipeProvider
{
    private RecipeData _data;

    internal RecipeProvider(RecipeData data)
    {
        this._data = data;
    }

    public Recipe[] GetRecipesForItem(ItemType item)
    {
        if (!this._data.Recipes.TryGetValue(item, out var recipes))
            return Array.Empty<Recipe>();

        return recipes;
    }
    
}
