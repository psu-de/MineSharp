using MineSharp.Auth.Json;
using MineSharp.Auth.Responses;
using MineSharp.Core.Common;
using Newtonsoft.Json;
using NLog;
using System.Net;
using System.Net.Http.Headers;

namespace MineSharp.Auth;

/// <summary>
/// Wrapper for the minecraft services
/// </summary>
public class MinecraftApi
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    
    private const string SERVICE_URL = "https://api.minecraftservices.com";
    private const string SESSION_URL = "https://sessionserver.mojang.com";
    
    private readonly HttpClient client;
    private readonly string ServiceUrl;
    private readonly string SessionUrl;
    
    /// <summary>
    /// Create a new MinecraftApi instance
    /// </summary>
    public MinecraftApi()
        : this(new HttpClient())
    { }

    /// <summary>
    /// Create a new MinecraftApi with custom minecraft and session services
    /// </summary>
    /// <param name="minecraftService"></param>
    /// <param name="sessionService"></param>
    public MinecraftApi(string minecraftService, string sessionService)
        : this(new HttpClient(), minecraftService, sessionService)
    { }

    /// <summary>
    /// Create a new MinecraftApi with a custom http client, minecraft and session service
    /// </summary>
    /// <param name="client"></param>
    /// <param name="minecraftService"></param>
    /// <param name="sessionService"></param>
    public MinecraftApi(HttpClient client, string minecraftService = SERVICE_URL, string sessionService = SESSION_URL)
    {
        this.client = client;
        this.ServiceUrl = minecraftService;
        this.SessionUrl = sessionService;
    }

    /// <summary>
    /// Send a Join server request to the mojang session servers
    /// </summary>
    /// <param name="hash"></param>
    /// <param name="sessionToken"></param>
    /// <param name="uuid"></param>
    /// <returns></returns>
    public async Task<bool> JoinServer(string hash, string sessionToken, UUID uuid)
    {
        Logger.Debug($"Sending join server request to mojang.");
        var request = new HttpRequestMessage(HttpMethod.Post, $"{this.SessionUrl}/session/minecraft/join");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var content = new JoinServerBlob() {
            ServerId = hash,
            AccessToken = sessionToken,
            SelectedProfile = uuid.ToString().Replace("-", "").ToLower()
        };
        
        HttpContent body = new StringContent(JsonConvert.SerializeObject(content));
        body.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        request.Content = body;
        var response = await this.client.SendAsync(request);

        if (response.IsSuccessStatusCode)
            return true;

        Logger.Error("Session server returned error code: " + response.StatusCode + "  " + await response.Content.ReadAsStringAsync());
        return false;
    }
    
    /// <summary>
    /// Fetch user certificates from minecraft services
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<PlayerCertificate> FetchCertificates(string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{this.ServiceUrl}/player/certificates");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await this.client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        var blob = JsonConvert.DeserializeObject<CertificateBlob>(body)!;

        return PlayerCertificate.FromBlob(blob);
    }
    
}
