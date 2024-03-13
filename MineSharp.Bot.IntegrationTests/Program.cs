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

var bot = await new BotBuilder()
    .Host("localhost")
    .OfflineSession("MineSharpBot")
    .WithPlugin<Pathfinder>()
    .WithPlugin<AutoRespawn>()
    .AutoConnect()
    .CreateAsync();

await bot.Connect();

var chat = bot.GetPlugin<ChatPlugin>();
var physics = bot.GetPlugin<PhysicsPlugin>();
var pathfinder = bot.GetPlugin<Pathfinder>();
var player = bot.GetPlugin<PlayerPlugin>();
var world = bot.GetPlugin<WorldPlugin>();

if (!player.IsAlive!.Value)
    await player.Respawn();

chat.OnChatMessageReceived += (sender, playerUuid, message, position, name) =>
{
    var splits = message.Message.Split(' ');
    
    if (splits.Length != 3)
        return;

    var ints = splits.Select(x => Convert.ToInt32(x)).ToArray();
    pathfinder.Goto(new Position(ints[0], ints[1], ints[2])).Wait();
};

Console.ReadKey();
//
// return;
//
// await PlayerTests.RunAll();
// await WindowTests.RunAll();
// await WorldTests.RunAll();
// await CraftingTests.RunAll();
// await PhysicsTests.RunAll();
