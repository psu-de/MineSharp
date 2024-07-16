using MineSharp.Bot.Blocks;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Geometry;

namespace MineSharp.Bot.IntegrationTests.Tests;

public static class WorldTests
{
    public static async Task RunAll()
    {
        await TestPlaceBlock();
        await TestMineBlock();
        await TestMultiBlockUpdate();
    }

    public static Task TestBlockUpdate()
    {
        return IntegrationTest.RunTest("testBlockUpdate", async (bot, source) =>
        {
            var world = bot.GetPlugin<WorldPlugin>();
            var chat  = bot.GetPlugin<ChatPlugin>();
            await world.WaitForInitialization();
            await chat.WaitForInitialization();

            var expectedPosition = new Position(-9, -61, 22);

            world.World!.OnBlockUpdated += (sender, block) =>
            {
                if (block.Position == expectedPosition && block.Info.Type == BlockType.RedstoneBlock)
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
            await bot.GetPlugin<WorldPlugin>().WaitForInitialization();
            await bot.GetPlugin<ChatPlugin>().WaitForInitialization();

            var relative = new Position(-9, -61, 21);
            var expectedBlocks = new List<ulong>(
                Enumerable.Range(0, 8)
                          .Select(x => new Position(relative.X - x, relative.Y, relative.Z))
                          .Select(x => x.ToULong()));

            bot.GetPlugin<WorldPlugin>().World.OnBlockUpdated += (sender, block) =>
            {
                expectedBlocks.Remove(block.Position.ToULong());

                if (expectedBlocks.Count == 0)
                    source.TrySetResult(true);
            };
        });
    }

    public static Task TestMineBlock()
    {
        return IntegrationTest.RunTest("testMineBlock", async (bot, source) =>
        {
            await bot.GetPlugin<WorldPlugin>().WaitForChunks();

            var position = new Position(-8, -59, 20);
            await bot.GetPlugin<ChatPlugin>().SendChat("/tp @p -5 -60 20");

            bot.GetPlugin<ChatPlugin>().OnChatMessageReceived += (sender, player, chat, type, senderName) =>
            {
                if (chat.GetMessage(bot.Data).Contains("testMineBlock success"))
                {
                    source.TrySetResult(true);
                }
            };

            await Task.Delay(1000);

            var result = await bot.GetPlugin<WorldPlugin>().MineBlock(
                bot.GetPlugin<WorldPlugin>().World!.GetBlockAt(position));

            if (result != MineBlockStatus.Finished)
                source.TrySetResult(false);
        }, commandDelay: 1000);
    }

    public static Task TestPlaceBlock()
    {
        return IntegrationTest.RunTest("testPlaceBlock", async (bot, source) =>
        {
            await bot.GetPlugin<WorldPlugin>().WaitForChunks();

            var position = new Position(-8, -59, 19);
            await bot.GetPlugin<ChatPlugin>().SendChat("/tp @p -5 -60 20");
            await bot.GetPlugin<ChatPlugin>().SendChat("/clear");
            await bot.GetPlugin<ChatPlugin>().SendChat("/give @p dirt");

            bot.GetPlugin<ChatPlugin>().OnChatMessageReceived += (sender, player, chat, type, senderName) =>
            {
                if (chat.GetMessage(bot.Data).Contains("testPlaceBlock success"))
                {
                    source.TrySetResult(true);
                }
            };

            await Task.Delay(1000);
            await bot.GetPlugin<WorldPlugin>().PlaceBlock(position);
        });
    }
}
