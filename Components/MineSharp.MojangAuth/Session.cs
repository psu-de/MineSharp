using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MojangSharp.Endpoints;
using MojangSharp.Responses;
using Newtonsoft.Json;

namespace MineSharp.MojangAuth {
    public class Session {


        private static Logger Logger = Logger.GetLogger();
        
        public static async Task<Session> Login(string usernameOrEmail, string password) {
            AuthenticateResponse auth = await new Authenticate(new Credentials() { Username = usernameOrEmail, Password = password }).PerformRequestAsync();

            if (auth.IsSuccess) {
                Logger.Debug("Successfully logged in as " + auth.SelectedProfile.PlayerName);
                return new Session(auth.SelectedProfile.PlayerName, password, UUID.Parse(auth.SelectedProfile.Value), auth.ClientToken, auth.AccessToken, true);
            } else {
                Logger.Warning("Could not Login, Invalid Credentials?");
                throw new Exception("AuthException");
            }
        }

        public static async Task<Session> Load(string filepath) {
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read)) {
                using (TextReader reader = new StreamReader(fs)) {

                    string? username = await reader.ReadLineAsync();
                    string? password = await reader.ReadLineAsync();
                    UUID uuid     = UUID.Parse(await reader.ReadLineAsync());
                    string? cToken   = await reader.ReadLineAsync();
                    string? sToken   = await reader.ReadLineAsync();
                    string? isOnline = await reader.ReadLineAsync();

                    if (username == null || password == null || cToken == null || sToken == null || isOnline == null) {
                        throw new Exception("Invalid file");
                    }

                    return new Session(username, password, uuid, cToken, sToken, bool.Parse(isOnline));
                }
            }
        }

        public static Session OfflineSession(string username) {
            return new Session(username, "", Guid.NewGuid(), "", "", false);
        }


        public string Username { get; private set; }
        public string Password { get; private set; }
        public UUID UUID { get; private set; }
        public string ClientToken { get; private set; }
        public string SessionToken { get; private set; }
        public bool OnlineSession { get; private set; }

        public Session(string username, string password, UUID uuid, string clientToken, string sessionToken, bool isOnline) {
            Username = username;
            Password = password;
            UUID = uuid;
            ClientToken = clientToken;
            SessionToken = sessionToken;
            OnlineSession = isOnline;
        }

        // This is insecure af
        // TODO: Encryption?
        public async Task Save(string filepath) {
            using (FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write)) {
                using (TextWriter writer = new StreamWriter(fs)) {
                    await writer.WriteLineAsync(this.Username);
                    await writer.WriteLineAsync(this.Password);
                    await writer.WriteLineAsync(this.UUID.ToString());
                    await writer.WriteLineAsync(this.ClientToken);
                    await writer.WriteLineAsync(this.SessionToken);
                    await writer.WriteLineAsync(this.OnlineSession.ToString());
                }
            }
        }




        private struct JoinServerBlob {
            [JsonProperty("accessToken")]
            public string AccessToken;

            [JsonProperty("selectedProfile")]
            public string SelectedProfile;

            [JsonProperty("serverId")]
            public string ServerId;
        }

        public async Task<bool> JoinServer(string serverHash) {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


            HttpContent body = new StringContent(JsonConvert.SerializeObject(new JoinServerBlob() {
                ServerId = serverHash,
                AccessToken = this.SessionToken,
                SelectedProfile = this.UUID.ToString().Replace("-", "").ToLower(),
            }));

            body.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.PostAsync(new Uri("https://sessionserver.mojang.com/session/minecraft/join"), body);
                        
            if (!response.IsSuccessStatusCode) {
                Logger.Error("Sessionserver returned error code: " + response.StatusCode + "  " + await response.Content.ReadAsStringAsync());
                return false;
                //throw new Exception("Could not authenticate");
            }

            return true;
        }
    }
}