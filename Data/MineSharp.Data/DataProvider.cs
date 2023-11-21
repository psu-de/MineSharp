using System.Diagnostics.CodeAnalysis;

namespace MineSharp.Data;

public class DataProvider<TEnum, T> where TEnum : Enum where T : class
{
    internal readonly DataVersion<TEnum, T> Version;
    
    internal DataProvider(DataVersion<TEnum, T> version)
    {
        this.Version = version;
    }

    public T GetByType(TEnum type) => this.Version.Palette[type];

    public bool TryGetByType(TEnum type, [NotNullWhen(true)] out T? block)
        => this.Version.Palette.TryGetValue(type, out block);

    public T this[TEnum type] => GetByType(type);
}
