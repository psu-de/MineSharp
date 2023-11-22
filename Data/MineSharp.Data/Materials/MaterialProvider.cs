using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Items;

namespace MineSharp.Data.Materials;

public class MaterialsProvider
{
    private MaterialVersion _version { get; }

    internal MaterialsProvider(MaterialVersion version)
    {
        this._version = version;
    }

    public float GetToolMultiplier(Material material, ItemType type)
    {
        var mat = this._version.Palette[material];
        if (!mat.TryGetValue(type, out var multiplier))
        {
            return 1.0f;
        }
        return multiplier;
    }
}
