using MineSharp.Core.Common;
using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Effects;
using MineSharp.Core.Common.Enchantments;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Common.Items;
using MineSharp.Core.Common.Particles;
using MineSharp.Core.Registries;

namespace MineSharp.Data;

/// <summary>
/// Registry for biomes
/// </summary>
public class BiomeRegistry() : Registry<BiomeInfo, BiomeType>(RegistryName)
{
    /// <summary>
    /// Identifier of the biome registry
    /// </summary>
    public static Identifier RegistryName { get; } = Identifier.Parse("worldgen/biome");
}

/// <summary>
/// Registry for blocks
/// </summary>
public class BlockRegistry() : Registry<BlockInfo, BlockType>(RegistryName)
{
    /// <summary>
    /// Identifier of the block registry
    /// </summary>
    public static Identifier RegistryName { get; } = Identifier.Parse("block");

    /// <summary>
    /// The total number of block states in this registry
    /// </summary>
    public int TotalBlockStateCount { get; private set; } = 0;

    /// <inheritdoc />
    protected override void OnRegistered(BlockInfo obj)
    {
        TotalBlockStateCount = Math.Max(obj.MaxState, TotalBlockStateCount);
    }
    
    /// <summary>
    /// Get block info by a block state
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public BlockInfo? ByState(int state)
    {
        var half = Values.Count / 2;
        var start = state < Values.ElementAt(half).MaxState
            ? 0
            : half;

        for (var i = start; i < Values.Count; i++)
        {
            if (state <= Values.ElementAt(i).MaxState)
            {
                return Values.ElementAt(i);
            }
        }

        return null;
    }
}

/// <summary>
/// Registry for effects
/// </summary>
public class EffectRegistry() : Registry<EffectInfo, EffectType>(RegistryName)
{
    /// <summary>
    /// Identifier of the effect registry
    /// </summary>
    public static Identifier RegistryName { get; } = Identifier.Parse("mob_effect");
}

/// <summary>
/// Registry for enchantments
/// </summary>
public class EnchantmentRegistry() : Registry<EnchantmentInfo, EnchantmentType>(RegistryName)
{
    /// <summary>
    /// Identifier of the enchantment registry
    /// </summary>
    public static Identifier RegistryName { get; } = Identifier.Parse("enchantment");
}

/// <summary>
/// Registry for entities
/// </summary>
public class EntityRegistry() : Registry<EntityInfo, EntityType>(RegistryName)
{
    /// <summary>
    /// Identifier of the entity registry
    /// </summary>
    public static Identifier RegistryName { get; } = Identifier.Parse("entity_type");
}

/// <summary>
/// Registry for items
/// </summary>
public class ItemRegistry() : Registry<ItemInfo, ItemType>(RegistryName)
{
    /// <summary>
    /// Identifier of the item registry
    /// </summary>
    public static Identifier RegistryName { get; } = Identifier.Parse("item");
}

/// <summary>
/// Registry for particles
/// </summary>
public class ParticleRegistry() : Registry<RegistryResource<ParticleType>, ParticleType>(RegistryName)
{
    /// <summary>
    /// Identifier of the particle registry
    /// </summary>
    public static Identifier RegistryName { get; } = Identifier.Parse("particle_type");
}
