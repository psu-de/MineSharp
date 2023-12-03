using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Items;
using MineSharp.Core.Common.Recipes;
using MineSharp.Windows;
using NLog;

namespace MineSharp.Bot.Plugins;

public class CraftingPlugin : Plugin
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    
    private WindowPlugin? windowPlugin; 
    
    public CraftingPlugin(MinecraftBot bot) : base(bot)
    { }
    protected override async Task Init()
    {
        this.windowPlugin = this.Bot.GetPlugin<WindowPlugin>();
        
        await this.windowPlugin.WaitForInventory();
    }
    /// <summary>
    /// Find all recipes containing only resources the bot has currently in the inventory.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public IEnumerable<Recipe> FindRecipes(ItemType type)
    {
        return this.Bot.Data.Recipes.GetRecipesForItem(type)
            .Where(recipe => 
                recipe.IngredientsCount.All(
                    kvp => this.windowPlugin!.Inventory!.CountItems(kvp.Key) > kvp.Value));
    }
    /// <summary>
    /// Get the first recipe that can currently be crafted by the bot
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Recipe? FindRecipe(ItemType type)
    {
        return this.FindRecipes(type).FirstOrDefault();
    }
    /// <summary>
    /// Returns how often this recipe can be crafted with the bots current inventory.
    /// </summary>
    /// <param name="recipe"></param>
    /// <returns></returns>
    public int CraftableAmount(Recipe recipe)
    {
        return recipe.IngredientsCount
            .Select(kvp => this.windowPlugin!.Inventory!.CountItems(kvp.Key) / kvp.Value)
            .Min();
    }
    public async Task Craft(Recipe recipe, Block? craftingTable = null, int amount = 1)
    {
        if (amount > this.CraftableAmount(recipe))
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
            craftingWindow = await this.windowPlugin!.OpenContainer(craftingTable);
        }
        else craftingWindow = this.windowPlugin!.Inventory!;
        var resultType = this.Bot.Data.Items.GetByType(recipe.Result);
        
        
        var perIteration = resultType.StackSize / recipe.ResultCount;
        perIteration = recipe.IngredientsCount.Keys
            .Select(x => this.Bot.Data.Items.GetByType(x))
            .Select(x => x.StackSize)
            .Prepend(perIteration)
            .Min();
        
        var iterations = Math.Max(1, amount / perIteration);
        var done = 0;
        for (int i = 0; i < iterations; i++)
        {
            await this._Craft(recipe, craftingWindow, Math.Min(perIteration, amount - done));
            done += Math.Min(perIteration, amount - done);
        }
    }

    private async Task _Craft(Recipe recipe, Window craftingWindow, int count)
    {
        // 0 is always the result slot
        // crafting slots are indexed from top left to bottom right, starting at 1
        for (short i = 0; i < recipe.Ingredients.Length; i++)
        {
            var item = recipe.Ingredients[i];
            if (null == item)
            {
                continue;
            }
            Console.WriteLine($"Setting ingredient {i} ({item})");
            var craftingSlot = (short)(i + 1);
            craftingWindow.PickupInventoryItems(item.Value, count);
            Console.WriteLine($"Picked up items, selected={craftingWindow.GetSelectedSlot()}");
            craftingWindow.PutDownItems(count, craftingSlot);
            Console.WriteLine($"put down items, selected={craftingWindow.GetSelectedSlot()}");
        }
        
        Console.WriteLine("Trying to get result");
        var resultSlot = craftingWindow.GetSlot(0);
        var selected = craftingWindow.GetSelectedSlot();

        var timeout = new CancellationTokenSource();
        timeout.CancelAfter(TimeSpan.FromSeconds(5));

        try
        {
            await Task.Run(() => {
                for (int i = 0; i < count; i++) {
                    while (resultSlot.IsEmpty())
                        Task.Delay(10);

                    // pickup result slot
                    // TODO: _Craft(): All items could be picked up at once with shift click
                    craftingWindow.DoSimpleClick(WindowMouseButton.MouseLeft, resultSlot.SlotIndex);
                }
            }).WaitAsync(timeout.Token);
        } catch (TaskCanceledException) 
        { }

        if (selected.Item?.Count != recipe.ResultCount * count)
        {
            Logger.Warn("Crafting: Expected {Result} result items, but got {Actual}", recipe.ResultCount * count, selected.Item?.Count);
            // TODO: _Craft(): if the correct amount wasn't crafted, somehow return the actual amount crafted 
        }

        Console.WriteLine(selected); // TODO: Checken ob craftingWindow.GetSelectedSlot() == inventory.GetSelectedSlot()
        
        if (null == selected.Item)
            return;
        
        // put down result items somewhere in the inventory
        craftingWindow.StackSelectedSlotInInventory(selected.Item.Count); 

        await windowPlugin!.CloseWindow(craftingWindow.WindowId);
    }
}