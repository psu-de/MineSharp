# MineSharp.Bot

Functions and logic to directly interact with a minecraft server

## API

### Basic

- `public Task WaitForBot()`   
  Waits until the bot entity has loaded. BotEntity has been set at this point.

- `public Task<T> WaitForPacket<T>()`   
  Waits for a specific packet from the server

- `public void On<T>(Func<T,Task> handler)`   
  Calls handler every time a specific packet is received

- `public Task Respawn()`   
  Respawns the bot. Only possible when the bot is dead.

- `public Task Attack(MineSharp.Core.Types.Entity entity)`   
  Attacks a given entity

- `public Task Chat(string message)`   
  Sends a chat message to the server

### Physics

- `public void ForceSetRotation(float yaw, float pitch)`   
  Sets the bots rotation to a given yaw and pitch

- `public void ForceLookAt(MineSharp.Core.Types.Position pos)`   
  Forces the bot to look at a given position

### Window

- `public Task WaitForInventory()`   
  Waits until the bots inventory has been loaded.

- `public Task<MineSharp.Windows.Window> OpenContainer(MineSharp.Core.Types.Block block)`   
  Opens a window from the block

- `public Task CloseWindow(int windowId)`   
  Closes a window

- `public Task CloseWindow(MineSharp.Windows.Window window)`   
  Closes a window

- `public Task SelectHotbarIndex(byte hotbarIndex)`   
  Selects a slot on the hotbar

### World

- `public MineSharp.Core.Types.Block GetBlockAt(MineSharp.Core.Types.Position pos)`   
  Gets the block at the given position

- `public MineSharp.Core.Types.Biome GetBiomeAt(MineSharp.Core.Types.Position pos)`   
  Gets the biome at the given position

- `public Task<MineSharp.Core.Types.Block[]> FindBlocksAsync(MineSharp.Data.Blocks.BlockType type, int count, CancellationToken? cancellation)`   
  Finds a number of blocks of a given block type

- `public Task<MineSharp.Core.Types.Block> FindBlockAsync(MineSharp.Data.Blocks.BlockType type, CancellationToken? cancellation)`   
  Finds a block of the given block type

- `public Task<MineSharp.Bot.Enums.MineBlockStatus> MineBlock(MineSharp.Core.Types.Block block, BlockFace? face, CancellationToken? cancellation)`   
  Mines the block

- `public Task<MineSharp.Core.Types.Block> Raycast(int length)`   
  Returns the block the bot is looking at

- `public Task WaitForChunksToLoad(int length)`   
  Waits until the chunks around the bot have been loaded 
  

 