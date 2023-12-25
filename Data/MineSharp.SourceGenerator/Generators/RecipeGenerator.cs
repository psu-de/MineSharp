using MineSharp.SourceGenerator.Code;
using MineSharp.SourceGenerator.Utils;
using Newtonsoft.Json.Linq;

namespace MineSharp.SourceGenerator.Generators;

public class RecipeGenerator : IGenerator
{
    public string Name => "Recipe";

    public async Task Run(MinecraftDataWrapper wrapper)
    {
        foreach (var version in Config.IncludedVersions)
        {
            await GenerateVersion(wrapper, version);
        }
    }

    private async Task GenerateVersion(MinecraftDataWrapper wrapper, string version)
    {
        var path = wrapper.GetPath(version, "recipes");
        if (VersionMapGenerator.GetInstance().IsRegistered("recipes", path))
        {
            VersionMapGenerator.GetInstance().RegisterVersion("recipes", version, path);
            return;
        }
        
        VersionMapGenerator.GetInstance().RegisterVersion("recipes", version, path);
        
        var outdir = DirectoryUtils.GetDataSourceDirectory(Path.Join("Recipes", "Versions"));

        var data = await wrapper.Parse(version, "recipes");
        var items = await wrapper.Parse(version, "items");
        var v = version.Replace(".", "_");

        var writer = new CodeWriter();
        writer.Disclaimer();
        writer.WriteLine("using MineSharp.Core.Common.Recipes;");
        writer.WriteLine("using MineSharp.Core.Common.Items;");
        writer.WriteLine();
        writer.WriteLine("namespace MineSharp.Data.Recipes.Versions;");
        writer.WriteLine();
        writer.Begin($"internal class Recipes_{v} : RecipeData");
        writer.Begin("private static Dictionary<ItemType, Recipe[]> _recipes = new()");

        foreach (var recipe in ((JObject)data).Properties())
        {
            writer.Begin();
            writer.WriteLine($"{FindItem(items, recipe.Name)},");
            writer.WriteLine($"{GetRecipes(recipe.Value, items)}");
            writer.Finish(colon: true);
        }

        writer.Finish(semicolon: true);
        writer.WriteLine("public override Dictionary<ItemType, Recipe[]> Recipes => _recipes;");
        writer.Finish();
        
        await File.WriteAllTextAsync(Path.Join(outdir, $"Recipes_{v}.cs"), writer.ToString());
    }

    string FindItem(JToken items, string id)
    {
        foreach (var token in items)
        {
            var itemId = (int)token.SelectToken("id")!;
            if (itemId.ToString() == id)
                return "ItemType." + NameUtils.GetItemName((string)token.SelectToken("name")!);
        }
        throw new Exception($"Item not found: {id}");
    }

    string GetRecipes(JToken token, JToken items)
    {
        var list = new List<string>();
        foreach (var element in (JArray)token)
        {
            list.Add(StringifyRecipe(element, items));
        }

        return $"new [] {{ {string.Join(", ", list) }}}";
    }

    string StringifyRecipe(JToken token, JToken items)
    {
        int?[] ingredients;
        var requiresTable = false;

        if (token.SelectToken("ingredients") != null)
        {
            (ingredients, requiresTable) = this.FromIngredientRecipe(token);
        }
        else
        {
            (ingredients, requiresTable) = this.FromInShapeRecipe(token);
        }

        
        int?[]? outShape = null;
            
        if (token.SelectToken("outShape") != null)
        {
            outShape = Flatten(ConvertShape(token.SelectToken("outShape")!));
        }

        var resultId = (int)token.SelectToken("result.id")!;
        var resultCount = (int)token.SelectToken("result.count")!;

        return $"new Recipe({StringifyItemsArray(ingredients, items)}, " +
               $"{StringifyItemsArray(outShape, items)}, " +
               $"{requiresTable.ToString().ToLower()}, " +
               $"{FindItem(items, resultId.ToString())}, " +
               $"{resultCount})";
    }

    string StringifyItemsArray(int?[]? itemIds, JToken items)
    {
        if (itemIds == null)
            return "null";

        var list = new List<string>();
        foreach (int? id in itemIds)
        {
            if (id == null)
            {
                list.Add("null");
                continue;
            }
            
            list.Add(FindItem(items, id.Value.ToString()));
        }

        return $"new ItemType?[] {{ {string.Join(", ", list)} }}";
    }

    (int?[], bool) FromIngredientRecipe(JToken token)
    {
        var ingredients = (JArray)token.SelectToken("ingredients")!;
        var itemIngredients = ingredients
            .Select(x => (int?)x)
            .ToArray();
        
        var requiresTable = itemIngredients.Length > 4;
        return (itemIngredients, requiresTable);
    }

    (int?[], bool) FromInShapeRecipe(JToken token)
    {
        var shape = ConvertShape(token.SelectToken("inShape")!);
        var ingredients = Flatten(shape);
        ingredients = RemoveTrailingNulls(ingredients);
        var requiresTable = shape.Length > 2 || shape.Any(x => x.Length > 2);
        if (!requiresTable)
        {
            if (ingredients.Length > 3)
            {
                if (ingredients[2] != null)
                {
                    throw new Exception();
                }
                    
                var ing = ingredients.ToList();
                ing.RemoveAt(2);
                ingredients = ing.ToArray();   
            }
                    
            if (ingredients.Length > 4)
            {
                throw new Exception();
            }   
        }

        return (ingredients, requiresTable);
    }

    int?[][] ConvertShape(JToken token)
    {
        return ((JArray)token)
            .Select(arr => ((JArray)arr).Select(x => (int?)x).ToArray())
            .ToArray();
    }

    int?[] Flatten(int?[][] twoD)
    {
        var flattened = new int?[9];
        for (int x = 0; x < twoD.Length; x++)
        for (int y = 0; y < twoD[x].Length; y++)
            flattened[x * 3 + y] = twoD[x][y];

        return flattened;
    }
    
    int?[] RemoveTrailingNulls(int?[] arr)
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

        return nullIndex == -1 ? arr : arr.Take(nullIndex).ToArray();

    }
}
