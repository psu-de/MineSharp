using MineSharp.Core.Common.Items;

namespace MineSharp.Core.Common.Recipes;

public class Recipe
{
    /// <summary>
    /// Ingredients in shape (null for no item) from top left to bottom right
    /// </summary>
    public ItemType?[] Ingredients { get; private set; }

    /// <summary>
    /// Leftover items in crafting slots after crafting
    /// </summary>
    public ItemType?[]? OutShape { get; private set; }
        
    /// <summary>
    /// Whether the recipe requires a crafting table.
    /// </summary>
    public bool RequiresCraftingTable { get; private set; }
        
    /// <summary>
    /// Item id of the result
    /// </summary>
    public ItemType Result { get; private set; }

    /// <summary>
    /// Number of result items
    /// </summary>
    public int ResultCount { get; private set; }

    public Dictionary<ItemType, int> IngredientsCount => Ingredients.Aggregate(new Dictionary<ItemType, int>(), (x, y) =>
    {
        if (y == null)
        {
            return x;
        }

        if (!x.TryGetValue(y.Value, out var count))
        {
            x.Add(y.Value, 1);
            return x;
        }

        count += 1;
        x[y.Value] = count;
        return x;

    });

    public Recipe(ItemType?[] ingredients, ItemType?[]? outShape, bool requiresCraftingTable, ItemType result, int count)
    {
        this.Ingredients = ingredients;
        this.OutShape = outShape;
        this.RequiresCraftingTable = requiresCraftingTable;
        this.Result = result;
        this.ResultCount = count;
    }
}
