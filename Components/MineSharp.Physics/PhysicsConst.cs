using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Geometry;

namespace MineSharp.Physics;

internal static class PhysicsConst
{
    public const double GRAVITY              = 0.08d;
    public const double SLOW_FALLING_GRAVITY = 0.01d;

    public const double VELOCITY_THRESHOLD = 0.003d;
    public const double VELOCITY_SCALE     = 0.0045000000000000005D;

    public const double HONEY_BLOCK_JUMP_FACTOR = 0.5d;

    public const double CLIMBING_SPEED = 0.15;

    public const double MAX_UP_STEP          = 0.6;
    public const double FLUID_JUMP_THRESHOLD = 0.4d;
    public const double FLUID_JUMP_FACTOR    = 0.04d;
    public const int    JUMP_DELAY           = 10;

    public const string ATTR_MOVEMENT_SPEED    = "generic.movement_speed";
    public const string SPRINTING_UUID         = "662a6b8d-da3e-4c1c-8813-96ea6097278d";
    public const double PLAYER_SPRINT_SPEED    = 0.3d;
    public const double DEFAULT_PLAYER_SPEED   = 0.1d;
    public const double FLYING_SPEED           = 0.02f;
    public const double SPRINTING_FLYING_SPEED = 0.025999999F;

    public static readonly Vector3 WaterDrag = new Vector3(0, -0.04, 0);

    public static double GetBlockFriction(BlockType type)
    {
        return type switch
        {
            BlockType.SlimeBlock => 0.8f,
            BlockType.Ice        => 0.98,
            BlockType.PackedIce  => 0.98,
            BlockType.FrostedIce => 0.98,
            BlockType.BlueIce    => 0.989,
            _                    => 0.6
        };
    }
}
