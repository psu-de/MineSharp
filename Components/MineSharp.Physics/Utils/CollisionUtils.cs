using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.World;
using MineSharp.World.Iterators;

namespace MineSharp.Physics.Utils;

public static class CollisionUtils
{
    public static bool CollidesWithWorld(AABB bb, IWorld world, MinecraftData data)
    {
        var iterator = new BoundingBoxIterator(bb);

        return iterator.Iterate()
            .Select(world.GetBlockAt)
            .Where(x => x.IsSolid())
            .Select(data.BlockCollisionShapes.GetForBlock)
            .Any(x => x.Any(y => y.Intersects(bb)));
    }
}
