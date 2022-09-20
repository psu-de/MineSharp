[![Gitter](https://badges.gitter.im/MineSharp-net/community.svg)](https://gitter.im/MineSharp-net/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

# Minesharp

**This Project is not finished and under development!**

Create Minecraft bots with C#
Inspired by [Mineflayer](https://github.com/PrismarineJS/mineflayer)

If you're interested in this project, feel free to contribute!

Currently, MineSharp is only supporting Minecraft version 1.18.1, I want to provide support for mutliple versions tho.

# Projects Overview
## [MineSharp.Core](https://github.com/psu-de/MineSharp/tree/main/MineSharp.Core)
Contains core functionality like Logging, Basic Minecraft Types and (versioning)

### [MineSharp.Data](https://github.com/psu-de/MineSharp/tree/main/Data/MineSharp.Data)
Dont reference this project directly, use MineSharp.Data.Wrapper
Contains generated sourcecode from [minecraft-data](https://github.com/PrismarineJS/minecraft-data)

### [MineSharp.Data.Generator](https://github.com/psu-de/MineSharp/tree/main/Data/MineSharp.Data.Generator)
Transforms the json data from [minecraft-data](https://github.com/PrismarineJS/minecraft-data) into C# source code

### [MineSharp.Data.Wrapper](https://github.com/psu-de/MineSharp/tree/main/Data/MineSharp.Data.Wrapper)
Provides helper functions and extension methods to help with the generated types and the MineSharp.Core types.

### [MineSharp.Bot](https://github.com/psu-de/MineSharp/tree/main/MineSharp.Bot)
 API to directly interact with a minecraft server. \
 See [MineSharp.Bot README](https://github.com/psu-de/MineSharp/tree/main/MineSharp.Bot)

## Components
### [MineSharp.MojangAuth](https://github.com/psu-de/MineSharp/tree/main/Components/MineSharp.MojangAuth)
Used to connect to Mojang Auth servers and create a Minecraft Session

### [MineSharp.Physics](https://github.com/psu-de/MineSharp/tree/main/Components/MineSharp.Physics)
Logic to simulate entity physics from minecraft\
Thanks to [ConcreteMC/Alex](https://github.com/ConcreteMC/Alex)

### [MineSharp.Protocol](https://github.com/psu-de/MineSharp/tree/main/Components/MineSharp.Protocol)
Implements the Minecraft Protocol. Contains logic to connect to a Minecraft server and read/write packets from/to it.

### [MineSharp.World](https://github.com/psu-de/MineSharp/tree/main/Components/MineSharp.World)
Basic functionality to represent a Minecraft World

## Clients
### [MineSharp.ConsoleClient](https://github.com/psu-de/MineSharp/tree/main/Clients/MineSharp.ConsoleClient)
Console Prompt for MineSharp.Bot\
Used for testing\
See [MineSharp.ConsoleClient README](https://github.com/psu-de/MineSharp/blob/main/Clients/MineSharp.ConsoleClient/README.md) for a list of commands

 ![MineSharp.ConsoleClient](https://i.ibb.co/HgYtkN0/Bild-2022-07-20-141355981.png)
