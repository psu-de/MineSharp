using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using MineSharp.Core.Common;
using MineSharp.Core.Registries;

namespace MineSharp.Data;

/// <summary>
/// Represents available registries
/// </summary>
public class Registries : IDictionary<Identifier, IRegistry>
{
    private readonly ConcurrentDictionary<Identifier, IRegistry> registries = new();

    /// <summary>
    /// The biome registry
    /// </summary>
    public BiomeRegistry Biomes => (BiomeRegistry)registries[BiomeRegistry.RegistryName];

    /// <summary>
    /// The block registry
    /// </summary>
    public BlockRegistry Blocks => (BlockRegistry)registries[BlockRegistry.RegistryName];

    /// <summary>
    /// The effect registry
    /// </summary>
    public EffectRegistry Effects => (EffectRegistry)registries[EffectRegistry.RegistryName];

    /// <summary>
    /// The enchantment registry
    /// </summary>
    public EnchantmentRegistry Enchantments => (EnchantmentRegistry)registries[EnchantmentRegistry.RegistryName];

    /// <summary>
    /// The entity registry
    /// </summary>
    public EntityRegistry Entities => (EntityRegistry)registries[EntityRegistry.RegistryName];

    /// <summary>
    /// The item registry
    /// </summary>
    public ItemRegistry Items => (ItemRegistry)registries[ItemRegistry.RegistryName];

    /// <summary>
    /// The particle registry
    /// </summary>
    public ParticleRegistry Particles => (ParticleRegistry)registries[ParticleRegistry.RegistryName];

    /// <summary>
    /// The menu registry
    /// </summary>
    public Registry<RegistryResource> Menus => (Registry<RegistryResource>)registries["menu"];
    
    /// <summary>
    /// Add a registry
    /// </summary>
    public void Add(IRegistry registry)
    {
        Add(registry.Name, registry);
    }

    #region IDictionary

    
    /// <inheritdoc />
    public int Count => registries.Count;
    
    /// <inheritdoc />
    public bool IsReadOnly => false;
    
    /// <inheritdoc />
    public IEnumerator<KeyValuePair<Identifier, IRegistry>> GetEnumerator()
        => registries.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public void Clear()
    {
        registries.Clear();
    }

    /// <inheritdoc />
    public void CopyTo(KeyValuePair<Identifier, IRegistry>[] array, int arrayIndex)
    {
        registries.ToArray().CopyTo(array, arrayIndex);
    }
    
    /// <inheritdoc />
    public bool Contains(KeyValuePair<Identifier, IRegistry> item)
    {
        if (!registries.TryGetValue(item.Key, out var registry))
        {
            return false;
        }
        
        return registry.Equals(item.Value);
    }

    /// <inheritdoc />
    public bool ContainsKey(Identifier key)
    {
        return registries.ContainsKey(key);
    }

    /// <inheritdoc />
    public bool Remove(Identifier key)
    {
        return registries.TryRemove(key, out _);
    }
    
    /// <inheritdoc />
    public bool Remove(KeyValuePair<Identifier, IRegistry> item)
    {
        return Remove(item.Key);
    }
    
    /// <inheritdoc />
    public void Add(Identifier key, IRegistry value)
    {
        if (!registries.TryAdd(key, value))
        {
            throw new ArgumentException("An item with the same key has already been added.");
        }
    }
    
    /// <inheritdoc />
    public void Add(KeyValuePair<Identifier, IRegistry> item)
    {
        Add(item.Key, item.Value);
    }

    /// <inheritdoc />
    public bool TryGetValue(Identifier key, [MaybeNullWhen(false)] out IRegistry value)
    {
        return registries.TryGetValue(key, out value);
    }

    /// <inheritdoc />
    public IRegistry this[Identifier key]
    {
        get => registries[key];
        set => registries[key] = value;
    }

    /// <inheritdoc />
    public ICollection<Identifier> Keys => registries.Keys;
    
    /// <inheritdoc />
    public ICollection<IRegistry> Values => registries.Values;

    #endregion
}
