using MineSharp.Core.Common.Recipes;
using MineSharp.Data.Exceptions;
using MineSharp.Data.Json;
using Newtonsoft.Json;

namespace MineSharp.Data.Providers;

public class RecipeDataProvider : IDataProvider
{
    private readonly Dictionary<int, Recipe[]> _byItemId;
    private readonly string _path;

    public bool IsLoaded { get; private set; }

    public RecipeDataProvider(string recipePath)
    {
        this._path = recipePath;
        this._byItemId = new Dictionary<int, Recipe[]>();

        this.IsLoaded = false;
    }
    
    public void Load()
    {
        if (IsLoaded)
        {
            return;
        }
        
        this.LoadData();

        this.IsLoaded = true;
    }

    private void LoadData()
    {
        var recipeData = JsonConvert.DeserializeObject<Dictionary<int, RecipeInfoBlob[]>>(File.ReadAllText(this._path))!;

        foreach (var kvp in recipeData)
        {
            var itemId = kvp.Key;
            var recipeBlobs = kvp.Value;

            var recipes = recipeBlobs.Select(GetRecipe).ToArray();

            this._byItemId.Add(itemId, recipes);
        }
    }
    
    public Recipe[] GetRecipesForItem(int itemId)
    {
        if (!this._byItemId.TryGetValue(itemId, out var bi))
        {
            throw new MineSharpDataException($"No Recipes for item with id {itemId}.");
        }

        return bi;
    }

    private Recipe GetRecipe(RecipeInfoBlob recipe)
    {
        int?[] ingredients;
        var requiresTable = false;
            
        if (recipe.Ingredients != null)
        {
            ingredients = recipe.Ingredients
                .Select(x => (int?)x)    
                .ToArray();
            requiresTable = recipe.Ingredients.Length > 4;
        } 
        else
        {
            if (recipe.InShape == null)
            {
                throw new MineSharpDataException("Recipe Data: Ingredients and inShape are null.");
            }
            
            ingredients = ConvertShape(recipe.InShape!);
            ingredients = RemoveTrailingNulls(ingredients);
            requiresTable = recipe.InShape!.Length > 2 || recipe.InShape!.Any(x => x.Length > 2);
            if (!requiresTable)
            {
                if (ingredients.Length > 3)
                {
                    if (ingredients[2] != null)
                    {
                        throw new Exception($"[{string.Join(", ", ingredients)}], [{string.Join(", ", ConvertShape(recipe.InShape!))}], {recipe.InShape.Length}, [{string.Join(", ", recipe.InShape.Select(x => x.Length))}]");
                    }
                    
                    var ing = ingredients.ToList();
                    ing.RemoveAt(2);
                    ingredients = ing.ToArray();   
                }
                    
                if (ingredients.Length > 4)
                {
                    throw new Exception($"[{string.Join(", ", ingredients)}], [{string.Join(", ", ConvertShape(recipe.InShape!))}], {recipe.InShape.Length}, [{string.Join(", ", recipe.InShape.Select(x => x.Length))}]");
                }   
            }
        }

        int?[]? outShape = null;
            
        if (recipe.OutShape != null)
        {
            outShape = ConvertShape(recipe.OutShape!);
        }

        return new Recipe(ingredients, outShape, requiresTable, recipe.Result.Id, recipe.Result.Count);
    }
    
    private int?[] ConvertShape(int?[][] shape)
    {
        var @out = new int?[9];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                var idx = (i * 3) + j;
                int? id = null;
                if (shape.Length > i && shape[i].Length > j)
                {
                    id = shape[i][j];
                }
                @out[idx] = id;
            }
        }
        return @out;
    }
    
    private int?[] RemoveTrailingNulls(int?[] arr)
    {
        int nullIndex = -1;

        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] != null)
            {
                nullIndex = -1;
                continue;
            }

            if (nullIndex > 0)
            {
                continue;
            }

            nullIndex = i;
        }

        if (nullIndex == -1)
        {
            return arr;
        }

        return arr.Take(nullIndex).ToArray();
    }
}
