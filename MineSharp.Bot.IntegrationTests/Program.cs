using MineSharp.Bot.IntegrationTests.Tests;
using MineSharp.Bot.Utils;
using MineSharp.Data;

await MinecraftData.FromVersion("1.20.4");
Console.WriteLine("done");
Console.ReadKey();

LoggingHelper.EnableDebugLogs(true);

await PlayerTests.RunAll();
await WindowTests.RunAll();
await WorldTests.RunAll();
await CraftingTests.RunAll();
await PhysicsTests.RunAll();
