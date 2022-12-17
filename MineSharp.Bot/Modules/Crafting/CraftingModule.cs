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
            
                if (!ingredients.All(i => i == null || this.Bot.Inventory!.FindInventoryItems((ItemType)i).Any()))
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
            
            // Crafting result slot is always 1
            // crafting slots are indexed from 1 (top left) to 4 or 9 (bottom right)

            for (int i = 0; i < recipe.Ingredients.Length; i++)
            {
                var ingredient = recipe.Ingredients[i];
                if (!ingredient.HasValue)
                {
                    continue;
                }

                var ingredientCount = ingredient!.Value * count;
                var slotIndex = (short)(i + 1);

                foreach (var slot in craftingWindow.FindInventoryItems((ItemType)ingredient))
                {
                    int toTake = Math.Min(ingredientCount, slot.Item!.Count);
                    ingredientCount -= slot.Item!.Count;
                    craftingWindow.MoveItemsFromSlot(slot.SlotNumber, slotIndex, toTake);

                    if (ingredientCount <= 0)
                    {
                        break;
                    }
                }
            }

            var resultSlot = craftingWindow.GetSlot(0);
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(10);
                resultSlot = craftingWindow.GetSlot(0);
                if (!resultSlot.IsEmpty() && resultSlot.Item!.Id == recipe.Result)
                {
                    break;
                }
            }
            
            if (resultSlot.IsEmpty())
            {
                throw new Exception("result slot is empty");
            }

            if (resultSlot.Item!.Id != recipe.Result)
            {
                throw new Exception($"unexpected result item! got: {resultSlot.Item!.Id}, expected: {recipe.Result}");
            }
            
            craftingWindow.DoSimpleClick(WindowMouseButton.MouseLeft, 0);
            this.Logger.Debug(craftingWindow.GetSelectedSlot().ToString());
            craftingWindow.StackSelectedSlotInInventory(craftingWindow.GetSelectedSlot().Item!.Count);
        }
    }
}
