using System.Text.RegularExpressions;
using MineSharp.ChatComponent;
using MineSharp.Core.Common;
using MineSharp.Data;
using Newtonsoft.Json.Linq;
using NLog;

namespace MineSharp.Protocol.Packets;

/// <summary>
///     Represents the status of a server
/// </summary>
public partial class ServerStatus
{
    /// <summary>
    ///     The server brand. 'Vanilla' if the server does not further specifies it.
    /// </summary>
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    ///     The max number of players that can join.
    /// </summary>
    public int MaxPlayers { get; set; }

    /// <summary>
    ///     How many players are currently on the server.
    /// </summary>
    public int Online { get; set; }

    /// <summary>
    ///     A sample of players currently playing.
    /// </summary>
    public string[] PlayerSample { get; set; } = [];

    /// <summary>
    ///     The protocol version used by the server.
    /// </summary>
    public int ProtocolVersion { get; set; }

    /// <summary>
    ///     The version string (fe. 1.20.1). Some server's send version ranges, in this case, the latest version is used.
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    ///     Whether the server enforces secure chat.
    /// </summary>
    public bool EnforceSecureChat { get; set; }

    /// <summary>
    ///     The servers favicon as a png data uri.
    /// </summary>
    public string FavIcon { get; set; } = string.Empty;

