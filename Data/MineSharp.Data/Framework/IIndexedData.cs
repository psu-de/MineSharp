
namespace MineSharp.Data.Framework;

/// <summary>
/// Interface for implementing indexed data
/// </summary>
/// <typeparam name="TEnum"></typeparam>
/// <typeparam name="TInfo"></typeparam>
public interface IIndexedData<in TEnum, out TInfo> where TEnum : Enum where TInfo : class 
{
    /// <summary>
    /// Get <typeparamref name="TInfo"/> by type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public TInfo ByType(TEnum type);
    
    /// <summary>
    /// Get <typeparamref name="TInfo"/> by numeric id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public TInfo ById(int id);
  
    /// <summary>
    /// Get <typeparamref name="TInfo"/> by name id
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public TInfo ByName(string name);

    /// <inheritdoc cref="ByType"/>
    public TInfo this[TEnum type] => ByType(type);
    
    /// <inheritdoc cref="ById"/>
    public TInfo this[int id] => ById(id);

    /// <inheritdoc cref="ByName"/>
    public TInfo this[string name] => ByName(name);
}
