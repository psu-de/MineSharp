using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Items;
using MineSharp.Core.Common.Recipes;
using MineSharp.Windows;
using NLog;

namespace MineSharp.Bot.Plugins;

/// <summary>
///     Crafting plugin provides methods to craft items using the player's inventory
///     crafting menu or a crafting table.
/// </summary>
/// <param name="bot"></param>
public class CraftingPlugin(MineSharpBot bot) : Plugin(bot)
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private WindowPlugin? windowPlugin;

    /// <inheritdoc />
    protected override async Task Init()
    {
        windowPlugin = Bot.GetPlugin<WindowPlugin>();

        await windowPlugin.WaitForInventory().WaitAsync(Bot.CancellationToken);
    }

    /// <summary>
    ///     Find all recipes containing only resources the bot has currently in the inventory.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public IEnumerable<Recipe> FindRecipes(ItemType type)
    {
        return Bot.Data.Recipes.ByItem(type)!
                  .Where(recipe =>
                             recipe.IngredientsCount.All(
                                 kvp => windowPlugin!.Inventory!.CountItems(kvp.Key) > kvp.Value));
    }

    /// <summary>
    ///     Get the first recipe that can currently be crafted by the bot
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Recipe? FindRecipe(ItemType type)
    {
        return FindRecipes(type).FirstOrDefault();
    }

    /// <summary>
    ///     Returns how often this recipe can be crafted with the bots current inventory.
    /// </summary>
    /// <param name="recipe"></param>
    /// <returns></returns>
    public int CraftableAmount(Recipe recipe)
    {
        return recipe.IngredientsCount
                     .Select(kvp => windowPlugin!.Inventory!.CountItems(kvp.Key) / kvp.Value)
                     .Min();
    }

    /// <summary>
    ///     Craft the given recipe <paramref name="amount" /> of times.
    ///     If the recipe requires a crafting table, <paramref name="craftingTable" /> is used.
    /// </summary>
    /// <param name="recipe"></param>
    /// <param name="craftingTable"></param>
    /// <param name="amount"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task Craft(Recipe recipe, Block? craftingTable = null, int amount = 1)
    {
        if (amount > CraftableAmount(recipe))
        {
            throw new InvalidOperationException($"The bot cannot craft this recipe {amount} times.");
        }

        Window craftingWindow;
        if (recipe.RequiresCraftingTable)
        {
            if (craftingTable?.Info.Type != BlockType.CraftingTable)
            {
                throw new InvalidOperationException("The recipe requires a crafting table but none was provided.");
            }

            craftingWindow = await windowPlugin!.OpenContainer(craftingTable);
        }
        else
        {
            craftingWindow = windowPlugin!.Inventory!;
        }

        var resultType = Bot.Data.Items.ByType(recipe.Result)!;

        var perIteration = resultType.StackSize / recipe.ResultCount;
        perIteration = recipe.IngredientsCount.Keys
                             .Select(x => Bot.Data.Items.ByType(x)!)
                             .Select(x => x.StackSize)
                             .Prepend(perIteration)
                             .Min();

        var iterations = (int)Math.Ceiling(amount / (float)perIteration);
        var done = 0;
        for (var i = 0; i < iterations; i++)
        {
            var count = Math.Min(perIteration, amount - done);
            await _Craft(recipe, craftingWindow, count);
            done += count;
        }

        await windowPlugin!.CloseWindow(craftingWindow.WindowId);
    }

    private async Task _Craft(Recipe recipe, Window craftingWindow, int count)
    {
        // 0 is always the result slot
        // crafting slots are indexed from top left to bottom right, starting at 1

        // put in ingredients
        for (short i = 0; i < recipe.Ingredients.Length; i++)
        {
            var item = recipe.Ingredients[i];
            if (null == item)
            {
                continue;
            }

            var craftingSlot = (short)(i + 1);
            craftingWindow.PickupInventoryItems(item.Value, count);
            craftingWindow.PutDownItems(count, craftingSlot);
        }

        var timeout = new CancellationTokenSource();
        timeout.CancelAfter(TimeSpan.FromSeconds(1 * Math.Max(5, count)));

        var stackSlots = craftingWindow
                        .FindInventorySlotsToStack(recipe.Result, recipe.ResultCount * count)
                        .GetEnumerator();

        if (!stackSlots.MoveNext())
        {
            throw new InvalidOperationException("Could not find any slot to put result item");
        }

        var currentStackSlot = stackSlots.Current;
        var done = 0;

        // pickup from result slot
        for (; done < count; done++)
        {
            while (craftingWindow.GetSlot(0).Item?.Info.Type != recipe.Result && !timeout.IsCancellationRequested)
            {
                await Task.Delay(10, timeout.Token);
            }

            if (timeout.IsCancellationRequested)
            {
                break;
            }

            if (craftingWindow.GetSlot(currentStackSlot.SlotIndex).IsFull())
            {
                if (!stackSlots.MoveNext())
                {
                    throw new InvalidOperationException("Could not find any slot to put result item");
                }

                currentStackSlot = stackSlots.Current;
            }

            craftingWindow.DoSimpleClick(WindowMouseButton.MouseLeft, 0);
            craftingWindow.DoSimpleClick(WindowMouseButton.MouseLeft, currentStackSlot.SlotIndex);
        }

        stackSlots.Dispose();

        if (done != count)
        {
            Logger.Warn("Could crafted {Actual} instead of {Expected}", done, count);
        }
    }
}