    /// <summary>
    ///     The servers MOTD.
    /// </summary>
    public string Motd { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    public bool PreviewsChat { get; set; }

    /// <summary>
    /// Whether this server is using forge
    /// </summary>
    public bool IsForgeServer => this is ForgeServerStatus;

    /// <summary>
    /// Parse a ServerStatus from Json
    /// </summary>
    /// <exception cref="ArgumentException">thrown if <paramref name="token"/> is not of type <see cref="JTokenType.Object"/></exception>
    public static ServerStatus Parse(JToken token, MinecraftData data)
    {
        if (token.Type != JTokenType.Object)
        {
            throw new ArgumentException($"expected token of type {JTokenType.Object}");
        }
        
        if (token.SelectToken("forgeData") != null)
        {
            var forgeStatus = new ForgeServerStatus();
            forgeStatus.Parse((JObject)token, data);
            
            return forgeStatus;
        }

        var status = new ServerStatus();
        status.Parse((JObject)token, data);
        
        return status;
    }
    
    /// <summary>
    /// Parse properties from json
    /// </summary>
    protected virtual void Parse(JObject token, MinecraftData data)
    {
        if (token.TryGetValue("version", out var versionToken))
        {
            var versionString = (string)versionToken.SelectToken("name")!;
            var protocol      = (int)versionToken.SelectToken("protocol")!;
            
            (var brand, var version) = ParseVersion(versionString);

            Brand           = brand;
            Version         = version;
            ProtocolVersion = protocol;
        }

        if (token.TryGetValue("max", out var maxPlayers))
        {
            MaxPlayers = (int)maxPlayers;
        }

        if (token.TryGetValue("online", out var online))
        {
            Online = (int)online;
        }

        if (token.TryGetValue("sample", out var sampleToken))
        {
            var array = (JArray)sampleToken;
            PlayerSample = array is { Count: > 0 }
                ? sampleToken.Select(x => (string)x.SelectToken("name")!).ToArray()
                : [];
        }

        if (token.TryGetValue("description", out var description))
        {
            Motd = Chat.Parse(description).GetMessage(data);
        }

        if (token.TryGetValue("favicon", out var favicon))
        {
            FavIcon = (string)favicon!;
        }

        if (token.TryGetValue("enforcesSecureChat", out var enforceSecureChatToken))
        {
            EnforceSecureChat = (bool)enforceSecureChatToken;
        }
        
        if (token.TryGetValue("previewsChat", out var previewsChatToken))
        {
            PreviewsChat = (bool)previewsChatToken;
        }
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
    
    /// <summary>
    /// Represents the status of a forge server.
    /// </summary>
    public class ForgeServerStatus : ServerStatus
    {
        /// <summary>
        /// The FML version used by the server
        /// </summary>
        public int FmlVersion { get; set; }

        /// <summary>
        /// A map of mod id to ModInfo
        /// </summary>
        public Dictionary<string, ModInfo> Mods { get; set; } = [];

        /// <summary>
        /// A map of channel identifiers to channels
        /// </summary>
        public Dictionary<string, Channel> Channels { get; set; } = [];

        /// <inheritdoc />
        protected override void Parse(JObject token, MinecraftData minecraftData)
        {
            var forgeData = token.SelectToken("forgeData")
                ?? throw new ArgumentException($"{nameof(token)} has not forgeData object");
            
            var fmlVersion = forgeData.SelectToken("fmlNetworkVersion")!.Value<int>();

            if (3 != fmlVersion)
            {
                throw new NotSupportedException("only fml version 3 is currently supported.");
            }
            
            var       data   = forgeData.SelectToken("d")!.Value<string>()!;
            using var buffer = DecodeOptimized(data);

            // https://github.com/MinecraftForge/MinecraftForge/blob/643b972e97a51a9a2706786ea9123ed6ade1349f/src/main/java/net/minecraftforge/network/ServerStatusPing.java#L301
            var truncated = buffer.ReadBool();
            var modsSize  = buffer.ReadShort();

            var mods     = new Dictionary<string, ModInfo>();
            var channels = new Dictionary<string, Channel>();
            
            for (int i = 0; i < modsSize; i++)
            {
                (var mod, var modChannels) = ReadMod(buffer);
                mods.Add(mod.ModId, mod);

                foreach (var channel in modChannels)
                {
                    channels.Add($"{mod.ModId}:{channel.Name}", channel);
                }
            }
            
            var otherChannelsCount = buffer.ReadVarInt();
            for (int i = 0; i < otherChannelsCount; i++)
            {
                var channel = ReadChannel(buffer);
                channels.Add(channel.Name, channel);
            }

            Mods       = mods;
            Channels   = channels;
            FmlVersion = fmlVersion;
            
            base.Parse(token, minecraftData);
        }

        private static (ModInfo Mod, Channel[] channels) ReadMod(PacketBuffer buffer)
        {
            var channelSizeAndVersionFlag = buffer.ReadVarInt();
            var channelSize               = channelSizeAndVersionFlag >>> 1;
            var isIgnoreServerOnly        = (channelSizeAndVersionFlag & 0x01) != 0;
            var modId                     = buffer.ReadString();
            var modVersion                = isIgnoreServerOnly ? "SERVER_ONLY" : buffer.ReadString();
            var channels                  = new Channel[channelSize];
            
            for (var i = 0; i < channelSize; i++)
            {
                channels[i] = ReadChannel(buffer);
            }

            return (new(modId, modVersion), channels);
        }

        private static Channel ReadChannel(PacketBuffer buffer)
        {
            return new(
                buffer.ReadString(),
                buffer.ReadString(),
                buffer.ReadBool());
        }

        // https://github.com/MinecraftForge/MinecraftForge/blob/643b972e97a51a9a2706786ea9123ed6ade1349f/src/main/java/net/minecraftforge/network/ServerStatusPing.java#L375
        private static PacketBuffer DecodeOptimized(string data)
        {
            var size  = data[0] | (data[1] << 15);
            
            var buf = new PacketBuffer(Core.ProtocolVersion.V_18_1);

            var stringIndex = 2;
            var buffer      = 0;
            var bitsInBuf   = 0;

            while (stringIndex < data.Length)
            {
                while (bitsInBuf >= 8)
                {
                    buf.WriteByte((byte)buffer);
                    buffer >>>= 8;
                    bitsInBuf -= 8;
                }

                var c = data[stringIndex];
                buffer    |= (c & 0x7FFF) << bitsInBuf;
                bitsInBuf += 15;
                stringIndex++;
            }

            while (buf.Position < size)
            {
                buf.WriteByte((byte)buffer);
                buffer >>>= 8;
                bitsInBuf -= 8;
            }

            buf.SetPosition(0);

            return buf;
        }
        
        /// <summary>
        /// Dataclass describing a Mod
        /// </summary>
        /// <param name="ModId">The id (name) of the mod</param>
        /// <param name="Version">The Version of the mod</param>
        public record ModInfo(string ModId, string Version);

        /// <summary>
        /// Dataclass describing a Channel.
        /// See https://wiki.vg/Plugin_channels#Definitions
        /// </summary>
        /// <param name="Name">The name of the channel</param>
        /// <param name="Version">The version of the channel</param>
        /// <param name="Required">Whether this channel is required on the client side</param>
        public record Channel(string Name, string Version, bool Required);
    }

    [GeneratedRegex(@"^([a-zA-Z_ ]*)(1\.\d\d?(?:\.(?:\d\d?|x))?-?)*")]
    private static partial Regex ParseVersionString();
}
