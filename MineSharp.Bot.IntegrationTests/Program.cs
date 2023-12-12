using MineSharp.Bot;
using MineSharp.Bot.IntegrationTests.Tests;

var bot = await MinecraftBot.CreateBot("MineSharpBot", "localhost", offline: true);
await bot.Connect();

while (true)
{
    await Task.Delay(1000);
}

return;

await PlayerTests.RunAll();
await WindowTests.RunAll();
await WorldTests.RunAll();
await CraftingTests.RunAll();