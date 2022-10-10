/*
 *  How to use:
 *    - When starting the bot will connect to 127.0.0.1:25565 
 *    - Enter commands in the minecraft chat with a different player
 *    - Commands:
 *       - pathfind <x> <y> <z> | Tries to find a path and move along that path. Currently only a basic moveset is supported. See MineSharp.Pathfinder
 * 
 *      Example:
 *       - 'pathfind -10 84 25'
 */

using MineSharp.Bot;
using MineSharp.Core.Types;
using MineSharp.Pathfinding;
using MineSharp.Pathfinding.Goals;

MinecraftBot bot = new MinecraftBot(new MinecraftBot.BotOptions() {
    Host = "127.0.0.1",
    Offline = true,
    Port = 25565,
    Version = "1.18.1",
    UsernameOrEmail = "MineSharpBot"
});

Console.CancelKeyPress += OnCtrlC;

_ = Task.Run(async () =>                                                        // Must run in another Task to prevent blocking
{
    Console.WriteLine("Connecting...");
    bool successful = await bot.Connect();                                      // Connect to the server

    if (!successful)
    {
        Console.WriteLine("Could not connect to server");
        Environment.Exit(0);
    }
    Console.WriteLine("Connected!");

    await bot.WaitForBot();                                                     // Wait until the bot entity is initialized
    bot.Died += (sender, chat) => bot.Respawn();                     // Auto Respawn the bot when he dies
    
    await bot.WaitForChunksToLoad();                                            // Wait until the world chunks around the bot have been loaded
    bot.ChatMessageReceived += OnMessageReceived;                               // Event fired when a chat message is received

    await bot.LoadModule(new PathfinderModule(bot));                            // Load pathfinder module

    void OnMessageReceived(MinecraftBot sender, string message, MinecraftPlayer messageSender)
    {
        if (messageSender.Username == sender.Player!.Username)
        {
            return;
        }

        var cmd = message.Split(' ').First();
        var arg = string.Join(' ', message.Split(' ').Skip(1));

        switch (cmd)
        {
            case "help": Help(); break;
            case "pathfind": Pathfind(arg); break;
        }
    }
});

void Pathfind(string arg)
{
    var splits = arg.Split(' ');
    if (splits.Length != 3)
    {
        bot.Chat("Expected 3 coordinates: pathfind <x> <y> <z>");
        return;
    }

    try
    {
        var coords = splits.Select(int.Parse).ToArray();

        var path = bot.FindPath(new GoalXYZ(coords[0], coords[1], coords[2]))
            .GetAwaiter().GetResult();                                          // Find path to coordinates

        bot.Chat($"Found a path with {path.Nodes.Length} nodes");
        bot.MovePath(path).Wait();                                              // Move along path
        bot.Chat("Arrived!");
    } catch (Exception e)
    {
        bot.Chat("Something went wrong: " + e.Message);
    }
}

void Help()
{
    bot.Chat(" - pathfind <x> <y> <z>");
}


void OnCtrlC(object? sender, ConsoleCancelEventArgs e)
{
    Console.WriteLine("Exiting...");
    bot.Client.ForceDisconnect("");
    Task.Delay(1000).Wait();
    Environment.Exit(0);
}

Console.ReadKey();
