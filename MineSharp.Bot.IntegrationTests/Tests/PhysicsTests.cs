using MineSharp.Bot.Plugins;

namespace MineSharp.Bot.IntegrationTests.Tests;

public static class PhysicsTests
{
    public static async Task RunAll()
    {
        await TestGravity();
        await TestWalkForward();
        await TestWalkBackward();
        await TestWalkRight();
        await TestWalkLeft();
        await TestWaterPushing();
    }

    public static Task TestGravity()
    {
        return IntegrationTest.RunTest("testGravity", (bot, source) =>
        {
            ExpectChatMessage(bot, source, "testGravity success");

            return Task.CompletedTask;
        });
    }

    public static Task TestWalkForward()
    {
        return IntegrationTest.RunTest("testWalkForward", async (bot, source) =>
        {
            ExpectChatMessage(bot, source, "testWalk success");
            var physics = bot.GetPlugin<PhysicsPlugin>();
            
            await Task.Delay(1000);
            physics.InputControls.ForwardKeyDown = true;
        });
    }
    
    public static Task TestWalkBackward()
    {
        return IntegrationTest.RunTest("testWalkBackwards", async (bot, source) =>
        {
            ExpectChatMessage(bot, source, "testWalk success");
            var physics = bot.GetPlugin<PhysicsPlugin>();
            
            await Task.Delay(1000);
            physics.InputControls.BackwardKeyDown = true;
        });
    }

    public static Task TestWalkLeft()
    {
        return IntegrationTest.RunTest("testWalkLeft", async (bot, source) =>
        {
            ExpectChatMessage(bot, source, "testWalk success");
            var physics = bot.GetPlugin<PhysicsPlugin>();
            
            await Task.Delay(1000);
            physics.InputControls.LeftKeyDown = true;
        });
    }
    
    public static Task TestWalkRight()
    {
        return IntegrationTest.RunTest("testWalkRight", async (bot, source) =>
        {
            ExpectChatMessage(bot, source, "testWalk success");
            var physics = bot.GetPlugin<PhysicsPlugin>();
            
            await Task.Delay(1000);
            physics.InputControls.RightKeyDown = true;
        });
    }

    public static Task TestWaterPushing()
    {
        return IntegrationTest.RunTest("testWaterPushing", (bot, source) =>
        {
            ExpectChatMessage(bot, source, "testWaterPushing success");
            return Task.CompletedTask;
        });
    }

    private static void ExpectChatMessage(MinecraftBot bot, TaskCompletionSource<bool> source, string expectedMessage)
    {
        var chat = bot.GetPlugin<ChatPlugin>();

        chat.OnChatMessageReceived += (sender, player, message, position, name) =>
        {
            var msg = new Core.Common.Chat(message);
            if (msg.Message == expectedMessage)
                source.TrySetResult(true);
        };
    }
}