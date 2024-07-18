using MineSharp.Core.Common.Items;

namespace MineSharp.Core.Common.Recipes;

/// <summary>
///     Represents a crafting recipe
/// </summary>
/// <param name="ingredients"></param>
/// <param name="outShape"></param>
/// <param name="requiresCraftingTable"></param>
/// <param name="result"></param>
/// <param name="count"></param>
public class Recipe(
    ItemType?[] ingredients,
    ItemType?[]? outShape,
    bool requiresCraftingTable,
    ItemType result,
    int count)
{
    /// <summary>
    ///     Ingredients in shape (null for no item) from top left to bottom right
    /// </summary>
    public ItemType?[] Ingredients { get; } = ingredients;

    /// <summary>
    ///     Leftover items in crafting slots after crafting
    /// </summary>
    public ItemType?[]? OutShape { get; private set; } = outShape;

    /// <summary>
    ///     Whether the recipe requires a crafting table.
    /// </summary>
    public bool RequiresCraftingTable { get; private set; } = requiresCraftingTable;

    /// <summary>
    ///     Item id of the result
    /// </summary>
    public ItemType Result { get; private set; } = result;

    /// <summary>
    ///     Number of result items
    /// </summary>
    public int ResultCount { get; private set; } = count;

    /// <summary>
    ///     How many of each <see cref="ItemType" /> are used.
    /// </summary>
    public Dictionary<ItemType, int> IngredientsCount => Ingredients.Aggregate(new Dictionary<ItemType, int>(),
                                                                               (x, y) =>
                                                                               {
                                                                                   if (y == null)
                                                                                   {
                                                                                       return x;
                                                                                   }

                                                                                   if (!x.TryGetValue(
                                                                                       y.Value, out var count))
                                                                                   {
                                                                                       x.Add(y.Value, 1);
                                                                                       return x;
                                                                                   }

                                                                                   count += 1;
                                                                                   x[y.Value] = count;
                                                                                   return x;
                                                                               });
}
