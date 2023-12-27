using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using System.Diagnostics.CodeAnalysis;

namespace MineSharp.Data.BlockCollisionShapes;

/// <summary>
/// Provides Block collision data.
/// Indexes collision shapes by block type.
/// </summary>
public class BlockCollisionShapesProvider
{

    private readonly BlockCollisionShapesVersion _version;
    
    internal BlockCollisionShapesProvider(BlockCollisionShapesVersion version)
    {
        this._version = version;
    }

    /// <summary>
    /// Return all shape indices for the given block type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int[] GetShapeIndices(BlockType type)
        => this._version.BlockToIndicesMap[type];

    /// <summary>
    /// Tries to get all shape indices for the given block type.
    /// Returns false if not found.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="indices"></param>
    /// <returns></returns>
    public bool TryGetShapeIndices(BlockType type, [NotNullWhen(true)] out int[]? indices)
        => this._version.BlockToIndicesMap.TryGetValue(type, out indices);

    /// <summary>
    /// Get collision shapes for blocks of type <paramref name="type"/>
    /// </summary>
    /// <param name="type"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public AABB[] GetShapes(BlockType type, int index)
    {
        var shapes = GetShapeIndices(type);
        var shape = shapes.Length > 1 ? shapes[index] : shapes[0];
        return this._version.BlockShapes[shape]
            .Select(x => x.Clone())
            .ToArray();   
    }

    /// <summary>
    /// Returns collision shapes for a block
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    public AABB[] GetForBlock(Block block)
        => GetShapes(block.Info.Type, block.Metadata);
}
