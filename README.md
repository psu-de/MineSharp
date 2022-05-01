# Minesharp

**This Project is not finished and under development!**

Create Minecraft bots with C#
Inspired by [Mineflayer](https://github.com/PrismarineJS/mineflayer)

# Projects
### MineSharp.Core
Contains core functionality like Logging, Basic Minecraft Types and versioning

ToDo's
 - Versioning

### MineSharp.Data
Contains mostly (generated) static code for
 - Biomes
 - Blocks
 - Effects
 - Entities
 - Items
 - (Windows)

ToDo's:
 - Block Loot
 - Enchantments
 - Entity Loot
 - Materials
 - Recipies
 - Foods
 - Attributes?
 - Tints?

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
Contains all Minecraft Packets and Logic to connect and join a Minecraft Server

### Minesharp.World
Basic functionality to represend a Minecraft World