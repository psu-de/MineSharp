using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using MineSharp.Core.Geometry;
using MineSharp.World.Chunks;
using MineSharp.World.New.Chunks;

namespace MineSharp.World.New;

public class World<TChunk> where TChunk : Chunk
{
    private ConcurrentDictionary<ulong, TChunk> chunks = new();

    [Pure]
    public bool TryGetChunkAt(int chunkX, int chunkZ, [NotNullWhen(true)] out TChunk? chunk)
    {
        var idx = GetChunkCoordinateHash(chunkX, chunkZ);
        return chunks.TryGetValue(idx, out chunk);
    }
    
    [Pure]
    public bool TryGetChunkAt(ChunkCoordinates coordinates, [NotNullWhen(true)] out TChunk? chunk)
        => TryGetChunkAt(coordinates.X, coordinates.Z, out chunk);
    
    [Pure]
    public bool TryGetChunkAt(Position position, out TChunk? chunk)
        => TryGetChunkAt(
            ToChunkCoordinate(position.X),
            ToChunkCoordinate(position.Z),
            out chunk);

    [Pure]
    public bool IsChunkLoaded(int chunkX, int chunkZ)
        => TryGetChunkAt(chunkX, chunkZ, out _);
    
    [Pure]
    public bool IsChunkLoaded(ChunkCoordinates coordinates)
        => TryGetChunkAt(coordinates, out _);
    
    [Pure]
    public bool IsChunkLoaded(Position position)
        => TryGetChunkAt(position, out _);
    
    
    private static ulong GetChunkCoordinateHash(int x, int z)
    {
        var ux = (ulong)x;
        var uz = (ulong)z;
        return ux << 32 | uz;
    }

    private static int ToChunkCoordinate(int coordinate)
        => coordinate >> (Chunk.SIZE >> 2);
}
