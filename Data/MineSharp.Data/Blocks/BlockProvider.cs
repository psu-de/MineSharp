using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Blocks.Property;
using MineSharp.Core.Common.Items;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;
using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Blocks;

internal class BlockProvider : IDataProvider<BlockInfo[]>
{
    private static readonly EnumNameLookup<BlockType> BlockTypeLookup = new();
    private static readonly EnumNameLookup<Material> MaterialLookup = new();
    private readonly IItemData items;

    private readonly JArray token;

    public BlockProvider(JToken token, IItemData items)
    {
        if (token.Type != JTokenType.Array)
        {
            throw new InvalidOperationException("Expected the token to be an array");
        }

        this.token = (token as JArray)!;
        this.items = items;
    }

    public BlockInfo[] GetData()
    {
        var length = token.Count;
        var data = new BlockInfo[length];

        for (var i = 0; i < length; i++)
        {
            data[i] = FromToken(token[i], items);
        }

        return data;
    }

    private static BlockInfo FromToken(JToken dataToken, IItemData items)
    {
        var id = (int)dataToken.SelectToken("id")!;
        var name = (string)dataToken.SelectToken("name")!;
        var displayName = (string)dataToken.SelectToken("displayToken")!;
        var hardness = (float?)dataToken.SelectToken("hardness") ?? float.MaxValue;
        var resistance = (float)dataToken.SelectToken("resistance")!;
        var minState = (int)dataToken.SelectToken("minStateId")!;
        var maxState = (int)dataToken.SelectToken("maxStateId")!;
        var unbreakable = !(bool)dataToken.SelectToken("diggable")!;
        var transparent = (bool)dataToken.SelectToken("transparent")!;
        var filterLight = (byte)dataToken.SelectToken("filterLight")!;
        var emitLight = (byte)dataToken.SelectToken("emitLight")!;
        var materials = (string)dataToken.SelectToken("material")!;
        var harvestTools = (JObject?)dataToken.SelectToken("harvestTools");
        var states = (JArray)dataToken.SelectToken("states")!;
        var defaultState = (int)dataToken.SelectToken("defaultState")!;

        return new(
            id,
            BlockTypeLookup.FromName(NameUtils.GetBlockName(name)),
            name,
            displayName,
            hardness,
            resistance,
            minState,
            maxState,
            unbreakable,
            transparent,
            filterLight,
            emitLight,
            GetMaterials(materials),
            GetHarvestTools(harvestTools, items),
            defaultState,
            GetBlockState(states)
        );
    }

    private static Material[] GetMaterials(string str)
    {
        return str.Split(";")
                  .Select(NameUtils.GetMaterial)
                  .Select(MaterialLookup.FromName)
                  .ToArray();
    }

    private static ItemType[] GetHarvestTools(JObject? array, IItemData items)
    {
        if (array == null)
        {
            return Array.Empty<ItemType>();
        }

        return array.Properties()
                    .Select(x => x.Name)
                    .Select(x => Convert.ToInt32(x))
                    .Select(items.ById)
                    .Select(x => x!.Type)
                    .ToArray();
    }

    private static BlockState GetBlockState(JArray states)
    {
        if (states.Count == 0)
        {
            return new(Array.Empty<IBlockProperty>());
        }

        var properties = states
                        .Select(x => GetBlockProperty((JObject)x))
                        .ToArray();

        return new(properties);
    }

    private static IBlockProperty GetBlockProperty(JObject obj)
    {
        var name = (string)obj.SelectToken("name")!;
        var type = (string)obj.SelectToken("type")!;
        var numValues = (int)obj.SelectToken("num_values")!;

        return type switch
        {
            "bool" => new BoolProperty(name),
            "int" => new IntProperty(name, numValues),
            "enum" => new EnumProperty(name, obj.SelectToken("values")!.ToObject<string[]>()!),
            _ => throw new NotSupportedException($"Property of type '{type}' is not supported.")
        };
    }
}
