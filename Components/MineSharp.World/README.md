## MineSharp.World

Create a minecraft world and query it for blocks or modify it.

### Example
```csharp
    var data = MinecraftData.FromVersion("1.20.1");
    var world = WorldVersion.CreateWorld(data);
    var chunk = world.CreateChunk(new ChunkCoordinates(0, 0), Array.Empty<BlockEntity>());
    chunk.LoadData(chunkBuffer);
    
    world.FindBlocks(BlockType.DiamondOre); // Search in all chunks for the specified block
```