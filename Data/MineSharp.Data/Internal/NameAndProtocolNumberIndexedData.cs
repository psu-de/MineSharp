using System.Collections.Frozen;
using MineSharp.Core.Common;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;

namespace MineSharp.Data.Internal;

internal class NameAndProtocolNumberIndexedData(IDataProvider<IReadOnlyDictionary<Identifier, int>> provider) 
    : IndexedData<IReadOnlyDictionary<Identifier, int>>(provider), INameAndProtocolNumberIndexedData
{
    public int Count { get; private set; }

    protected IReadOnlyDictionary<int, Identifier>? ProtocolNumberToName;
    protected IReadOnlyDictionary<Identifier, int>? NameToProtocolNumber;
    
    protected override void InitializeData(IReadOnlyDictionary<Identifier, int> data)
    {
        Count = data.Count;
        NameToProtocolNumber = data.ToFrozenDictionary();
        ProtocolNumberToName = NameToProtocolNumber.ToFrozenDictionary(x => x.Value, x => x.Key);
    }

    public int GetProtocolId(Identifier name)
    {
        if (!Loaded)
        {
            Load();
        }
        
        return NameToProtocolNumber![name];
    }

    public Identifier GetName(int id)
    {        
        if (!Loaded)
        {
            Load();
        }
        
        return ProtocolNumberToName![id];
    }
}

internal class NameAndProtocolNumberIndexedData<TEnum>(IDataProvider<IReadOnlyDictionary<Identifier, int>> provider) 
    : NameAndProtocolNumberIndexedData(provider), INameAndProtocolNumberIndexedData<TEnum>
    where TEnum : struct, Enum
{
    private readonly EnumNameLookup<TEnum> enumNameLookup = new();
    private IReadOnlyDictionary<int, TEnum>? protocolNumberToType;
    private IReadOnlyDictionary<TEnum, int>? typeToProtocolNumber;
    
    protected override void InitializeData(IReadOnlyDictionary<Identifier, int> data)
    {
        base.InitializeData(data);
        
        protocolNumberToType = ProtocolNumberToName!
           .ToFrozenDictionary(
                x => x.Key, 
                x => enumNameLookup.FromName(NameUtils.GetParticleName(x.Value.Name)));
        typeToProtocolNumber = protocolNumberToType.ToFrozenDictionary(x => x.Value, x => x.Key);
    }

    public int GetProtocolId(TEnum type)
    {
        if (!Loaded)
        {
            Load();
        }
        
        return typeToProtocolNumber![type];
    }

    public TEnum GetType(int id)
    {
        if (!Loaded)
        {
            Load();
        }
        
        return protocolNumberToType![id];
    }
}
