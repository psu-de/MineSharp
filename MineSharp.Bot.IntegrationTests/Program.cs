using MineSharp.Bot.IntegrationTests.Tests;

// await WorldTests.TestBlockUpdate();
// Environment.Exit(0);

await PlayerTests.TestWeatherChange();

await PlayerTests.TestHealth();
await PlayerTests.TestDeath();
await PlayerTests.TestRespawn();
await PlayerTests.TestPlayerJoin();
await PlayerTests.TestPlayerLeave();