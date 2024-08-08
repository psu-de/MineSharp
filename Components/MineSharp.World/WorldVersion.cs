using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.World.V1_18;

namespace MineSharp.World;

/// <summary>
///     Utility class to map Minecraft versions to the correct classes
/// </summary>
public static class WorldVersion
{
    /// <summary>
    ///     Create a new IWorld for the given version
    /// </summary>
    /// <param name="version"></param>
    /// <param name="dimension"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static IAsyncWorld CreateWorld(MinecraftData version, Dimension dimension)
    {
        if (version.Version >= DimensionInfo.MinecraftVersionMajor118)
        {
            return new World118(version, DimensionInfo.FromDimension(dimension));
        }

        throw new NotSupportedException(
            $"MineSharp.World does currently not support minecraft version {version.Version}.");
    }
}
