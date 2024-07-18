namespace MineSharp.Bot.Blocks;

/// <summary>
///     Specifies the status of a player action
/// </summary>
public enum PlayerActionStatus
{
    /// <summary>
    ///     Digging has started
    /// </summary>
    StartedDigging = 0,

    /// <summary>
    ///     Digging has been cancelled
    /// </summary>
    CancelledDigging = 1,

    /// <summary>
    ///     Digging is complete
    /// </summary>
    FinishedDigging = 2,

    /// <summary>
    ///     Drop the item stack
    /// </summary>
    DropItemStack = 3,

    /// <summary>
    ///     Drop a single item
    /// </summary>
    DropItem = 4,

    /// <summary>
    ///     Finished eating or shot bow
    /// </summary>
    ShootArrowFinishedEating = 5,

    /// <summary>
    ///     Swap item to off hand
    /// </summary>
    SwapItemHand = 6
}
