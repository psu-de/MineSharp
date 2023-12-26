## MineSharp.Data

Provides static data for different Minecraft versions.
The Data is code generated from from MineSharp.SourceGenerator, using data from [minecraft-data](https://github.com/PrismarineJS/minecraft-data).

### Example
```csharp
    var data = MinecraftData.FromVersion("1.20.2");
    var blockInfo = data.Blocks.GetByType(BlockType.DiamondOre);
```