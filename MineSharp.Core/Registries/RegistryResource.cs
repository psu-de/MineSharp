using MineSharp.Core.Common;

namespace MineSharp.Core.Registries;

/// <summary>
/// Represents the simplest object a registry can store.
/// </summary>
/// <param name="Name">The identifier of the object</param>
/// <param name="Id">The numerical id of the object</param>
public record RegistryResource(Identifier Name, int Id) : IRegistryObject;

/// <summary>
/// Represents the simplest object in a registry with a corresponding type enum.
/// </summary>
public record RegistryResource<TEnum>(Identifier Name, TEnum Type, int Id) : RegistryResource(Name, Id), IRegistryObject<TEnum>
    where TEnum : struct, Enum;
