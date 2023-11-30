using Newtonsoft.Json;

namespace MineSharp.Protocol.Packets;

#pragma warning disable CS8618
public class ServerStatusResponseBlob
{
    [JsonProperty("version")]
    public ServerVersion Version;

    [JsonProperty("players")]
    public PlayersBlob Players;

    [JsonProperty("description")]
    public DescriptionBlob Description;

    [JsonProperty("favicon")]
    public string FavIcon;

    [JsonProperty("enforcesSecureChat")]
    public bool EnforcesSecureChat;
    
    public class DescriptionBlob
    {
        [JsonProperty("text")]
        public string Text;
    }

    public class PlayersBlob
    {
        [JsonProperty("max")]
        public int Max;

        [JsonProperty("online")]
        public int Online;

        [JsonProperty("sample")]
        public PlayerBlob[] Sample;
        
        public class PlayerBlob
        {
            [JsonProperty("name")]
            public string Name;

            [JsonProperty("id")]
            public string Id;
        }
    }
    
    public class ServerVersion
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("protocol")]
        public int Protocol;
    }
}
#pragma warning restore CS8618