using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.World;

/// <summary>
///     Specifies the Dimension.
/// </summary>
public record DimensionInfo(Dimension Dimension, int WorldMinY, int WorldMaxY)
{
    public static readonly MinecraftVersion MinecraftVersionMajor118 = new("1.18", -1);

    // TODO: Select Version Specific Dimension Info
    // since we currently only support 1.18 and above this is fine

    public static readonly DimensionInfo Overworld = new DimensionInfo(Dimension.Overworld, -64, 320);
    public static readonly DimensionInfo Nether = new DimensionInfo(Dimension.Nether, 0, 256);
    public static readonly DimensionInfo End = new DimensionInfo(Dimension.End, 0, 256);

    /// <summary>
    ///     Gets the height of the world.
    /// </summary>
    /// <returns>
    ///     The height of the world, calculated as the difference between WorldMaxY and WorldMinY.
    /// </returns>
    public int GetWorldHeight()
    {
        return WorldMaxY - WorldMinY;
    }

    /// <summary>
    ///     Gets the DimensionInfo for the specified Dimension.
    /// </summary>
    /// <param name="dimension">The dimension to get the DimensionInfo for.</param>
    /// <returns>The DimensionInfo for the specified dimension.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the specified dimension is not recognized.
    /// </exception>
    public static DimensionInfo FromDimension(Dimension dimension)
    {
        return dimension switch
        {
            Dimension.Overworld => Overworld,
            Dimension.Nether => Nether,
            Dimension.End => End,
            _ => throw new ArgumentOutOfRangeException(nameof(dimension), dimension, null)
        };
    }
}
