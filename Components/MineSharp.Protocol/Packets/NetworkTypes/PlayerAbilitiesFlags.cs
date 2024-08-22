namespace MineSharp.Protocol.Packets.NetworkTypes;

/// <summary>
/// Flags representing various player abilities.
/// </summary>
[Flags]
public enum PlayerAbilitiesFlags : byte
{
    /// <summary>
    /// Player is invulnerable.
    /// </summary>
    Invulnerable = 0x01,
    /// <summary>
    /// Player is flying.
    /// </summary>
    Flying = 0x02,
    /// <summary>
    /// Player is allowed to fly.
    /// </summary>
    AllowFlying = 0x04,
    /// <summary>
    /// Player is in creative mode.
    /// And can instantly break blocks.
    /// </summary>
    CreativeMode = 0x08,
}
