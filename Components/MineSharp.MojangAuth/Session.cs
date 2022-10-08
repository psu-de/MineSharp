using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MojangSharpCore.Endpoints;
using Newtonsoft.Json;
using System.Net.Http.Headers;
namespace MineSharp.MojangAuth
{
    public class Session
    {


        private static readonly Logger Logger = Logger.GetLogger();

        public Session(string username, string password, UUID uuid, string clientToken, string sessionToken, bool isOnline)
        {
            this.Username = username;
            this.Password = password;
            this.UUID = uuid;
            this.ClientToken = clientToken;
            this.SessionToken = sessionToken;
            this.OnlineSession = isOnline;
        }


        public string Username {
            get;
        }
        public string Password {
            get;
        }
        public UUID UUID {
            get;
        }
        public string ClientToken {
            get;
        }
        public string SessionToken {
            get;
        }
        public bool OnlineSession {
            get;
        }

        public static async Task<Session> Login(string usernameOrEmail, string password)
        {
            var auth = await new Authenticate(new Credentials {
                Username = usernameOrEmail,
                Password = password
            }).PerformRequestAsync();

            if (auth.IsSuccess)
            {
                Logger.Debug("Successfully logged in as " + auth.SelectedProfile.PlayerName);
                return new Session(auth.SelectedProfile.PlayerName, password, UUID.Parse(auth.SelectedProfile.Value), auth.ClientToken, auth.AccessToken, true);
            }
            Logger.Warning("Could not Login, Invalid Credentials?");
            throw new Exception("AuthException");
        }

        public static async Task<Session> Load(string filepath)
        {
            using (var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                using (TextReader reader = new StreamReader(fs))
                {

                    var username = await reader.ReadLineAsync();
                    var password = await reader.ReadLineAsync();
                    var uuid = UUID.Parse(await reader.ReadLineAsync() ?? throw new ArgumentException("Invalid file"));
                    var cToken = await reader.ReadLineAsync();
                    var sToken = await reader.ReadLineAsync();
                    var isOnline = await reader.ReadLineAsync();

                    if (username == null || password == null || cToken == null || sToken == null || isOnline == null)
                    {
                        throw new Exception("Invalid file");
                    }

                    return new Session(username, password, uuid, cToken, sToken, bool.Parse(isOnline));
                }
            }
        }

        public static Session OfflineSession(string username) => new Session(username, "", Guid.NewGuid(), "", "", false);

        // This is insecure af
        // TODO: Encryption?
        public async Task Save(string filepath)
        {
            using (var fs = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (TextWriter writer = new StreamWriter(fs))
                {
                    await writer.WriteLineAsync(this.Username);
                    await writer.WriteLineAsync(this.Password);
                    await writer.WriteLineAsync(this.UUID.ToString());
                    await writer.WriteLineAsync(this.ClientToken);
                    await writer.WriteLineAsync(this.SessionToken);
                    await writer.WriteLineAsync(this.OnlineSession.ToString());
                }
            }
        }

        public async Task<bool> JoinServer(string serverHash)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            HttpContent body = new StringContent(JsonConvert.SerializeObject(new JoinServerBlob {
                ServerId = serverHash,
                AccessToken = this.SessionToken,
                SelectedProfile = this.UUID.ToString().Replace("-", "").ToLower()
            }));

            body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(new Uri("https://sessionserver.mojang.com/session/minecraft/join"), body);

            if (!response.IsSuccessStatusCode)
            {
                Logger.Error("Sessionserver returned error code: " + response.StatusCode + "  " + await response.Content.ReadAsStringAsync());
                return false;
                //throw new Exception("Could not authenticate");
            }

            return true;
        }




        private struct JoinServerBlob
        {
            [JsonProperty("accessToken")]
            public string AccessToken;

            [JsonProperty("selectedProfile")]
            public string SelectedProfile;

            [JsonProperty("serverId")]
            public string ServerId;
        }
    }
}
