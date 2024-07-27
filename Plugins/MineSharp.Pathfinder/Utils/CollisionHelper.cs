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
    private static readonly Aabb BlockBb = new(0.0, 0.0, 0.0, 1.0, 1.0, 1.0); 
    
    public static bool CollidesWithWord(Aabb aabb, IWorld world, MinecraftData data)
    {
        var iterator = new BoundingBoxIterator(aabb);
        
        foreach (var position in iterator.Iterate())
        {
            var block = world.GetBlockAt(position);
            var boxes = data.BlockCollisionShapes.GetForBlock(block);

            foreach (var box in boxes)
            {
                box.Clone().Offset(block.Position.X, block.Position.Y, block.Position.Z);

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

    public static bool IntersectsBbWithBlock(Aabb bb, Position block)
    {
        // TODO: not correct, (min.x, max.z) can be on block, but is not detected
        // return IsPositionInBlock(bb.Min, block) || IsPositionInBlock(bb.Max, block);
        return BlockBb.Intersects(bb.Clone().Offset(-block.X, -block.Y, -block.Z));
    }

    public static bool IntersectsBbWithBlockXz(Aabb bb, Position block)
    {
        return BlockBb.Intersects(bb.Clone().Offset(-block.X, -bb.Min.Y, -block.Z));
    }

    public static MutableAabb[] GetBoundingBoxes(Block block, MinecraftData data)
    {
        return data.BlockCollisionShapes.GetForBlock(block)
            .Select(x => x.Clone().Offset(block.Position.X, block.Position.Y, block.Position.Z))
            .ToArray();
    }

    public static MutableAabb GetAabbForPlayer(Vector3 pos)
    {
        var bb = new MutableAabb(-0.3, 0, -0.3, 0.3, 1.8, 0.3)
            .Offset(pos.X, pos.Y, pos.Z);
        return bb;
    }
    
    public static MutableAabb SetAabbToPlayerBB(Vector3 pos, MutableAabb Aabb)
    {
        Aabb.Min.Set(-0.3, 0, -0.3);
        Aabb.Max.Set(0.3, 1.8, 0.3);

        return Aabb.Offset(pos.X, pos.Y, pos.Z);
    }
}
