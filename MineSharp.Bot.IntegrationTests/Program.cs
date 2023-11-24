using MineSharp.Bot.IntegrationTests.Tests;

await WorldTests.TestPlaceBlock();
Environment.Exit(0);

await PlayerTests.TestHealth();
await PlayerTests.TestDeath();
await PlayerTests.TestRespawn();
await PlayerTests.TestPlayerJoin();
await PlayerTests.TestPlayerLeave();
await PlayerTests.TestWeatherChange();

await WorldTests.TestMultiBlockUpdate();

await WindowTests.TestInventoryUpdate();
await WindowTests.TestOpenContainer();
