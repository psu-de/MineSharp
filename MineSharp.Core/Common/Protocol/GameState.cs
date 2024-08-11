namespace MineSharp.Core.Common.Protocol;

/// <summary>
///     Specifies the GameState
/// </summary>
public enum GameState
{
#pragma warning disable CS1591
    Handshaking = 0,
    Status = 1,
    Login = 2,
    Play = 3,
    Configuration = 4,

    None = -1
#pragma warning restore CS1591
}
