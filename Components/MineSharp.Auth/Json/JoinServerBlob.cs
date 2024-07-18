using Newtonsoft.Json;

namespace MineSharp.Auth.Json;

#pragma warning disable CS8618
internal class JoinServerBlob
{
    [JsonProperty("accessToken")] public string AccessToken;

    [JsonProperty("selectedProfile")] public string SelectedProfile;

    [JsonProperty("serverId")] public string ServerId;
}
#pragma warning restore CS8618
