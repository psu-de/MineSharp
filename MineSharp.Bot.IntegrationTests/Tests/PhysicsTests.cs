using MineSharp.Bot.Plugins;

namespace MineSharp.Bot.IntegrationTests.Tests;

public static class PhysicsTests
{
    public static async Task RunAll()
    {
        await TestGravity();
    }

    public static Task TestGravity()
    {
        return IntegrationTest.RunTest("testGravity", (bot, source) =>
        {
            ExpectChatMessage(bot, source, "testGravity success");

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