using MineSharp.Core.Cache;
using NLog;
using System.Diagnostics;

namespace MineSharp.Data;

public static class MinecraftDataRepo
{
    private const string MINECRAFT_DATA_REPO = "https://github.com/PrismarineJS/minecraft-data";
    private const string REPO_FOLDER = "minecraft-data";

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private static void DownloadMinecraftData(string directory)
    {
        Logger.Info("Downloading minecraft-data. This is a one time fix.");
        
        var startInfo = new ProcessStartInfo() 
        {
            WorkingDirectory = directory,
            FileName = "git",
            Arguments = $"clone {MINECRAFT_DATA_REPO}"
        };

        var process = Process.Start(startInfo);

        if (process == null || process.HasExited)
        {
            throw new IOException("Could not download minecraft-data. Make sure git is installed.");
        }
        
        process.WaitForExit();

        if (0 != process.ExitCode)
        {
            throw new IOException("git exited with an error exit code.");
        }
        Logger.Debug($"Downloaded minecraft-data to '{directory}'.");
    }
    
    public static void DownloadIfNecessary(string cacheDirectory)
    {
        var folder = Path.Join(cacheDirectory, REPO_FOLDER);
        if (!Directory.Exists(folder))
        {
            DownloadMinecraftData(cacheDirectory);
        }
    }

    public static string GetPath()
    {
        string cache = CacheManager.Get(MinecraftData.CACHE_NAME);
        return Path.Join(cache, REPO_FOLDER);
    }

    public static string GetAbsoluteDataPath(string relative)
    {
        return Path.Join(GetPath(), "data", relative);
    }

    internal static string GetJsonFile(string path, string which)
    {
        return Path.Join(GetAbsoluteDataPath(path), which + ".json");
    }
}
