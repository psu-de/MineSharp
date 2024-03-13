using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.Physics.Input;
using MineSharp.World;
using MineSharp.World.Iterators;

namespace MineSharp.Pathfinder.Utils;

internal static class CollisionHelper
{
    private static readonly AABB BlockBb = new(0.0, 0.0, 0.0, 1.0, 1.0, 1.0); 
    
    public static bool CollidesWithWord(AABB aabb, IWorld world)
    {
        var iterator = new BoundingBoxIterator(aabb);
        
        foreach (var position in iterator.Iterate())
        {
            var block = world.GetBlockAt(position);
            var boxes = world.Data.BlockCollisionShapes.GetForBlock(block);

            foreach (var box in boxes)
            {
                box.Offset(block.Position.X, block.Position.Y, block.Position.Z);

                if (aabb.Intersects(box))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool IsPointInBlockBb(Vector3 position, Position block)
    {
        var pos = position.Minus(block.X, block.Y, block.Z);
        return BlockBb.Contains(pos);
    }

    public static bool IsXzPointInBlockBb(Vector3 position, Position block)
    {
        var pos = position.Minus(block.X, position.Y, block.Z);
        return BlockBb.Contains(pos);
    }

    public static bool IntersectsBbWithBlock(AABB bb, Position block)
    {
        // TODO: not correct, (min.x, max.z) can be on block, but is not detected
        // return IsPositionInBlock(bb.Min, block) || IsPositionInBlock(bb.Max, block);
        return BlockBb.Intersects(bb.Clone().Offset(-block.X, -block.Y, -block.Z));
    }

    public static AABB[] GetBoundingBoxes(Block block, MinecraftData data)
    {
        return data.BlockCollisionShapes.GetForBlock(block)
            .Select(x => x.Offset(block.Position.X, block.Position.Y, block.Position.Z))
            .ToArray();
    }

    public static AABB SetAABBToPlayerBB(Vector3 pos)
    {
        var bb = new AABB(-0.3, 0, -0.3, 0.3, 1.8, 0.3)
            .Offset(pos.X, pos.Y, pos.Z);
        return bb;
    }
    
    public static AABB SetAABBToPlayerBB(Vector3 pos, ref AABB aabb)
    {
        aabb.Min.X = -0.3;
        aabb.Min.Y = 0;
        aabb.Min.Z = -0.3;
        aabb.Max.X = 0.3;
        aabb.Max.Y = 1.8;
        aabb.Max.Z = 0.3;
        return aabb.Offset(pos.X, pos.Y, pos.Z);
    }
}
