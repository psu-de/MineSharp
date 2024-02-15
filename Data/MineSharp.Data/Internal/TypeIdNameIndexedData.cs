using System.Reflection;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;

namespace MineSharp.Data.Internal;

internal class TypeIdNameIndexedData<TEnum, TInfo>(IDataProvider<TInfo[]> provider)
    : IndexedData<TInfo[]>(provider), ITypeIdNameIndexedData<TEnum, TInfo>
    where TEnum : struct, Enum
    where TInfo : class
{
    private readonly Dictionary<TEnum, TInfo> typeToInfo = new();
    private readonly Dictionary<int, TInfo> idToInfo = new();
    private readonly Dictionary<string, TInfo> nameToInfo = new();
    
    protected override void InitializeData(TInfo[] data)
    {
        var tInfo = typeof(TInfo);
        var idField = tInfo.GetField("Id", BindingFlags.Instance)!;
        var typeField = tInfo.GetField("Type", BindingFlags.Instance)!;
        var nameField = tInfo.GetField("Name", BindingFlags.Instance)!;
        
        foreach (var entry in data)
        {
            typeToInfo.Add((TEnum)typeField.GetValue(entry)!, entry);
            nameToInfo.Add((string)nameField.GetValue(entry)!, entry);
            idToInfo.Add((int)idField.GetValue(entry)!, entry);
        }
    }

    public TInfo ByType(TEnum type)
    {
        if (!this.Loaded)
            this.Load();

        return this.typeToInfo[type];
    }

    public TInfo ById(int id)
    {
        if (!this.Loaded)
            this.Load();

        return this.idToInfo[id];
    }

    public TInfo ByName(string name)
    {
        if (!this.Loaded)
            this.Load();

        return this.nameToInfo[name];
    }
}