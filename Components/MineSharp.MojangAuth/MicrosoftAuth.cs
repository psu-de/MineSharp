using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft;
using CmlLib.Core.Auth.Microsoft.MsalClient;
using Microsoft.Identity.Client;
using MineSharp.Core.Types;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MineSharp.MojangAuth
{
    public static class MicrosoftAuth
    {
        private static string ClientID = "3dff3eb7-2830-4d92-b2cb-033c3f47dce0";

        public delegate void DeviceCodeHandler(DeviceCodeResult deviceCode);
        
        /// <summary>
        /// Logins using a Microsoft Account
        /// </summary>
        /// <param name="handler">
        /// When the user has to login in the browser, handler() is called. It should open up a browser window and show the user the deviceCode.UserCode
        /// If none is provided, the link will open up in the default browser and the device code is written to the console
        /// </param>
        /// <returns>A Session instance</returns>
        public static async Task<Session> MicrosoftLogin(DeviceCodeHandler? handler = null)
        {
            handler ??= DefaultDeviceCodeHandler;
            
            var app = await MsalMinecraftLoginHelper.BuildApplicationWithCache(ClientID);
            var loginHandler = new LoginHandlerBuilder()
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
                if (cached) // If the cached session is invalid, try to get a new session
                {
                    var result = await loginHandler.LoginFromOAuth();
                    mSession = result.GameSession;
                } else
                {
                    throw new Exception("Could not login to a valid session");
                }
            }

            return new Session(
                mSession.Username!,
                UUID.Parse(mSession.UUID!),
                mSession.ClientToken!,
                mSession.AccessToken!,
                true);
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
}
