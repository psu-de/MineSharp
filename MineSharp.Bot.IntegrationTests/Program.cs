using MineSharp.Bot.IntegrationTests;
using MineSharp.Bot.IntegrationTests.Tests;


await IntegrationTest.RunTest("testHealth", PlayerTests.TestHealth);
await IntegrationTest.RunTest("testDeath", PlayerTests.TestDeath);
await IntegrationTest.RunTest("testRespawn", PlayerTests.TestRespawn);
await IntegrationTest.RunTest("testPlayerJoin", PlayerTests.TestPlayerJoin);
await IntegrationTest.RunTest("testPlayerLeave", PlayerTests.TestPlayerLeave);