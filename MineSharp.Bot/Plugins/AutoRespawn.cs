namespace MineSharp.Bot.Plugins;

/// <summary>
/// Auto Respawn Plugin
/// </summary>
public class AutoRespawn : Plugin
{
    private PlayerPlugin? player;

    /// <summary>
    /// Create a new AutoRespawn instance
    /// </summary>
    /// <param name="bot"></param>
    public AutoRespawn(MineSharpBot bot) : base(bot)
    { }

    /// <inheritdoc />
    protected override Task Init()
    {
        this.player = this.Bot.GetPlugin<PlayerPlugin>();
        this.player.OnDied += this.OnBotDied;

        return Task.CompletedTask;
    }

    private void OnBotDied(MineSharpBot bot)
    {
        this.player!.Respawn().Wait();
    }
}
