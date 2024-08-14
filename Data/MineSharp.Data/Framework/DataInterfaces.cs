using MineSharp.Core.Common;
using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Effects;
using MineSharp.Core.Common.Enchantments;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Common.Items;
using MineSharp.Core.Common.Protocol;
using MineSharp.Core.Common.Recipes;
using MineSharp.Core.Geometry;
using MineSharp.Data.Protocol;
using MineSharp.Data.Windows;

namespace MineSharp.Data.Framework;

/// <summary>
///     Interface for implementing indexed biome data
/// </summary>
public interface IBiomeData : ITypeIdNameIndexedData<BiomeType, BiomeInfo>;

/// <summary>
///     Interface for implementing indexed block data
/// </summary>
public interface IBlockData : ITypeIdNameIndexedData<BlockType, BlockInfo>
{
    /// <summary>
    ///     The total number of block states
    /// </summary>
    public int TotalBlockStateCount { get; }

    /// <summary>
    ///     Get a block info by state
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public BlockInfo? ByState(int state);
}

/// <summary>
///     Interface for implementing indexed effect data
/// </summary>
public interface IEffectData : ITypeIdNameIndexedData<EffectType, EffectInfo>;

/// <summary>
///     Interface for implementing indexed enchantment data
/// </summary>
public interface IEnchantmentData : ITypeIdNameIndexedData<EnchantmentType, EnchantmentInfo>;

/// <summary>
///     Interface for implementing indexed entity data
/// </summary>
public interface IEntityData : ITypeIdNameIndexedData<EntityType, EntityInfo>;

/// <summary>
///     Interface for implementing indexed item data
/// </summary>
public interface IItemData : ITypeIdNameIndexedData<ItemType, ItemInfo>;

/// <summary>
///     Interface for implementing indexed block collision shape data
/// </summary>
public interface IBlockCollisionShapeData
{
    /// <summary>
    ///     Returns indices of collision shapes
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int[] GetShapeIndices(BlockType type);

    /// <summary>
    ///     Returns the bounding boxes for a collision shape
    /// </summary>
    /// <param name="shapeIndex"></param>
    /// <returns></returns>
    public Aabb[] GetShapes(int shapeIndex);

    /// <summary>
    ///     Return the bounding boxes for a block type and index
    /// </summary>
    /// <param name="type"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public Aabb[] GetShapes(BlockType type, int index)
    {
        var indices = GetShapeIndices(type);
        var entry = indices.Length > 1 ? indices[index] : indices[0];
        return GetShapes(entry);
    }

    /// <summary>
    ///     Return the bounding boxes for a block
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    public Aabb[] GetForBlock(Block block)
    {
        return GetShapes(block.Info.Type, block.Metadata);
    }
}

/// <summary>
///     Interface for implementing language data
/// </summary>
public interface ILanguageData
{
    /// <summary>
    ///     Get the format string for a rule
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string? GetTranslation(string name);
}

/// <summary>
///     Interface for implementing material data
/// </summary>
public interface IMaterialData
{
    /// <summary>
    ///     Returns a multiplier a given material and item type
    /// </summary>
    /// <param name="material"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public float GetMultiplier(Material material, ItemType type);
}

/// <summary>
///     Interface for implementing protocol data
/// </summary>
public interface IProtocolData
{
    /// <summary>
    ///     Get the packet id for a <see cref="PacketType" />
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int GetPacketId(PacketType type);

    /// <summary>
    ///     Return the <see cref="PacketType" /> for a packet id
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="state"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public PacketType GetPacketType(PacketFlow flow, GameState state, int id);
}

/// <summary>
///     Interface for implementing recipe data
/// </summary>
public interface IRecipeData
{
    /// <summary>
    ///     Get a list of recipes for an <see cref="ItemType" />
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Recipe[]? ByItem(ItemType type);
}

/// <summary>
///     Inteface for implementing window data
/// </summary>
public interface IWindowData
{
    /// <summary>
    ///     A list of blocks that can be opened
    /// </summary>
    public IList<BlockType> AllowedBlocksToOpen { get; }

    /// <summary>
    ///     Get a window info by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public WindowInfo ById(int id);

    /// <summary>
    ///     Get a window info by name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public WindowInfo ByName(Identifier name);
}
