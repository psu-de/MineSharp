using Newtonsoft.Json;

namespace MineSharp.Protocol.Packets;

#pragma warning disable CS8618
/// <summary>
///     JSON Response from the server about it's status
/// </summary>
public class ServerStatusResponseBlob
{
    /// <summary>
    ///     The servers MOTD
    /// </summary>
    [JsonProperty("description")] public DescriptionBlob Description;

    /// <summary>
    ///     Whether the server enforces secure chats
    /// </summary>
    [JsonProperty("enforcesSecureChat")] public bool EnforcesSecureChat;

    /// <summary>
    ///     The servers favicon as data url
    /// </summary>
    [JsonProperty("favicon")] public string FavIcon;

    /// <summary>
    ///     A list of player's currently on the server
    /// </summary>
    [JsonProperty("players")] public PlayersBlob Players;

    /// <summary>
    ///     The version string of the server
    /// </summary>
    [JsonProperty("version")] public ServerVersion Version;

    /// <summary>
    ///     Description of the server
    /// </summary>
    public class DescriptionBlob
    {
        /// <summary>
        ///     Text
        /// </summary>
        [JsonProperty("text")] public string Text;
    }

    /// <summary>
    ///     JSON Response from the server about online players
    /// </summary>
    public class PlayersBlob
    {
        /// <summary>
        ///     The maximum amount of players allowed to connect
        /// </summary>
        [JsonProperty("max")] public int Max;

        /// <summary>
        ///     Whether the server is in online mode
        /// </summary>
        [JsonProperty("online")] public int Online;

        /// <summary>
        ///     A list of players on the server
        /// </summary>
        [JsonProperty("sample")] public PlayerBlob[] Sample;

        /// <summary>
        ///     Json response describing a player
        /// </summary>
        public class PlayerBlob
        {
            /// <summary>
            ///     The player's id
            /// </summary>
            [JsonProperty("id")] public string Id;

            /// <summary>
            ///     The Username of the player
            /// </summary>
            [JsonProperty("name")] public string Name;
        }
    }

    /// <summary>
    ///     Json response with information about the server's version
    /// </summary>
    public class ServerVersion
    {
        /// <summary>
        ///     Minecraft version string
        /// </summary>
        [JsonProperty("name")] public string Name;

        /// <summary>
        ///     Protocol number
        /// </summary>
        [JsonProperty("protocol")] public int Protocol;
    }
}
#pragma warning restore CS8618
