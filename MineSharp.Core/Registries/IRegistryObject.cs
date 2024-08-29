using MineSharp.Core.Common;

namespace MineSharp.Core.Registries;

/// <summary>
/// Represents an object that can be registered in a <see cref="Registry{T}"/>
/// </summary>
public interface IRegistryObject
{
    /// <summary>
    /// The numerical id for this object
    /// </summary>
    public int Id { get; }
    
    /// <summary>
    /// The identifier for this object
    /// </summary>
    public Identifier Name { get; }
}


/// <summary>
/// Represents an object that can be registered in a <see cref="Registry{T}"/>
/// </summary>
public interface IRegistryObject<out TEnum>
    : IRegistryObject
    where TEnum : struct, Enum
{
    /// <summary>
    /// The type of this object
    /// </summary>
    public TEnum Type { get; }
}
