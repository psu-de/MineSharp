using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Entities;
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

    public static bool IsPositionInBlock(Vector3 position, Position block)
    {
        var pos = position.Minus(block.X, block.Y, block.Z);
        return BlockBb.Contains(pos);
    }

    public static bool IsXZPositionInBlock(Vector3 position, Position block)
    {
        var pos = position.Minus(block.X, position.Y, block.Z);
        return BlockBb.Contains(pos);
    }

    public static bool IsOnPositionXZ(double x, double z, Position block)
    {
        return (int)Math.Floor(x) == block.X
            && (int)Math.Floor(z) == block.Z;
    }

    public static bool IsOnPosition(Vector3 position, Position block)
    {
        return (int)Math.Floor(position.X) == block.X
            && (int)Math.Floor(position.Y) == block.Y
            && (int)Math.Floor(position.Z) == block.Z;
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
