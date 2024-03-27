using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.World;
using MineSharp.World.Iterators;

namespace MineSharp.Physics.Utils;

internal static class WorldUtils
{
    public static AABB[] GetWorldBoundingBoxes(AABB bb, IWorld world, MinecraftData data)
    {
        var iterator = new BoundingBoxIterator(bb);

        var blocks = iterator.Iterate()
                             .Select(world.GetBlockAt)
                             .Where(x => x.IsSolid()); // TODO: IsSolid() is not the same as in java

        var bbs = new List<AABB>();
        foreach (var block in blocks)
        {
            var shapes = data.BlockCollisionShapes.GetForBlock(block);
            bbs.AddRange(shapes.Select(x => x.Clone().Offset(block.Position.X, block.Position.Y, block.Position.Z)));
        }

        return bbs.ToArray();
    }

    public static bool CollidesWithWorld(AABB bb, IWorld world, MinecraftData data)
    {
        return GetWorldBoundingBoxes(bb, world, data)
           .Any(y => y.Intersects(bb));
    }

    public static bool IsOnClimbable(MinecraftPlayer player, IWorld world, ref Vector3 lastClimbPosition)
    {
        if (player.GameMode == GameMode.Spectator)
            return false;

        var position    = (Position)player.Entity!.Position;
        var blockAtFeet = world.GetBlockAt(position);

        // TODO: Change once tags are implemented
        if (blockAtFeet.Info.Type is BlockType.GlowLichen or BlockType.Ladder or BlockType.Scaffolding
            or BlockType.TwistingVines or BlockType.Vine or BlockType.CaveVines or BlockType.WeepingVines)
        {
            lastClimbPosition = position;
            return true;
        }

        if (!blockAtFeet.Info.Name.StartsWith("trapdoor"))
            return false;

        var blockBelowPos = (Position)Vector3.Down.Plus(blockAtFeet.Position);
        var blockBelow    = world.GetBlockAt(blockBelowPos);
        if (!blockBelow.GetProperty<bool>("open")
         || blockBelow.GetProperty<string>("facing") != blockAtFeet.GetProperty<string>("facing"))
            return false;

        lastClimbPosition = position;
        return true;
    }

    public static double GetBlockJumpFactor(Position position, IWorld world)
    {
        var below      = (Position)Vector3.Down.Plus(position);
        var blockBelow = world.GetBlockAt(below);

        return blockBelow.Info.Type == BlockType.HoneyBlock
            ? PhysicsConst.HONEY_BLOCK_JUMP_FACTOR
            : 1.0f;
    }
}
