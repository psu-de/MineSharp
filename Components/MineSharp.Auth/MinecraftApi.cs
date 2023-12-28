using MineSharp.Auth.Json;
using MineSharp.Auth.Responses;
using MineSharp.Core.Common;
using Newtonsoft.Json;
using NLog;
using System.Net.Http.Headers;

namespace MineSharp.Auth;

/// <summary>
/// Helper class to send requests to minecraft services
/// </summary>
public static class MinecraftApi
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    
    private static HttpClient _client = new HttpClient();
    private const string SERVICE_URL = "https://api.minecraftservices.com";
    private const string SESSION_URL = "https://sessionserver.mojang.com";

    /// <summary>
    /// Send a Join server request to the mojang session servers
    /// </summary>
    /// <param name="hash"></param>
    /// <param name="sessionToken"></param>
    /// <param name="uuid"></param>
    /// <returns></returns>
    public static async Task<bool> JoinServer(string hash, string sessionToken, UUID uuid)
    {
        Logger.Debug($"Sending join server request to mojang.");
        var request = new HttpRequestMessage(HttpMethod.Post, $"{SESSION_URL}/session/minecraft/join");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var content = new JoinServerBlob() {
            ServerId = hash,
            AccessToken = sessionToken,
            SelectedProfile = uuid.ToString().Replace("-", "").ToLower()
        };
        
        HttpContent body = new StringContent(JsonConvert.SerializeObject(content));
        body.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        request.Content = body;
        var response = await _client.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            Logger.Error("Session server returned error code: " + response.StatusCode + "  " + await response.Content.ReadAsStringAsync());
            return false;
        }

        return true;
    }
    
    /// <summary>
    /// Fetch user certificates from minecraft services
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async Task<PlayerCertificate> FetchCertificates(string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{SERVICE_URL}/player/certificates");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        var blob = JsonConvert.DeserializeObject<CertificateBlob>(body)!;

        return PlayerCertificate.FromBlob(blob);
    }
    
}
