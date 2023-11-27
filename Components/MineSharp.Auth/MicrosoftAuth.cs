using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft;
using CmlLib.Core.Auth.Microsoft.MsalClient;
using Microsoft.Identity.Client;
using MineSharp.Auth.Cache;
using MineSharp.Auth.Exceptions;
using MineSharp.Auth.Responses;
using MineSharp.Core.Common;
using NLog;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace MineSharp.Auth;

internal static class MicrosoftAuth
{
    private static readonly string ClientID = "3dff3eb7-2830-4d92-b2cb-033c3f47dce0";
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    public delegate void DeviceCodeHandler(DeviceCodeResult deviceCode);
    
    /// <summary>
    /// Logins using a Microsoft Account
    /// </summary>
    /// <param name="username">The username, only used for caching.</param>
    /// <param name="handler">
    /// When the user has to login in the browser, handler() is called. It should open up a browser window and show the user the deviceCode.UserCode
    /// If none is provided, the link will open up in the default browser and the device code is written to the console
    /// </param>
    /// <returns>A Session instance</returns>
    public static async Task<Session> MicrosoftLogin(string username, DeviceCodeHandler? handler = null)
    {
        handler ??= DefaultDeviceCodeHandler;
        var cacheFolder = GetCacheForUser(username);
        
        var cacheSettings = new MsalCacheSettings() 
        {
            CacheDir = cacheFolder
        };

        var app = await MsalMinecraftLoginHelper.BuildApplicationWithCache(ClientID, cacheSettings);
        var loginHandler = new LoginHandlerBuilder()
            .WithCachePath(Path.Join(cacheFolder, "session.json"))
            .ForJavaEdition()
            .WithMsalOAuth(app, factory => factory.CreateDeviceCodeApi(result =>
            {
                handler(result);
                return Task.CompletedTask;
            }))
            .Build();

        MSession mSession;
        bool cached = false;
        try
        {
            var result = await loginHandler.LoginFromCache();
            mSession = result.GameSession;
            cached = true;
        } catch (Exception)
        {
            var result = await loginHandler.LoginFromOAuth();
            mSession = result.GameSession;
        }

        if (!mSession.CheckIsValid())
        {
            if (!cached) // If the cached session is invalid, try to get a new session
            {
                throw new MineSharpAuthException("Could not login to a valid session");
            } 
            
            var result = await loginHandler.LoginFromOAuth();
            mSession = result.GameSession;
        }
        
        var certificates = PlayerCertificate.Deserialize(cacheFolder);
        if (certificates == null || certificates.RequiresRefresh())
        {
            Logger.Debug($"Fetching new certificates.");
            certificates = await MinecraftAPI.FetchCertificates(mSession.AccessToken!);
            certificates.Serialize(cacheFolder);
        }

        return new Session(
            mSession.Username!,
            UUID.Parse(mSession.UUID!),
            mSession.ClientToken!,
            mSession.AccessToken!,
            true,
            certificates);
    }

    private static string GetCacheForUser(string username)
    {
        var filename = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(username)));
        var cache = CacheManager.Get("Sessions");
        var path = Path.Join(cache, filename);

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        
        return Path.Join(path);
    }

    private static void DefaultDeviceCodeHandler(DeviceCodeResult result)
    {
        Console.WriteLine($"Microsoft login: ");
        Console.WriteLine($"Goto {result.VerificationUrl} and enter the following code: '{result.UserCode}'");
        OpenUrl(result.VerificationUrl);
    }
    
    /// <summary>
    /// Open URL
    /// </summary>
    /// <param name="url">Url to open</param>
    // https://stackoverflow.com/a/43232486/13228835
    private static void OpenUrl(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }
}