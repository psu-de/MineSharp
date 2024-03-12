using MineSharp.Core.Common;
using MineSharp.Core.Geometry;

namespace MineSharp.Physics;

/// <summary>
/// Holds values about physic environment
/// </summary>
public class PlayerState
{
    /// <summary>
    /// Whether the entity has been touching water in the last tick
    /// </summary>
    public bool WasTouchingWater { get; internal set; }

    /// <summary>
    /// Whether the entity was underwater in the last tick
    /// </summary>
    public bool WasUnderwater { get; internal set; }

    /// <summary>
    /// The water height of the last tick (0 if entity is not in water)
    /// </summary>
    public double WaterHeight { get; internal set; }

    /// <summary>
    /// Lava height of the last tick (0 if entity is not in lava)
    /// </summary>
    public double LavaHeight { get; internal set; }

    /// <summary>
    /// Whether the entity is sprinting
    /// </summary>
    public bool IsSprinting { get; internal set; }

    /// <summary>
    /// Whether the entity is crouching
    /// </summary>
    public bool IsCrouching { get; internal set; }

    /// <summary>
    /// Whether the entity has been collided horizontally in the last tick
    /// </summary>
    public bool HorizontalCollision { get; internal set; }

    /// <summary>
    /// Whether the entity has been collided vertically in the last tick
    /// </summary>
    public bool VerticalCollision { get; internal set; }

    /// <summary>
    /// Whether the last horizontal collision was considered minor
    /// </summary>
    public bool MinorHorizontalCollision { get; internal set; }

    /// <summary>
    /// The last climbable position
    /// </summary>
    public Vector3 LastClimbablePosition = Vector3.Zero;

    /// <summary>
    /// Jumping delay
    /// </summary>
    public int NoJumpDelay { get; internal set; } = 0;

    /// <summary>
    /// If the player is stuck in a block (like cobweb), this stuck multiplier
    /// is applied
    /// </summary>
    public Vector3 StuckSpeedMultiplier { get; set; } = Vector3.Zero;
}
