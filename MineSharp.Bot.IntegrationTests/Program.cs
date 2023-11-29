using MineSharp.Bot.IntegrationTests.Tests;


await PlayerTests.TestHealth();
await PlayerTests.TestDeath();
await PlayerTests.TestRespawn();
await PlayerTests.TestPlayerJoin();
await PlayerTests.TestPlayerLeave();
await PlayerTests.TestWeatherChange();
await PlayerTests.TestAttack();

await WorldTests.TestMultiBlockUpdate();

await WindowTests.TestInventoryUpdate();
await WindowTests.TestOpenContainer();

await WorldTests.TestPlaceBlock();
await WorldTests.TestMineBlock();