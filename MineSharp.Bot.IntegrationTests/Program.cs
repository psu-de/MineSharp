using MineSharp.Bot.IntegrationTests.Tests;

await PlayerTests.TestHealth();
await PlayerTests.TestDeath();
await PlayerTests.TestRespawn();
await PlayerTests.TestPlayerJoin();
await PlayerTests.TestPlayerLeave();
await PlayerTests.TestWeatherChange();

await WorldTests.TestMultiBlockUpdate();

await WindowTests.TestInventoryUpdate();
await WindowTests.TestOpenContainer();
