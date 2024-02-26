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

    public static bool IsOnPositionXZ(double x, double z, Position block)
    {
        return (int)x == block.X
            && (int)z == block.Z;
    }

    public static bool IsOnPosition(Vector3 position, Position block)
    {
        return (int)position.X == block.X
            && (int)position.Y == block.Y
            && (int)position.Z == block.Z;
    }

    public static AABB[] GetBoundingBoxes(Block block, MinecraftData data)
    {
        return data.BlockCollisionShapes.GetForBlock(block)
            .Select(x => x.Offset(block.Position.X, block.Position.Y, block.Position.Z))
            .ToArray();
    }

    public static AABB GetPlayerBoundingBox(Vector3 pos)
    {
        var bb = new AABB(-0.3, 0, -0.3, 0.3, 1.8, 0.3)
            .Offset(pos.X, pos.Y, pos.Z);
        return bb;
    }
}
