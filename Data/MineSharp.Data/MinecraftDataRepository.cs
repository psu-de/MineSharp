using System.Net.Http.Headers;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace MineSharp.Data;

internal class MinecraftDataRepository
{
    private static ILogger Logger           = LogManager.GetCurrentClassLogger();
    private const  string  GithubRepository = "https://raw.githubusercontent.com/PrismarineJS/minecraft-data/master/data/";

    private readonly string     cache;
    private readonly HttpClient client;

    private const    int                          CHECK_FOR_UPDATES_EVERY_N_HOURS = 24;
    private readonly    string                       assetTimeCheckTable;
    private readonly Dictionary<string, DateTime> assetLastTimeChecked;

    public MinecraftDataRepository(string cache, HttpClient client)
    {
        this.client = client;
        this.cache  = cache;

        if (!Directory.Exists(this.cache))
        {
            Directory.CreateDirectory(this.cache);
        }

        this.assetTimeCheckTable = Path.Join(this.cache, "asset_updates.json");
        if (File.Exists(this.assetTimeCheckTable))
            assetLastTimeChecked = JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(File.ReadAllText(this.assetTimeCheckTable))
                                ?? throw new InvalidDataException(
                                       $"the asset update table is not valid. look for '{this.assetTimeCheckTable}' and delete the file.");
        else assetLastTimeChecked = new Dictionary<string, DateTime>();
    }

    public async Task<JToken> GetResourceMap()
    {
        var file = Path.Join(this.cache, "dataPaths.json");
        if (await CheckIfFileNeedsDownload(file))
            await DownloadAsset("dataPaths.json");

        return JToken.Parse(await File.ReadAllTextAsync(file));
    }

    public async Task<JToken> GetAsset(string file, string version)
    {
        var relativePath = this.GetFilePath(file, version);
        var cachePath    = Path.Combine(this.cache, relativePath);

        if (await CheckIfFileNeedsDownload(relativePath))
        {
            Logger.Debug($"Updating '{file}', because a newer version is available or it does not exist");
            await DownloadAsset(relativePath);
        }

        return JToken.Parse(await File.ReadAllTextAsync(cachePath));
    }

    private async Task<bool> CheckIfFileNeedsDownload(string file)
    {
        var localFilePath = Path.Join(this.cache, file);
        if (!File.Exists((localFilePath)))
            return true;
        
        if (assetLastTimeChecked.TryGetValue(file, out var time)
            && (DateTime.Now - time).Hours <= CHECK_FOR_UPDATES_EVERY_N_HOURS)
        {
            return false;
        }
        
        var localFileTime  = File.GetLastWriteTimeUtc(localFilePath);
        var urlEncodedFile = HttpUtility.UrlEncode($"data/{file.Replace('\\', '/')}");
        var url            = $"https://api.github.com/repos/PrismarineJS/minecraft-data/commits?path={urlEncodedFile}&per_page=1";

        var request = new HttpRequestMessage()
        {
            RequestUri = new Uri(url), Method = HttpMethod.Get,
        };
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("MineSharp", "1.0"));
        
        try
        {
            Logger.Debug($"Checking {file} for updates");
            var message = await this.client.SendAsync(request);
            var token   = JToken.Parse(await message.Content.ReadAsStringAsync());
            
            var timeToken = token.SelectToken("[0].commit.committer.date");
            
            // something probably went wrong, ignore for now
            if (timeToken is null)
                return false;

            var dt            = ((DateTime)timeToken).ToUniversalTime();
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

        var cacheFilePath  = Path.Combine(this.cache, file);
        var cacheDirectory = Path.GetDirectoryName(cacheFilePath) ?? throw new FileNotFoundException();

        if (!Directory.Exists(cacheDirectory))
            Directory.CreateDirectory(cacheDirectory!);

        try
        {
            await using var stream     = await this.client.GetStreamAsync(url);
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

        return File.WriteAllTextAsync(this.assetTimeCheckTable, json);
    }
}
