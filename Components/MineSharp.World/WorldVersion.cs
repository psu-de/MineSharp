using MineSharp.Core.Common.Blocks;
using MineSharp.Data;
using MineSharp.World.Chunks;
using MineSharp.World.V1_18;

namespace MineSharp.World;

/// <summary>
/// Utility class to map Minecraft versions to the correct classes
/// </summary>
public static class WorldVersion
{
    private static readonly MinecraftVersion Major_1_18 = new MinecraftVersion("1.18", -1);
    
    /// <summary>
    /// Create a new IWorld for the given version
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static IWorld CreateWorld(MinecraftData version)
    {
        if (version.Version >= Major_1_18)
        {
            return new World_1_18(version);
        }

        throw new NotSupportedException($"MineSharp.World does currently not support minecraft version {version.Version}.");
    }
}
