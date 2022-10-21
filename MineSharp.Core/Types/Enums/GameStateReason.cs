namespace MineSharp.Core.Types.Enums
{
    public enum GameStateReason : byte
    {
        NoRespawnBlock = 0,
        EndRaining = 1,
        BeginRaining = 2,
        ChangeGameMode = 3,
        WinGame = 4,
        DemoEvent = 5,
        ArrowHit = 6,
        RainLevelChanged = 7,
        ThunderLevelChanged = 8,
        PufferfishSting = 9,
        ElderGuardianMobAppeared = 10,
        EnableRespawnScreen = 11
    }
}
