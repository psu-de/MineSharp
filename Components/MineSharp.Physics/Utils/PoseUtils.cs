using MineSharp.Core.Common;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.World;

namespace MineSharp.Physics.Utils;

internal static class PoseUtils
{
    private static readonly IDictionary<EntityPose, AABB> PoseDimensions = new Dictionary<EntityPose, AABB>()
    {
        { EntityPose.Standing, CreateAABB(0.6f, 1.6f) },
        { EntityPose.Sleeping, CreateAABB(0.2f, 0.2f) },
        { EntityPose.FallFlying, CreateAABB(0.6f, 0.6f) },
        { EntityPose.Swimming, CreateAABB(0.6f, 0.6f) },
        { EntityPose.SpinAttack, CreateAABB(0.6f, 0.6f) },
        { EntityPose.Crouching, CreateAABB(0.6f, 1.5f) },
        { EntityPose.Dying, CreateAABB(0.2f, 0.2f) },
    };

    public static MutableAABB GetBBForPose(EntityPose pose)
        => PoseDimensions[pose].Clone();

    public static bool WouldPlayerCollideWithPose(MinecraftPlayer player, EntityPose pose, IWorld world, MinecraftData data)
    {
        var bb = GetBBForPose(pose);
        bb.Offset(
            player.Entity!.Position.X,
            player.Entity.Position.Y,
            player.Entity.Position.Z);
        bb.Deflate(1.0E-7D, 1.0E-7D, 1.0E-7D);

        return WorldUtils.CollidesWithWorld(bb, world, data);
    }


    private static AABB CreateAABB(float width, float height)
    {
        var half = width / 2.0f;
        return new AABB(-half, 0, -half, half, height, half);
    }
}
