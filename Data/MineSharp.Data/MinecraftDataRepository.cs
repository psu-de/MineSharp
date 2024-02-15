using Newtonsoft.Json.Linq;
using NLog;

namespace MineSharp.Data;

internal class MinecraftDataRepository
{
    private static ILogger Logger = LogManager.GetCurrentClassLogger();
    private const string GithubRepository = "https://raw.githubusercontent.com/PrismarineJS/minecraft-data/master/data/";

    private readonly string cache;
    private readonly HttpClient client;

    public MinecraftDataRepository(string cache, HttpClient client)
    {
        this.client = client;
        this.cache = cache;

        if (!Directory.Exists(this.cache))
        {
            Directory.CreateDirectory(this.cache);
        }
    }

    public async Task<JToken> GetResourceMap()
    {
        var file = Path.Join(this.cache, "dataPaths.json");
        if (!File.Exists(file))
            await DownloadAsset("dataPaths.json");

        return JToken.Parse(await File.ReadAllTextAsync(file));
    }

    public async Task<JToken> GetAsset(string file, string version)
    {
        var filePath = this.GetFilePath(file, version);
        if (!File.Exists(Path.Combine(this.cache, filePath)))
            await DownloadAsset(filePath);

        return JToken.Parse(await File.ReadAllTextAsync(filePath));
    }

    private async Task DownloadAsset(string file)
    {
        file = file.Replace(Path.DirectorySeparatorChar, '/');
        var url = GithubRepository + file;

        var cacheFilePath = Path.Combine(this.cache, file);
        var cacheDirectory = Path.GetDirectoryName(cacheFilePath) ?? throw new FileNotFoundException();
        
        if (!Directory.Exists(cacheDirectory))
            Directory.CreateDirectory(cacheDirectory!);

        try
        {
            await using var stream = await this.client.GetStreamAsync(url);
            await using var fileStream = new FileStream(cacheFilePath, FileMode.OpenOrCreate, FileAccess.Write);

            await stream.CopyToAsync(fileStream);
        }
        catch (HttpRequestException e)
        {
            Logger.Error(e, $"Could not fetch asset '{file}'. Request url: '{url}'");
        }
    }

    private string GetFilePath(string file, string version)
    {
        return Path.Combine(version, file.Replace('/', Path.DirectorySeparatorChar));
    }
}