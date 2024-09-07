namespace MineSharp.Core.Common;

/// <summary>
///     A Minecraft Version
/// </summary>
public class MinecraftVersion
{
    /// <summary>
    ///     Create a new MinecraftVersion instance.
    /// </summary>
    /// <param name="version"></param>
    /// <param name="protocol"></param>
    public MinecraftVersion(string version, int protocol)
    {
        var versionNumbers = version.Split(".").Select(x => Convert.ToInt32(x)).ToArray();
        Major = versionNumbers[0];
        Minor = versionNumbers[1];
        Patch = versionNumbers.Length > 2 ? versionNumbers[2] : 0;
        Protocol = protocol;
    }

    /// <summary>
    ///     The protocol version number
    /// </summary>
    public int Protocol { get; }

    /// <summary>
    ///     The major version
    /// </summary>
    public int Major { get; }

    /// <summary>
    ///     The minor version
    /// </summary>
    public int Minor { get; }

    /// <summary>
    ///     The patch number
    /// </summary>
    public int Patch { get; }

    /// <summary>
    ///     Whether <paramref name="a" /> is a greater version than <paramref name="b" />
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator >=(MinecraftVersion a, MinecraftVersion b)
    {
        if (a.Major != b.Major)
        {
            return a.Major > b.Major;
        }

        if (a.Minor != b.Minor)
        {
            return a.Minor > b.Minor;
        }

        return a.Patch >= b.Patch;
    }

    /// <summary>
    ///     Whether version <paramref name="a" /> is a smaller version than <paramref name="b" />
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator <=(MinecraftVersion a, MinecraftVersion b)
    {
        if (a.Major != b.Major)
        {
            return a.Major < b.Major;
        }

        if (a.Minor != b.Minor)
        {
            return a.Minor < b.Minor;
        }

        return a.Patch <= b.Patch;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Major}.{Minor}.{Patch}";
    }
}
