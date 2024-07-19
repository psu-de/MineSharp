using MineSharp.World.Containers;

namespace MineSharp.World.New.Chunks.Sections;

internal class ChunkSection114<TBitArray> : IChunkSection where TBitArray : IBitArray
{
    public short SolidBlockCount { get; set; }
    
    public int GetBlockStateAt(int x, int y, int z)
    {
        throw new NotImplementedException();
    }

    public void SetBlockStateAt(int state, int x, int y, int z)
    {
        throw new NotImplementedException();
    }

    public int GetBiomeStateAt(int x, int y, int z)
    {
        throw new NotImplementedException();
    }

    public void SetBiomeStateAt(int state, int x, int y, int z)
    {
        throw new NotImplementedException();
    }
}
