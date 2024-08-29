using System.Collections;
using MineSharp.Core.Common;

namespace MineSharp.Core.Registries;

/// <summary>
/// Represents a registry with data objects
/// </summary>
/// <param name="name">The name of this registry</param>
/// <typeparam name="T">The resource object type</typeparam>
public class Registry<T>(Identifier name) : IRegistry<T>
    where T : IRegistryObject
{
    private readonly IDictionary<Identifier, T> byName = new Dictionary<Identifier, T>();
    private readonly IDictionary<int, T> byId = new Dictionary<int, T>();
    
    /// <inheritdoc />
    public Identifier Name => name;

    /// <summary>
    /// All values stored in this registry
    /// </summary>
    public ICollection<T> Values => byName.Values;

    /// <inheritdoc />
    public Identifier ByProtocolId(int id)
        => byId[id].Name;

    /// <inheritdoc />
    public int GetProtocolId(Identifier key)
        => byName[key].Id;

    /// <summary>
    /// Get a resource by numerical id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public T ById(int id)
        => byId[id];

    /// <summary>
    /// Get a resource by name
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public T ByName(Identifier key)
        => byName[key];

    /// <summary>
    /// Register a new resource
    /// </summary>
    /// <param name="obj"></param>
    public virtual void Register(T obj)
    {
        byId.Add(obj.Id, obj);
        byName.Add(obj.Name, obj);
        
        OnRegistered(obj);
    }
    
    /// <summary>
    /// Invoked when a new entry has been registered
    /// </summary>
    protected virtual void OnRegistered(T obj)
    { }

    #region ICollection
    
    
    /// <inheritdoc />
    public int Count => byName.Count;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public void Add(T item)
    {
        Register(item);
    }

    /// <inheritdoc />
    public void Clear()
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public bool Contains(T item)
    {
        return byId.ContainsKey(item.Id);
    }

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex)
    {
        byId.Values.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc />
    public bool Remove(T item)
    {
        throw new NotSupportedException();
    }
    
    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        return byName.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion
}

/// <summary>
/// Represents a registry that indexes entries by name, id and a type enum
/// </summary>
public class Registry<T, TEnum>(Identifier name) : Registry<T>(name)
    where TEnum : struct, Enum
    where T : IRegistryObject<TEnum>
{
    private readonly IDictionary<TEnum, T> byType = new Dictionary<TEnum, T>();

    /// <summary>
    /// Get a resource by <typeparamref name="TEnum"/>
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public T ByType(TEnum type)
        => byType[type];

    /// <inheritdoc />
    public override void Register(T obj)
    {
        base.Register(obj);
        byType.Add(obj.Type, obj);
    }
}
