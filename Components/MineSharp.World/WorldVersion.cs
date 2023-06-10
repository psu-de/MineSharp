using MineSharp.Core.Common.Blocks;
using MineSharp.Data;
using MineSharp.World.Chunks;
using MineSharp.World.V1_18;

namespace MineSharp.World;

public static class WorldVersion
{
    private static readonly MinecraftVersion Major_1_18 = new MinecraftVersion("1.18_major");
    
    public static IWorld CreateWorld(MinecraftData version)
    {
        if (version.Version >= Major_1_18)
        {
            return new World_1_18(version);
        }

        throw new NotSupportedException($"MineSharp.World does currently not support minecraft version {version.Version}.");
    }

    public static IChunk CreateChunk(MinecraftData version, ChunkCoordinates coordinates, BlockEntity[] entities)
    {
        if (version.Version >= Major_1_18)
        {
            return new Chunk_1_18(version, coordinates, entities);
        }

        throw new NotSupportedException($"MineSharp.World does currently not support minecraft version {version.Version}.");
    }
}
