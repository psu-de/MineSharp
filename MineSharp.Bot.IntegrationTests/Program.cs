using MineSharp.Bot;
using MineSharp.Bot.IntegrationTests.Tests;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Effects;
using MineSharp.Core.Common.Entities;
using MineSharp.Data;
using MineSharp.Pathfinder;
using MineSharp.Physics;
using MineSharp.Physics.Input;
using MineSharp.World;
using MineSharp.World.Chunks;

// Forward = True
// Pos = (8,5 / 2 / 8,63005), Vel = (0 / -0,0784 / 0,07101) IsOnGround=False
// Pos = (8,5 / 2 / 8,72065), Vel = (0 / -0,0784 / 0,08245) IsOnGround=True)
// Pos = (8,5 / 2 / 8,93315), Vel = (0 / -0,0784 / 0,11602) IsOnGround=True)
// Pos = (8,5 / 2 / 9,17922), Vel = (0 / -0,0784 / 0,13436) IsOnGround=True
// Backward = True
// Pos = (8,5 / 2 / 9,31358), Vel = (0 / -0,0784 / 0,07336) IsOnGround=True
// Pos = (8,5 / 1,9216 / 9,38694), Vel = (0 / -0,15523 / 0,04005) IsOnGround=False
// Backward = False
// Forward = False
// Pos = (8,5 / 1,76637 / 9,42699), Vel = (0 / -0,23053 / 0,03645) IsOnGround=False
// Pos = (8,5 / 1,53584 / 9,46344), Vel = (0 / -0,30432 / 0,03317) IsOnGround=False
// Pos = (8,5 / 1,23152 / 9,49661), Vel = (0 / -0,37663 / 0,03018) IsOnGround=False
// Pos = (8,5 / 1 / 9,52679), Vel = (0 / -0,0784 / 0,02747) IsOnGround=True
// Pos = (8,5 / 1 / 9,55426), Vel = (0 / -0,0784 / 0,015) IsOnGround=True
// Pos = (8,5 / 1 / 9,56925), Vel = (0 / -0,0784 / 0,00819) IsOnGround=True


var data = await MinecraftData.FromVersion("1.20.2");
var world = WorldVersion.CreateWorld(data);
var chunk = world.CreateChunk(new ChunkCoordinates(0, 0), Array.Empty<BlockEntity>());

var input = new InputControls();
var entity = new Entity(
    data.Entities.ByType(EntityType.Player)!, 
    0, 
    new Vector3(8.5, 2, 8.5), 
    0.0f, 
    -45.0f, 
    new Vector3(0, 0, 0), 
    true, 
    new Dictionary<EffectType, Effect?>());

var minecraftPlayer = new MinecraftPlayer("yea", new UUID(), 0, GameMode.Survival, entity, Dimension.Overworld, PermissionLevel.Admin);

var chunkBuffer = new List<byte>();
for (int i = 0; i < 24; i++)
    chunkBuffer.AddRange([0, 0, 0, 0, 0, 0, 0, 0]);

chunk.LoadData(chunkBuffer.ToArray());
world.LoadChunk(chunk);

var stone = data.Blocks.ByType(BlockType.Stone)!;
world.SetBlock(new Block(stone, stone.DefaultState, new Position(8, 1, 8)));
world.SetBlock(new Block(stone, stone.DefaultState, new Position(9, 1, 9)));

var sim = new PlayerPhysics(data, minecraftPlayer, world, input);

while (true)
{
    var key = Console.ReadKey();

    if (key.Key == ConsoleKey.W)
    {
        input.ForwardKeyDown = !input.ForwardKeyDown;
        Console.WriteLine($"Forward = {input.ForwardKeyDown}");
        continue;
    }

    if (key.Key == ConsoleKey.S)
    {
        input.BackwardKeyDown = !input.BackwardKeyDown;
        Console.WriteLine($"Backward = {input.BackwardKeyDown}");
        continue;
    }
    
    sim.Tick();
    Console.WriteLine($"Pos = {entity.Position}, Vel = {entity.Velocity} IsOnGround={entity.IsOnGround}");
}

// Console.ReadKey();
//
// var bot = await new BotBuilder()
//     .Host("localhost")
//     .OfflineSession("MineSharpBot")
//     .WithPlugin<Pathfinder>()
//     .WithPlugin<AutoRespawn>()
//     .AutoConnect()
//     .CreateAsync();
//
// await bot.Connect();
//
// var chat = bot.GetPlugin<ChatPlugin>();
// var physics = bot.GetPlugin<PhysicsPlugin>();
// var pathfinder = bot.GetPlugin<Pathfinder>();
// var player = bot.GetPlugin<PlayerPlugin>();
// var world = bot.GetPlugin<WorldPlugin>();
//
// if (!player.IsAlive!.Value)
//     await player.Respawn();
//
// chat.OnChatMessageReceived += (sender, playerUuid, message, position, name) =>
// {
//     var splits = message.Message.Split(' ');
//     
//     if (splits.Length != 3)
//         return;
//
//     var ints = splits.Select(x => Convert.ToInt32(x)).ToArray();
//     pathfinder.Goto(new Position(ints[0], ints[1], ints[2])).Wait();
// };
//
// Console.ReadKey();
//
// return;
//
// await PlayerTests.RunAll();
// await WindowTests.RunAll();
// await WorldTests.RunAll();
// await CraftingTests.RunAll();
// await PhysicsTests.RunAll();
