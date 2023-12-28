using MineSharp.Core.Common;

namespace MineSharp.Physics;

internal class PlayerState
{
    public bool WasTouchingWater { get; internal set; }
    public bool WasUnderwater { get; internal set; }
    
    public double WaterHeight { get; internal set; }
    public double LavaHeight { get; internal set; }
    
    public bool IsSprinting { get; internal set; }
    public bool IsCrouching { get; internal set; }
    
    public bool HorizontalCollision { get; internal set; }
    public bool VerticalCollision { get; internal set; }
    
    public bool MinorHorizontalCollision { get; internal set; }

    public Vector3 LastClimbablePosition = Vector3.Zero;

    public int NoJumpDelay { get; internal set; } = 0;

    public Vector3 StuckSpeedMultiplier { get; set; } = Vector3.Zero;
}
