using System.Diagnostics.CodeAnalysis;

namespace MineSharp.Data;

/// <summary>
/// Base Data provider.
/// Indexes <typeparamref name="T"/> by <typeparamref name="TEnum"/>
/// </summary>
/// <typeparam name="TEnum"></typeparam>
/// <typeparam name="T"></typeparam>
public class DataProvider<TEnum, T> where TEnum : Enum where T : class
{
    internal readonly DataVersion<TEnum, T> Version;
    
    internal DataProvider(DataVersion<TEnum, T> version)
    {
        this.Version = version;
    }

    /// <summary>
    /// Get a <typeparamref name="T"/> by <typeparamref name="TEnum"/>
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public T GetByType(TEnum type) => this.Version.Palette[type];

    /// <summary>
    /// Try to get a <typeparamref name="T"/> by <typeparamref name="TEnum"/>
    /// </summary>
    /// <param name="type"></param>
    /// <param name="block"></param>
    /// <returns></returns>
    public bool TryGetByType(TEnum type, [NotNullWhen(true)] out T? block)
        => this.Version.Palette.TryGetValue(type, out block);

    /// <summary>
    /// Get a <typeparamref name="T"/> by <typeparamref name="TEnum"/>
    /// </summary>
    /// <param name="type"></param>
    public T this[TEnum type] => GetByType(type);
}
