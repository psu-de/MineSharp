using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;
using System.Collections.Concurrent;

namespace MineSharp.Bot.IntegrationTests.Tests;

public static class WorldTests
{
    public static Task TestBlockUpdate()
    {
        return IntegrationTest.RunTest("testBlockUpdate", async (bot, source) =>
        {
            var world = bot.GetPlugin<WorldPlugin>();
            var chat = bot.GetPlugin<ChatPlugin>();
            await world.WaitForInitialization();
            await chat.WaitForInitialization();
        
            var expectedPosition = new Position(-9, -61, 22);

            world.World!.OnBlockUpdated += (sender, block) =>
            {
                if (block.Position == expectedPosition && block.Info.Name == "redstone_block")
                {
                    source.TrySetResult(true);
                }
            };
        });
    }

    public static Task TestMultiBlockUpdate()
    {
        return IntegrationTest.RunTest("testMultiBlockUpdate", async (bot, source) =>
        {
            await bot.World!.WaitForInitialization();
            await bot.Chat!.WaitForInitialization();

            var relative = new Position(-9, -61, 21);
            var expectedBlocks = new List<ulong>(
                Enumerable.Range(0, 8)
                .Select(x => new Position(relative.X - x, relative.Y, relative.Z))
                .Select(x => x.ToULong()));

            bot.World.World!.OnBlockUpdated += (sender, block) =>
            {
                expectedBlocks.Remove(block.Position.ToULong());
                
                if (expectedBlocks.Count == 0)
                    source.TrySetResult(true);
            };
        });
    }
}
