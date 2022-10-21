using MineSharp.Core.Types;
using MineSharp.Data.Blocks;
using MineSharp.Data.Items;

namespace MineSharp.Bot
{
    public partial class MinecraftBot
    {

        public Recipe[]? GetRecipes(ItemType itemType) => this.CraftingModule!.GetRecipes(itemType);
        public IEnumerable<Recipe> FindRecipes(ItemType itemType) => this.CraftingModule!.FindRecipes(itemType);
        
        public Recipe? FindRecipe(ItemType itemType) => this.CraftingModule!.FindRecipe(itemType);
        
        public int CraftableAmount(Recipe recipe) => this.CraftingModule!.CraftableAmount(recipe);
        
        public Task Craft(Recipe recipe, CraftingTable? craftingTable = null, int count = 1) => this.CraftingModule!.Craft(recipe, craftingTable, count);
    }
}
