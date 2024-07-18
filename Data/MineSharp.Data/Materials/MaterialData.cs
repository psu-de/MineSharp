using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Items;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;

namespace MineSharp.Data.Materials;

internal class MaterialData(IDataProvider<MaterialDataBlob> provider)
    : IndexedData<MaterialDataBlob>(provider), IMaterialData
{
    private Dictionary<Material, Dictionary<ItemType, float>> multiplierMap = new();

    public float GetMultiplier(Material material, ItemType type)
    {
        if (!Loaded)
        {
            Load();
        }

        return multiplierMap[material].GetValueOrDefault(type, 1.0f);
    }

    protected override void InitializeData(MaterialDataBlob data)
    {
        multiplierMap = data.MultiplierMap;
    }
}
