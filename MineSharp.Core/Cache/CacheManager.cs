namespace MineSharp.Core.Cache;

/// <summary>
/// A simple cache manager that can create cache directories
/// </summary>
public static class CacheManager
{
    private static readonly string BasePath = Path.Join(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
        "MineSharp");

    /// <summary>
    /// Get or create a cache directory.
    /// </summary>
    /// <param name="directory">The relative path of the cache.</param>
    /// <returns></returns>
    public static string GetCacheDirectory(string directory)
    {
        var path = Path.Join(BasePath, directory);
        Directory.CreateDirectory(path);

        return path;
    }
}
