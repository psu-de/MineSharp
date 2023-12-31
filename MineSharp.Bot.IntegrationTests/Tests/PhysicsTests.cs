using MineSharp.Bot.Plugins;
using MineSharp.ChatComponent;

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
        await TestJump();
        await TestCrouch();
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

    public static Task TestJump()
    {
        return IntegrationTest.RunTest("testJump", async (bot, source) =>
        {
            ExpectChatMessage(bot, source, "testJump success");
            var physics = bot.GetPlugin<PhysicsPlugin>();

            await Task.Delay(1000);

            physics.InputControls.ForwardKeyDown = true;
            physics.InputControls.JumpingKeyDown = true;
        });
    }
    
    public static Task TestCrouch()
    {
        return IntegrationTest.RunTest("testCrouch", async (bot, source) =>
        {
            ExpectChatMessage(bot, source, "testCrouch success");
            var physics = bot.GetPlugin<PhysicsPlugin>();

            await Task.Delay(1000);

            physics.InputControls.ForwardKeyDown = true;
            physics.InputControls.SneakingKeyDown = true;
        });
    }

    private static void ExpectChatMessage(MineSharpBot bot, TaskCompletionSource<bool> source, string expectedMessage)
    {
        var chat = bot.GetPlugin<ChatPlugin>();

        chat.OnChatMessageReceived += (sender, player, chatComponent, position, name) =>
        {
            if (chatComponent.Message.Contains(expectedMessage))
                source.TrySetResult(true);
        };
    }
}