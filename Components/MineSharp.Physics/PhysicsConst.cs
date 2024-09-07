using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Geometry;

namespace MineSharp.Physics;

internal static class PhysicsConst
{
    public const double Gravity = 0.08d;
    public const double SlowFallingGravity = 0.01d;

    public const double VelocityThreshold = 0.003d;
    public const double VelocityScale = 0.0045000000000000005D;

    public const double HoneyBlockJumpFactor = 0.5d;

    public const double ClimbingSpeed = 0.15;

    public const double MaxUpStep = 0.6;
    public const double FluidJumpThreshold = 0.4d;
    public const double FluidJumpFactor = 0.04d;
    public const int JumpDelay = 10;

    public static readonly Identifier AttrMovementSpeed = Identifier.Parse("generic.movement_speed");
    public const string SprintingUuid = "662a6b8d-da3e-4c1c-8813-96ea6097278d";
    public const double PlayerSprintSpeed = 0.3d;
    public const double DefaultPlayerSpeed = 0.1d;
    public const double FlyingSpeed = 0.02f;
    public const double SprintingFlyingSpeed = 0.025999999F;

    public static readonly Vector3 WaterDrag = new(0, -0.04, 0);

    public static double GetBlockFriction(BlockType type)
    {
        return type switch
        {
            BlockType.SlimeBlock => 0.8f,
            BlockType.Ice => 0.98,
            BlockType.PackedIce => 0.98,
            BlockType.FrostedIce => 0.98,
            BlockType.BlueIce => 0.989,
            _ => 0.6
        };
    }
}
