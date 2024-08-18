namespace MineSharp.Protocol.Packets.NetworkTypes;

/// <summary>
///     Specifies the status of a player action
/// </summary>
public enum PlayerActionStatus
{
    /// <summary>
    /// Sent when the player starts digging a block.
    /// If the block was instamined or the player is in creative mode, the client will not send Status = Finished digging, and will assume the server completed the destruction.
    /// To detect this, it is necessary to calculate the block destruction speed server-side. 
    /// </summary>
    StartDigging = 0,
    /// <summary>
    /// Sent when the player lets go of the Mine Block key (default: left click).
    /// 
    /// Face is always set to -Y. 
    /// </summary>
    CancelledDigging = 1,
    /// <summary>
    /// Sent when the client thinks it is finished.
    /// </summary>
    FinishedDigging = 2,
    /// <summary>
    /// Triggered by using the Drop Item key (default: Q) with the modifier to drop the entire selected stack (default: Control or Command, depending on OS).
    /// 
    /// Location is always set to 0/0/0, Face is always set to -Y.
    /// Sequence is always set to 0.
    /// </summary>
    DropItemStack = 3,
    /// <summary>
    /// Triggered by using the Drop Item key (default: Q).
    /// 
    /// Location is always set to 0/0/0, Face is always set to -Y.
    /// Sequence is always set to 0. 
    /// </summary>
    DropItem = 4,
    /// <summary>
    /// Indicates that the currently held item should have its state updated such as eating food, pulling back bows, using buckets, etc.
    /// 
    /// Location is always set to 0/0/0, Face is always set to -Y.
    /// Sequence is always set to 0. 
    /// </summary>
    ShootArrowOrFinishEating = 5,
    /// <summary>
    /// Used to swap or assign an item to the second hand.
    /// 
    /// Location is always set to 0/0/0, Face is always set to -Y.
    /// Sequence is always set to 0. 
    /// </summary>
    SwapItemInHand = 6,
}
