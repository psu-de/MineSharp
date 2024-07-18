using MineSharp.Core.Common.Items;
using MineSharp.Core.Common.Recipes;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;
using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Recipes;

internal class RecipeProvider : IDataProvider<RecipeDataBlob>
{
    private readonly IItemData items;
    private readonly JObject token;

    public RecipeProvider(JToken token, IItemData items)
    {
        if (token.Type != JTokenType.Object)
        {
            throw new ArgumentException("Expected token to be an object");
        }

        this.token = (JObject)token;
        this.items = items;
    }

    public RecipeDataBlob GetData()
    {
        var data = new Dictionary<ItemType, Recipe[]>();

        foreach (var property in token.Properties())
        {
            var itemId = Convert.ToInt32(property.Name);
            var recipes = (JArray)property.Value;

            data.Add(items.ById(itemId)!.Type, CollectRecipes(recipes));
        }

        return new(data);
    }

    private Recipe[] CollectRecipes(JArray array)
    {
        return array
              .Select(RecipeFromToken)
              .ToArray();
    }

    private Recipe RecipeFromToken(JToken recipe)
    {
        var resultId = (int)recipe.SelectToken("result.id")!;
        var resultCount = (int)recipe.SelectToken("result.count")!;

        var ingredientsToken = recipe.SelectToken("ingredients");
        var ingredients = ingredientsToken is not null
            ? IngredientsFromArray((JArray)ingredientsToken)
            : IngredientsFromShape((JArray)recipe.SelectToken("inShape")!)!;

        var outShapeToken = (JArray?)recipe.SelectToken("outShape");
        var outShape = IngredientsFromShape(outShapeToken);

        var itemType = items.ById(resultId)!.Type;

        return new(
            ingredients,
            outShape,
            ingredients.Length > 4,
            itemType,
            resultCount);
    }

    private ItemType?[] IngredientsFromArray(JArray array)
    {
        return array.ToObject<int[]>()!
                    .Select(x => (ItemType?)items.ById(x)!.Type)
                    .ToArray();
    }

    private ItemType?[]? IngredientsFromShape(JArray? array)
    {
        if (array is null)
        {
            return null;
        }

        var shape = array.ToObject<int?[][]>()!;
        var ingredients = new List<ItemType?>(9);

        var skippedNulls = 0; // null values are only added to the list if there comes another non null item after
        for (var x = 0; x < shape.Length; x++)
        {
            for (var y = 0; y < shape[x].Length; y++)
            {
                var id = shape[x][y];
                if (!id.HasValue)
                {
                    skippedNulls++;
                    continue;
                }

                if (skippedNulls > 0)
                {
                    ingredients.AddRange(Enumerable.Repeat<ItemType?>(null, skippedNulls));
                }

                skippedNulls = 0;

                ingredients.Add(items.ById(id.Value)!.Type);
            }
        }

        return ingredients.ToArray();
    }
}
