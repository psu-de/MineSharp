using MineSharp.Auth.Json;
using MineSharp.Auth.Responses;
using MineSharp.Core.Common;
using Newtonsoft.Json;
using NLog;

namespace MineSharp.Auth;

/// <summary>
///     Wrapper for the minecraft services
/// </summary>
public class MinecraftApi
{
    /// <summary>
    /// Default instance
    /// </summary>
    public static readonly MinecraftApi Instance = new();
    
    private const string ServiceUrl = "https://api.minecraftservices.com";
    private const string SessionUrl = "https://sessionserver.mojang.com";
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    private readonly HttpClient client;

    private readonly string serviceUrl;
    private readonly string sessionUrl;

    /// <summary>
    ///     Create a new MinecraftApi instance
    /// </summary>
    public MinecraftApi()
        : this(new())
    { }

    /// <summary>
    ///     Create a new MinecraftApi with custom minecraft and session services
    /// </summary>
    /// <param name="minecraftService"></param>
    /// <param name="sessionService"></param>
    public MinecraftApi(string minecraftService, string sessionService)
        : this(new(), minecraftService, sessionService)
    { }

    /// <summary>
    ///     Create a new MinecraftApi with a custom http client, minecraft and session service
    /// </summary>
    /// <param name="client"></param>
    /// <param name="minecraftService"></param>
    /// <param name="sessionService"></param>
    public MinecraftApi(HttpClient client, string minecraftService = ServiceUrl, string sessionService = SessionUrl)
    {
        this.client = client;
        serviceUrl = minecraftService;
        sessionUrl = sessionService;
    }

    /// <summary>
    ///     Send a Join server request to the mojang session servers
    /// </summary>
    /// <param name="hash"></param>
    /// <param name="sessionToken"></param>
    /// <param name="uuid"></param>
    /// <returns></returns>
    public async Task<bool> JoinServer(string hash, string sessionToken, Uuid uuid)
    {
        Logger.Debug("Sending join server request to mojang.");
        var request = new HttpRequestMessage(HttpMethod.Post, $"{sessionUrl}/session/minecraft/join");
        request.Headers.Accept.Add(new("application/json"));
        var content = new JoinServerBlob
        {
            ServerId = hash,
            AccessToken = sessionToken,
            SelectedProfile = uuid.ToString().Replace("-", "").ToLower()
        };

        HttpContent body = new StringContent(JsonConvert.SerializeObject(content));
        body.Headers.ContentType = new("application/json");

        request.Content = body;
        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        Logger.Error("Session server returned error code: " + response.StatusCode + "  " +
                     await response.Content.ReadAsStringAsync());
        return false;
    }

    /// <summary>
    ///     Fetch user certificates from minecraft services
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<PlayerCertificate> FetchCertificates(string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{serviceUrl}/player/certificates");
        request.Headers.Authorization = new("Bearer", token);

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        var blob = JsonConvert.DeserializeObject<CertificateBlob>(body)!;

        return PlayerCertificate.FromBlob(blob);
    }
}
