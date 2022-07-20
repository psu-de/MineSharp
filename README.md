# Minesharp

**This Project is not finished and under development!**

Create Minecraft bots with C#
Inspired by [Mineflayer](https://github.com/PrismarineJS/mineflayer)

# Projects
## MineSharp.Core
Contains core functionality like Logging, Basic Minecraft Types and versioning

ToDo's
 - Versioning

### MineSharp.Data
Dont reference this project directly, use MineSharp.Data.Wrapper
Contains mostly (generated) code for
 - Biomes
 - Blocks
 - Effects
 - Enchantments
 - Entities
 - Items
 - Protocol
 - (Windows)

ToDo's:
 - Block Loot
 - ~~Enchantments~~
 - Entity Loot
 - Materials
 - Recipies
 - Foods
 - Attributes?
 - Tints?

### MineSharp.Data.Generator
Transforms the json data from [minecraft-data](https://github.com/PrismarineJS/minecraft-data) into C# source code

### MineSharp.Data.Wrapper
This is really the MineSharp.Data module. It helps with the (sometimes weird) generated data from MineSharp.Data

### MineSharp.Bot
 Functions and logic to directly interact with a minecraft server\
 See [MineSharp.Bot README](https://github.com/psu-de/MineSharp/blob/main/MineSharp.Bot/README.md)

## Components

### MineSharp.MojangAuth
Used to connect to Mojang Auth servers and create a Minecraft Session

### MineSharp.Physics
Logic to simulation player physics from minecraft\
By now mostly copied from Mineflayer 😇

ToDo's:
 - Water Movement doesnt work
 - Jumping doesnt work

### MineSharp.Protocol
Implements the Minecraft Protocol. Contains logic to connect to a Minecraft server and read/write packets from/to it.

### Minesharp.World
Basic functionality to represent a Minecraft World

## Clients
### MineSharp.ConsoleClient
Console Prompt for MineSharp.Bot\
Used for testing\
See [MineSharp.ConsoleClient README](https://github.com/psu-de/MineSharp/blob/main/Clients/MineSharp.ConsoleClient/README.md) for a list of commands

 ![MineSharp.ConsoleClient](https://i.ibb.co/HgYtkN0/Bild-2022-07-20-141355981.png)