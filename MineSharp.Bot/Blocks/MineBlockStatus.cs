namespace MineSharp.Bot.Blocks;

/// <summary>
///     Specifies the status of a mining operation
/// </summary>
public enum MineBlockStatus
{
    /// <summary>
    ///     The block has been mined
    /// </summary>
    Finished,

    /// <summary>
    ///     The block could not be mined, because it is in a chunk that is not loaded
    /// </summary>
    BlockNotLoaded,

    /// <summary>
    ///     The block is too far away
    /// </summary>
    TooFar,

    /// <summary>
    ///     The block is not diggable
    /// </summary>
    NotDiggable,

    /// <summary>
    ///     Something went wrong
    /// </summary>
    Failed,

    /// <summary>
    ///     The mining operation has been cancelled
    /// </summary>
    Cancelled
}
