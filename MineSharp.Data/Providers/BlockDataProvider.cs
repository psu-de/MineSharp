using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Blocks.Property;
using MineSharp.Data.Exceptions;
using MineSharp.Data.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;

namespace MineSharp.Data.Providers;

public class BlockDataProvider : IDataProvider
{
    private readonly Dictionary<int, BlockInfo> _byId;
    private readonly Dictionary<string, BlockInfo> _byName;
    private readonly Dictionary<int, BlockInfo> _byBlockState;
    
    private readonly string _blocksPath;
    private readonly string _collisionsPath;
    
    public int TotalBlockStateCount => this._byBlockState.Count;
    public bool IsLoaded { get; private set; }

    private IList<int> _unsolidBlocks;
    
    public BlockDataProvider(string blocksPath, string collisionsPath)
    {
        this._blocksPath = blocksPath;
        this._collisionsPath = collisionsPath;
        this._byId = new Dictionary<int, BlockInfo>();
        this._byName = new Dictionary<string, BlockInfo>();
        this._byBlockState = new Dictionary<int, BlockInfo>();
        this._unsolidBlocks = new List<int>();
        
        this.IsLoaded = false;
    }

    public BlockInfo GetByState(int state)
    {
        return this._byBlockState[state];
    }

    public bool TryGetById(int id, [NotNullWhen(true)] out BlockInfo? info)
    {
        return this._byId.TryGetValue(id, out info);
    }

    public bool TryGetByName(string name, [NotNullWhen(true)] out BlockInfo? info)
    {
        return this._byName.TryGetValue(name, out info);
    }
    
    public BlockInfo GetById(int id)
    {
        if (!this._byId.TryGetValue(id, out var bi))
        {
            throw new MineSharpDataException($"The Block with id {id} was not found.");
        }

        return bi;
    }
    
    public BlockInfo GetByName(string name)
    {
        if (!this._byName.TryGetValue(name, out var bi))
        {
            throw new MineSharpDataException($"The Block with name {name} was not found.");
        }

        return bi;
    }

    
    public bool IsSolid(int id)
    {
        return !this._unsolidBlocks.Contains(id);
    }
    
    private void LoadData()
    {
        BlockInfoBlob[] blockData = JsonConvert.DeserializeObject<BlockInfoBlob[]>(File.ReadAllText(this._blocksPath))!;
        CollisionDataBlob collisionData = JsonConvert.DeserializeObject<CollisionDataBlob>(File.ReadAllText(this._collisionsPath))!;

        foreach (var blockBlob in blockData)
        {
            var bi = new BlockInfo(
                blockBlob.Id,
                blockBlob.Name,
                blockBlob.DisplayName,
                blockBlob.Hardness ?? float.MaxValue,
                blockBlob.Resistance,
                blockBlob.Diggable,
                blockBlob.Transparent,
                blockBlob.FilterLight,
                blockBlob.EmitLight,
                blockBlob.BoundingBox,
                blockBlob.StackSize,
                blockBlob.Material ?? "",
                blockBlob.DefaultState!.Value,
                blockBlob.MinStateId!.Value,
                blockBlob.MaxStateId!.Value,
                blockBlob.HarvestTools?.Keys.Select(x => Convert.ToInt32(x)).ToArray(),
                this.GetBlockProperties(blockBlob.States!),
                this.GetShapeEntries(collisionData.Blocks[blockBlob.Name]));
            
            this._byId.Add(blockBlob.Id, bi);
            this._byName.Add(blockBlob.Name, bi);

            for (int i = bi.MinStateId; i <= bi.MaxStateId; i++)
            {
                this._byBlockState.Add(i, bi);
            }
        }
        
        this._unsolidBlocks.Add(this.GetByName("air").Id);
        if (this.TryGetByName("cave_air", out var caveAir))
            this._unsolidBlocks.Add(caveAir.Id);
        if (this.TryGetByName("void_air", out var voidAir))
            this._unsolidBlocks.Add(voidAir.Id);
    }
    
    public void Load()
    {
        if (IsLoaded)
        {
            return;
        }
        
        this.LoadData();

        this.IsLoaded = true;
    }

    private int[] GetShapeEntries(object value)
    {
        return value switch {
            int i => new[] { i },
            long l => new[] { (int)l },
            JArray arr => arr.Select(x => x.Value<int>()).ToArray(),
            _ => throw new MineSharpDataException("Invalid block data.")
        };
    }

    private BlockProperties GetBlockProperties(BlockStateBlob[] blobs)
    {
        BlockProperty[] properties = blobs.Select(this.GetProperty).ToArray();
        return new BlockProperties(properties);
    }

    private BlockProperty GetProperty(BlockStateBlob blob)
    {
        return blob.Type switch {
            "bool" => new BoolProperty(blob.Name),
            "int" => new IntProperty(blob.Name, blob.NumValues),
            "enum" => new StringProperty(blob.Name, blob.Values!),
            _ => throw new Exception("Unexpected data")
        };
    }
}
