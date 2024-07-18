using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace MineSharp.Data;

internal class MinecraftDataRepository
{
    private const string GithubRepository =
        "https://raw.githubusercontent.com/PrismarineJS/minecraft-data/master/data/";

    private const int CheckForUpdatesEveryNHours = 24;
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    private readonly Dictionary<string, DateTime> assetLastTimeChecked;
    private readonly string assetTimeCheckTable;

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

        assetTimeCheckTable = Path.Join(this.cache, "asset_updates.json");
        if (File.Exists(assetTimeCheckTable))
        {
            assetLastTimeChecked =
                JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(File.ReadAllText(assetTimeCheckTable))
                ?? throw new InvalidDataException(
                    $"the asset update table is not valid. look for '{assetTimeCheckTable}' and delete the file.");
        }
        else
        {
            assetLastTimeChecked = new();
        }
    }

    public async Task<JToken> GetResourceMap()
    {
        var file = Path.Join(cache, "dataPaths.json");
        if (await CheckIfFileNeedsDownload(file))
        {
            await DownloadAsset("dataPaths.json");
        }

        return JToken.Parse(await File.ReadAllTextAsync(file));
    }

    public async Task<JToken> GetAsset(string file, string version)
    {
        var relativePath = GetFilePath(file, version);
        var cachePath = Path.Combine(cache, relativePath);

        if (await CheckIfFileNeedsDownload(relativePath))
        {
            Logger.Debug($"Updating '{file}', because a newer version is available or it does not exist");
            await DownloadAsset(relativePath);
        }

        return JToken.Parse(await File.ReadAllTextAsync(cachePath));
    }

    private async Task<bool> CheckIfFileNeedsDownload(string file)
    {
        var localFilePath = Path.Join(cache, file);
        if (!File.Exists(localFilePath))
        {
            return true;
        }

        if (assetLastTimeChecked.TryGetValue(file, out var time)
            && (DateTime.Now - time).Hours <= CheckForUpdatesEveryNHours)
        {
            return false;
        }

        var localFileTime = File.GetLastWriteTimeUtc(localFilePath);
        var urlEncodedFile = HttpUtility.UrlEncode($"data/{file.Replace('\\', '/')}");
        var url = $"https://api.github.com/repos/PrismarineJS/minecraft-data/commits?path={urlEncodedFile}&per_page=1";

        var request = new HttpRequestMessage { RequestUri = new(url), Method = HttpMethod.Get };
        request.Headers.UserAgent.Add(new("MineSharp", "1.0"));

        try
        {
            Logger.Debug($"Checking {file} for updates");
            var message = await client.SendAsync(request);
            var token = JToken.Parse(await message.Content.ReadAsStringAsync());

            var timeToken = token.SelectToken("[0].commit.committer.date");

            // something probably went wrong, ignore for now
            if (timeToken is null)
            {
                return false;
            }

            var dt = ((DateTime)timeToken).ToUniversalTime();
            await UpdateAssetTable(file, dt);

            return localFileTime - dt <= TimeSpan.Zero;
        }
        catch (HttpRequestException e)
        {
            Logger.Error(e, $"Could not get last commit info for '{file}'. Request url: '{url}'");
            return false;
        }
    }

    private async Task DownloadAsset(string file)
    {
        file = file.Replace(Path.DirectorySeparatorChar, '/');
        var url = GithubRepository + file;

        var cacheFilePath = Path.Combine(cache, file);
        var cacheDirectory = Path.GetDirectoryName(cacheFilePath) ?? throw new FileNotFoundException();

        if (!Directory.Exists(cacheDirectory))
        {
            Directory.CreateDirectory(cacheDirectory!);
        }

        try
        {
            await using var stream = await client.GetStreamAsync(url);
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

    private Task UpdateAssetTable(string key, DateTime value)
    {
        assetLastTimeChecked[key] = value;
        var json = JsonConvert.SerializeObject(assetLastTimeChecked);

        return File.WriteAllTextAsync(assetTimeCheckTable, json);
    }
}
