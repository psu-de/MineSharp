using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;

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
}
