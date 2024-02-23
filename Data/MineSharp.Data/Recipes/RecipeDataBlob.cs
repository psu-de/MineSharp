using MineSharp.Core.Common.Items;
using MineSharp.Core.Common.Recipes;

namespace MineSharp.Data.Recipes;

internal record RecipeDataBlob(Dictionary<ItemType, Recipe[]> Recipes);
