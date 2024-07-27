using MineSharp.Core.Common;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.World;

namespace MineSharp.Physics.Utils;

internal static class PoseUtils
{
    private static readonly IDictionary<EntityPose, Aabb> PoseDimensions = new Dictionary<EntityPose, Aabb>
    {
        { EntityPose.Standing, CreateAabb(0.6f,   1.6f) },
        { EntityPose.Sleeping, CreateAabb(0.2f,   0.2f) },
        { EntityPose.FallFlying, CreateAabb(0.6f, 0.6f) },
        { EntityPose.Swimming, CreateAabb(0.6f,   0.6f) },
        { EntityPose.SpinAttack, CreateAabb(0.6f, 0.6f) },
        { EntityPose.Crouching, CreateAabb(0.6f,  1.5f) },
        { EntityPose.Dying, CreateAabb(0.2f,      0.2f) }
    };

    public static MutableAabb GetBbForPose(EntityPose pose)
    {
        return PoseDimensions[pose].Clone();
    }

    public static bool WouldPlayerCollideWithPose(MinecraftPlayer player, EntityPose pose, IWorld world,
                                                  MinecraftData data)
    {
        var bb = GetBbForPose(pose);
        bb.Offset(
            player.Entity!.Position.X,
            player.Entity.Position.Y,
            player.Entity.Position.Z);
        bb.Deflate(1.0E-7D, 1.0E-7D, 1.0E-7D);

        return WorldUtils.CollidesWithWorld(bb, world, data);
    }


    private static Aabb CreateAabb(float width, float height)
    {
        var half = width / 2.0f;
        return new(-half, 0, -half, half, height, half);
    }
}
