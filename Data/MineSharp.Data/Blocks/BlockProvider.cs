using MineSharp.Core.Common.Blocks;
using System.Diagnostics.CodeAnalysis;

namespace MineSharp.Data.Blocks;

/// <summary>
/// Provides static data bout blocks for a version.
/// Indexes blocks by id, name, type and state.
/// </summary>
public class BlockProvider : DataProvider<BlockType, BlockInfo>
{
    private Dictionary<int, BlockInfo> IdToBlockMap { get; }
    private Dictionary<string, BlockInfo> NameToBlockMap { get; }
    private Dictionary<int, BlockInfo> StateToBlockMap { get; }


    /// <summary>
    /// The total number of block states.
    /// </summary>
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
    
    /// <summary>
    /// Get a Block inf o by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public BlockInfo GetById(int id) => IdToBlockMap[id];

    /// <summary>
    /// Try to get a block info by id. Returns false when not found.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="block"></param>
    /// <returns></returns>
    public bool TryGetById(int id, [NotNullWhen(true)] out BlockInfo? block)
        => IdToBlockMap.TryGetValue(id, out block);

    /// <summary>
    /// Get a block info by state
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public BlockInfo GetByState(int state) => StateToBlockMap[state];

    /// <summary>
    /// Tries to get a block info by state. Returns false when not found.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="block"></param>
    /// <returns></returns>
    public bool TryGetByState(int state, [NotNullWhen(true)] out BlockInfo? block)
        => StateToBlockMap.TryGetValue(state, out block);

    /// <summary>
    /// Get a BlockInfo by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public BlockInfo GetByName(string name) => NameToBlockMap[name];

    /// <summary>
    /// Try to get a BlockInfo by name. Returns false if not found
    /// </summary>
    /// <param name="name"></param>
    /// <param name="block"></param>
    /// <returns></returns>
    public bool TryGetByName(string name, [NotNullWhen(true)] out BlockInfo? block)
        => NameToBlockMap.TryGetValue(name, out block);
    

    /// <summary>
    /// Get a BlockInfo by id.
    /// </summary>
    /// <param name="id"></param>
    public BlockInfo this[int id] => GetById(id);

    /// <summary>
    /// Get a BlockInfo by name
    /// </summary>
    /// <param name="name"></param>
    public BlockInfo this[string name] => GetByName(name);
}
