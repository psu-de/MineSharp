using MineSharp.ChatComponent;
using MineSharp.Core.Common;
using MineSharp.Data;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace MineSharp.Protocol.Packets;

/// <summary>
/// Represents the status of a server
/// </summary>
public partial class ServerStatus
{
    /// <summary>
    /// The version string (fe. 1.20.1). Some server's send version ranges, in this case, the latest version is used.
    /// </summary>
    public readonly string Version;

    /// <summary>
    /// The protocol version used by the server.
    /// </summary>
    public readonly int ProtocolVersion;

    /// <summary>
    /// The server brand. 'Vanilla' if the server does not further specifies it.
    /// </summary>
    public readonly string Brand;

    /// <summary>
    /// The max number of players that can join.
    /// </summary>
    public readonly int MaxPlayers;

    /// <summary>
    /// How many players are currently on the server.
    /// </summary>
    public readonly int Online;

    /// <summary>
    /// A sample of players currently playing.
    /// </summary>
    public readonly string[] PlayerSample;

    /// <summary>
    /// The servers MOTD.
    /// </summary>
    public string MOTD;

    /// <summary>
    /// The servers favicon as a png data uri.
    /// </summary>
    public string FavIcon;

    /// <summary>
    /// Whether the server enforces secure chat.
    /// </summary>
    public bool EnforceSecureChat;

    /// <summary>
    /// 
    /// </summary>
    public bool PreviewsChat;

    private ServerStatus(string version, int  protocolVersion, string brand, int maxPlayers, int online, string[] playerSample, string motd,
                         string favIcon, bool enforceSecureChat, bool previewsChat)
    {
        this.Version           = version;
        this.ProtocolVersion   = protocolVersion;
        this.Brand             = brand;
        this.MaxPlayers        = maxPlayers;
        this.Online            = online;
        this.PlayerSample      = playerSample;
        this.MOTD              = motd;
        this.FavIcon           = favIcon;
        this.EnforceSecureChat = enforceSecureChat;
        this.PreviewsChat      = previewsChat;
    }

    internal static ServerStatus FromJToken(JToken token, MinecraftData data)
    {
        var versionToken = token.SelectToken("version") ?? throw new InvalidOperationException();
        var playersToken = token.SelectToken("players") ?? throw new InvalidOperationException();

        var versionString = (string)versionToken.SelectToken("name")!;
        var protocol      = (int)versionToken.SelectToken("protocol")!;

        var maxPlayers    = (int)playersToken.SelectToken("max")!;
        var onlinePlayers = (int)playersToken.SelectToken("online")!;

        var sampleToken = (JArray?)playersToken.SelectToken("sample");
        var sample = (sampleToken != null && sampleToken.Count > 0)
            ? sampleToken.Select(x => (string)x.SelectToken("name")!).ToArray()
            : Array.Empty<string>();

        var description = new Chat(token.SelectToken("description")!.ToString(), data).Message;
        var favIcon     = (string)token.SelectToken("favicon")!;

        var enforceSecureChatToken = token.SelectToken("enforcesSecureChat");
        var enforceSecureChat      = enforceSecureChatToken != null && (bool)enforceSecureChatToken;

        var previewsChatToken = token.SelectToken("previewsChat");
        var previewsChat      = previewsChatToken != null && (bool)previewsChatToken;

        (var brand, var version) = ParseVersion(versionString);

        return new ServerStatus(
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

        var brand   = match.Groups[1].Value.TrimEnd(' ');
        var version = match.Groups[2].Value;

        if (string.IsNullOrEmpty(brand))
            brand = "Vanilla";

        if (version.EndsWith('x'))
            version = version.Replace('x', '1');

        return (brand, version);
    }

    /// <inheritdoc />
    public override string ToString() => $"ServerStatus (Brand={Brand}, "                 +
                                         $"Version={this.Version}, "                      +
                                         $"Protocol={this.ProtocolVersion}, "             +
                                         $"MaxPlayers={this.MaxPlayers}, "                +
                                         $"Online={this.Online}, "                        +
                                         $"MOTD={this.MOTD}, "                            +
                                         $"EnforcesSecureChat={this.EnforceSecureChat}, " +
                                         $"PreviewsChat={this.PreviewsChat})";

    [GeneratedRegex(@"^([a-zA-Z_ ]*)(1\.\d\d?(?:\.(?:\d\d?|x))?-?)*")]
    private static partial Regex ParseVersionString();
}
