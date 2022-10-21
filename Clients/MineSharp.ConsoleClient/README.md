# MineSharp.ConsoleClient

Command Prompt for [MineSharp.Bot](https://github.com/psu-de/MineSharp/tree/main/MineSharp.Bot)

![console picture](https://i.ibb.co/HgYtkN0/Bild-2022-07-20-141355981.png)

ToDo's:

- Exit the prompt when the bot disconnects

## Commands

Supports following commands by now:

### Prompt specific commands

- Help: Displays help about another command
- Show: Shows something about a given option

### Chat

- Say: Writes something into the chat

### Entity

- GetEntities: Prints all entities within a given range and/or filter by entity id

### Misc

- Display: Displays informations about a specified option
- EnablePhysics: Enable or Disable the bot's physics module

### Player

- Attack: Attacks an entity given by their entityId
- LookAt: Forces the bot to look at give coordinates
- Move: Makes the bot move forward/backward/right/left
- PhysicsTick: Simulates a physics tick
- Respawn: Makes the bot respawn
- SelectHotbarSlot: Selects a given slot (1-9) in the hotbar

### World

- FindBlocks: Searches through the world for a specific block Type
- FindBlock: Searches through the world for one specific block of a type
- GetBlockAt: Prints the block at the given coordinates
- MineBlockAt: Mines the block at the given coordinates