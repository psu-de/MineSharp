using System.Reflection;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;

namespace MineSharp.Data.Internal;

internal class TypeIdNameIndexedData<TEnum, TInfo>(IDataProvider<TInfo[]> provider)
    : IndexedData<TInfo[]>(provider), ITypeIdNameIndexedData<TEnum, TInfo>
    where TEnum : struct, Enum
    where TInfo : class
{
    public int Count { get; private set; } = -1;

    private readonly Dictionary<TEnum, TInfo>  typeToInfo = new();
    private readonly Dictionary<int, TInfo>    idToInfo   = new();
    private readonly Dictionary<string, TInfo> nameToInfo = new();

    protected override void InitializeData(TInfo[] data)
    {
        this.Count = data.Length;

        var tInfo     = typeof(TInfo);
        var idField   = tInfo.GetProperty("Id")!;
        var typeField = tInfo.GetProperty("Type")!;
        var nameField = tInfo.GetProperty("Name")!;

        foreach (var entry in data)
        {
            typeToInfo.Add((TEnum)typeField.GetValue(entry)!, entry);
            nameToInfo.Add((string)nameField.GetValue(entry)!, entry);
            idToInfo.Add((int)idField.GetValue(entry)!, entry);
        }
    }

    public TInfo? ByType(TEnum type)
    {
        if (!this.Loaded)
            this.Load();

        this.typeToInfo.TryGetValue(type, out var value);
        return value;
    }

    public TInfo? ById(int id)
    {
        if (!this.Loaded)
            this.Load();

        this.idToInfo.TryGetValue(id, out var value);
        return value;
    }

    public TInfo? ByName(string name)
    {
        if (!this.Loaded)
            this.Load();

        this.nameToInfo.TryGetValue(name, out var value);
        return value;
    }
}
