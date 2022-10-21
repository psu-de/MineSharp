using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Blocks;
using MineSharp.Data.Items;
using MineSharp.Data.Recipes;
using MineSharp.Windows;
using System.Runtime.CompilerServices;

namespace MineSharp.Bot.Modules.Crafting
{
    public class CraftingModule : Module
    {

        public CraftingModule(MinecraftBot bot) : base(bot) {}
        protected override Task Load() => this.Bot.WaitForInventory();

        /// <summary>
        /// Returns all recipes for <paramref name="itemType"/>
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public Recipe[]? GetRecipes(ItemType itemType)
        {
            if (!RecipePalette.Recipes.TryGetValue((int)itemType, out var recipes))
            {
                return null;
            }

            return recipes;
        }

        /// <summary>
        /// Returns all recipes for <paramref name="itemType"/> with items in the bots inventory
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public IEnumerable<Recipe> FindRecipes(ItemType itemType)
        {
            var recipes = this.GetRecipes(itemType);

            if (recipes == null)
            {
                yield break;
            }

            foreach (var recipe in recipes)
            {
                var ingredients = recipe.Ingredients;

                if (!ingredients.All(i => i == null || this.Bot.Inventory!.FindInventoryItem((ItemType)i) != null))
                {
                    continue;
                }
                yield return recipe;
            }
        }
        
        public Recipe? FindRecipe(ItemType itemType) => FindRecipes(itemType).FirstOrDefault();

        /// <summary>
        /// Returns how often the <paramref name="recipe"/> can be crafted with the bots inventory
        /// </summary>
        /// <param name="recipe"></param>
        /// <returns></returns>
        public int CraftableAmount(Recipe recipe)
        {
            int totalCraftableAmount = int.MaxValue;
            foreach (var ingredient in recipe.IngredientsCount)
            {

                int count = this.Bot.Inventory!.GetContainerSlots()
                    .Select(x => x.Item)
                    .Where(x => x != null && x.Id == ingredient.Value)
                    .Select(x => x!.Count)
                    .Sum(x => x);

                var craftableAmount = count / ingredient.Value;
                totalCraftableAmount = Math.Min(craftableAmount, totalCraftableAmount);
            }
            return totalCraftableAmount;
        }

        public async Task Craft(Recipe recipe, CraftingTable? craftingTable = null, int count = 1)
        {
            Window craftingWindow;
            if (recipe.RequiresCraftingTable)
            {
                if (craftingTable == null)
                {
                    throw new ArgumentNullException(nameof(craftingTable));
                }

                if (this.Bot.Player!.Entity.Position.DistanceSquared((Vector3)craftingTable.Position!) > 36)
                {
                    throw new InvalidOperationException("CraftingTable is too far away");
                }

                craftingWindow = await this.Bot.OpenContainer(craftingTable);
            } else
            {
                craftingWindow = this.Bot.Inventory!;
            }

            var tsc = new TaskCompletionSource();
            void windowSlotUpdated(Window sender, int slot)
            {
                if (slot == 0 && sender.GetAllSlots()[slot].Item?.Id == recipe.Result)
                {
                    tsc.SetResult();
                }
            }
            this.Logger.Debug(craftingWindow.ToString());
            this.Logger.Debug($"InvStart={craftingWindow.InventoryStart} InvEnd={craftingWindow.InventoryEnd} ContainerStart={craftingWindow.ContainerStart} ContainerEnd={craftingWindow.ContainerEnd}");

            this.Logger.Debug(string.Join(", ", craftingWindow.GetAllSlots().ToList()));
            this.Logger.Debug(string.Join(", ", craftingWindow.InventoryWindow!.GetContainerSlots().ToList()));
            craftingWindow.WindowSlotUpdated += windowSlotUpdated;
            for (int i = 0; i < recipe.Ingredients.Length; i++)
            {
                if (recipe.Ingredients[i] == null)
                {
                    continue;
                }
                this.Logger.Debug(i.ToString());
                this.Logger.Debug(craftingWindow.SelectedSlot?.Item?.ToString() ?? craftingWindow.SelectedSlot?.IsEmpty().ToString() ?? "null");
                
                var itemType = (ItemType)recipe.Ingredients[i]!;
                craftingWindow.PickupInventoryItems(itemType, 1);
                
                await Task.Delay(50);
                var click = new WindowClick(WindowOperationMode.SimpleClick, (byte)WindowMouseButton.MouseLeft, (short)i);
                craftingWindow.PerformClick(click);
            }

            await tsc.Task;
            craftingWindow.PerformClick(
                new WindowClick(WindowOperationMode.SimpleClick, (byte)WindowMouseButton.MouseLeft, 0));

            await Task.Delay(10);
            
            craftingWindow.PutDownInventoryItems(recipe.ResultCount);
            
            await Task.Delay(50);
        }
    }
}
