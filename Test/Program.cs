using MineSharp.Bot;
using MineSharp.Bot.Plugins;
using MineSharp.Data;



var bot = await MinecraftBot.CreateBot(
    "MineSharpBot",
    "127.0.0.1",
    25565,
    offline: true);

var worldPl = bot.GetPlugin<WorldPlugin>();
var player = bot.GetPlugin<PlayerPlugin>();
var entities = bot.GetPlugin<EntityPlugin>();
var chat = bot.GetPlugin<ChatPlugin>();

player.OnDied += _ => Task.Run(async () =>
{
    Console.WriteLine("Died");
    await Task.Delay(1000);
    await player.Respawn();
});
player.OnRespawned += _ => Console.WriteLine("Respawned!");
player.OnHealthChanged += _ => Console.WriteLine($"Health changed: {player.Health}.");

entities.OnEntitySpawned += (sender, entity) =>
{
    Console.WriteLine(entity);
};

chat.OnChatMessageReceived += (sender, uuid, message, position) =>
{
    Console.WriteLine($"{uuid} wrote: {message}");
};

await bot.Connect();
await player.WaitForInitialization();

while (true)
{
    var input = Console.ReadLine();
    if (input != null)
        await chat.SendChat(input);
}

Console.ReadKey();
await player.Respawn();


while (true)
{
    string input = Console.ReadLine()!;
    var start = DateTime.Now;
    int id = bot.Data.Blocks.GetByName(input).Id;
    var blocks = worldPl.World!.FindBlocks(id).ToArray();

    foreach (var block in blocks.Take(100))
    {
        Console.WriteLine(block);
    }

    Console.WriteLine($"Found {blocks.Length} blocks in {(DateTime.Now - start).TotalMilliseconds}ms.");
}

Console.ReadKey();