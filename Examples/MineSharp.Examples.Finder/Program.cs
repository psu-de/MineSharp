/*
 *  How to use:
 *    - When starting the bot will connect to 127.0.0.1:25565 
 *    - Enter commands in the minecraft chat with a different player
 *    - Commands:
 *       - findBlock <blockName> (see MineSharp.Data.Blocks.BlockType)  | Don't try 'findBlock Air', it will take up to 5GB of RAM in a normal world
 *       - findEntity <entityName> (see MineSharp.Data.Entities.EntityType)
 * 
 *      Example:
 *       - 'findBlock DiamondOre'
 *       - 'findEntity Zombie'
 */

using MineSharp.Bot;
using MineSharp.Core.Types;
using MineSharp.Data.Blocks;
using MineSharp.Data.Entities;

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
            case "findBlock": FindBlock(arg); break;
            case "findEntity": FindEntity(arg); break;
        }
    }
});

void FindBlock(string arg)
{
    if (!Enum.TryParse(arg, true, out BlockType blockType))
    {
        bot.Chat($"Unknown block name: {arg}. See enum {typeof(BlockType).FullName} for all available block names.");
        return;
    }

    bot.Chat("Searching...");

    var blocks = bot.FindBlocksAsync(blockType)
        .GetAwaiter().GetResult();                                              // Query the world for the block type without a limit

    if (blocks == null)
    {
        bot.Chat($"Could not find any blocks of type {blockType.ToString()}");
        return;
    }

    bot.Chat($"Found {blocks.Length} blocks of type {blockType.ToString()}");

    if (blocks.Length > 0)
    {
        bot.Chat($"{blocks[0]}");
    }
}

void FindEntity(string arg)
{
    if (!Enum.TryParse(arg, true, out EntityType entityType))
    {
        bot.Chat($"Unknown Entity name: {arg}. See enum {typeof(EntityType).FullName} for all available block names.");
        return;
    }

    bot.Chat("Searching...");

    var entities = bot.Entities.Values
        .Where(x => x.Info.Id == (int)entityType)
        .ToArray();                                 // Find the entities with id 

    if (entities.Length == 0)
    {
        bot.Chat($"Could not find any entities of type {entityType.ToString()}");
        return;
    }

    bot.Chat($"Found {entities.Length} entities of type {entityType.ToString()}");

    if (entities.Length > 0)
    {
        bot.Chat($"{entities[0]}");
    }
}

void Help()
{
    bot.Chat(" - findBlock <blockName>");
    bot.Chat(" - findEntity <entityName>");
}


void OnCtrlC(object? sender, ConsoleCancelEventArgs e)
{
    Console.WriteLine("Exiting...");
    bot.Client.ForceDisconnect("");
    Task.Delay(1000).Wait();
    Environment.Exit(0);
}

Console.ReadKey();
