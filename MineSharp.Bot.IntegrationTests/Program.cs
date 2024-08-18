using MineSharp.Bot.IntegrationTests.Tests;
using MineSharp.Bot.Utils;

LoggingHelper.EnableDebugLogs(true);

await PlayerTests.RunAll();
await WindowTests.RunAll();
await WorldTests.RunAll();
await CraftingTests.RunAll();
await PhysicsTests.RunAll();
