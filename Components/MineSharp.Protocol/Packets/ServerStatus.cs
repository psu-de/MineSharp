using System.Text.RegularExpressions;
using MineSharp.ChatComponent;
using MineSharp.Data;
using Newtonsoft.Json.Linq;

namespace MineSharp.Protocol.Packets;

/// <summary>
///     Represents the status of a server
/// </summary>
public partial class ServerStatus
{
    /// <summary>
    ///     The server brand. 'Vanilla' if the server does not further specifies it.
    /// </summary>
    public readonly string Brand;

    /// <summary>
    ///     The max number of players that can join.
    /// </summary>
    public readonly int MaxPlayers;

    /// <summary>
    ///     How many players are currently on the server.
    /// </summary>
    public readonly int Online;

    /// <summary>
    ///     A sample of players currently playing.
    /// </summary>
    public readonly string[] PlayerSample;

    /// <summary>
    ///     The protocol version used by the server.
    /// </summary>
    public readonly int ProtocolVersion;

    /// <summary>
    ///     The version string (fe. 1.20.1). Some server's send version ranges, in this case, the latest version is used.
    /// </summary>
    public readonly string Version;

    /// <summary>
    ///     Whether the server enforces secure chat.
    /// </summary>
    public bool EnforceSecureChat;

    /// <summary>
    ///     The servers favicon as a png data uri.
    /// </summary>
    public string FavIcon;

    /// <summary>
    ///     The servers MOTD.
    /// </summary>
    public string Motd;

    /// <summary>
    /// </summary>
    public bool PreviewsChat;

    private ServerStatus(string version, int protocolVersion, string brand, int maxPlayers, int online,
                         string[] playerSample, string motd,
                         string favIcon, bool enforceSecureChat, bool previewsChat)
    {
        Version = version;
        ProtocolVersion = protocolVersion;
        Brand = brand;
        MaxPlayers = maxPlayers;
        Online = online;
        PlayerSample = playerSample;
        Motd = motd;
        FavIcon = favIcon;
        EnforceSecureChat = enforceSecureChat;
        PreviewsChat = previewsChat;
    }

    internal static ServerStatus FromJToken(JToken token, MinecraftData data)
    {
        var versionToken = token.SelectToken("version") ?? throw new ArgumentException("invalid token");
        var playersToken = token.SelectToken("players") ?? throw new ArgumentException("invalid token");

        var versionString = (string)versionToken.SelectToken("name")!;
        var protocol = (int)versionToken.SelectToken("protocol")!;

        var maxPlayers = (int)playersToken.SelectToken("max")!;
        var onlinePlayers = (int)playersToken.SelectToken("online")!;

        var sampleToken = (JArray?)playersToken.SelectToken("sample");
        var sample = sampleToken != null && sampleToken.Count > 0
            ? sampleToken.Select(x => (string)x.SelectToken("name")!).ToArray()
            : Array.Empty<string>();

        var description = Chat.Parse(token.SelectToken("description")!).GetMessage(data);
        var favIcon = (string)token.SelectToken("favicon")!;

        var enforceSecureChatToken = token.SelectToken("enforcesSecureChat");
        var enforceSecureChat = enforceSecureChatToken != null && (bool)enforceSecureChatToken;

        var previewsChatToken = token.SelectToken("previewsChat");
        var previewsChat = previewsChatToken != null && (bool)previewsChatToken;

        (var brand, var version) = ParseVersion(versionString);

        return new(
            version,
            protocol,
            brand,
            maxPlayers,
            onlinePlayers,
            sample,
            description,
            favIcon,
            enforceSecureChat,
            previewsChat);
    }

    private static (string Brand, string Version) ParseVersion(string versionString)
    {
        var match = ParseVersionString().Match(versionString);

        var brand = match.Groups[1].Value.TrimEnd(' ');
        var version = match.Groups[2].Value;

        if (string.IsNullOrEmpty(brand))
        {
            brand = "Vanilla";
        }

        if (version.EndsWith('x'))
        {
            version = version.Replace('x', '1');
        }

        return (brand, version);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"ServerStatus (Brand={Brand}, " +
            $"Version={Version}, " +
            $"Protocol={ProtocolVersion}, " +
            $"MaxPlayers={MaxPlayers}, " +
            $"Online={Online}, " +
            $"MOTD={Motd}, " +
            $"EnforcesSecureChat={EnforceSecureChat}, " +
            $"PreviewsChat={PreviewsChat})";
    }

    [GeneratedRegex(@"^([a-zA-Z_ ]*)(1\.\d\d?(?:\.(?:\d\d?|x))?-?)*")]
    private static partial Regex ParseVersionString();
}
