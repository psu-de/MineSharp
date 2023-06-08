using MineSharp.Bot.Plugins;

namespace MineSharp.Bot.IntegrationTests.Tests;

public class PlayerTests
{
    public static Task TestHealth(MinecraftBot bot, TaskCompletionSource<bool> source)
    {
        var player = bot.GetPlugin<PlayerPlugin>();
        var before = player.Health!.Value;

        player.OnHealthChanged += sender =>
        {
            // the test should deal 6 damage.
            source.TrySetResult(Math.Abs(before - player.Health.Value - 6) < 0.2f);
        };

        return Task.CompletedTask;
    }

    public static Task TestDeath(MinecraftBot bot, TaskCompletionSource<bool> source)
    {
        var player = bot.GetPlugin<PlayerPlugin>();

        player.OnDied += sender =>
        {
            source.TrySetResult(true);
        };

        return Task.CompletedTask;
    }

    public static Task TestRespawn(MinecraftBot bot, TaskCompletionSource<bool> source)
    {
        var player = bot.GetPlugin<PlayerPlugin>();

        player.OnRespawned += sender =>
        {
            source.TrySetResult(true);
        };

        player.OnDied += async sender =>
        {
            await player.Respawn();
        };

        return Task.CompletedTask;
    }
}
