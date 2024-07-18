namespace MineSharp.Auth.Cache;

internal static class CacheManager
{
    public static string GetCachePath()
    {
        var baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Join(baseFolder, "MineSharp");
    }

    public static string Get(string cache)
    {
        var path = Path.Join(GetCachePath(), cache);

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }
}
