using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Items;
using MineSharp.Core.Geometry;
using Spectre.Console;

namespace MineSharp.Bot.IntegrationTests.Tests;

public static class WindowTests
{
    public static async Task RunAll()
    {
        await TestInventoryUpdate();
        await TestOpenContainer();
        await TestCreativeInventory();
    }

    public static Task TestInventoryUpdate()
    {
        return IntegrationTest.RunTest("testInventoryUpdate", async (bot, source) =>
        {
            await bot.GetPlugin<WindowPlugin>().WaitForInventory();
            await bot.GetPlugin<ChatPlugin>().SendChat("/clear");

            bot.GetPlugin<WindowPlugin>().Inventory!.OnSlotChanged += (window, index) =>
            {
                var slot = window.GetSlot(index);
                if (slot.Item?.Info.Name == "diamond" && slot.Item?.Count == 33)
                {
                    source.TrySetResult(true);
                }
            };
        }, commandDelay: 1000);
    }

    public static Task TestOpenContainer()
    {
        return IntegrationTest.RunTest("testOpenContainer", async (bot, source) =>
        {
            await bot.GetPlugin<WindowPlugin>().WaitForInventory();
            await bot.GetPlugin<WorldPlugin>().WaitForChunks();

            await Task.Delay(1000);

            var blockPos = new Position(17, -58, 24);
            var block    = bot.GetPlugin<WorldPlugin>().World.GetBlockAt(blockPos);

            var window = await bot.GetPlugin<WindowPlugin>().OpenContainer(block);
            await Task.Delay(1000);
            var slot = window.GetSlot(0);

            await Task.Delay(1000);

            source.TrySetResult(
                window.SlotCount     == 3 * 9
             && slot.Item?.Info.Name == "soul_sand"
             && slot.Item?.Count     == 48);
        });
    }

    public static Task TestCreativeInventory()
    {
        return IntegrationTest.RunTest("testCreativeInventory", async (bot, source) =>
        {
            var window = bot.GetPlugin<WindowPlugin>();
            var chat   = bot.GetPlugin<ChatPlugin>();

            await window.WaitForInventory();

            if (window.CreativeInventory.Available)
            {
                AnsiConsole.MarkupLine("[red]Expected creative inventory not to be available[/]");
                source.TrySetResult(false);
                return;
            }

            await chat.SendChat("/gamemode creative");
            await Task.Delay(50);

            if (!window.CreativeInventory.Available)
            {
                AnsiConsole.MarkupLine("[red]Expected creative inventory to be available[/]");
                source.TrySetResult(false);
                return;
            }

            await window.CreativeInventory.GetItem(ItemType.NetherStar, 22, 9);
            await Task.Delay(50);

            var inventoryItem = window.Inventory?.GetSlot(9).Item;

            var result = inventoryItem?.Info.Type != ItemType.NetherStar || inventoryItem.Count != 22;

            await chat.SendChat("/gamemode survival");
            source.TrySetResult(result);
            return;
        });
    }
}
