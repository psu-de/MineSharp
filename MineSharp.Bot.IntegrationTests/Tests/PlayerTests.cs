using MineSharp.Bot.Plugins;

namespace MineSharp.Bot.IntegrationTests.Tests;

public static class PlayerTests
{
    public static Task TestHealth()
    {
        return IntegrationTest.RunTest("testHealth", (bot, source) =>
        {
            var player = bot.GetPlugin<PlayerPlugin>();
            var before = player.Health!.Value;

            player.OnHealthChanged += sender =>
            {
                // the test should deal 6 damage.
                source.TrySetResult(Math.Abs(before - player.Health.Value - 6) < 0.2f);
            };

            return Task.CompletedTask;
        });
    }

    public static Task TestDeath()
    {
        return IntegrationTest.RunTest("testDeath", (bot, source) =>
        {
            var player = bot.GetPlugin<PlayerPlugin>();

            player.OnDied += sender =>
            {
                source.TrySetResult(true);
            };

            return Task.CompletedTask;
        });
    }

    public static Task TestRespawn()
    {
        return IntegrationTest.RunTest("testRespawn", (bot, source) =>
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
        });
    }

    public static Task TestPlayerJoin()
    {
        return IntegrationTest.RunTest("testPlayerJoin", async (bot, source) =>
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
        });
    }
    
    public static Task TestPlayerLeave()
    {
        return IntegrationTest.RunTest("testPlayerLeave", async (bot, source) =>
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
        });
    }

    public static Task TestWeatherChange()
    {
        return IntegrationTest.RunTest("testWeatherChange", (bot, source) =>
        {
            var player = bot.GetPlugin<PlayerPlugin>();

            player.OnWeatherChanged += sender =>
            {
                if (Math.Abs(player.RainLevel - 1) < 0.02f)
                {
                    source.TrySetResult(true);
                }
            };

            return Task.CompletedTask;
        }, 15 * 1000);
    }
}