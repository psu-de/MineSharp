using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Data;
using MineSharp.Physics.Input;
using MineSharp.World;

namespace MineSharp.Pathfinder.Utils;

internal static class CollisionHelper
{
    public static bool HasBlockSpaceForStanding(Vector3 pos, IWorld world, MinecraftData data)
    {
        var playerBB = GetPlayerBoundingBox(pos).Offset(0.5d, 0,0.5d);

        var targetBlock = world.GetBlockAt((Position)pos);
        var targetBBs = GetBoundingBoxes(targetBlock, data);
        
        if (targetBBs.Length == 0 || !targetBBs.Any(x => playerBB.Intersects(x)))
        {
            var blockBelow = world.GetBlockAt((Position)pos.Plus(Vector3.Down));
            var blockBelowBBs = GetBoundingBoxes(blockBelow, data);
            if (blockBelowBBs.Length > 0 && blockBelowBBs.All(x => playerBB.MinY - x.MaxY is >= 0 and < 0.2))
            {
                var blockAbove = world.GetBlockAt((Position)pos.Plus(Vector3.Up));
                var blockAboveBBs = GetBoundingBoxes(blockAbove, data);
                if (blockAboveBBs.Length == 0 || blockAboveBBs.All(x => x.MinY >= playerBB.MaxY)) // TODO: Sneaking
                {
                    return true;
                }
            }
        }

        return false;
    }

    internal static bool IntersectsWithBlock(AABB bb, Block block, MinecraftData data)
    {
        var blockBbs = GetBoundingBoxes(block, data);
        return blockBbs.Any(b => b.Intersects(bb));
    }

    internal static AABB[] GetBoundingBoxes(Block block, MinecraftData data)
    {
        return data.BlockCollisionShapes.GetForBlock(block)
            .Select(x => x.Offset(block.Position.X, block.Position.Y, block.Position.Z))
            .ToArray();
    }

    internal static AABB GetPlayerBoundingBox(Vector3 pos)
    {
        var bb = new AABB(-0.3, 0, -0.3, 0.3, 1.8, 0.3)
            .Offset(pos.X, pos.Y, pos.Z);
        return bb;
    }
}
