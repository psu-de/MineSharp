namespace MineSharp.Core.Geometry;

/// <summary>
///     Face of a block
/// </summary>
public enum BlockFace
{
    /// <summary>
    /// Offset: -Y
    /// </summary>
    Bottom = 0,
    /// <summary>
    /// Offset: +Y
    /// </summary>
    Top = 1,
    /// <summary>
    /// Offset: -Z
    /// </summary>
    North = 2,
    /// <summary>
    /// Offset: +Z
    /// </summary>
    South = 3,
    /// <summary>
    /// Offset: -X
    /// </summary>
    West = 4,
    /// <summary>
    /// Offset: +X
    /// </summary>
    East = 5
}
