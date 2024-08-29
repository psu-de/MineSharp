namespace MineSharp.Data.Framework.Providers;

/// <summary>
///     Interface for implementing a data provider
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IDataProvider<T>
{
    /// <summary>
    ///     Return all data entries
    /// </summary>
    /// <returns></returns>
    public T GetData();
}
