![banner](banner.png)

# MineSharp

[![Gitter](https://img.shields.io/gitter/room/MineSharp-net/community?style=for-the-badge)](https://gitter.im/MineSharp-net/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)
[![Discord](https://img.shields.io/badge/Discord-Join-green?style=for-the-badge)](https://discord.gg/Pt6JT5nXMr)
[![License](https://img.shields.io/github/license/psu-de/MineSharp?style=for-the-badge)](https://github.com/psu-de/MineSharp/blob/main/LICENSE)

**This Project is not finished and under development!**

Create Minecraft bots with C#
Inspired by [Mineflayer](https://github.com/PrismarineJS/mineflayer)

If you're interested in this project, feel free to contribute!

Currently, MineSharp is only supporting Minecraft version 1.18.1, I want to provide support for mutliple versions tho.

# Current features

- ✨Supported Version: Minecraft Java 1.18.1
- 📈 Player Stats
- ⚡ Events
- 🐖 Entity tracking
- 🌍 World tracking (query the world for blocks)
- ⛏️ Mining
- 👷‍♂️ Building
- 🛠️ Crafting
- 🪟 High-Level window api (needs some more work)
- ⚔️ Attacking entities
- 🏃 Movements (Walking, Sprinting, Jumping, Sneaking)
- 🔎 Simple Pathfinder
- 📝 Chat (Reading and Writing)

# Roadmap

- 🏊 Support more forms of movement (Swimming, climbing, maybe flying)
- 🪄 Support more versions

# Projects Overview

### [MineSharp.Core](https://github.com/psu-de/MineSharp/tree/main/MineSharp.Core)

Contains core functionality like Logging, Basic Minecraft Types and (versioning)

### [MineSharp.Data](https://github.com/psu-de/MineSharp/tree/main/Data/MineSharp.Data)
Contains generated sourcecode from [minecraft-data](https://github.com/PrismarineJS/minecraft-data) as well as some extension methods.

### [MineSharp.Data.Generator](https://github.com/psu-de/MineSharp/tree/main/Data/MineSharp.Data.Generator)

Transforms the json data from [minecraft-data](https://github.com/PrismarineJS/minecraft-data) into C# source code

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
See [MineSharp.ConsoleClient README](https://github.com/psu-de/MineSharp/blob/main/Clients/MineSharp.ConsoleClient/README.md)
for a list of commands

![MineSharp.ConsoleClient](https://i.ibb.co/HgYtkN0/Bild-2022-07-20-141355981.png)
