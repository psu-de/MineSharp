using MineSharp.Bot;
using MineSharp.Bot.IntegrationTests.Tests;
using MineSharp.Bot.Plugins;

var bot = await MinecraftBot.CreateBot("MineSharpBot", "localhost", offline: true);

var physics = bot.GetPlugin<PhysicsPlugin>();
await physics.SetEnabled(false);

await bot.Connect();
await bot.GetPlugin<PlayerPlugin>().WaitForInitialization();

if (!bot.GetPlugin<PlayerPlugin>().IsAlive!.Value)
{
    await bot.GetPlugin<PlayerPlugin>().Respawn();
}

ConsoleKeyInfo info;
while ((info = Console.ReadKey(true)) != null)
{
    if (info.Key == ConsoleKey.Q)
    {
        break;
    }

    switch (info.Key)
    {
        case ConsoleKey.T:
            physics.OnTick().Wait();
            Console.WriteLine("--- --- --- --- --- --- --- --- ---");
            break;
        
        case ConsoleKey.W:
            physics.InputControls.UpKeyDown = !physics.InputControls.UpKeyDown;
            break;
        
        case ConsoleKey.A:
            physics.InputControls.LeftKeyDown = !physics.InputControls.LeftKeyDown;
            break;
        
        case ConsoleKey.S:
            physics.InputControls.DownKeyDown = !physics.InputControls.DownKeyDown;
            break;
        
        case ConsoleKey.D:
            physics.InputControls.RightKeyDown = !physics.InputControls.RightKeyDown;
            break;
        
        case ConsoleKey.Spacebar:
            physics.InputControls.JumpingKeyDown = !physics.InputControls.JumpingKeyDown;
            break;
    }
}

return;

await PlayerTests.RunAll();
await WindowTests.RunAll();
await WorldTests.RunAll();
await CraftingTests.RunAll();