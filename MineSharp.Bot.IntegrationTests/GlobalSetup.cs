namespace MineSharp.Bot.IntegrationTests;

public static class GlobalSetup
{
    public static async Task<MinecraftBot> CreateBotAndConnect()
    {
        var bot = await MinecraftBot.CreateBot(
            "MineSharpBot",
            "127.0.0.1",
            25565,
            true);

        if (!await bot.Connect())
        {
            Assert.Fail("Could not connect. Is the test server running? (localhost on port 25565 in offline mode)");
        }

        return bot;
    }
}
