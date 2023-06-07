using Newtonsoft.Json;

namespace MineSharp.Auth.Json;

internal class JoinServerBlob
{
    [JsonProperty("accessToken")]
    public string AccessToken;

    [JsonProperty("selectedProfile")]
    public string SelectedProfile;

    [JsonProperty("serverId")]
    public string ServerId;
}
