namespace MineSharp.Auth.Cache;

public static class CacheManager
{
    public static string GetCachePath()
    {
        string baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Join(baseFolder, "MineSharp");
    }

    public static string Get(string cache)
    {
        string path = Path.Join(GetCachePath(), cache);

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }
}
