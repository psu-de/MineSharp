namespace MineSharp.Bot.Plugins;

/// <summary>
///     Auto Respawn Plugin
/// </summary>
public class AutoRespawn : Plugin
{
    private PlayerPlugin? player;

    /// <summary>
    ///     The time waited before respawning
    /// </summary>
    public TimeSpan RespawnDelay = TimeSpan.Zero;

    /// <summary>
    ///     Create a new AutoRespawn instance
    /// </summary>
    /// <param name="bot"></param>
    public AutoRespawn(MineSharpBot bot) : base(bot)
    { }

    /// <inheritdoc />
    protected override Task Init()
    {
        player = Bot.GetPlugin<PlayerPlugin>();
        player.OnDied += OnBotDied;

        return Task.CompletedTask;
    }

    private void OnBotDied(MineSharpBot bot)
    {
        Task.Run(Respawn);
    }

    private async Task Respawn()
    {
        if (RespawnDelay.TotalMilliseconds > 0)
        {
            await Task.Delay(RespawnDelay);
        }

        await player!.Respawn();
    }
}
