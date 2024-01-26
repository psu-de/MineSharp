## MineSharp.Bot

Connect and interact with Minecraft servers. \
A `MineSharpBot` uses a `MinecraftClient` (see MineSharp.Protocol) to connect to a Minecraft Server. \
The bot can have multiple plugins. Each plugin can handle some packets sent by the server and/or provide methods to interact with the world.

## Creating Bots
A bot can be created using a `MinecraftClient`. 
To help out, you can use the `BotBuilder` class to fluently create a bot.

### Bot Builder
 - `.Host()` configure the hostname
 - `.Port()` configure the port (default = 25565)
 - `.Data()` configure the `MinecraftData` instance
 - `.AutoDetectData()` (default) auto detect minecraft data version if none was configured
 - `.Session()` configure the session object
 - `.OfflineSession()` configure session to be an offline session
 - `.OnlineSession()` configure session to be an online session (login happens when calling `Create()` or `CreateAsync()`)
 - `.WithPlugin<T>()` add Plugin of type `T`
 - `.ExcludeDefaultPlugins()` do not add default plugins (listed below)
 - `.AutoConnect()` automatically connect to the server when creating the bot
 - `.WithProxy()` configure a proxy
 - `.CreateAsync()` create a new bot with the configuration
 - `.Create()` equivalent of `CreateAsync().Result`

## Plugins

A plugin can handle packets sent by the server and/or provide methods to interact with the server.

Currently, these are the plugins enabled by default:
 - Chat Plugin (Read and Write chat messages)
 - Crafting Plugin (Craft items)
 - Entity Plugin (Keeps track of entities)
 - Physics Plugin (Simulate player physics)
 - Player Plugin (Keeps track of the bot himself as well as other players and the weather)
 - Window Plugin (Bot's inventory and open chests or other blocks)
 - World Plugin (Keep track of the world)

Other plugins not enabled by default:
 - Auto Respawn (Automatically respawn when dead)

To add a plugin to the bot, `bot.LoadPlugin(plugin)` can be used. \
To access a plugin, use `bot.GetPlugin<>()`

#### Chat Plugin
 - Handles all chat packets and provides abstraction for different minecraft versions
 - Handle and parse the CommandTree
 - ⚡ `OnChatMessageReceived` event. Fired when any chat message or game information message is received
 - 📨 `SendChat()` method. Send a chat message to the server.
 - 📧 `SendCommand()` method. Send a '/' command to the server. Only for mc >= 1.19

#### Crafting Plugin
 - 🔎 `FindRecipes()`. Find recipes for a given item that can be crafted with the items in the bots inventory
 - 🔎 `FindRecipe()`. Equivalent of `FindRecipes().FirstOrDefault()`
 - 🔢 `CraftableAmount()`. Calculate how often a recipe can be crafting with the items in the bots inventory.
 - 🪚 `Craft()`. Craft the given recipe `n` times.

#### Entity Plugin
 - Handles all packets regarding entities (position, effects, etc..)
 - ⚡ `OnEntitySpawn`. Fired when an entity spawned
 - ⚡ `OnEntityDespawned`. Fired when an entity despawned
 - ⚡ `OnEntityMoved`. Fired when an entity moved
 - 🐷 `Entities`. Dictionary mapping all entities from their numerical server id to the `Entity` object

#### Physics Plugin
 - Update the bots position on the server
 - ⚡ `BotMoved`. Fired when the bot moved
 - ⚡ `PhysicsTick`. Fired after each tick of the physics simulation
 - 🎮 `InputControls`. Input controls used to control movement
 - 🪂 `Engine`. The underlying physics simulation / engine
 - ⏳ `WaitForTicks()`. Wait until a number of physics ticks are completed
 - ⛰️ `WaitForOnGround()`. Wait until the bot is on the ground
 - 🔃 `ForceSetRotation()`. Set the bots rotation in a single tick
 - 👓 `ForceLookAt()`. Look at the given position in a single tick
 - 👀 `Look()`. Slowly transition to the given rotation
 - 👀 `LookAt()`. Slowly look at the given position
 - 🔫 `Raycast()`. Returns the block the bot is currently looking at

#### Player Plugin
 - Handles packets regarding the Bot entity and other players on the server
 - ⚡ `OnHealthChanged`. Fired when the bots health, food or saturation was updated.
 - ⚡ `OnRespawed`. Fired when the bot respawned or changed the dimension.
 - ⚡ `OnDied`. Fired when the bot died.
 - ⚡ `OnPlayerJoined`. Fired when another player joined the server
 - ⚡ `OnPlayerLeft`. Fired when another player left the server
 - ⚡ `OnPlayerLoaded`. Fired when another player came into the visible range of the bot and their entity was loaded.
 - ⚡ `OnWeatherChanged`. Fired when the weather has changed. (TODO: Move to WorldPlugin)
 - 🤖 `Self`. The `MinecraftPlayer` representing the bot itself
 - 🤖 `Entity`. The `Entity` representing the bot itself (equivalent of `Self.Entity`)
 - 👨‍👧‍👦 `Players`. A dictionary mapping all player's uuids to their `MinecraftPlayer` object
 - 👨‍👧‍👦 `PlayerMap`. A dictionary mapping all player's numerical server id to their `MinecraftPlayer` object.
 - 💓 `Health`. Health of the Bot (value between 0.0 - 20.0)
 - 🍗 `Saturation`. The Saturation level of the bot
 - 🍕 `Food`. The food level of the bot
 - 🌘 `Dimension`. The name of the dimension the bot is currently in
 - 🍃 `IsAlive`. Boolean indicating whether the bot is alive
 - 🌧️ `IsRaining`. Boolean indicating whether it is raining
 - ☔ `RainLevel`. Float indicating how much it is raining
 - ⛈️ `ThunderLevel`. The thunder level
 - ☀️ `Respawn()`. Respawn the bot if it is dead.
 - 💪 `SwingArm()`. Plays the swing arm animation.
 - 🤺 `Attack()`. Attack the given entity

#### Window Plugin
 - Handles packets regarding windows
 - ⚡ `OnWindowOpened`. Fired when a window opened
 - ⚡ `OnHeldItemChanged`. Fired when the held item changed
 - 📦 `Inventory`. The *Window* representing the bots inventory
 - 🪟 `CurrentlyOpenedWindow`. The window which is currently open.
 - 🎈 `HeldItem`. The *Item* the bot is currently holding in the main hand
 - 👉 `SelectedHotbarIndex`. The index of the selected hotbar slot
 - ⌛ `WaitForInventory()`. Wait until the inventory's item are loaded
 - 🧰 `OpenContainer()`. Try to open the given block (eg. chest, crafting table, ...)
 - ❌ `CloseWindow()`. Close the window
 - 👉 `SelectHotbarIndex()`. Set the selected hotbar index
 - 🙋 `UseItem()`. Use the item the bot is currently holding
 - 👨‍🔧 `EquipItem()`. Find and equip an item

#### World Plugin
 - Handles all block and chunk packets
 - 🌍 `World`. The world of the minecraft server
 - ⏳ `WaitForChunks()`. Wait until all chunks in a radius around the bot are loaded
 - ⌨️ `UpdateCommandBlock()`. Update a command block
 - ⛏️ `MineBlock()`. Mine the given block
 - 👷 `PlaceBlock()`. Place a block at the given position

#### Auto Respawn
 - Automatically respawns the bot when it died
 - ⏰️ `RespawnDelay`. Delay before respawning