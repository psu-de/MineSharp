using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Items;

namespace MineSharp.Bot.IntegrationTests.Tests;

public static class CraftingTests
{
    public static async Task RunAll()
    {
        await TestCrafting();
    }
    
    public static Task TestCrafting()
    {
        return IntegrationTest.RunTest("testCrafting", async (bot, source) =>
        {
            var chat = bot.GetPlugin<ChatPlugin>();
            var crafting = bot.GetPlugin<CraftingPlugin>();
            var world = bot.GetPlugin<WorldPlugin>();
            var window = bot.GetPlugin<WindowPlugin>();
            await chat.WaitForInitialization();
            await crafting.WaitForInitialization();
            await world.WaitForChunks();
            await chat.SendChat("/tp @p 15 -60.00 21");
            await chat.SendChat("/give @p glass 49");
            await chat.SendChat("/give @p ghast_tear 7");
            await chat.SendChat("/give @p ender_eye 7");
            await Task.Delay(1000);
            var block = world.World.GetBlockAt(new Position(15, -59, 24));
            var recipe = crafting.FindRecipe(ItemType.EndCrystal);
            await crafting.Craft(recipe!, block, 7);
            source.TrySetResult(window.Inventory!.CountItems(ItemType.EndCrystal) == 7);
        }, 60 * 1000);
    }
}
