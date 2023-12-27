using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Items;

namespace MineSharp.Data.Materials;

/// <summary>
/// Provides information about materials.
/// </summary>
public class MaterialsProvider
{
    private readonly MaterialVersion _version;

    internal MaterialsProvider(MaterialVersion version)
    {
        this._version = version;
    }

    /// <summary>
    /// Gets a multiplier for a given material and item type.
    /// </summary>
    /// <param name="material"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public float GetToolMultiplier(Material material, ItemType type)
    {
        var mat = this._version.Palette[material];
        return mat.GetValueOrDefault(type, 1.0f);
    }
}
