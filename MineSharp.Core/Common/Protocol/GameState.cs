namespace MineSharp.Core.Common.Protocol;

public enum GameState
{
    Handshaking = 0,
    Status = 1,
    Login = 2,
    Play = 3,
    Configuration = 4,
}
