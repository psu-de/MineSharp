using MineSharp.Core.Common;

namespace MineSharp.Core.Registries;

/// <summary>
/// Represents a registry.
/// A registry is a mapping from a version independent Identifier to a version specific numerical id and vice versa.
/// </summary>
public interface IRegistry
{
    /// <summary>
    /// The name of this registry
    /// </summary>
    public Identifier Name { get; }

    /// <summary>
    /// Get the Identifier associated with the given numerical id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Identifier ByProtocolId(int id);
    
    /// <summary>
    /// Get the numerical id associated with the given identifier.
    /// </summary>
    /// <param name="key">the identifier</param>
    /// <returns></returns>
    public int GetProtocolId(Identifier key);
}

/// <inheritdoc cref="IRegistry"/>
public interface IRegistry<T> : IRegistry, ICollection<T>
    where T : IRegistryObject
{
    /// <summary>
    /// Get a resource by numerical id
    /// </summary>
    public T ById(int id);

    /// <summary>
    /// Get a resource by name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public T ByName(Identifier name);
}

/// <inheritdoc cref="IRegistry{T}"/>
/// also gives access to resources by <typeparamref name="TEnum"/>
public interface IRegistry<T, in TEnum> : IRegistry<T>
    where T : IRegistryObject<TEnum>
    where TEnum : struct, Enum
{
    /// <summary>
    /// Get a resource by TEnum type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public T ById(TEnum type);
}
