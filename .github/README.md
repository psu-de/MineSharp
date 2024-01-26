[![Gitter](https://img.shields.io/gitter/room/MineSharp-net/community?style=for-the-badge)](https://gitter.im/MineSharp-net/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)
[![Discord](https://img.shields.io/badge/Discord-Join-green?style=for-the-badge)](https://discord.gg/Pt6JT5nXMr)
[![License](https://img.shields.io/github/license/psu-de/MineSharp?style=for-the-badge)](https://github.com/psu-de/MineSharp/blob/main/LICENSE)
[![Nuget](https://img.shields.io/nuget/v/MineSharp.Bot?style=for-the-badge)](https://www.nuget.org/packages/MineSharp.Bot)

\
![banner](banner.png)

# MineSharp
**This Project is not finished and under development!**

MineSharp is a framework to work with minecraft. \
My goal is to create a bot framework which is able to do anything you can do in the vanilla client,
but theoretically MineSharp could also be used to create a Server or custom client.

Currently MineSharp works with **Minecraft Java 1.18 - 1.20.4**.

If you're interested in this project, feel free to contribute!

# Current features
- ✨ Supported Versions: 1.18.x - 1.20.4
- 📈 Player Stats
- ⚡ Events
- 🐖 Entity tracking
- 🌍 World tracking (query the world for blocks)
- ⛏️ Mining (very simple, needs some more work)
- 👷‍♂️ Building (very simple, needs some more work)
- 🛠️ Crafting
- 🪟 High-Level window Api
- ⚔️ Attacking entities
- 🏃 Movements (Walking, Sprinting, Jumping, Sneaking)
- 📝 Chat (Reading and Writing)

# Roadmap
- 🔎 Simple Pathfinder (see [feature/pathfinder](https://github.com/psu-de/MineSharp/pull/32))

# Example Snippet
See [MineSharp.Bot](../MineSharp.Bot/README.md) for more information about creating bots.
```csharp
using MineSharp.Bot;
using MineSharp.Bot.Plugins;

MineSharpBot bot = await new BotBuilder()
    .Host("localhost")
    .OfflineSession("MineSharpBot")
    .CreateAsync();

ChatPlugin chat = bot.GetPlugin<ChatPlugin>();

if (!await bot.Connect()) 
{
    Console.WriteLine("Could not connect to server! Is the server running?");
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

### Credits
Without the following resources this project would not be possible. Thanks to all people involved in those projects!

- [wiki.vg](https://wiki.vg)
- [Minecraft-Console-Client](https://github.com/MCCTeam/Minecraft-Console-Client)
- [mineflayer](https://github.com/PrismarineJS/mineflayer)
- [Alex](https://github.com/ConcreteMC/Alex)
