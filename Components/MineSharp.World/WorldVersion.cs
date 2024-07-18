using MineSharp.Data;
using MineSharp.World.V1_18;

namespace MineSharp.World;

/// <summary>
///     Utility class to map Minecraft versions to the correct classes
/// </summary>
public static class WorldVersion
{
    private static readonly MinecraftVersion Major118 = new("1.18", -1);

    /// <summary>
    ///     Create a new IWorld for the given version
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static IWorld CreateWorld(MinecraftData version)
    {
        if (version.Version >= Major118)
        {
            return new World118(version);
        }

        throw new NotSupportedException(
            $"MineSharp.World does currently not support minecraft version {version.Version}.");
    }
}
