using MineSharp.Core.Common.Blocks;
using System.Diagnostics.CodeAnalysis;

namespace MineSharp.Data.Blocks;

public class BlockProvider : DataProvider<BlockType, BlockInfo>
{
    private Dictionary<int, BlockInfo> IdToBlockMap { get; }
    private Dictionary<string, BlockInfo> NameToBlockMap { get; }
    private Dictionary<int, BlockInfo> StateToBlockMap { get; }


    public int TotalBlockStateCount => StateToBlockMap.Count;
    
    internal BlockProvider(DataVersion<BlockType, BlockInfo> version) : base(version)
    {
        IdToBlockMap = version.Palette.ToDictionary(x => x.Value.Id, x => x.Value);
        NameToBlockMap = version.Palette.ToDictionary(x => x.Value.Name, x => x.Value);
        StateToBlockMap = BuildStateMap();
    }

    private Dictionary<int, BlockInfo> BuildStateMap()
    {
        var map = new Dictionary<int, BlockInfo>();
        foreach (var block in this.Version.Palette)
        {
            for (int i = block.Value.MinState; i <= block.Value.MaxState; i++)
            {
                map.Add(i, block.Value);
            }
        }
        return map;
    }
    
    public BlockInfo GetById(int id) => IdToBlockMap[id];

    public bool TryGetById(int id, [NotNullWhen(true)] out BlockInfo? block)
        => IdToBlockMap.TryGetValue(id, out block);

    public BlockInfo GetByState(int state) => StateToBlockMap[state];

    public bool TryGetByState(int state, [NotNullWhen(true)] out BlockInfo? block)
        => StateToBlockMap.TryGetValue(state, out block);

    public BlockInfo GetByName(string name) => NameToBlockMap[name];

    public bool TryGetByName(string name, [NotNullWhen(true)] out BlockInfo? block)
        => NameToBlockMap.TryGetValue(name, out block);
    

    public BlockInfo this[int id] => GetById(id);

    public BlockInfo this[string name] => GetByName(name);
}
