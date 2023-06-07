using MineSharp.Core.Common.Blocks;
using MineSharp.Data.Exceptions;
using MineSharp.Data.Json;
using Newtonsoft.Json;

namespace MineSharp.Data.Providers;

public class CollisionDataProvider : IDataProvider
{
    private readonly Dictionary<int, float[][]> _cache;
    private readonly string _path;

    public bool IsLoaded { get; private set; }

    public CollisionDataProvider(string collisionPath)
    {
        this._path = collisionPath;
        this._cache = new Dictionary<int, float[][]>();

        this.IsLoaded = false;
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
    
    private void LoadData()
    {
        CollisionDataBlob collisionData = JsonConvert.DeserializeObject<CollisionDataBlob>(File.ReadAllText(this._path))!;

        foreach (var entry in collisionData.Shapes)
        {
            this._cache.Add(entry.Key, entry.Value);
        }
    }

    public float[][] Get(int index)
    {
        if (!this._cache.TryGetValue(index, out var shape))
        {
            throw new MineSharpDataException($"The Block with id {index} was not found.");
        }

        return shape;
    }

    public float[][] GetForBlock(Block block)
    {
        var idx = 0;
        if (block.Info.BlockShapeIndices.Length > 1)
        {
            idx = block.Metadata;
        }
        var shapeData = this.Get(idx);

        return shapeData;
    }
}
