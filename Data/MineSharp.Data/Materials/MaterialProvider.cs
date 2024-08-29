using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Items;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;
using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Materials;

internal class MaterialsProvider : IDataProvider<MaterialDataBlob>
{
    private static readonly EnumNameLookup<Material> MaterialLookup = new();
    private readonly ItemRegistry items;

    private readonly JObject token;

    public MaterialsProvider(JToken token, ItemRegistry items)
    {
        if (token.Type != JTokenType.Object)
        {
            throw new ArgumentException("Expected token to be an object");
        }

        this.token = (JObject)token;
        this.items = items;
    }

    public MaterialDataBlob GetData()
    {
        var data = new Dictionary<Material, Dictionary<ItemType, float>>();

        foreach (var property in token.Properties())
        {
            if (property.Name.Contains(';'))
            {
                continue;
            }

            var material = MaterialLookup.FromName(NameUtils.GetMaterial(property.Name));
            var multipliers = CollectItemMultipliers((JObject)property.Value);
            data.Add(material, multipliers);
        }

        return new(data);
    }

    private Dictionary<ItemType, float> CollectItemMultipliers(JObject obj)
    {
        return obj.Properties()
                  .ToDictionary(
                       x => items.ById(Convert.ToInt32(x.Name))!.Type,
                       x => (float)x.Value);
    }
}
