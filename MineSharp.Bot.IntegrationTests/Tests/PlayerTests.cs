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

    public static async Task TestPlayerJoin(MinecraftBot bot, TaskCompletionSource<bool> source)
    {
        const string secondBotName = "MineSharpBot2";
        var player = bot.GetPlugin<PlayerPlugin>();

        player.OnPlayerJoined += (sender, player) =>
        {
            if (player.Username == secondBotName)
            {
                source.TrySetResult(true);
            }
        };

        var bot2 = await MinecraftBot.CreateBot(
            secondBotName,
            "localhost",
            25565,
            offline: true);

        if (!await bot2.Connect())
        {
            source.TrySetResult(false);
        }

        await Task.Delay(1000);
        await bot2.Disconnect();
    }
    
    public static async Task TestPlayerLeave(MinecraftBot bot, TaskCompletionSource<bool> source)
    {
        const string secondBotName = "MineSharpBot2";
        var player = bot.GetPlugin<PlayerPlugin>();

        player.OnPlayerLeft += (sender, player) =>
        {
            if (player.Username == secondBotName)
            {
                source.TrySetResult(true);
            }
        };

        var bot2 = await MinecraftBot.CreateBot(
            secondBotName,
            "localhost",
            25565,
            offline: true);

        if (!await bot2.Connect())
        {
            source.TrySetResult(false);
        }

        await Task.Delay(500);
        await bot2.Disconnect();
    }

    public static Task TestWeatherRain(MinecraftBot bot, TaskCompletionSource<bool> source)
    {
        var player = bot.GetPlugin<PlayerPlugin>();

        player.OnWeatherChanged += sender =>
        {
            source.TrySetResult(true);
        };
        
        return Task.CompletedTask;
    }
}
