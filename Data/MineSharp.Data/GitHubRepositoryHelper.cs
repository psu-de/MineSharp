using System.Web;
using MineSharp.Core.Cache;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace MineSharp.Data;

internal class GitHubRepositoryHelper
{
    private const string GITHUB_CONTENT_URL = "https://raw.githubusercontent.com";
    private const string GITHUB_API_URL = "https://api.github.com/repos";
    
    private static readonly HttpClient HttpClient = new();
    private static readonly TimeSpan InvalidateFilesAfter = TimeSpan.FromDays(1);

    private readonly string repository;
    private readonly string repositoryContentUrl;
    private readonly string cache;
    private readonly string assetInvalidationTableFile;
    private readonly IDictionary<string, DateTime> assetsLastChecked;

    public GitHubRepositoryHelper(string repositoryName)
    {
        repository = repositoryName;
        repositoryContentUrl = $"{GITHUB_CONTENT_URL}/{repositoryName}";
        cache = CacheManager.GetCacheDirectory(repositoryName.Split('/')[^1]);
        assetInvalidationTableFile = Path.Combine(cache, "asset_updates.json");
        
        Directory.CreateDirectory(cache);
        
        if (File.Exists(assetInvalidationTableFile))
        {
            assetsLastChecked =
                JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(File.ReadAllText(assetInvalidationTableFile))
                ?? throw new InvalidDataException(
                    $"the asset update table is not valid. look for '{assetInvalidationTableFile}' and delete the file.");
        }
        else
        {
            assetsLastChecked = new Dictionary<string, DateTime>();
        }
    }
    
    public async Task<JToken> GetAsset(string file, string branchOrTag = "master")
    {
        var relativePath = GetFilenameWithBranch(file, branchOrTag);
        var cachePath = Path.Combine(cache, relativePath);

        if (await CheckIfFileNeedsDownload(file, branchOrTag))
        {
            Logger.Debug($"Updating '{relativePath}', because a newer version is available or it does not exist");
            await DownloadAsset(relativePath);
        }

        return JToken.Parse(await File.ReadAllTextAsync(cachePath));
    }
    
    private async Task<bool> CheckIfFileNeedsDownload(string file, string branchOrTag)
    {
        var relativePath = GetFilenameWithBranch(file, branchOrTag);
        var absolutePath = Path.Join(cache, relativePath);
        
        if (!File.Exists(absolutePath))
        {
            return true;
        }

        if (IsAssetValid(relativePath))
        {
            return false;
        }

        var localFileTime = File.GetLastWriteTimeUtc(absolutePath);
        var urlEncodedFile = HttpUtility.UrlEncode($"{file.Replace('\\', '/')}");
        var url = $"{GITHUB_API_URL}/{repository}/commits?sha={branchOrTag}&path={urlEncodedFile}&per_page=1";

        var request = new HttpRequestMessage { RequestUri = new(url), Method = HttpMethod.Get };
        request.Headers.UserAgent.Add(new("MineSharp", "1.0"));

        try
        {
            Logger.Debug($"Checking {file} for updates");
            var message = await HttpClient.SendAsync(request);
            var token = JToken.Parse(await message.Content.ReadAsStringAsync());
            var timeToken = token.SelectToken("[0].commit.committer.date");

            if (timeToken is null)
            {
                // something probably went wrong, ignore for now
                Logger.Debug($"Unexpected response from github api: {token}");
                return false;
            }

            await MarkAssetAsChecked(relativePath);
            
            var dt = ((DateTime)timeToken).ToUniversalTime();
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
        var remoteFile = file.Replace(Path.DirectorySeparatorChar, '/');
        var url = $"{repositoryContentUrl}/{remoteFile}";

        var cacheFilePath  = Path.Combine(cache, file);
        var cacheDirectory = Path.GetDirectoryName(cacheFilePath) ?? throw new FileNotFoundException();

        Directory.CreateDirectory(cacheDirectory);
        
        try
        {
            await using var stream = await HttpClient.GetStreamAsync(url);
            await using var fileStream = new FileStream(cacheFilePath, FileMode.OpenOrCreate, FileAccess.Write);

            await stream.CopyToAsync(fileStream);
            await MarkAssetAsChecked(file);
        }
        catch (HttpRequestException e)
        {
            Logger.Error(e, $"Could not fetch asset '{file}'. Request url: '{url}'");
        }
    }

    private bool IsAssetValid(string key)
    {
        if (!assetsLastChecked.TryGetValue(key, out var lastChecked))
        {
            return true;
        }
        
        return (DateTime.UtcNow - lastChecked) < InvalidateFilesAfter;
    }
    
    private Task MarkAssetAsChecked(string key)
    {
        assetsLastChecked[key] = DateTime.UtcNow;
        var json = JsonConvert.SerializeObject(assetsLastChecked);
        
        return File.WriteAllTextAsync(assetInvalidationTableFile, json);
    }
    
    private static string GetFilenameWithBranch(string file, string branch)
    {
        return Path.Combine(branch, file.Replace('/', Path.DirectorySeparatorChar));
    }
    
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
}
