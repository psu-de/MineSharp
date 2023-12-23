using MineSharp.Bot;
using MineSharp.Bot.IntegrationTests.Tests;
using MineSharp.Bot.Plugins;

await PlayerTests.RunAll();
await WindowTests.RunAll();
await WorldTests.RunAll();
await CraftingTests.RunAll();
await PhysicsTests.RunAll();