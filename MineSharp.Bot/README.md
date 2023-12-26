## MineSharp.Bot

Connect and interact with Minecraft servers. \
A `MinecraftBot` is a composition of multiple `Plugin`s. \
Currently, these are the default plugins:
 - Chat Plugin (Read and Write chat messages)
 - Crafting Plugin (Craft items)
 - Entity Plugin (Keeps track of entities)
 - Player Plugin (Keeps track of the bot himself as well as other players and the weather)
 - Window Plugin (Bot's inventory and open chests or other blocks)
 - World Plugin (Keep track of the world)

### Example
This is an example for a simple chat bot.
```csharp
using MineSharp.Bot;
using MineSharp.Bot.Plugins;

MinecraftBot bot = await MinecraftBot.CreateBot(
    "MineSharpBot",
    "127.0.0.1",
    25565,
    offline: true);

var chat = bot.GetPlugin<ChatPlugin>();

if (!await bot.Connect()) 
{
    Console.WriteLine("Could not connect to server!");
    Environment.Exit(1);
}

while (true)
{
    var input = Console.ReadLine();
    if (input == "exit") 
    {
        await bot.Disconnect();
        break;
    }
    
    if (input != null)
        await chat.SendChat(input);
}
```

TODO: Docs for every plugin