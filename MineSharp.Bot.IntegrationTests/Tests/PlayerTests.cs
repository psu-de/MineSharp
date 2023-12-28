using MineSharp.Bot.Plugins;
using MineSharp.Core.Common.Entities;

namespace MineSharp.Bot.IntegrationTests.Tests;

public static class PlayerTests
{
    public static async Task RunAll()
    {
        await TestHealth();
        await TestDeath();
        await TestRespawn();
        await TestPlayerJoin();
        await TestPlayerLeave();
        await TestWeatherChange();
        await TestAttack();
    }
    
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

            var bot2 = await MineSharpBot.CreateBot(
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

            var bot2 = await MineSharpBot.CreateBot(
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

    public static Task TestAttack()
    {
        return IntegrationTest.RunTest("testAttack", async (bot, source) =>
        {
            var player = bot.GetPlugin<PlayerPlugin>();
            var entities = bot.GetPlugin<EntityPlugin>();

            await bot.GetPlugin<ChatPlugin>().SendChat("/tp @p 16 -60 21");

            Entity? chicken = null;
            
            entities.OnEntitySpawned += async (sender, entity) =>
            {
                if (entity.Info.Type != EntityType.Chicken)
                    return;

                chicken = entity;
                await player.Attack(entity);
            };
            
            entities.OnEntityDespawned += (sender, entity) =>
            {
                if (entity.ServerId == chicken?.ServerId)
                {
                    source.TrySetResult(true);
                }
            };
            
            await Task.Delay(1000);
        }, commandDelay: 1000);
    }
}
