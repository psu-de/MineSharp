namespace MineSharp.Protocol.Packets.NetworkTypes;

/// <summary>
///     Enum representing the possible results of a resource pack response
/// </summary>
public enum ResourcePackResult
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    Success = 0,
    Declined = 1,
    FailedDownload = 2,
    Accepted = 3,
    Downloaded = 4,
    InvalidUrl = 5,
    FailedToReload = 6,
    Discarded = 7
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
