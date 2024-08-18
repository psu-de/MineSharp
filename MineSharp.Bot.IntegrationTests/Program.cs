using MineSharp.Bot;
using MineSharp.Bot.IntegrationTests.Tests;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Effects;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.Pathfinder;
using MineSharp.Physics;
using MineSharp.Physics.Input;
using MineSharp.World;
using MineSharp.World.Chunks;

MineSharpBot.EnableDebugLogs(true);

var bot = await new BotBuilder()
    .Host("localhost")
    .OfflineSession("MineSharpBot")
    .WithPlugin<Pathfinder>()
    .WithPlugin<AutoRespawn>()
    .AutoConnect()
    .CreateAsync();

var chat = bot.GetPlugin<ChatPlugin>();
var physics = bot.GetPlugin<PhysicsPlugin>();
var pathfinder = bot.GetPlugin<Pathfinder>();
var player = bot.GetPlugin<PlayerPlugin>(); 
var world = bot.GetPlugin<WorldPlugin>();

if (!player.IsAlive!.Value)
    await player.Respawn();

chat.OnChatMessageReceived += async (sender, playerUuid, message, position) =>
{
    Console.WriteLine(message.GetMessage(bot.Data));
    var splits = message.GetMessage(bot.Data).Split(' ').Skip(1).ToArray();
    
    if (splits.Length != 3)
        return;
    
    var ints = splits.Select(x => Convert.ToInt32(x)).ToArray();
    await pathfinder.Goto(new Position(ints[0], ints[1], ints[2]));
};

while (true)
{
    var input = Console.ReadLine();
    
    if (string.IsNullOrEmpty(input))
    {
        continue;
    }

    await chat.SendChat(input);
}

Console.ReadKey();
//
// return;
//
// await PlayerTests.RunAll();
// await WindowTests.RunAll();
// await WorldTests.RunAll();
// await CraftingTests.RunAll();
// await PhysicsTests.RunAll();
